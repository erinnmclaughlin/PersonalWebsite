namespace Site.AI;

internal sealed class AIOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string TextCompletionModel { get; set; } = string.Empty;
    public string TextEmbeddingModel { get; set; } = string.Empty;
}
