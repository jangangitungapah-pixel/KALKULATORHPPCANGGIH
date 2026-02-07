using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Hpp.WinUI.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    public SettingsViewModel()
    {
        AccentOptions = new ObservableCollection<string> { "Blue", "Green", "Orange", "Red" };
        SelectedAccent = AccentOptions.Contains(App.CurrentAccent) ? App.CurrentAccent : AccentOptions.First();
        IsDarkTheme = App.IsDarkThemeEnabled;
        StatusMessage = $"Current theme {(IsDarkTheme ? "Dark" : "Light")} with {SelectedAccent} accent.";
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
        App.ApplyAppearance(IsDarkTheme, SelectedAccent);
        StatusMessage = $"Applied theme {(IsDarkTheme ? "Dark" : "Light")} with {SelectedAccent} accent.";
    }

    [RelayCommand]
    private void Reset()
    {
        IsDarkTheme = true;
        SelectedAccent = AccentOptions.First();
        ConfirmDeleteMode = false;
        App.ApplyAppearance(IsDarkTheme, SelectedAccent);
        StatusMessage = "Settings reset to defaults.";
    }

    partial void OnIsDarkThemeChanged(bool value)
    {
        App.ApplyAppearance(value, SelectedAccent);
    }

    partial void OnSelectedAccentChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            App.ApplyAppearance(IsDarkTheme, value);
        }
    }
}
