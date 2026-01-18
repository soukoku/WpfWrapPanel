using System.Windows;
using System.Windows.Media;

namespace WpfWrapPanel;

public static class WpfExtensions
{

    /// <summary>
    /// Helper to find a child of a specific type in the visual tree.
    /// </summary>
    public static T? FindVisualChild<T>(this DependencyObject parent) where T : DependencyObject
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
