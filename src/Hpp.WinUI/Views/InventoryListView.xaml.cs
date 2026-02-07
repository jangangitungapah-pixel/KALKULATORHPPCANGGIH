using Microsoft.UI.Xaml.Controls;

using Microsoft.Extensions.DependencyInjection;

namespace Hpp.WinUI.Views;

public sealed partial class InventoryListView : Page
{
    public InventoryListView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ViewModels.InventoryListViewModel>();
    }
}
