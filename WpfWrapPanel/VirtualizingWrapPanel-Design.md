# VirtualizingWrapPanel Design Document

## Overview
A VirtualizingWrapPanel is a WPF panel that combines the wrapping behavior of WrapPanel with UI virtualization capabilities. It only creates and maintains UI elements for items that are currently visible in the viewport, significantly improving performance when displaying large collections.

## Core Requirements

### 1. Panel Base Class
- **Inherit from**: `VirtualizingPanel` (not `WrapPanel`)
  - WrapPanel doesn't support virtualization infrastructure
  - VirtualizingPanel provides the necessary hooks for ItemsControl virtualization

### 2. Orientation Support
- **Horizontal Orientation** (default)
  - Items flow left-to-right, wrapping to next row
  - Scrolling is primarily vertical
- **Vertical Orientation**
  - Items flow top-to-bottom, wrapping to next column
  - Scrolling is primarily horizontal

### 3. Item Sizing Strategies

#### 3.1 Uniform Item Size (Recommended for Performance)
- All items assumed to be the same size
- Size determined from first realized item or explicitly specified
- Properties:
  - `ItemWidth` (double, optional)
  - `ItemHeight` (double, optional)
  - Auto-calculation if not specified

#### 3.2 Variable Item Size (Advanced)
- Each item can have different dimensions
- Requires caching item sizes
- More complex virtualization logic
- Performance considerations for large datasets

**Decision**: Start with uniform sizing, add variable sizing in future iteration if needed

### 4. Virtualization Features

#### 4.1 UI Virtualization
- Only create containers for visible items
- Implement `MeasureOverride` and `ArrangeOverride`
- Calculate which items are in viewport
- Generate containers only for visible items

#### 4.2 Container Recycling
- Use `IItemContainerGenerator` to recycle containers
- Implement proper generator lifecycle:
  - `StartAt()` - Begin generation
  - `GenerateNext()` - Generate next container
  - `Remove()` - Remove containers
  - `GeneratorPositionFromIndex()` - Convert index to position

#### 4.3 Viewport Management
- Track viewport offset and size
- Integrate with `IScrollInfo` interface
- Calculate visible item range based on scroll position

### 5. Scrolling Integration

#### 5.1 IScrollInfo Implementation
Required properties and methods:
- **Properties**:
  - `CanHorizontallyScroll` (get/set)
  - `CanVerticallyScroll` (get/set)
  - `ExtentWidth` (get) - Total content width
  - `ExtentHeight` (get) - Total content height
  - `ViewportWidth` (get) - Visible width
  - `ViewportHeight` (get) - Visible height
  - `HorizontalOffset` (get) - Current horizontal scroll position
  - `VerticalOffset` (get) - Current vertical scroll position
  - `ScrollOwner` (get/set) - ScrollViewer owner

- **Methods**:
  - `LineUp()`, `LineDown()`, `LineLeft()`, `LineRight()`
  - `PageUp()`, `PageDown()`, `PageLeft()`, `PageRight()`
  - `MouseWheelUp()`, `MouseWheelDown()`, `MouseWheelLeft()`, `MouseWheelRight()`
  - `SetHorizontalOffset(double)`, `SetVerticalOffset(double)`
  - `MakeVisible(Visual, Rect)` - Scroll item into view

#### 5.2 Scrolling Modes
- **Pixel-based scrolling**: Smooth scrolling by pixels
- **Item-based scrolling**: Snap to item boundaries
- Property: `ScrollUnit` (Pixel/Item)

### 6. Layout Algorithm

#### 6.1 Measure Phase
1. Determine available space from constraint
2. Calculate item size (uniform or from first item)
3. Calculate items per row/column based on orientation
4. Calculate viewport boundaries
5. Determine visible item range (first index, last index)
6. Generate containers for visible items only
7. Measure each visible container
8. Calculate total extent (all items, including non-visible)
9. Return desired size

#### 6.2 Arrange Phase
1. Calculate starting offset based on scroll position
2. For each realized container:
   - Calculate wrap position (row/column index)
   - Calculate exact position within viewport
   - Arrange container at calculated position
3. Update scroll information

### 7. Additional Features

#### 7.1 Spacing Support
- `HorizontalSpacing` - Space between items horizontally
- `VerticalSpacing` - Space between items vertically
- Apply to layout calculations

#### 7.2 Stretch Behavior
- `StretchItems` - Whether items should stretch to fill available space
- Affects item width in horizontal orientation
- Affects item height in vertical orientation

#### 7.3 Alignment
- Leverage existing WPF alignment properties
- `HorizontalAlignment`
- `VerticalAlignment`

### 8. Performance Optimizations

#### 8.1 Extent Caching
- Cache calculated extent values
- Invalidate on:
  - Item count changes
  - Item size changes
  - Orientation changes
  - Viewport size changes

#### 8.2 Lazy Realization
- Don't realize items until absolutely necessary
- Use buffer zone for smoother scrolling
- Generate 1-2 items beyond viewport edges

#### 8.3 Cleanup of Unrealized Items
- Remove containers that scroll out of view
- Balance between memory and performance
- Implement cleanup threshold

### 9. Event Handling

#### 9.1 Collection Changes
- Subscribe to `ItemsChanged` event from generator
- Handle Add, Remove, Replace, Reset actions
- Invalidate measure when collection changes
- Clean up containers for removed items

#### 9.2 Scroll Changes
- Respond to scroll offset changes
- Efficiently update visible range
- Minimize container regeneration

### 10. Dependency Properties

