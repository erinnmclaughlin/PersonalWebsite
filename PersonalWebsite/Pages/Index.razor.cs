using Microsoft.AspNetCore.Components;

namespace PersonalWebsite.Pages;

public sealed partial class Index
{
    [CascadingParameter]
    public required Dictionary<string, BlogEntryMeta> BlogEntries { get; set; }
}