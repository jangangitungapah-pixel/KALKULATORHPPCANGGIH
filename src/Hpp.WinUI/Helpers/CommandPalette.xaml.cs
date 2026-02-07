using Microsoft.UI.Xaml.Controls;

namespace Hpp.WinUI.Helpers;

public sealed partial class CommandPalette : UserControl
{
    public CommandPalette()
    {
        InitializeComponent();
    }

    public void Toggle()
    {
        PalettePopup.IsOpen = !PalettePopup.IsOpen;
        if (PalettePopup.IsOpen)
        {
            CommandTextBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
        }
    }
}
