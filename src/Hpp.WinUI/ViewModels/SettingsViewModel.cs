using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Hpp.WinUI.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    public SettingsViewModel()
    {
        AccentOptions = new ObservableCollection<string> { "Blue", "Green", "Orange", "Red" };
        SelectedAccent = AccentOptions.First();
        StatusMessage = "Ready.";
    }

    [ObservableProperty]
    private bool _isDarkTheme;

    public ObservableCollection<string> AccentOptions { get; }

    [ObservableProperty]
    private string _selectedAccent;

    [ObservableProperty]
    private string _statusMessage;

    [ObservableProperty]
    private bool _confirmDeleteMode;

    [RelayCommand]
    private void Apply()
    {
        StatusMessage = $"Applied theme {(IsDarkTheme ? "Dark" : "Light")} with {SelectedAccent} accent.";
    }

    [RelayCommand]
    private void Reset()
    {
        IsDarkTheme = false;
        SelectedAccent = AccentOptions.First();
        ConfirmDeleteMode = false;
        StatusMessage = "Settings reset to defaults.";
    }
}
