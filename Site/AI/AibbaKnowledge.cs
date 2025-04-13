using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Site.AI;

public sealed class AibbaKnowledge
{
    private readonly MemoryServerless _memory;

    public AibbaKnowledge(IOptions<AIOptions> options)
    {
        var aiOptions = options.Value;

        _memory = new KernelMemoryBuilder()
            .WithOpenAI(new OpenAIConfig
            {
                TextModel = aiOptions.TextCompletionModel,
                TextModelMaxTokenTotal = 16384,
                EmbeddingModel = aiOptions.TextEmbeddingModel,
                EmbeddingModelMaxTokenTotal = 8191,
                APIKey = aiOptions.ApiKey
            })
            .Build<MemoryServerless>();      
    }

    internal void ApplyToKernel(Kernel kernel)
    {
        kernel.Plugins.AddFromObject(this);
    }

    [KernelFunction, Description("Get a link to Erin's GitHub profile.")]
    public static string GetGitHubUrl() => "https://github.com/erinnmclaughlin";

    [KernelFunction, Description("Get a link to Erin's LinkedIn profile.")]
    public static string GetLinkedInUrl() => "https://www.linkedin.com/in/e1mclaughlin";

    [KernelFunction, Description("Get a link to Erin's resume.")]
    public static string GetResume() => "https://erinnmclaughlin.github.io/Resume";

    [KernelFunction, Description("Ask a question about Erin.")]
    [return: Description("The answer to the question.")]
    public async Task<string> Ask([Description("The input text to recall information for.")] string question, CancellationToken cancellationToken = default)
    {
        var answer = await _memory.AskAsync(question, cancellationToken: cancellationToken);
        return answer.NoResult ? "I don't know the answer to that question." : answer.Result;
    }

    internal async Task AddMemoriesAsync()
    {
        var facts = new[]
        {
            "Erin is a full stack software engineer that specializes in C#, .NET, and Blazor.",
            "Erin received her Master's degree in Software Engineering from Penn State University in 2023.",
            "Erin received her Bachelor's degree in Biology from Bridgewater State University in 2016.",
            "Erin is passionate about continuous learning and development.",
            "Erin strongly believes that context is key for making technical decisions. She believes there is never a 'one-size-fits-all' solution in software.",
            "Erin has mainly focused on building internal web applications for small companies, helping them to increase process efficiency and cross-departmental collaboration.",
            "Erin has two cats: Mia and Jax.",
            "Erin currently works as a Senior Software Engineer / Platform Analyst at Cobalt Benefits Group, LLC. She started there in August 2023.",
            "Erin previously worked as a Software Engineer at Lighthouse Instruments. She started there as a laboratory technician in 2016 and transitioned to software engineer by 2019. Erin left Lighthouse in 2023.",
            "Erin's favorite color is yellow."
        };

        foreach (var fact in facts)
        {
            await _memory.ImportTextAsync(fact);
        }
    }
}
