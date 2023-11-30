using Microsoft.AspNetCore.Components;

namespace PersonalWebsite.Components;

public sealed partial class Card
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? UserAttributes { get; set; }
}