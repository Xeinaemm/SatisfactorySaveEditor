using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SatisfactorySaveEditor.Cheats;
using SatisfactorySaveEditor.Data;
using SatisfactorySaveEditor.View;
using SatisfactorySaveEditor.ViewModel;
using Serilog;
using Serilog.Events;
using System.IO;
using System.Text;
using System.Windows;

namespace SatisfactorySaveEditor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly ServiceProvider serviceProvider;
    public static IConfiguration Configuration { get; private set; }

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton(x => SetupConfiguration());
        var cfg = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        services.AddDbContext<AppSettingsDbContext>(
            options => options.UseSqlite(cfg.GetConnectionString("database")))
        .AddLogging(logging => logging.AddSerilog())
        .AddSingleton<MainWindow>()
        .AddSingleton<MainViewModel>()
        .AddSingleton<AddWindow>()
        .AddTransient<AddViewModel>()
        .AddSingleton<CheatInventoryWindow>()
        .AddTransient<CheatInventoryViewModel>()
        .AddSingleton<FillWindow>()
        .AddTransient<FillViewModel>()
        .AddSingleton<MassDismantleWindow>()
        .AddSingleton<PreferencesWindow>()
        .AddTransient<PreferencesWindowViewModel>()
        .AddSingleton<StringPromptWindow>()
        .AddTransient<StringPromptViewModel>()
        .AddSingleton<UnlockResearchWindow>()
        .AddTransient<UnlockResearchWindowViewModel>()
        .AddSingleton<UpdateWindow>()
        .AddTransient<UpdateWindowViewModel>();
    }

    private IConfiguration SetupConfiguration()
    {
        var builder = new ConfigurationBuilder();

        var flushInterval = new TimeSpan(0, 0, 1);
        var file = Path.Combine(Directory.GetCurrentDirectory(), "SSE.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(
                file,
                flushToDiskInterval: flushInterval,
                encoding: Encoding.UTF8,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 22)
            .CreateLogger();
        var cfg = builder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();
        Configuration = cfg;
        return cfg;
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var mainWindow = serviceProvider.GetService<MainWindow>();
        mainWindow.Show();
    }
}
