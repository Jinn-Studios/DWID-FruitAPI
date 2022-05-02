using FruitUI.API.ServiceCore;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.OpenApi.Models;
using Serilog;

namespace FruitUI.API
{
    public static class Program
    {
        static void Main(string[] args)
        {
            ConfigureApp(ConfigureBuilder(args)).Run();
        }

        private static WebApplicationBuilder ConfigureBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen(x => ConfigureSwagger(x));

            builder.Services.AddTransient<IAuthService>(x => new AuthService());

            return ConfigureLogging(builder);
        }

        private static WebApplicationBuilder ConfigureLogging(WebApplicationBuilder builder)
        {
            var logger = new LoggerConfiguration()
              .ReadFrom.Configuration(builder.Configuration)
              .Enrich.FromLogContext()
              .WriteTo.File(path: "../serilog/.log", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, rollingInterval: RollingInterval.Minute,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Properties}{NewLine}{Exception}")
              .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);
            builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Information);
            builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Error);

            var telemetryChannel = new CustomTelemetryChannel();
            builder.Services.AddSingleton<ITelemetryChannel>(x => telemetryChannel);
            builder.Services.AddSingleton<ICustomTelemetryChannel>(x => telemetryChannel);

            builder.Services.AddApplicationInsightsTelemetry();

            return builder;
        }

        private static WebApplication ConfigureApp(WebApplicationBuilder builder)
        {
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseMiddleware<SwaggerMW>();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<JwtMW>();

            app.MapControllers();
            return app;
        }

        private static void ConfigureSwagger(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
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

            var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        }
    }
}