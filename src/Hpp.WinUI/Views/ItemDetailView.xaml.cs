using Microsoft.UI.Xaml.Controls;

using Microsoft.Extensions.DependencyInjection;

namespace Hpp.WinUI.Views;

public sealed partial class ItemDetailView : Page
{
    public ItemDetailView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ViewModels.ItemDetailViewModel>();
    }
}
