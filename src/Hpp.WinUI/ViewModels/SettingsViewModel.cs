using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Hpp.WinUI.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    public SettingsViewModel()
    {
        AccentOptions = new ObservableCollection<string> { "Blue", "Green", "Orange", "Purple" };
        SelectedAccent = AccentOptions.First();
    }

    [ObservableProperty]
    private bool _isDarkTheme;

    public ObservableCollection<string> AccentOptions { get; }

    [ObservableProperty]
    private string _selectedAccent;
}
