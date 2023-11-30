using Microsoft.AspNetCore.Components;

namespace PersonalWebsite.Layout;

public sealed partial class PageHeader
{
    [Parameter]
    public PageHeaderStyle Style { get; set; } = PageHeaderStyle.Full;
}
