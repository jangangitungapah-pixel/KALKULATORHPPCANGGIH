using Microsoft.UI.Xaml.Controls;

namespace Hpp.WinUI.Views;

public sealed partial class ItemDetailView : Page
{
    public ItemDetailView(ViewModels.ItemDetailViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
