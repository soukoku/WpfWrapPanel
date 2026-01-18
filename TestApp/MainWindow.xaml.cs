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
        // try to get virtualizing item to scroll into view
        if (StandardListBox.SelectedItem is not null)
        {
            VirtualizingListBox.SelectedItem = StandardListBox.SelectedItem;
            VirtualizingListBox.ScrollIntoView(StandardListBox.SelectedItem);
        }
    }
}