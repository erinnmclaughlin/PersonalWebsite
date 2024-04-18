using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Site.AI;
using System.Diagnostics.CodeAnalysis;

namespace Site.Components;

public sealed partial class Home
{
    [Parameter, SupplyParameterFromQuery]
    public bool Preview { get; set; }

    [Inject, NotNull]
    private IOptionsMonitor<AIOptions>? AIOptionsMonitor { get; set; }

    private bool ShowAibbaChat { get; set; }

    protected override void OnParametersSet()
    {
        ShowAibbaChat = Preview && AIOptionsMonitor.CurrentValue.Enabled;
    }
}
