using Microsoft.OpenApi.Models;
using CodeMeet.Application;
using CodeMeet.Infrastructure;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Asp.Versioning;

namespace CodeMeet.Api;

public static class CodeMeetApiModule
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(opts => opts.Conventions.Add(new RouteTokenTransformerConvention(LowercaseRouteTransformer.Default)));
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true; 
        });
        services.AddSwagger();
        services.AddApplication(configuration);
        services.AddInfrastructure();

        return services;
    }

    public static WebApplication UseApi(this WebApplication app)
    {
        app.MapControllers();
        return app;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CodeMeet API",
                Version = "v1"
            });
        });

        return services;
    }

    public static WebApplication UseSwaggerUI(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("swagger/v1/swagger.json", "CodeMeet API v1");
            options.RoutePrefix = "";
        });

        return app;
    }
}

public class LowercaseRouteTransformer : IOutboundParameterTransformer
{
    public static readonly LowercaseRouteTransformer Default = new();
    
    public string? TransformOutbound(object? value)
    {
        return value?.ToString()?.ToLower();
    }
}