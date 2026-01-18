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
        var test = this.VirtualizingListBox.ItemContainerGenerator;
        VirtualizingListBox.Loaded+= VirtualizingListBox_Loaded;
    }

    private void VirtualizingListBox_Loaded(object sender, RoutedEventArgs e)
    {
        var test = this.VirtualizingListBox.ItemContainerGenerator;
    }

    private void StandardListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (StandardListBox.SelectedItem is null)
            return;


        VirtualizingListBox.SelectedItems.Clear();
        vwpVirtualizingListBox.SelectedItems.Clear();
        foreach (var item in StandardListBox.SelectedItems)
        {
            VirtualizingListBox.SelectedItems.Add(item);
            vwpVirtualizingListBox.SelectedItems.Add(item);
        }

        // Sync first selection
#if NETFRAMEWORK
        vwpVirtualizingListBox.ScrollIntoView(VirtualizingListBox.SelectedItem);

        // For virtualized items, we need to get the panel and call BringIndexIntoView directly
        var index = StandardListBox.SelectedIndex;
        if (index >= 0)
        {
            // Get the VirtualizingWrapPanel from the ListBox
            var panel = VirtualizingListBox.FindVisualChild<VirtualizingWrapPanel>();
            panel?.BringIndexIntoView(index);
        }
#else
        VirtualizingListBox.ScrollIntoView(VirtualizingListBox.SelectedItem);
#endif
    }
}