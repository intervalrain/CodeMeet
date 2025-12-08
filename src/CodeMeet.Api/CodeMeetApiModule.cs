using Microsoft.OpenApi.Models;
using CodeMeet.Application;
using CodeMeet.Infrastructure;

namespace CodeMeet.Api;

public static class CodeMeetApiModule
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
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