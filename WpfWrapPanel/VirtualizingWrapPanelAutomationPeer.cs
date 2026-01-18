using System.Windows.Automation.Peers;

namespace WpfWrapPanel;

/// <summary>
/// AutomationPeer for VirtualizingWrapPanel to support UI Automation and accessibility.
/// </summary>
public class VirtualizingWrapPanelAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualizingWrapPanelAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The VirtualizingWrapPanel that this peer represents.</param>
    public VirtualizingWrapPanelAutomationPeer(VirtualizingWrapPanel owner) : base(owner)
    {
    }

    /// <summary>
    /// Gets the VirtualizingWrapPanel that this peer represents.
    /// </summary>
    private VirtualizingWrapPanel OwnerPanel => (VirtualizingWrapPanel)Owner;

    /// <inheritdoc/>
    protected override string GetClassNameCore() => nameof(VirtualizingWrapPanel);

    /// <inheritdoc/>
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.List;

    /// <inheritdoc/>
    protected override bool IsContentElementCore() => false;

    /// <inheritdoc/>
    protected override bool IsControlElementCore() => true;
}
