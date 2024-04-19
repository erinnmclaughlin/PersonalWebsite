using Microsoft.SemanticKernel;

namespace Site.AI;

public sealed class AIOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public string TextCompletionModel { get; set; } = string.Empty;
    public string TextEmbeddingModel { get; set; } = string.Empty;

    public Kernel BuildDefaultKernel() => Kernel.CreateBuilder()
        .AddOpenAIChatCompletion(TextCompletionModel, ApiKey)
        .AddOpenAITextEmbeddingGeneration(TextEmbeddingModel, ApiKey)
        .Build();
}
