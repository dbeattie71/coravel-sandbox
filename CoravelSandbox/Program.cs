using Coravel.Pro.EntityFramework;
using Coravel.Pro;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel;
using CoravelSandbox;
using Elevate.Scheduler;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var options = new WebApplicationOptions
    {
        Args = args,
        ContentRootPath = WindowsServiceHelpers.IsWindowsService()
            ? AppContext.BaseDirectory
            : default
    };

    var builder = WebApplication.CreateBuilder(options);
    var configuration = builder.Configuration;

    configuration.AddJsonFile("appsettings.json", true);
    configuration.AddEnvironmentVariables();

    builder.Host.UseWindowsService();

    //[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}
    builder.Host.UseSerilog((c, s, lc) => lc
        .ReadFrom.Configuration(configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{Properties:j}{NewLine}{Exception}")
    );

    builder.Services.AddRazorPages().AddNewtonsoftJson();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var dbPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "scheduler.db");
    var schedulerConnectionString = $"Data Source={dbPath}";
    builder.Services.AddDbContext<SchedulerDbContext>(schedulerConnectionString);
    builder.Services.AddCoravelPro(typeof(SchedulerDbContext));

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<SchedulerDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();

        await dbContext.SaveChangesAsync();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapRazorPages();
    });


    app.Services.UseScheduler(s =>
        {
            s.Schedule<DynamicScheduler>().Daily().RunOnceAtStart();
        })
        //.LogScheduledTaskProgress(test)
        ;

    await app.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Host terminated unexpectedly");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}