namespace PersonalWebsite;

public sealed class BlogEntryMeta
{
    public required string Title { get; init; }
    public required DateTime PublishedOn { get; init; }

    public string[] Tags { get; init; } = Array.Empty<string>();
}
