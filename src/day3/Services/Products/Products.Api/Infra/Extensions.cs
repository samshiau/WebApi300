using System.Reflection;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Products.Api.Endpoints.Management.Handlers;
using Products.Api.Endpoints.Management.ReadModels;
using Wolverine;
using Wolverine.Marten;

namespace Products.Api.Infra;

public static class Extensions
{
    private static string CorsPolicyName => Assembly.GetCallingAssembly().GetName().Name ??= "ShoppingCart.Api";

    extension(WebApplicationBuilder builder)
    {
        public WebApplicationBuilder AddPersistenceAndMessaging(string dataSourceName)
        {
            builder.AddNpgsqlDataSource(dataSourceName);
            builder.Host.UseWolverine((options) =>
            {
                options.Policies.AutoApplyTransactions();
            }); // have to change the mode for this.
            builder.Services.AddMarten(options =>
                {
                    options.Projections.Add<UserInfoProjection>(ProjectionLifecycle.Inline);
                    options.Projections.Add<ProductReadModelProjection>(ProjectionLifecycle.Inline);
                    options.Projections.Snapshot<InventoryChangeReport>(SnapshotLifecycle.Async);
                    options.Projections.Add<ManagerSummaryProjection>(ProjectionLifecycle.Async);
                }).UseNpgsqlDataSource().UseLightweightSessions().IntegrateWithWolverine()
                .AddAsyncDaemon(JasperFx.Events.Daemon.DaemonMode.Solo); // turns on the background worker
            return builder;
        }

        public WebApplicationBuilder AddCorsForDevelopment()
        {
            if (builder.Environment.IsDevelopment() == false) return builder;
            builder.Services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy(CorsPolicyName, pol =>
                {
                  
                    pol.WithOrigins("http://localhost:9561", "http://localhost");
                    pol.AllowAnyHeader();
                    pol.AllowAnyMethod();
                    pol.AllowCredentials();
                });
                corsOptions.DefaultPolicyName = CorsPolicyName;
            });
            return builder;
        }

        /// <summary>
        /// Used to configure OpenAPI generation for API
        /// </summary>
        /// <param name="apiName">like "vendors"</param>
        /// <param name="apiVersion">like "v1"</param>
        /// <returns></returns>
        public WebApplicationBuilder AddDevelopmentOpenApiGeneration(string apiName, string apiVersion)
        {
            if (!builder.Environment.IsDevelopment()) return builder;
            var baseVersion = $"{apiName}.{apiVersion}";
            builder.Services.AddOpenApi(baseVersion,
                options => options.AddDocumentTransformer<ServiceOpenApiTransform>());
            return builder;
        }
    }

    extension(WebApplication app)
    {
        public WebApplication MapOpenApiForDevelopment()
        {
            if (!app.Environment.IsDevelopment()) return app;
            app.UseCors(CorsPolicyName);
            app.MapOpenApi("/openapi/{documentName}.json").AllowAnonymous();
            return app;
        }
    }

    extension(IServiceCollection services)
    {
        public IServiceCollection AddAuthenticationSchemes()
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.MetadataAddress = options.Authority + "/.well-known/openid-configuration";
                options.MapInboundClaims = false;
            });
            return services;
        }

        public IServiceCollection AddAuthorizationAndPolicies()
        {
            services.AddAuthorizationBuilder().AddPolicy(AuthPolicies.ProductManagerPolicy, pol =>
            {
                // you can do whatever here. look them up in your database, whatever.
                pol.RequireRole(AuthPolicies.ProductManagerRoles);
                pol.RequireAuthenticatedUser();
            });
            return services;
        }
    }
}

public class AuthPolicies
{
    public const string ProductManagerPolicy = "ProductManagement";
    public static readonly List<string> ProductManagerRoles = ["ProductManager"];
}