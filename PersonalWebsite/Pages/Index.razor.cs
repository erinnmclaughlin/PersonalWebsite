using Microsoft.AspNetCore.Components;

namespace PersonalWebsite.Pages;

public sealed partial class Index
{
    private Dictionary<DateTime, string>? _blogs;

    [Inject]
    private BlogsClient BlogsClient { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        _blogs = await BlogsClient.GetBlogEntriesAsync();
    }
}