using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ServiceDiscovery;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.Hosting
{
    // Adds common Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
    // This project should be referenced by each service project in your solution.
    // To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
    public static class Extensions
    {
        private const string HealthEndpointPath = "/health";
        private const string AlivenessEndpointPath = "/alive";

        public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder)
            where TBuilder : IHostApplicationBuilder
        {
            builder.ConfigureOpenTelemetry();

            builder.AddDefaultHealthChecks();

            builder.Services.AddServiceDiscovery();

            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                // Turn on resilience by default
                http.AddStandardResilienceHandler();

                // Turn on service discovery by default
                http.AddServiceDiscovery();
            });

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()); // I always add this.
            });
            builder.Services.AddProblemDetails(); // Every API in our "domain" will use this.
            // Uncomment the following to restrict the allowed schemes for service discovery.
            // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
            // {
            //     options.AllowedSchemes = ["https"];
            // });

            return builder;
        }

        public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder)
            where TBuilder : IHostApplicationBuilder
        {
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddMeter(WolverineMetricsConstants.InboxCount,
                            WolverineMetricsConstants.OutboxCount,
                            WolverineMetricsConstants.DeadLetterQueue,
                            WolverineMetricsConstants.MessagesSent,
                            WolverineMetricsConstants.MessagesFailed);
                })
                .WithTracing(tracing =>
                {
                    tracing
                        .AddSource("Wolverine")
                        .AddSource(builder.Environment.ApplicationName)
                        .AddAspNetCoreInstrumentation(tracing =>
                            // Exclude health check requests from tracing
                            tracing.Filter = context =>
                                !context.Request.Path.StartsWithSegments(HealthEndpointPath)
                                && !context.Request.Path.StartsWithSegments(AlivenessEndpointPath)
                        )
                        // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                        //.AddGrpcClientInstrumentation()
                        .AddHttpClientInstrumentation();
                });

            builder.AddOpenTelemetryExporters();

            return builder;
        }

        private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder)
            where TBuilder : IHostApplicationBuilder
        {
            var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

            if (useOtlpExporter) builder.Services.AddOpenTelemetry().UseOtlpExporter();

            // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
            //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
            //{
            //    builder.Services.AddOpenTelemetry()
            //       .UseAzureMonitor();
            //}

            return builder;
        }

        public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder)
            where TBuilder : IHostApplicationBuilder
        {
            builder.Services.AddHealthChecks()
                // Add a default liveness check to ensure app is responsive
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

            return builder;
        }

        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            // Adding health checks endpoints to applications in non-development environments has security implications.
            // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
            if (app.Environment.IsDevelopment())
            {
                // All health checks must pass for app to be considered ready to accept traffic after starting
                app.MapHealthChecks(HealthEndpointPath);

                // Only health checks tagged with the "live" tag must pass for app to be considered alive
                app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains("live")
                });
            }

            return app;
        }
    }
}

public class WolverineMetricsConstants
{
    public const string MessagesSent = "wolverine-messages-sent";
    public const string ExecutionTime = "wolverine-execution-time";
    public const string MessagesSucceeded = "wolverine-messages-succeeded";
    public const string DeadLetterQueue = "wolverine-dead-letter-queue";
    public const string EffectiveMessageTime = "wolverine-effective-time";

    public const string Milliseconds = "Milliseconds";
    public const string Messages = "Messages";

    public const string InboxCount = "wolverine-inbox-count";
    public const string OutboxCount = "wolverine-outbox-count";
    public const string ScheduledCount = "wolverine-scheduled-count";

    public const string MessageTypeKey = "message.type";
    public const string MessageDestinationKey = "message.destination";
    public const string TenantIdKey = "tenant.id";
    public const string MessagesFailed = "wolverine-execution-failure";
    public const string ExceptionType = "exception.type";
}