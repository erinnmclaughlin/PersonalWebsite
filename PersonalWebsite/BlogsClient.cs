using Markdig;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace PersonalWebsite;

public sealed class BlogsClient
{
    private readonly HttpClient _httpClient;

    public BlogsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Dictionary<DateTime, string>> GetBlogEntriesAsync(CancellationToken cancellationToken = default)
    {
        var entries = await _httpClient.GetFromJsonAsync<Dictionary<string, string>>("blog-data.json", cancellationToken);

        if (entries is null)
            return new();

        return entries
            .Select(e => new
            {
                Date = DateTime.Parse(e.Key),
                e.Value
            })
            .OrderByDescending(e => e.Date)
            .ToDictionary(x => x.Date, x => x.Value);
    }

    public async Task<MarkupString> GetBlogContent(string title, CancellationToken cancellationToken = default)
    {
        var content = await _httpClient.GetStringAsync($"blog/{title}.md", cancellationToken);

        return (MarkupString)Markdown.ToHtml(content);
    }
}
