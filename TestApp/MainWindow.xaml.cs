using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfWrapPanel;

namespace TestApp;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void StandardListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (StandardListBox.SelectedItem is null)
            return;

        // Sync selection
        VirtualizingListBox.SelectedItem = StandardListBox.SelectedItem;
#if NETFRAMEWORK
        // For virtualized items, we need to get the panel and call BringIndexIntoView directly
        var index = StandardListBox.SelectedIndex;
        if (index >= 0)
        {
            // Get the VirtualizingWrapPanel from the ListBox
            var panel = FindVisualChild<VirtualizingWrapPanel>(VirtualizingListBox);
            panel?.BringIndexIntoView(index);
        }
#else
        VirtualizingListBox.ScrollIntoView(VirtualizingListBox.SelectedItem);
#endif
    }

    /// <summary>
    /// Helper to find a child of a specific type in the visual tree.
    /// </summary>
    private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T found)
                return found;

            var result = FindVisualChild<T>(child);
            if (result != null)
                return result;
        }
        return null;
    }
}