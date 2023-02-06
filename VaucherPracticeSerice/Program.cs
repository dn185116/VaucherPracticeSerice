using Serilog;
using VaucherPracticeSerice;
using VaucherPracticeService.Services.VPS;


// msiexec /i VaucherService.msi TARGETDIR=C:\ APSBASEURL=https://test APSUSERNAME=ARA6/RMH14== APSPASSWORD=WjVFMVdTcGViaw== APSREQUESTTIMEOUTSECONDS=30 APSPROMPTCASHIERTIMEOUTSECONDS=20 APSDISPLAYMESSAGETIMEOUTSECONDS=5 PINPADIPADDRESS=127.0.0.1 PINPADPORT=2008 PINPADRECEIVETIMEOUTMILIS=60000 POLLINGPERIODMILIS=2000 CASHBACKPROMPTENABLED=false /quiet

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
