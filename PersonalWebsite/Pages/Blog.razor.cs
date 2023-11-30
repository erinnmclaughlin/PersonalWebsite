using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace PersonalWebsite.Pages;

public partial class Blog
{
    private Dictionary<string, string>? _files;

    [Inject]
    private HttpClient HttpClient { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        _files = await HttpClient.GetFromJsonAsync<Dictionary<string, string>>("blog-data.json");
    }
}