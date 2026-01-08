//using System;
//using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Routing.Template;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;
//using Moq;
//using Swashbuckle.AspNetCore.Swagger;
//using Swashbuckle.AspNetCore.SwaggerUI;
//using Tsa.Submissions.Coding.WebApi;
//using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi;

[ExcludeFromCodeCoverage]
public class StartupTests
{
    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public void Configure_Should_Use_Development_When_Environment_Is_Development()
    //{
    //    // Arrange
    //    var mockedServiceScope = new Mock<IServiceScope>();

    //    var mockedServiceScopeFactory = new Mock<IServiceScopeFactory>();
    //    mockedServiceScopeFactory.Setup(serviceScopeFactory => serviceScopeFactory.CreateScope())
    //        .Returns(mockedServiceScope.Object);

    //    var mockedSwaggerOptions = new Mock<IOptionsSnapshot<SwaggerOptions>>();

    //    var mockedServiceProvider = new Mock<IServiceProvider>();
    //    mockedServiceProvider.Setup(serviceProvider => serviceProvider.GetService(typeof(IOptionsSnapshot<SwaggerOptions>)))
    //        .Returns(new Mock<IOptionsSnapshot<SwaggerOptions>>().Object);
    //    mockedServiceProvider.Setup(serviceProvider => serviceProvider.GetService(typeof(IOptionsSnapshot<SwaggerUIOptions>)))
    //        .Returns(mockedSwaggerOptions.Object);
    //    mockedServiceProvider.Setup(serviceProvider => serviceProvider.GetService(typeof(IServiceScopeFactory)))
    //        .Returns(mockedServiceScopeFactory.Object);
    //    mockedServiceProvider.Setup(serviceProvider => serviceProvider.GetService(typeof(TemplateBinderFactory)))
    //        .Returns(new Mock<TemplateBinderFactory>().Object);

    //    mockedServiceScope.Setup(serviceScope => serviceScope.ServiceProvider)
    //        .Returns(mockedServiceProvider.Object);

    //    var mockApplicationBuilder = new Mock<IApplicationBuilder>();
    //    mockApplicationBuilder.Setup(applicationBuilder => applicationBuilder.Properties)
    //        .Returns(new Dictionary<string, object?>());
    //    mockApplicationBuilder.Setup(applicationBuilder => applicationBuilder.ApplicationServices)
    //        .Returns(mockedServiceProvider.Object);

    //    var mockConfiguration = new Mock<IConfiguration>();

    //    var mockHostEnvironment = new Mock<IWebHostEnvironment>();
    //    mockHostEnvironment.Setup(hostEnvironment => hostEnvironment.EnvironmentName).Returns("Development");
    //    //mockHostEnvironment.Setup(hostEnvironment=> hostEnvironment.IsDevelopment()).Returns(true);

    //    // Act
    //    var startup = new Startup(mockConfiguration.Object);

    //    startup.Configure(mockApplicationBuilder.Object, mockHostEnvironment.Object);

    //    // Assert
    //    mockApplicationBuilder.Verify(applicationBuilder => applicationBuilder.UseDeveloperExceptionPage(), Times.Once);
    //    mockApplicationBuilder.Verify(applicationBuilder => applicationBuilder.UseSwagger(It.IsAny<SwaggerOptions>()), Times.Once);
    //    mockApplicationBuilder.Verify(applicationBuilder => applicationBuilder.UseSwaggerUI(It.IsAny<SwaggerUIOptions>()), Times.Once);
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public void ConfigureServices_RegistersAuthentication()
    //{
    //    // Arrange
    //    var jwtSettingsSection = new Mock<IConfigurationSection>();
    //    jwtSettingsSection.Setup(x => x.Get<JwtSettings>()).Returns(new JwtSettings
    //    {
    //        Audience = "TestAudience",
    //        Issuer = "TestIssuer",
    //        Key = "TestKey",
    //        ExpirationInHours = 1
    //    });

    //    _mockConfiguration.Setup(x => x.GetSection("Jwt")).Returns(jwtSettingsSection.Object);

    //    var startup = new Startup(_mockConfiguration.Object);

    //    // Act
    //    startup.ConfigureServices(_services);

    //    // Assert
    //    var serviceProvider = _services.BuildServiceProvider();
    //    var authenticationSchemeProvider = serviceProvider.GetService<IAuthenticationSchemeProvider>();
    //    Assert.NotNull(authenticationSchemeProvider);
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public void ConfigureServices_RegistersJwtSettings()
    //{
    //    // Arrange
    //    var jwtSettingsSection = new Mock<IConfigurationSection>();
    //    jwtSettingsSection.Setup(x => x.Get<JwtSettings>()).Returns(new JwtSettings
    //    {
    //        Audience = "TestAudience",
    //        Issuer = "TestIssuer",
    //        Key = "TestKey",
    //        ExpirationInHours = 1
    //    });

    //    _mockConfiguration.Setup(x => x.GetSection("Jwt")).Returns(jwtSettingsSection.Object);

    //    var startup = new Startup(_mockConfiguration.Object);

    //    // Act
    //    startup.ConfigureServices(_services);

    //    // Assert
    //    var serviceProvider = _services.BuildServiceProvider();
    //    var jwtSettings = serviceProvider.GetService<IOptions<JwtSettings>>();
    //    Assert.NotNull(jwtSettings);
    //    Assert.Equal("TestAudience", jwtSettings.Value.Audience);
    //    Assert.Equal("TestIssuer", jwtSettings.Value.Issuer);
    //    Assert.Equal("TestKey", jwtSettings.Value.Key);
    //    Assert.Equal(1, jwtSettings.Value.ExpirationInHours);
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public void ConfigureServices_RegistersMongoClient()
    //{
    //    // Arrange
    //    var submissionsDatabaseSection = new Mock<IConfigurationSection>();
    //    submissionsDatabaseSection.Setup(x => x.Get<SubmissionsDatabase>()).Returns(new SubmissionsDatabase
    //    {
    //        Host = "localhost",
    //        Port = 27017,
    //        Username = "testuser",
    //        Password = "testpassword",
    //        LoginDatabase = "admin",
    //        Name = "testdb"
    //    });

    //    _mockConfiguration.Setup(x => x.GetSection(ConfigurationKeys.SubmissionsDatabaseSection))
    //        .Returns(submissionsDatabaseSection.Object);

    //    var startup = new Startup(_mockConfiguration.Object);

    //    // Act
    //    startup.ConfigureServices(_services);

    //    // Assert
    //    var serviceProvider = _services.BuildServiceProvider();
    //    var mongoClient = serviceProvider.GetService<IMongoClient>();
    //    Assert.NotNull(mongoClient);
    //}

    //[Fact]
    //[Trait("TestCategory", "UnitTest")]
    //public void ConfigureServices_RegistersSwagger()
    //{
    //    // Arrange
    //    var startup = new Startup(_mockConfiguration.Object);

    //    // Act
    //    startup.ConfigureServices(_services);

    //    // Assert
    //    var serviceProvider = _services.BuildServiceProvider();
    //    var swaggerGenService = serviceProvider.GetService<IServiceCollection>();
    //    Assert.NotNull(swaggerGenService);
    //}
}
