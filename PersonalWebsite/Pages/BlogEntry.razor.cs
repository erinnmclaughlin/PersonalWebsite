using Microsoft.AspNetCore.Components;

namespace PersonalWebsite.Pages;
public partial class BlogEntry
{

    private MarkupString? _content;

    [Inject]
    private HttpClient HttpClient { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [CascadingParameter]
    public required Dictionary<string, BlogEntryMeta> BlogEntries { get; set; }

    [Parameter, SupplyParameterFromQuery]
    public required string Slug { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var content = await HttpClient.GetStringAsync(BlogEntries[Slug].Path);
            _content = (MarkupString)Markdig.Markdown.ToHtml(content);
        }
        catch
        {
            NavigationManager.NavigateTo("/");
        }
    }
}