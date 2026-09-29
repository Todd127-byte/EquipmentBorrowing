using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Desktop.ViewModels;
using EquipmentBorrowing.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

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

        // Repository instances retain in-memory borrowing state for this run.
        services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
        services.AddSingleton<IEquipmentRepository, InMemoryEquipmentRepository>();
        services.AddSingleton<IBorrowingRepository, InMemoryBorrowingRepository>();

        // Application services contain the operations used by the ViewModels.
        services.AddTransient<BorrowEquipmentService>();
        services.AddTransient<ReturnEquipmentService>();
        services.AddTransient<GetAllEquipmentService>();
        services.AddTransient<GetAllStudentsService>();
        services.AddTransient<GetActiveBorrowingsService>();

        // Shared ViewModels retain their state while the user navigates.
        services.AddSingleton<EquipmentViewModel>();
        services.AddSingleton<BorrowingsViewModel>();
        services.AddSingleton<MainViewModel>();

        _serviceProvider = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
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
}
