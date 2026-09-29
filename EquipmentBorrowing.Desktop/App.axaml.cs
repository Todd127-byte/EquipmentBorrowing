using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Desktop.ViewModels;
using EquipmentBorrowing.Infrastructure.Persistence;
using EquipmentBorrowing.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EquipmentBorrowing.Desktop;

public partial class App : Avalonia.Application
{
    private IServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        services.AddDbContextFactory<EquipmentBorrowingDbContext>(options =>
        {
            options.UseSqlite(DatabasePaths.GetConnectionString());
#if DEBUG
            options.LogTo(
                Console.WriteLine,
                new[] { DbLoggerCategory.Database.Command.Name },
                LogLevel.Information);
#endif
        });

        services.AddSingleton<IStudentRepository, EfStudentRepository>();
        services.AddSingleton<IEquipmentRepository, EfEquipmentRepository>();
        services.AddSingleton<IBorrowingRepository, EfBorrowingRepository>();
        services.AddSingleton<DatabaseInitializer>();

        services.AddTransient<BorrowEquipmentService>();
        services.AddTransient<ReturnEquipmentService>();
        services.AddTransient<GetAllEquipmentService>();
        services.AddTransient<GetAvailableEquipmentService>();
        services.AddTransient<GetAllStudentsService>();
        services.AddTransient<GetActiveBorrowingsService>();

        // Shared ViewModels preserve UI state while the user navigates.
        services.AddSingleton<EquipmentViewModel>();
        services.AddSingleton<BorrowingsViewModel>();
        services.AddSingleton<MainViewModel>();

        _serviceProvider = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            try
            {
                _serviceProvider.GetRequiredService<DatabaseInitializer>()
                    .InitializeAsync()
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception exception)
            {
                desktop.MainWindow = CreateDatabaseErrorWindow(exception);
                base.OnFrameworkInitializationCompleted();
                return;
            }

            var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
            var mainWindow = new Views.MainWindow
            {
                DataContext = mainViewModel
            };

            mainWindow.Opened += async (_, _) => await mainViewModel.InitializeAsync();
            desktop.MainWindow = mainWindow;
            desktop.Exit += (_, _) => (_serviceProvider as IDisposable)?.Dispose();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static Window CreateDatabaseErrorWindow(Exception exception)
    {
        return new Window
        {
            Title = "Database initialization failed",
            Width = 560,
            Height = 260,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(24),
                Spacing = 12,
                Children =
                {
                    new TextBlock
                    {
                        Text = "The equipment database could not be initialized.",
                        FontSize = 18,
                        FontWeight = Avalonia.Media.FontWeight.Bold
                    },
                    new TextBlock
                    {
                        Text = exception.Message,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    }
                }
            }
        };
    }
}
