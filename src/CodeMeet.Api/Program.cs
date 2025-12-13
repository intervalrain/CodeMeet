using CodeMeet.Api;
using CodeMeet.Api.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting CodeMeet API...");

    var builder = WebApplication.CreateBuilder(args);
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services));

        builder.Services.AddApi(builder.Configuration);
    }

    var app = builder.Build();
    {
        await app.SeedDataAsync();

        app.UseSerilogRequestLogging();
        app.UseApi();

        app.UseSwaggerUI();

        app.Run();
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}