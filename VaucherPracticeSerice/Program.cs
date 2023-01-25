using Serilog;
using VaucherPracticeSerice;
using VaucherPracticeService.Services.VPS;

IHost host = Host.CreateDefaultBuilder(args)
     .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(hostingContext.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
        )
    .ConfigureServices(services =>
    {
        services.AddSingleton<IVPSApi, VPSApi>();
        services.AddHostedService<Worker>();    
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
