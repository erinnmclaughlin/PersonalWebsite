using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace PersonalWebsite.Components;

public sealed partial class CascadingBlogContent : IDisposable
{
    private IDisposable? _changeListener;

    [Inject]
    private IOptionsMonitor<Dictionary<string, BlogEntryMeta>> BlogsMonitor { get; set; } = default!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    public void Dispose()
    {
        _changeListener?.Dispose();
    }

    protected override void OnInitialized()
    {
        _changeListener = BlogsMonitor.OnChange(context =>StateHasChanged());
    }
}
