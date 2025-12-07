using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace CodeMeet.Api;

public static class CodeMeetApiModule
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
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

    public static WebApplication UseApi(this WebApplication app)
    {
        return app;
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