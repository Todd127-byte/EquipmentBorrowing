using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using EquipmentBorrowing.Desktop.ViewModels;
using EquipmentBorrowing.Desktop.Views;

namespace EquipmentBorrowing.Desktop;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        return param switch
        {
            EquipmentViewModel => new EquipmentView(),
            BorrowingsViewModel => new BorrowingsView(),
            null => null,
            _ => new TextBlock { Text = $"Not Found: {param.GetType().Name}" }
        };
    }

    public bool Match(object? data)
    {
        return data is EquipmentViewModel or BorrowingsViewModel;
    }
}
