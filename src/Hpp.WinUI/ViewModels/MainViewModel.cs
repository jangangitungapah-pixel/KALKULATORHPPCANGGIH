using CommunityToolkit.Mvvm.ComponentModel;

namespace Hpp.WinUI.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "KALKULATORHPPCANGGIH";

    [ObservableProperty]
    private string _subtitle = "Next-Generation HPP Intelligence";
}
