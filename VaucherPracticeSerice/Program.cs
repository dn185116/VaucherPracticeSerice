using VaucherPracticeSerice;
using VaucherPracticeService.Services.VPS;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IVPSApi, VPSApi>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
