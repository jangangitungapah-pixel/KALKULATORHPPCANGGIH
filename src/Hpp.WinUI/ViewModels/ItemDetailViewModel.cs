using CommunityToolkit.Mvvm.ComponentModel;

namespace Hpp.WinUI.ViewModels;

public sealed partial class ItemDetailViewModel : ObservableObject
{
    [ObservableProperty]
    private string _status = "Select an item to view details.";
}
