using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Site.AI;
using System.Diagnostics.CodeAnalysis;

namespace Site.Components;

public sealed partial class Home : IDisposable
{
    [Parameter, SupplyParameterFromQuery]
    public bool Preview { get; set; }

    [Inject, NotNull]
    private IOptionsMonitor<AIOptions>? AIOptionsMonitor { get; set; }

    [Inject, NotNull]
    private NavigationManager? NavigationManager { get; set; }

    private IDisposable? AIOptionsSubscription { get; set; }
    private Kernel? Kernel { get; set; }

    public void Dispose()
    {
        AIOptionsSubscription?.Dispose();
    }

    protected override void OnInitialized()
    {
        UpdateAIOptions(AIOptionsMonitor.CurrentValue);
        AIOptionsSubscription = AIOptionsMonitor.OnChange(UpdateAIOptions);
    }

    private void UpdateAIOptions(AIOptions options)
    {
        Kernel = null;

        if (Preview || options.Enabled)
        {
            var kernelBuilder = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(options.TextCompletionModel, options.ApiKey)
                .AddOpenAITextEmbeddingGeneration(options.TextEmbeddingModel, options.ApiKey);

            kernelBuilder.Plugins.AddFromObject(new WebsitePlugin(NavigationManager));

            Kernel = kernelBuilder.Build();
        }

        InvokeAsync(StateHasChanged);
    }
}
