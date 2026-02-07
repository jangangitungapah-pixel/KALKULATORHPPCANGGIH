using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;

namespace Hpp.WinUI.Helpers;

public sealed partial class CommandPalette : UserControl
{
    private readonly List<PaletteCommandItem> _source;

    public CommandPalette()
    {
        InitializeComponent();
        _source = new List<PaletteCommandItem>();
        FilteredCommands = new ObservableCollection<PaletteCommandItem>();
    }

    public ObservableCollection<PaletteCommandItem> FilteredCommands { get; }

    public event EventHandler<PaletteCommandItem>? CommandInvoked;

    public void SetCommands(IEnumerable<PaletteCommandItem> commands)
    {
        _source.Clear();
        _source.AddRange(commands);
        ApplyFilter();
    }

    public void Toggle()
    {
        PalettePopup.IsOpen = !PalettePopup.IsOpen;
        if (PalettePopup.IsOpen)
        {
            CommandTextBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
            CommandTextBox.SelectAll();
            ApplyFilter();
        }
    }

    private void OnCommandTextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFilter();
    }

    private void OnListDoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
        TryInvokeSelected();
    }

    private void OnListSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListView { SelectedItem: PaletteCommandItem item })
        {
            return;
        }

        CommandInvoked?.Invoke(this, item);
        PalettePopup.IsOpen = false;
    }

    private void OnExecuteClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        TryInvokeSelected();
    }

    private void TryInvokeSelected()
    {
        if (CommandList.SelectedItem is not PaletteCommandItem item)
        {
            return;
        }

        CommandInvoked?.Invoke(this, item);
        PalettePopup.IsOpen = false;
    }

    private void ApplyFilter()
    {
        var term = CommandTextBox.Text?.Trim() ?? string.Empty;
        var rows = string.IsNullOrWhiteSpace(term)
            ? _source
            : _source.Where(command =>
                command.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                command.Description.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                command.Key.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();

        FilteredCommands.Clear();
        foreach (var row in rows.Take(12))
        {
            FilteredCommands.Add(row);
        }

        CommandList.SelectedIndex = FilteredCommands.Count > 0 ? 0 : -1;
    }
}

public sealed record PaletteCommandItem(string Key, string Title, string Description);
