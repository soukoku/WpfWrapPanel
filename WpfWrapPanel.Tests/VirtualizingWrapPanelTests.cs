using System.Windows;
using System.Windows.Controls;
using WpfWrapPanel;
using Xunit;

namespace WpfWrapPanel.Tests;

public class VirtualizingWrapPanelTests
{
    #region Dependency Property Tests

    [Fact]
    public void Orientation_DefaultValue_IsHorizontal()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.Equal(Orientation.Horizontal, panel.Orientation);
        });
    }

    [Fact]
    public void ItemWidth_DefaultValue_IsNaN()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.True(double.IsNaN(panel.ItemWidth));
        });
    }

    [Fact]
    public void ItemHeight_DefaultValue_IsNaN()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.True(double.IsNaN(panel.ItemHeight));
        });
    }

    [Fact]
    public void HorizontalSpacing_DefaultValue_IsZero()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.Equal(0.0, panel.HorizontalSpacing);
        });
    }

    [Fact]
    public void VerticalSpacing_DefaultValue_IsZero()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.Equal(0.0, panel.VerticalSpacing);
        });
    }

    [Fact]
    public void Orientation_CanBeSet()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            panel.Orientation = Orientation.Vertical;
            Assert.Equal(Orientation.Vertical, panel.Orientation);
        });
    }

    [Fact]
    public void ItemWidth_CanBeSet()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            panel.ItemWidth = 150;
            Assert.Equal(150, panel.ItemWidth);
        });
    }

    [Fact]
    public void ItemHeight_CanBeSet()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            panel.ItemHeight = 200;
            Assert.Equal(200, panel.ItemHeight);
        });
    }

    [Fact]
    public void HorizontalSpacing_CanBeSet()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            panel.HorizontalSpacing = 10;
            Assert.Equal(10, panel.HorizontalSpacing);
        });
    }

    [Fact]
    public void VerticalSpacing_CanBeSet()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            panel.VerticalSpacing = 15;
            Assert.Equal(15, panel.VerticalSpacing);
        });
    }

    [Fact]
    public void CacheLength_DefaultValue_IsTwo()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.Equal(2, panel.CacheLength);
        });
    }

    [Fact]
    public void CacheLength_CanBeSet()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            panel.CacheLength = 5;
            Assert.Equal(5, panel.CacheLength);
        });
    }

    [Fact]
    public void StretchItems_DefaultValue_IsFalse()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.False(panel.StretchItems);
        });
    }

    [Fact]
    public void StretchItems_CanBeSet()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            panel.StretchItems = true;
            Assert.True(panel.StretchItems);
        });
    }

    [Fact]
    public void ScrollUnit_DefaultValue_IsPixel()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.Equal(ScrollUnit.Pixel, panel.ScrollUnit);
        });
    }

    [Fact]
    public void ScrollUnit_CanBeSet()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            panel.ScrollUnit = ScrollUnit.Item;
            Assert.Equal(ScrollUnit.Item, panel.ScrollUnit);
        });
    }

    [Theory]
    [InlineData(ScrollUnit.Pixel)]
    [InlineData(ScrollUnit.Item)]
    public void ScrollUnit_AffectsScrollBehavior(ScrollUnit scrollUnit)
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            panel.ScrollUnit = scrollUnit;
            // Verify the property is set correctly
            Assert.Equal(scrollUnit, panel.ScrollUnit);
        });
    }

    #endregion

    #region IScrollInfo Tests

    [Fact]
    public void CanHorizontallyScroll_DefaultValue_IsFalse()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.False(panel.CanHorizontallyScroll);
        });
    }

    [Fact]
    public void CanVerticallyScroll_DefaultValue_IsFalse()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.False(panel.CanVerticallyScroll);
        });
    }

    [Fact]
    public void ExtentWidth_InitialValue_IsZero()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.Equal(0, panel.ExtentWidth);
        });
    }

    [Fact]
    public void ExtentHeight_InitialValue_IsZero()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.Equal(0, panel.ExtentHeight);
        });
    }

    [Fact]
    public void ViewportWidth_InitialValue_IsZero()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.Equal(0, panel.ViewportWidth);
        });
    }

    [Fact]
    public void ViewportHeight_InitialValue_IsZero()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.Equal(0, panel.ViewportHeight);
        });
    }

    [Fact]
    public void HorizontalOffset_InitialValue_IsZero()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.Equal(0, panel.HorizontalOffset);
        });
    }

    [Fact]
    public void VerticalOffset_InitialValue_IsZero()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.Equal(0, panel.VerticalOffset);
        });
    }

    [Fact]
    public void ScrollOwner_InitialValue_IsNull()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            Assert.Null(panel.ScrollOwner);
        });
    }

    [Fact]
    public void SetHorizontalOffset_ClampsToZero_WhenNegative()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            panel.SetHorizontalOffset(-100);
            Assert.Equal(0, panel.HorizontalOffset);
        });
    }

    [Fact]
    public void SetVerticalOffset_ClampsToZero_WhenNegative()
    {
        RunOnSTAThread(() =>
        {
            var panel = new VirtualizingWrapPanel();
            panel.SetVerticalOffset(-100);
            Assert.Equal(0, panel.VerticalOffset);
        });
    }

    #endregion

    #region Helper Methods

    private static void RunOnSTAThread(Action action)
    {
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (exception != null)
        {
            throw new Exception("Test failed on STA thread", exception);
        }
    }

    #endregion
}
