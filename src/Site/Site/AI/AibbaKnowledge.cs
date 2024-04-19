using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using System.ComponentModel;

namespace Site.AI;

public sealed class AibbaKnowledge
{
    private readonly ISemanticTextMemory _memory;

    public AibbaKnowledge(IOptions<AIOptions> options)
    {
        var memoryBuilder = new MemoryBuilder();
        memoryBuilder.WithOpenAITextEmbeddingGeneration(options.Value.TextEmbeddingModel, options.Value.ApiKey);
        memoryBuilder.WithMemoryStore(new VolatileMemoryStore());
        _memory = memoryBuilder.Build();      
    }

    internal void ApplyToKernel(Kernel kernel)
    {
        kernel.Plugins.AddFromObject(this);
        kernel.Plugins.AddFromObject(new TextMemoryPlugin(_memory));
    }

    [KernelFunction, Description("Get a link to Erin's GitHub profile.")]
    public static string GetGitHubUrl() => "https://github.com/erinnmclaughlin";

    [KernelFunction, Description("Get a link to Erin's LinkedIn profile.")]
    public static string GetLinkedInUrl() => "https://www.linkedin.com/in/e1mclaughlin";

    [KernelFunction, Description("Get a link to Erin's resume.")]
    public static string GetResume() => "https://erinnmclaughlin.github.io/Resume";

    [KernelFunction, Description("Recall information about Erin.")]
    public async Task<string> RecallInformationAsync([Description("The input text to recall information for.")] string question, CancellationToken cancellationToken = default)
    {
        return await new TextMemoryPlugin(_memory).RecallAsync(question, "AboutErin", cancellationToken: cancellationToken);
    }

    internal async Task AddMemoriesAsync()
    {
        var facts = new string[]
        {
            "Erin is a full stack software engineer that specializes in C#, .NET, and Blazor.",
            "Erin received her Master's degree in Software Engineering from Penn State University in 2023.",
            "Erin received her Bachelor's degree in Biology from Bridgewater State University in 2016.",
            "Erin is passionate about continuous learning and development.",
            "Erin strongly believes that context is key for making technical decisions. She believes there is never a 'one-size-fits-all' solution in software.",
            "Erin has mainly focused on building internal web applications for small companies, helping them to increase process efficiency and cross-departmental collaboration.",
            "Erin has two cats: Mia and Jax."
        };

        for (int i = 0; i < facts.Length; i++)
        {
            await _memory.SaveInformationAsync("AboutErin", facts[i], $"info{i+1}");
        }
    }
}