Required custom dependency properties:
- `Orientation` (Horizontal/Vertical)
- `ItemWidth` (double?, nullable for auto)
- `ItemHeight` (double?, nullable for auto)
- `HorizontalSpacing` (double, default 0)
- `VerticalSpacing` (double, default 0)
- `ScrollUnit` (Pixel/Item enumeration)
- `StretchItems` (bool, default false)

### 11. Edge Cases and Error Handling

#### 11.1 Empty Collection
- Handle zero items gracefully
- Return zero extent

#### 11.2 Single Item
- Don't break on single item scenarios
- Calculate size correctly

#### 11.3 Very Large Numbers
- Handle collections with millions of items
- Ensure calculations don't overflow
- Test with extreme scroll positions

#### 11.4 Dynamic Size Changes
- Respond to item size changes
- Invalidate and remeasure when needed
- Property change callbacks

#### 11.5 Rapid Scrolling
- Debounce if necessary
- Ensure smooth performance
- Container generation efficiency

### 12. Integration Requirements

#### 12.1 ItemsControl Integration
- Works with `ItemsControl`, `ListBox`, `ListView`
- Set as `ItemsPanel`:
  ```xaml
  <ListBox>
      <ListBox.ItemsPanel>
          <ItemsPanelTemplate>
              <local:VirtualizingWrapPanel/>
          </ItemsPanelTemplate>
      </ListBox.ItemsPanel>
  </ListBox>
  ```

#### 12.2 VirtualizingPanel Requirements
- Enable virtualization on ItemsControl:
  - `VirtualizingPanel.IsVirtualizing="True"`
  - `VirtualizingPanel.VirtualizationMode="Recycling"` (for container recycling)
- ScrollViewer integration required

### 13. Testing Strategy

#### 13.1 Unit Tests
- Layout calculations with various item counts
- Viewport calculation accuracy
- Index range calculations
- Orientation switching

#### 13.2 Integration Tests
- Scrolling behavior with ScrollViewer
- Container generation/recycling
- Collection change handling
- Performance with large datasets (10,000+ items)

#### 13.3 Visual Tests
- Correct visual layout
- No visual artifacts during scrolling
- Proper spacing and alignment
- Both orientations

### 14. Implementation Phases

#### Phase 1: Core Implementation (MVP) ✅ COMPLETED
1. ✅ Basic VirtualizingPanel inheritance
2. ✅ IScrollInfo implementation
3. ✅ Simple horizontal orientation with uniform sizing
4. ✅ Basic virtualization (visible items only)
5. ✅ ItemsControl integration

#### Phase 2: Enhanced Features ✅ COMPLETED
1. ✅ Vertical orientation support
2. ✅ Item and pixel scrolling modes
3. ✅ Spacing properties (HorizontalSpacing, VerticalSpacing)
4. ✅ Container recycling optimization
5. ✅ Better extent caching
6. ✅ CacheLength property for buffer zones

#### Phase 3: Advanced Features ✅ COMPLETED
1. ✅ Stretch behavior (StretchItems property)
2. ✅ Performance optimizations (buffer zones - implemented via CacheLength)
3. Variable item sizing support (deferred - uniform sizing sufficient for most use cases)
4. ✅ Comprehensive testing

#### Phase 4: Polish (IN PROGRESS)
1. Accessibility support
2. Keyboard navigation
3. ✅ Documentation and samples (basic)
4. Performance profiling and optimization

### 15. Known Limitations and Future Enhancements

#### Limitations
- Initial implementation: uniform item sizes only
- May not support grouped items initially
- Hierarchical virtualization not in scope

#### Future Enhancements
- Group header support
- Hierarchical data virtualization
- Animated layout transitions
- Touch/gesture optimizations
- Variable item size with size caching

### 16. Dependencies and Compatibility

#### Framework Requirements
- .NET Framework 4.6.2+ or .NET Core 3.1+ / .NET 5+
- WPF framework assemblies

#### Required Namespaces
- `System.Windows`
- `System.Windows.Controls`
- `System.Windows.Controls.Primitives`
- `System.Windows.Media`

### 17. API Design Example

```csharp
public class VirtualizingWrapPanel : VirtualizingPanel, IScrollInfo
{
    // Dependency Properties
    public static readonly DependencyProperty OrientationProperty;
    public static readonly DependencyProperty ItemWidthProperty;
    public static readonly DependencyProperty ItemHeightProperty;
    public static readonly DependencyProperty HorizontalSpacingProperty;
    public static readonly DependencyProperty VerticalSpacingProperty;
    
    // Properties
    public Orientation Orientation { get; set; }
    public double ItemWidth { get; set; }
    public double ItemHeight { get; set; }
    public double HorizontalSpacing { get; set; }
    public double VerticalSpacing { get; set; }
    
    // IScrollInfo properties
    public bool CanHorizontallyScroll { get; set; }
    public bool CanVerticallyScroll { get; set; }
    public double ExtentWidth { get; }
    public double ExtentHeight { get; }
    public double ViewportWidth { get; }
    public double ViewportHeight { get; }
    public double HorizontalOffset { get; }
    public double VerticalOffset { get; }
    public ScrollViewer ScrollOwner { get; set; }
    
    // Core overrides
    protected override Size MeasureOverride(Size availableSize);
    protected override Size ArrangeOverride(Size finalSize);
    
    // IScrollInfo methods
    public void LineUp();
    public void LineDown();
    // ... other scroll methods
    public void SetHorizontalOffset(double offset);
    public void SetVerticalOffset(double offset);
    public Rect MakeVisible(Visual visual, Rect rectangle);
}
```

## Summary

This design provides a robust foundation for a production-ready VirtualizingWrapPanel. The phased approach allows for incremental development while ensuring core functionality works correctly before adding advanced features. The focus on uniform item sizing in the initial implementation balances performance with complexity, while leaving room for future enhancement.
