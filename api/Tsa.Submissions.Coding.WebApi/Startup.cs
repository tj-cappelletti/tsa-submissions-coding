using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;
using Tsa.Submissions.Coding.WebApi.Validators;

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

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    public void ConfigureServices(IServiceCollection services)
    {
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
        var mongoDbServiceTypes = assemblyTypes
            .Where(type => type.Namespace == servicesNamespace && type is
            { IsAbstract: false, IsClass: true, IsGenericType: false, IsInterface: false, IsNested: false })
            .ToList();

        var mongoEntityServiceInterfaceType = typeof(IMongoEntityService<>);
        var pingableServiceInterfaceType = typeof(IPingableService);

        foreach (var mongoDbServiceType in mongoDbServiceTypes)
        {
            var interfaceType = mongoDbServiceType
                .GetInterfaces()
                .SingleOrDefault(type =>
                    type.Name != mongoEntityServiceInterfaceType.Name &&
                    type.Name != pingableServiceInterfaceType.Name);

            if (interfaceType == null) continue;

            services.AddScoped(interfaceType, mongoDbServiceType);
        }

        // Add Pingable Services - Should match MongoDB Services
        var pingableServiceType = typeof(IPingableService);
        var pingableServices = assemblyTypes
            .Where(type => pingableServiceType.IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false })
            .ToList();

        foreach (var pingableService in pingableServices)
        {
            services.AddSingleton(typeof(IPingableService), pingableService);
        }

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
            });

        // Add Validators
        services.AddScoped<IValidator<ProblemModel>, ProblemModelValidator>();
        services.AddScoped<IValidator<TeamModel>, TeamModelValidator>();
        services.AddScoped<IValidator<TestSetModel>, TestSetModelValidator>();
        services.AddScoped<IValidator<UserModel>, UserModelValidator>();

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
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Tsa.Submissions.Coding.WebApi", Version = "v1" });

            options.EnableAnnotations();

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}
