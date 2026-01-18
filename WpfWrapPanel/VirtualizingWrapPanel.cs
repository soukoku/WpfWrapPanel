using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

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

/// <summary>
/// A virtualizing panel that arranges items in a wrapping layout, similar to WrapPanel,
/// but with UI virtualization support for improved performance with large collections.
/// </summary>
public class VirtualizingWrapPanel : VirtualizingPanel, IScrollInfo
{
    #region Dependency Properties

    /// <summary>
    /// Identifies the <see cref="Orientation"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(
            nameof(Orientation),
            typeof(Orientation),
            typeof(VirtualizingWrapPanel),
            new FrameworkPropertyMetadata(
                Orientation.Horizontal,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>
    /// Identifies the <see cref="ItemWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ItemWidthProperty =
        DependencyProperty.Register(
            nameof(ItemWidth),
            typeof(double),
            typeof(VirtualizingWrapPanel),
            new FrameworkPropertyMetadata(
                double.NaN,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>
    /// Identifies the <see cref="ItemHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ItemHeightProperty =
        DependencyProperty.Register(
            nameof(ItemHeight),
            typeof(double),
            typeof(VirtualizingWrapPanel),
            new FrameworkPropertyMetadata(
                double.NaN,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>
    /// Identifies the <see cref="HorizontalSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HorizontalSpacingProperty =
        DependencyProperty.Register(
            nameof(HorizontalSpacing),
            typeof(double),
            typeof(VirtualizingWrapPanel),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>
    /// Identifies the <see cref="VerticalSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty VerticalSpacingProperty =
        DependencyProperty.Register(
            nameof(VerticalSpacing),
            typeof(double),
            typeof(VirtualizingWrapPanel),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));


    /// <summary>
    /// Identifies the <see cref="CacheLength"/> dependency property.
    /// Controls how many additional rows/columns of items are cached beyond the visible viewport.
    /// </summary>
    public static readonly DependencyProperty CacheLengthProperty =
        DependencyProperty.Register(
            nameof(CacheLength),
            typeof(int),
            typeof(VirtualizingWrapPanel),
            new FrameworkPropertyMetadata(
                2,
                FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// Identifies the <see cref="StretchItems"/> dependency property.
    /// When true, items are stretched to fill available space in each row/column.
    /// </summary>
    public static readonly DependencyProperty StretchItemsProperty =
        DependencyProperty.Register(
            nameof(StretchItems),
            typeof(bool),
            typeof(VirtualizingWrapPanel),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>
    /// Identifies the <see cref="ScrollUnit"/> dependency property.
    /// Controls whether scrolling is pixel-based (smooth) or item-based (snaps to items).
    /// </summary>
    public static readonly DependencyProperty ScrollUnitProperty =
        DependencyProperty.Register(
            nameof(ScrollUnit),
            typeof(ScrollUnit),
            typeof(VirtualizingWrapPanel),
            new FrameworkPropertyMetadata(
                ScrollUnit.Pixel,
                FrameworkPropertyMetadataOptions.AffectsArrange));

    #endregion

    #region CLR Properties

    /// <summary>
    /// Gets or sets the orientation of the panel.
    /// Horizontal means items flow left-to-right and wrap to new rows.
    /// Vertical means items flow top-to-bottom and wrap to new columns.
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of each item. Use NaN for auto-sizing from first item.
    /// </summary>
    public double ItemWidth
    {
        get => (double)GetValue(ItemWidthProperty);
        set => SetValue(ItemWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of each item. Use NaN for auto-sizing from first item.
    /// </summary>
    public double ItemHeight
    {
        get => (double)GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal spacing between items.
    /// </summary>
    public double HorizontalSpacing
    {
        get => (double)GetValue(HorizontalSpacingProperty);
        set => SetValue(HorizontalSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical spacing between items.
    /// </summary>
    public double VerticalSpacing
    {
        get => (double)GetValue(VerticalSpacingProperty);
        set => SetValue(VerticalSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of extra rows/columns of items to cache beyond the visible viewport.
    /// Higher values improve scrolling performance but use more memory.
    /// Default is 2.
    /// </summary>
    public int CacheLength
    {
        get => (int)GetValue(CacheLengthProperty);
        set => SetValue(CacheLengthProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether items should stretch to fill available space.
    /// When true, items in each row are expanded to use any remaining horizontal space (for Horizontal orientation)
    /// or items in each column are expanded to use any remaining vertical space (for Vertical orientation).
    /// </summary>
    public bool StretchItems
    {
        get => (bool)GetValue(StretchItemsProperty);
        set => SetValue(StretchItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets the scroll unit for the panel.
    /// Pixel provides smooth scrolling, Item snaps to item boundaries.
    /// Default is Pixel.
    /// </summary>
    public ScrollUnit ScrollUnit
    {
        get => (ScrollUnit)GetValue(ScrollUnitProperty);
        set => SetValue(ScrollUnitProperty, value);
    }

    #endregion

    #region Private Fields

    // Scroll tracking
    private Size _extent = new(0, 0);
    private Size _viewport = new(0, 0);
    private Point _offset = new(0, 0);
    private ScrollViewer? _scrollOwner;

    // Track if we have a pending deferred measure
    private bool _hasPendingMeasure;

    // Cached item size (calculated from first item or from ItemWidth/ItemHeight)
    private Size _itemSize = new(100, 100);

    // Stretched item size (when StretchItems is true)
    private Size _stretchedItemSize = new(100, 100);

    // Layout state
    private int _itemsPerRow = 1;
    private int _firstVisibleIndex = 0;
    private int _lastVisibleIndex = -1;

    #endregion

    #region IScrollInfo Implementation

    /// <inheritdoc/>
    public bool CanHorizontallyScroll { get; set; }

    /// <inheritdoc/>
    public bool CanVerticallyScroll { get; set; }

    /// <inheritdoc/>
    public double ExtentWidth => _extent.Width;

    /// <inheritdoc/>
    public double ExtentHeight => _extent.Height;

    /// <inheritdoc/>
    public double ViewportWidth => _viewport.Width;

    /// <inheritdoc/>
    public double ViewportHeight => _viewport.Height;

    /// <inheritdoc/>
    public double HorizontalOffset => _offset.X;

    /// <inheritdoc/>
    public double VerticalOffset => _offset.Y;

    /// <inheritdoc/>
    public ScrollViewer? ScrollOwner
    {
        get => _scrollOwner;
        set => _scrollOwner = value;
    }

    /// <inheritdoc/>
    public void LineUp() => SetVerticalOffset(VerticalOffset - GetLineScrollAmount());

    /// <inheritdoc/>
    public void LineDown() => SetVerticalOffset(VerticalOffset + GetLineScrollAmount());

    /// <inheritdoc/>
    public void LineLeft() => SetHorizontalOffset(HorizontalOffset - GetLineScrollAmount());

    /// <inheritdoc/>
    public void LineRight() => SetHorizontalOffset(HorizontalOffset + GetLineScrollAmount());

    /// <inheritdoc/>
    public void PageUp() => SetVerticalOffset(VerticalOffset - GetPageScrollAmount(isVertical: true));

    /// <inheritdoc/>
    public void PageDown() => SetVerticalOffset(VerticalOffset + GetPageScrollAmount(isVertical: true));

    /// <inheritdoc/>
    public void PageLeft() => SetHorizontalOffset(HorizontalOffset - GetPageScrollAmount(isVertical: false));

    /// <inheritdoc/>
    public void PageRight() => SetHorizontalOffset(HorizontalOffset + GetPageScrollAmount(isVertical: false));

    /// <inheritdoc/>
    public void MouseWheelUp() => SetVerticalOffset(VerticalOffset - GetMouseWheelScrollAmount());

    /// <inheritdoc/>
    public void MouseWheelDown() => SetVerticalOffset(VerticalOffset + GetMouseWheelScrollAmount());

    /// <inheritdoc/>
    public void MouseWheelLeft() => SetHorizontalOffset(HorizontalOffset - GetMouseWheelScrollAmount());

    /// <inheritdoc/>
    public void MouseWheelRight() => SetHorizontalOffset(HorizontalOffset + GetMouseWheelScrollAmount());

    /// <inheritdoc/>
    public void SetHorizontalOffset(double offset)
    {
        offset = Math.Max(0, Math.Min(offset, ExtentWidth - ViewportWidth));
        
        // Snap to item boundaries when using item-based scrolling
        if (ScrollUnit == ScrollUnit.Item && Orientation == Orientation.Vertical)
        {
            var effectiveSize = StretchItems ? _stretchedItemSize : _itemSize;
            var columnWidth = effectiveSize.Width + HorizontalSpacing;
            if (columnWidth > 0)
            {
                offset = Math.Round(offset / columnWidth) * columnWidth;
                offset = Math.Max(0, Math.Min(offset, ExtentWidth - ViewportWidth));
            }
        }
        
        if (offset != _offset.X)
        {
            _offset.X = offset;
            InvalidateMeasure();
            _scrollOwner?.InvalidateScrollInfo();
        }
    }

    /// <inheritdoc/>
    public void SetVerticalOffset(double offset)
    {
        offset = Math.Max(0, Math.Min(offset, ExtentHeight - ViewportHeight));
        
        // Snap to item boundaries when using item-based scrolling
        if (ScrollUnit == ScrollUnit.Item && Orientation == Orientation.Horizontal)
        {
            var effectiveSize = StretchItems ? _stretchedItemSize : _itemSize;
            var rowHeight = effectiveSize.Height + VerticalSpacing;
            if (rowHeight > 0)
            {
                offset = Math.Round(offset / rowHeight) * rowHeight;
                offset = Math.Max(0, Math.Min(offset, ExtentHeight - ViewportHeight));
            }
        }
        
        if (offset != _offset.Y)
        {
            _offset.Y = offset;
            InvalidateMeasure();
            _scrollOwner?.InvalidateScrollInfo();
        }
    }

    /// <inheritdoc/>
    public Rect MakeVisible(Visual visual, Rect rectangle)
    {
        if (visual is UIElement element)
        {
            var index = InternalChildren.IndexOf(element);
            if (index >= 0)
            {
                // Find the actual item index
                var generator = ItemContainerGenerator;
                var itemIndex = generator.IndexFromGeneratorPosition(new GeneratorPosition(index, 0));
                if (itemIndex >= 0)
                {
                    var itemRect = GetItemRect(itemIndex);

                    if (Orientation == Orientation.Horizontal)
                    {
                        // Vertical scrolling
                        if (itemRect.Bottom > VerticalOffset + ViewportHeight)
                        {
                            SetVerticalOffset(itemRect.Bottom - ViewportHeight);
                        }
                        if (itemRect.Top < VerticalOffset)
                        {
                            SetVerticalOffset(itemRect.Top);
                        }
                    }
                    else
                    {
                        // Horizontal scrolling
                        if (itemRect.Right > HorizontalOffset + ViewportWidth)
                        {
                            SetHorizontalOffset(itemRect.Right - ViewportWidth);
                        }
                        if (itemRect.Left < HorizontalOffset)
                        {
                            SetHorizontalOffset(itemRect.Left);
                        }
                    }

                    return itemRect;
                }
            }
        }
        return Rect.Empty;
    }

    private double GetLineScrollAmount()
    {
        if (ScrollUnit == ScrollUnit.Pixel)
        {
            // Pixel scrolling: use a fixed pixel amount for smooth scrolling
            const double PixelScrollAmount = 16.0;
            return PixelScrollAmount;
        }
        
        // Item scrolling: scroll by one row/column
        return Orientation == Orientation.Horizontal
            ? _itemSize.Height + VerticalSpacing
            : _itemSize.Width + HorizontalSpacing;
    }

    private double GetMouseWheelScrollAmount()
    {
        if (ScrollUnit == ScrollUnit.Pixel)
        {
            // Pixel scrolling: use system wheel scroll lines with a pixel multiplier
            const double PixelPerLine = 16.0;
            return PixelPerLine * SystemParameters.WheelScrollLines;
        }
        
        // Item scrolling: scroll by system wheel lines worth of rows/columns
        return GetLineScrollAmount() * SystemParameters.WheelScrollLines;
    }

    private double GetPageScrollAmount(bool isVertical)
    {
        if (ScrollUnit == ScrollUnit.Pixel)
        {
            // Pixel scrolling: use viewport size
            return isVertical ? ViewportHeight : ViewportWidth;
        }
        
        // Item scrolling: scroll by whole rows/columns that fit in viewport
        var effectiveSize = StretchItems ? _stretchedItemSize : _itemSize;
        if (isVertical)
        {
            var rowHeight = effectiveSize.Height + VerticalSpacing;
            var rowsPerPage = Math.Max(1, (int)(ViewportHeight / rowHeight));
            return rowsPerPage * rowHeight;
        }
        else
        {
            var columnWidth = effectiveSize.Width + HorizontalSpacing;
            var columnsPerPage = Math.Max(1, (int)(ViewportWidth / columnWidth));
            return columnsPerPage * columnWidth;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Scrolls the panel to bring the item at the specified index into view.
    /// Use this method to scroll to virtualized items that don't have containers yet.
    /// </summary>
    /// <param name="index">The index of the item to bring into view.</param>
    public new void BringIndexIntoView(int index)
    {
        BringIndexIntoViewInternal(index);
    }

    #endregion

    #region Layout Overrides

    private void BringIndexIntoViewInternal(int index)
    {
        // Ensure layout metrics are calculated before scrolling
        var itemCount = GetItemCount();
        if (index < 0 || index >= itemCount)
            return;

        // Force a measure pass if we haven't calculated layout yet
        if (_itemsPerRow <= 0 || (_viewport.Width == 0 && _viewport.Height == 0))
        {
            // Can't scroll yet, need to wait for layout
            // Queue a deferred scroll after measure
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                BringIndexIntoViewInternal(index);
            }));
            return;
        }

        var itemRect = GetItemRect(index);

        if (Orientation == Orientation.Horizontal)
        {
            // Vertical scrolling
            if (itemRect.Bottom > VerticalOffset + ViewportHeight)
            {
                SetVerticalOffset(itemRect.Bottom - ViewportHeight);
            }
            else if (itemRect.Top < VerticalOffset)
            {
                SetVerticalOffset(itemRect.Top);
            }
        }
        else
        {
            // Horizontal scrolling
            if (itemRect.Right > HorizontalOffset + ViewportWidth)
            {
                SetHorizontalOffset(itemRect.Right - ViewportWidth);
            }
            else if (itemRect.Left < HorizontalOffset)
            {
                SetHorizontalOffset(itemRect.Left);
            }
        }
    }




    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        var itemCount = GetItemCount();
        if (itemCount == 0)
        {
            UpdateScrollInfo(availableSize, new Size(0, 0));
            return new Size(0, 0);
        }

        // Determine item size
        CalculateItemSize(availableSize);

        // Calculate layout metrics
        CalculateLayoutMetrics(availableSize, itemCount);

        // Calculate which items are visible
        CalculateVisibleRange(itemCount);

        // Generate and measure visible items
        RealizeItems();

        // Calculate extent
        var extent = CalculateExtent(itemCount);

        // Update scroll info
        UpdateScrollInfo(availableSize, extent);

        return new Size(
            double.IsInfinity(availableSize.Width) ? extent.Width : availableSize.Width,
            double.IsInfinity(availableSize.Height) ? extent.Height : availableSize.Height);
    }


    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        var generator = ItemContainerGenerator;
        if (generator == null)
            return finalSize;

        for (int i = 0; i < InternalChildren.Count; i++)
        {
            var child = InternalChildren[i];
            var itemIndex = generator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));

            if (itemIndex >= 0)
            {
                var itemRect = GetItemRect(itemIndex);

                // Adjust for scroll offset
                if (Orientation == Orientation.Horizontal)
                {
                    itemRect.Y -= VerticalOffset;
                }
                else
                {
                    itemRect.X -= HorizontalOffset;
                }

                child.Arrange(itemRect);
            }
        }

        return finalSize;
    }

    /// <inheritdoc/>
    protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
    {
        switch (args.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
            case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
            case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                // Remove all generated containers and re-measure
                RemoveInternalChildRange(0, InternalChildren.Count);
                break;
        }

        InvalidateMeasure();
        base.OnItemsChanged(sender, args);
    }

    #endregion

    #region Private Methods - Layout Calculations

    private int GetItemCount()
    {
        var itemsControl = ItemsControl.GetItemsOwner(this);
        return itemsControl?.Items.Count ?? 0;
    }

    private void CalculateItemSize(Size availableSize)
    {
        var explicitWidth = ItemWidth;
        var explicitHeight = ItemHeight;

        if (!double.IsNaN(explicitWidth) && !double.IsNaN(explicitHeight))
        {
            _itemSize = new Size(explicitWidth, explicitHeight);
            return;
        }

        // Try to get size from first item if already realized
        if (InternalChildren.Count > 0)
        {
            var firstChild = InternalChildren[0];
            firstChild.Measure(new Size(
                double.IsNaN(explicitWidth) ? double.PositiveInfinity : explicitWidth,
                double.IsNaN(explicitHeight) ? double.PositiveInfinity : explicitHeight));

            _itemSize = new Size(
                double.IsNaN(explicitWidth) ? firstChild.DesiredSize.Width : explicitWidth,
                double.IsNaN(explicitHeight) ? firstChild.DesiredSize.Height : explicitHeight);
            return;
        }

        // Need to realize first item to get size
        var generator = ItemContainerGenerator;
        if (generator == null)
            return;

        var startPos = generator.GeneratorPositionFromIndex(0);

        using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
        {
            var child = generator.GenerateNext(out bool isNewlyRealized) as UIElement;
            if (child != null)
            {
                if (isNewlyRealized)
                {
                    AddInternalChild(child);
                    generator.PrepareItemContainer(child);
                }

                child.Measure(new Size(
                    double.IsNaN(explicitWidth) ? double.PositiveInfinity : explicitWidth,
                    double.IsNaN(explicitHeight) ? double.PositiveInfinity : explicitHeight));

                _itemSize = new Size(
                    double.IsNaN(explicitWidth) ? child.DesiredSize.Width : explicitWidth,
                    double.IsNaN(explicitHeight) ? child.DesiredSize.Height : explicitHeight);
            }
        }

        // Ensure minimum size
        if (_itemSize.Width <= 0) _itemSize.Width = 100;
        if (_itemSize.Height <= 0) _itemSize.Height = 100;
    }

    private void CalculateLayoutMetrics(Size availableSize, int itemCount)
    {
        if (Orientation == Orientation.Horizontal)
        {
            var availableWidth = double.IsInfinity(availableSize.Width) ? double.MaxValue : availableSize.Width;
            var itemWidthWithSpacing = _itemSize.Width + HorizontalSpacing;
            _itemsPerRow = Math.Max(1, (int)((availableWidth + HorizontalSpacing) / itemWidthWithSpacing));

            // Calculate stretched item size
            if (StretchItems && !double.IsInfinity(availableSize.Width))
            {
                var totalSpacing = (_itemsPerRow - 1) * HorizontalSpacing;
                var stretchedWidth = (availableWidth - totalSpacing) / _itemsPerRow;
                _stretchedItemSize = new Size(Math.Max(_itemSize.Width, stretchedWidth), _itemSize.Height);
            }
            else
            {
                _stretchedItemSize = _itemSize;
            }
        }
        else
        {
            var availableHeight = double.IsInfinity(availableSize.Height) ? double.MaxValue : availableSize.Height;
            var itemHeightWithSpacing = _itemSize.Height + VerticalSpacing;
            _itemsPerRow = Math.Max(1, (int)((availableHeight + VerticalSpacing) / itemHeightWithSpacing));

            // Calculate stretched item size
            if (StretchItems && !double.IsInfinity(availableSize.Height))
            {
                var totalSpacing = (_itemsPerRow - 1) * VerticalSpacing;
                var stretchedHeight = (availableHeight - totalSpacing) / _itemsPerRow;
                _stretchedItemSize = new Size(_itemSize.Width, Math.Max(_itemSize.Height, stretchedHeight));
            }
            else
            {
                _stretchedItemSize = _itemSize;
            }
        }
    }

    private void CalculateVisibleRange(int itemCount)
    {
        var cacheLength = CacheLength;

        // Use stretched size for layout calculations
        var effectiveSize = StretchItems ? _stretchedItemSize : _itemSize;


        if (Orientation == Orientation.Horizontal)
        {
            // Vertical scrolling
            var rowHeight = effectiveSize.Height + VerticalSpacing;
            var firstVisibleRow = Math.Max(0, (int)(VerticalOffset / rowHeight) - cacheLength);
            var visibleRows = (int)Math.Ceiling((_viewport.Height + rowHeight) / rowHeight) + 1 + (cacheLength * 2);

            _firstVisibleIndex = Math.Max(0, firstVisibleRow * _itemsPerRow);
            _lastVisibleIndex = Math.Min(itemCount - 1, _firstVisibleIndex + (visibleRows * _itemsPerRow) - 1);
        }
        else
        {
            // Horizontal scrolling
            var columnWidth = effectiveSize.Width + HorizontalSpacing;
            var firstVisibleColumn = Math.Max(0, (int)(HorizontalOffset / columnWidth) - cacheLength);
            var visibleColumns = (int)Math.Ceiling((_viewport.Width + columnWidth) / columnWidth) + 1 + (cacheLength * 2);

            _firstVisibleIndex = Math.Max(0, firstVisibleColumn * _itemsPerRow);
            _lastVisibleIndex = Math.Min(itemCount - 1, _firstVisibleIndex + (visibleColumns * _itemsPerRow) - 1);
        }
    }

    private void RealizeItems()
    {
        var generator = ItemContainerGenerator;

        // Generator can be null before the panel is connected to an ItemsControl,
        // during template application, or after disconnection from the visual tree.
        // Schedule a deferred measure to retry once the generator becomes available.
        if (generator == null)
        {
            ScheduleDeferredMeasure();
            return;
        }

        // Clean up items that are no longer visible
        CleanupItems();

        if (_firstVisibleIndex > _lastVisibleIndex)
            return;


        // Use stretched size for measuring when StretchItems is enabled
        var measureSize = StretchItems ? _stretchedItemSize : _itemSize;

        // Generate visible items
        var startPos = generator.GeneratorPositionFromIndex(_firstVisibleIndex);
        var childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;

        using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
        {
            for (int itemIndex = _firstVisibleIndex; itemIndex <= _lastVisibleIndex; itemIndex++)
            {
                var child = generator.GenerateNext(out bool isNewlyRealized) as UIElement;
                if (child == null) continue;

                if (isNewlyRealized)
                {
                    if (childIndex >= InternalChildren.Count)
                    {
                        AddInternalChild(child);
                    }
                    else
                    {
                        InsertInternalChild(childIndex, child);
                    }
                    generator.PrepareItemContainer(child);
                }

                child.Measure(new Size(measureSize.Width, measureSize.Height));
                childIndex++;
            }
        }
    }

    private void CleanupItems()
    {
        var generator = ItemContainerGenerator;
        if (generator == null)
            return;

        for (int i = InternalChildren.Count - 1; i >= 0; i--)
        {
            var pos = new GeneratorPosition(i, 0);
            var itemIndex = generator.IndexFromGeneratorPosition(pos);

            if (itemIndex < _firstVisibleIndex || itemIndex > _lastVisibleIndex)
            {
                generator.Remove(pos, 1);
                RemoveInternalChildRange(i, 1);
            }
        }
    }

    private Size CalculateExtent(int itemCount)
    {
        if (itemCount == 0)
            return new Size(0, 0);

        // Use stretched size when StretchItems is enabled
        var effectiveSize = StretchItems ? _stretchedItemSize : _itemSize;

        if (Orientation == Orientation.Horizontal)
        {
            var rows = (int)Math.Ceiling((double)itemCount / _itemsPerRow);
            var width = (_itemsPerRow * effectiveSize.Width) + ((_itemsPerRow - 1) * HorizontalSpacing);
            var height = (rows * effectiveSize.Height) + ((rows - 1) * VerticalSpacing);
            return new Size(width, height);
        }
        else
        {
            var columns = (int)Math.Ceiling((double)itemCount / _itemsPerRow);
            var width = (columns * effectiveSize.Width) + ((columns - 1) * HorizontalSpacing);
            var height = (_itemsPerRow * effectiveSize.Height) + ((_itemsPerRow - 1) * VerticalSpacing);
            return new Size(width, height);
        }
    }

    private Rect GetItemRect(int itemIndex)
    {
        // Use stretched size when StretchItems is enabled
        var effectiveSize = StretchItems ? _stretchedItemSize : _itemSize;

        if (Orientation == Orientation.Horizontal)
        {
            var row = itemIndex / _itemsPerRow;
            var column = itemIndex % _itemsPerRow;

            var x = column * (effectiveSize.Width + HorizontalSpacing);
            var y = row * (effectiveSize.Height + VerticalSpacing);

            return new Rect(x, y, effectiveSize.Width, effectiveSize.Height);
        }
        else
        {
            var column = itemIndex / _itemsPerRow;
            var row = itemIndex % _itemsPerRow;

            var x = column * (effectiveSize.Width + HorizontalSpacing);
            var y = row * (effectiveSize.Height + VerticalSpacing);

            return new Rect(x, y, effectiveSize.Width, effectiveSize.Height);
        }
    }

    private void UpdateScrollInfo(Size viewport, Size extent)
    {
        var changed = false;

        if (_viewport != viewport)
        {
            _viewport = viewport;
            changed = true;
        }

        if (_extent != extent)
        {
            _extent = extent;
            changed = true;
        }

        // Clamp offset
        var newOffsetX = Math.Max(0, Math.Min(_offset.X, ExtentWidth - ViewportWidth));
        var newOffsetY = Math.Max(0, Math.Min(_offset.Y, ExtentHeight - ViewportHeight));

        if (newOffsetX != _offset.X || newOffsetY != _offset.Y)
        {
            _offset = new Point(newOffsetX, newOffsetY);
            changed = true;
        }


        if (changed)
        {
            _scrollOwner?.InvalidateScrollInfo();
        }
    }

    private void ScheduleDeferredMeasure()
    {
        if (_hasPendingMeasure)
            return;

        _hasPendingMeasure = true;

        // Schedule a deferred measure after the current layout pass completes
        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
        {
            _hasPendingMeasure = false;
            InvalidateMeasure();
        }));
    }

    #endregion
}
