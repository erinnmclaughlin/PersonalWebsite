using Microsoft.AspNetCore.Components;

namespace PersonalWebsite.Components;

public sealed partial class Card
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}