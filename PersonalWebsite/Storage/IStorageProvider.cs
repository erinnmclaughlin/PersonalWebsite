namespace PersonalWebsite.Storage;

public interface IStorageProvider
{
    ValueTask<bool> ContainsKey(string key, CancellationToken cancellationToken = default);
    ValueTask Set<T>(string key, T? value, CancellationToken cancellationToken = default);
    ValueTask<T> Get<T>(string key, CancellationToken cancellationToken = default);
}