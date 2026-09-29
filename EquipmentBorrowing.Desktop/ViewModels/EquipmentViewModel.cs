using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using System.Collections.ObjectModel;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentViewModel : ViewModelBase
{
    private readonly GetAllEquipmentService _getAllEquipmentService;
    private readonly GetAvailableEquipmentService _getAvailableEquipmentService;
    private readonly GetAllStudentsService _getAllStudentsService;
    private readonly BorrowEquipmentService _borrowEquipmentService;

    public ObservableCollection<Equipment> Equipment { get; } = new();
    public ObservableCollection<Equipment> BorrowableEquipment { get; } = new();
    public ObservableCollection<Student> Students { get; } = new();

    [ObservableProperty]
    private Equipment? selectedEquipment;

    [ObservableProperty]
    private Student? selectedStudent;

    [ObservableProperty]
    private Equipment? selectedBorrowEquipment;

    [ObservableProperty]
    private DateTimeOffset? expectedReturnDate;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public EquipmentViewModel(
        GetAllEquipmentService getAllEquipmentService,
        GetAvailableEquipmentService getAvailableEquipmentService,
        GetAllStudentsService getAllStudentsService,
        BorrowEquipmentService borrowEquipmentService)
    {
        _getAllEquipmentService = getAllEquipmentService;
        _getAvailableEquipmentService = getAvailableEquipmentService;
        _getAllStudentsService = getAllStudentsService;
        _borrowEquipmentService = borrowEquipmentService;
    }

    [RelayCommand]
    private Task LoadAsync() => RefreshAsync();

    [RelayCommand]
    private async Task BorrowEquipmentAsync()
    {
        if (SelectedStudent is null)
        {
            StatusMessage = "Select a student before borrowing equipment.";
            return;
        }

        if (SelectedBorrowEquipment is null)
        {
            StatusMessage = "Select equipment before submitting the borrowing.";
            return;
        }

        if (ExpectedReturnDate is null)
        {
            StatusMessage = "Choose an expected return date.";
            return;
        }

        try
        {
            await _borrowEquipmentService.BorrowEquipmentAsync(
                SelectedStudent.Id,
                SelectedBorrowEquipment.Id,
                ExpectedReturnDate.Value.DateTime);

            await RefreshAsync(showStatus: false);
            StatusMessage = "Equipment borrowed successfully.";
        }
        catch (InvalidOperationException exception)
        {
            StatusMessage = $"Borrowing failed: {exception.Message}";
        }
        catch
        {
            StatusMessage = "Borrowing failed. Please try again.";
        }
    }

    public async Task RefreshAsync(bool showStatus = true)
    {
        try
        {
            var equipment = await _getAllEquipmentService.ExecuteAsync();
            var availableEquipment = await _getAvailableEquipmentService.ExecuteAsync();
            var students = await _getAllStudentsService.ExecuteAsync();

            Equipment.Clear();
            foreach (var item in equipment)
            {
                Equipment.Add(item);
            }

            BorrowableEquipment.Clear();
            foreach (var item in availableEquipment)
            {
                BorrowableEquipment.Add(item);
            }

            Students.Clear();
            foreach (var student in students)
            {
                Students.Add(student);
            }

            if (showStatus)
            {
                StatusMessage = $"Loaded {Equipment.Count} equipment items.";
            }
        }
        catch
        {
            if (showStatus)
            {
                StatusMessage = "Equipment could not be loaded. Please try again.";
            }
        }
    }
}
