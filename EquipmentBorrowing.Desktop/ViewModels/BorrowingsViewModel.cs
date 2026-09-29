using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Models;
using EquipmentBorrowing.Application.Services;
using System.Collections.ObjectModel;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class BorrowingsViewModel : ViewModelBase
{
    private readonly GetActiveBorrowingsService _getActiveBorrowingsService;
    private readonly ReturnEquipmentService _returnEquipmentService;
    private readonly EquipmentViewModel _equipmentViewModel;

    public ObservableCollection<ActiveBorrowingSummary> ActiveBorrowings { get; } = new();

    [ObservableProperty]
    private ActiveBorrowingSummary? selectedBorrowing;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private bool hasNoBorrowings;

    public BorrowingsViewModel(
        GetActiveBorrowingsService getActiveBorrowingsService,
        ReturnEquipmentService returnEquipmentService,
        EquipmentViewModel equipmentViewModel)
    {
        _getActiveBorrowingsService = getActiveBorrowingsService;
        _returnEquipmentService = returnEquipmentService;
        _equipmentViewModel = equipmentViewModel;
    }

    [RelayCommand]
    private Task LoadAsync() => RefreshAsync();

    [RelayCommand(CanExecute = nameof(CanReturnSelectedBorrowing))]
    private async Task ReturnEquipmentAsync()
    {
        if (SelectedBorrowing is null)
        {
            StatusMessage = "Select an active borrowing to return.";
            return;
        }

        try
        {
            await _returnEquipmentService.ReturnEquipmentAsync(SelectedBorrowing.BorrowingId);
            await RefreshAsync(showStatus: false);
            await _equipmentViewModel.RefreshAsync(showStatus: false);
            StatusMessage = "Equipment returned successfully.";
        }
        catch (InvalidOperationException exception)
        {
            StatusMessage = $"Return failed: {exception.Message}";
        }
        catch
        {
            StatusMessage = "Return failed. Please try again.";
        }
    }

    public async Task RefreshAsync(bool showStatus = true)
    {
        try
        {
            var borrowings = await _getActiveBorrowingsService.ExecuteAsync();

            SelectedBorrowing = null;
            ActiveBorrowings.Clear();
            foreach (var borrowing in borrowings)
            {
                ActiveBorrowings.Add(borrowing);
            }

            HasNoBorrowings = ActiveBorrowings.Count == 0;
            if (showStatus)
            {
                StatusMessage = HasNoBorrowings
                    ? "There are no active borrowings."
                    : $"Loaded {ActiveBorrowings.Count} active borrowing(s).";
            }
        }
        catch
        {
            if (showStatus)
            {
                StatusMessage = "Active borrowings could not be loaded. Please try again.";
            }
        }
    }

    private bool CanReturnSelectedBorrowing() => SelectedBorrowing is not null;

    partial void OnSelectedBorrowingChanged(ActiveBorrowingSummary? value)
    {
        ReturnEquipmentCommand.NotifyCanExecuteChanged();
    }
}
