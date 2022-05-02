using Microsoft.ApplicationInsights.Channel;
using Microsoft.Extensions.Logging.ApplicationInsights;
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
            builder.Services.AddSwaggerGen();
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
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapControllers();
            return app;
        }
    }
}