using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace PersonalWebsite.Storage;

public sealed class LocalStorage : IStorageProvider
{
    private readonly JsonOptions _jsonOptions;
    private readonly IJSRuntime _jsRuntime;

    public LocalStorage(IOptions<JsonOptions> jsonOptions, IJSRuntime jsRuntime)
    {
        _jsonOptions = jsonOptions.Value;
        _jsRuntime = jsRuntime;
    }

    public async ValueTask<bool> ContainsKey(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await Get<string?>(key, cancellationToken) is not null;
        }
        catch
        {
            return false;
        }
    }

    public async ValueTask Set<T>(string key, T? value, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(value);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", cancellationToken, key, json);
    }

    public async ValueTask<T> Get<T>(string key, CancellationToken cancellationToken)
    {
        var e = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", cancellationToken, key);
        return JsonSerializer.Deserialize<T>(e, _jsonOptions.SerializerOptions)!;
    }
}
