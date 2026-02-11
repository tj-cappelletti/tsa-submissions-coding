using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tsa.Submissions.Coding.Contracts.Authentication;
using Tsa.Submissions.Coding.Contracts.Users;
using Tsa.Submissions.Coding.Contracts.Validators;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi;

[ExcludeFromCodeCoverage]
public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseCors("AllowAll");

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        var jwtSection = Configuration.GetSection("Jwt");

        var jwtSettings = jwtSection.Get<JwtSettings>() ??
                          throw new NullReferenceException("The configuration for the JWT settings was null.");

        if (!jwtSettings.IsValid())
        {
            var error = jwtSettings.GetError();
            var errorMessage = error switch
            {
                JwtSettingsConfigError.Audience => "The configuration for the JWT settings was invalid. Audience is required.",
                JwtSettingsConfigError.ExpirationInHours => "The configuration for the JWT settings was invalid. ExpirationInHours is required.",
                JwtSettingsConfigError.Issuer => "The configuration for the JWT settings was invalid. Issuer is required.",
                JwtSettingsConfigError.Key => "The configuration for the JWT settings was invalid. Key is required.",
                _ => "An unknown error occurred while validating the configuration for the JWT settings."
            };
            throw new InvalidOperationException(errorMessage);
        }

        services.Configure<JwtSettings>(jwtSection);

        var submissionsDatabaseSection = Configuration.GetSection(ConfigurationKeys.SubmissionsDatabaseSection);

        var submissionsDatabase = submissionsDatabaseSection.Get<SubmissionsDatabase>() ??
                                  throw new NullReferenceException("The configuration for the Submissions database was null.");

        if (!submissionsDatabase.IsValid())
        {
            var error = submissionsDatabase.GetError();
            var errorMessage = error switch
            {
                SubmissionsDatabaseConfigError.Host => "The configuration for the Submissions database was invalid. Host is required.",
                SubmissionsDatabaseConfigError.LoginDatabase => "The configuration for the Submissions database was invalid. LoginDatabase is required.",
                SubmissionsDatabaseConfigError.Name => "The configuration for the Submissions database was invalid. Name is required.",
                SubmissionsDatabaseConfigError.Password => "The configuration for the Submissions database was invalid. Password is required.",
                SubmissionsDatabaseConfigError.Port => "The configuration for the Submissions database was invalid. Port is required.",
                SubmissionsDatabaseConfigError.Username => "The configuration for the Submissions database was invalid. Username is required.",
                _ => "An unknown error occurred while validating the configuration for the Submissions database."
            };

            throw new InvalidOperationException(errorMessage);
        }

        services.Configure<SubmissionsDatabase>(submissionsDatabaseSection);

        var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true)
        };

        ConventionRegistry.Register("DefaultConventionPack", conventionPack, _ => true);

        var mongoCredential = MongoCredential.CreateCredential(
            submissionsDatabase.LoginDatabase,
            submissionsDatabase.Username,
            submissionsDatabase.Password);

        var mongoClientSettings = new MongoClientSettings
        {
            ConnectTimeout = new TimeSpan(0, 0, 0, 10),
            Credential = mongoCredential,
            Scheme = ConnectionStringScheme.MongoDB,
            Server = new MongoServerAddress(submissionsDatabase.Host, submissionsDatabase.Port),
            ServerSelectionTimeout = new TimeSpan(0, 0, 0, 10)
        };

        services.Add(
            new ServiceDescriptor(typeof(IMongoClient),
                new MongoClient(mongoClientSettings)));

        var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();

        // Add MongoDB Services
        const string servicesNamespace = "Tsa.Submissions.Coding.WebApi.Services";
        var serviceTypes = assemblyTypes
            .Where(type => type.Namespace == servicesNamespace && type is
                { IsAbstract: false, IsClass: true, IsGenericType: false, IsInterface: false, IsNested: false })
            .ToList();

        var mongoEntityServiceInterfaceType = typeof(IMongoEntityService<>);
        var pingableServiceInterfaceType = typeof(IPingableService);

        foreach (var serviceType in serviceTypes)
        {
            var interfaceTypes = serviceType.GetInterfaces();

            if (interfaceTypes.Length == 0) continue;

            foreach (var interfaceType in interfaceTypes)
            {
                // Add Pingable Services
                if (interfaceType.Name == pingableServiceInterfaceType.Name)
                {
                    services.AddScoped(pingableServiceInterfaceType, serviceType);
                    continue;
                }

                var implementsMongoEntityService = interfaceType
                    .GetInterfaces()
                    .Any(type => type.Name == mongoEntityServiceInterfaceType.Name);

                if (implementsMongoEntityService)
                {
                    services.AddScoped(interfaceType, serviceType);
                }
            }
        }

        // Add RabbitMQ Service
        services.Configure<RabbitMQConfig>(Configuration.GetSection(RabbitMQConfig.SectionName));
        services.AddSingleton<ISubmissionsQueueService, RabbitMQService>();

        // Add Redis Service
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = Configuration.GetConnectionString(ConfigurationKeys.RedisConnectionString);
            options.InstanceName = "Tsa.Submissions.Coding.WebApi";
        });

        services.AddSingleton<ICacheService, CacheService>();

        // Add Authentication
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = jwtSettings.RequireHttpsMetadata;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    // JWT settings are validated up above
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key!))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Log when no token or header is present
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                        var authHeader = context.Request.Headers.Authorization.ToString();

                        if (string.IsNullOrWhiteSpace(authHeader))
                        {
                            logger.LogWarning("JWT message received without Authorization header. Path={Path}", context.Request.Path);
                        }
                        else if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        {
                            logger.LogWarning("Authorization header present but not Bearer. Path={Path}", context.Request.Path);
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                        logger.LogWarning(context.Exception, "JWT authentication failed. Path={Path}", context.Request.Path);

                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        // Fires when authentication fails and a 401 is about to be returned
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                        logger.LogInformation(
                            "JWT challenge issued. Path={Path}, Error={Error}, Description={Description}",
                            context.Request.Path,
                            context.Error,
                            context.ErrorDescription);

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                        logger.LogDebug("JWT token validated successfully. Path={Path}, Subject={Sub}", context.Request.Path,
                            context.Principal?.Identity?.Name);
                        return Task.CompletedTask;
                    }
                };
            });

        // Add Validators
        services.AddScoped<IValidator<AuthenticationRequest>, AuthenticationRequestValidator>();
        services.AddScoped<IValidator<UserCreateRequest>, UserCreateRequestValidator>();
        services.AddScoped<IValidator<UserModifyRequest>, UserModifyRequestValidator>();

        // Setup Controllers
        services
            .AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

        // Add Swagger
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });
        });
    }
}
