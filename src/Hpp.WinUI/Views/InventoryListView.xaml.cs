using Microsoft.UI.Xaml.Controls;

namespace Hpp.WinUI.Views;

public sealed partial class InventoryListView : Page
{
    public InventoryListView(ViewModels.InventoryListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
