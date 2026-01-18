namespace WpfWrapPanel;

/// <summary>
/// Specifies the unit of scrolling for the VirtualizingWrapPanel.
/// </summary>
public enum ScrollUnit
{
    /// <summary>
    /// Scrolling is performed in pixel increments for smooth scrolling.
    /// </summary>
    Pixel,

    /// <summary>
    /// Scrolling snaps to item boundaries.
    /// </summary>
    Item
}
