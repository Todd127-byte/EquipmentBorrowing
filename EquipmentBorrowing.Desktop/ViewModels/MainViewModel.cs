using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly EquipmentViewModel _equipmentViewModel;
    private readonly BorrowingsViewModel _borrowingsViewModel;

    [ObservableProperty]
    private ViewModelBase? currentViewModel;

    public MainViewModel(
        EquipmentViewModel equipmentViewModel,
        BorrowingsViewModel borrowingsViewModel)
    {
        _equipmentViewModel = equipmentViewModel;
        _borrowingsViewModel = borrowingsViewModel;
        CurrentViewModel = _equipmentViewModel;
    }

    public async Task InitializeAsync()
    {
        await _equipmentViewModel.RefreshAsync();
        await _borrowingsViewModel.RefreshAsync();
        CurrentViewModel = _equipmentViewModel;
    }

    [RelayCommand]
    private async Task ShowEquipmentAsync()
    {
        await _equipmentViewModel.RefreshAsync();
        CurrentViewModel = _equipmentViewModel;
    }

    [RelayCommand]
    private async Task ShowBorrowingsAsync()
    {
        await _borrowingsViewModel.RefreshAsync();
        CurrentViewModel = _borrowingsViewModel;
    }
}
