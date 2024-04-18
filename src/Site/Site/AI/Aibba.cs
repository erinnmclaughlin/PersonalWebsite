using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Site.Components;
using System.Diagnostics.CodeAnalysis;

namespace Site.AI;

internal sealed class Aibba
{
    private readonly Kernel _kernel;
    private readonly ChatHistory _messages = [];
    private readonly OpenAIPromptExecutionSettings _promptExecutionSettings = new();
    private readonly Queue<ChatMessage> _queue = [];

    public Aibba(IOptions<AIOptions> options, AibbaPlugin plugin)
    {
        _kernel = options.Value.BuildDefaultKernel();
        _kernel.Plugins.AddFromObject(plugin);

        _promptExecutionSettings.ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions;
        
        InitializeChatHistory();
    }

    public Task TriggerResponse(string message, CancellationToken cancellationToken = default)
    {
        AddUserMessage(message);
        return TriggerResponse(cancellationToken);
    }

    private async Task TriggerResponse(CancellationToken cancellationToken)
    {
        var chatService = _kernel.GetRequiredService<IChatCompletionService>();
        var chatMessageContent = await chatService.GetChatMessageContentAsync(_messages, _promptExecutionSettings, _kernel, cancellationToken);
        AddAibbaMessage(chatMessageContent.Content ?? string.Empty);
    }

    public bool TryGetNextMessage([NotNullWhen(true)] out ChatMessage? message)
    {
        return _queue.TryDequeue(out message);
    }

    private void InitializeChatHistory()
    {
        _messages.AddSystemMessage("""
            You are an AI assistant named Aibba and you are running on Erin McLaughlin's personal website. 
            Erin is the software engineer that programmed you. Talk to users about her.
            """);

        AddErinMessage("Hi! Welcome to my website. I'm a software engineer with a passion for building context-driven systems.");
        AddErinMessage("You can check out my work on [GitHub](https://github.com/erinnmclaughlin), or ask my AI friend Aibba about me!");
        AddErinMessage("Aibba is a large language model I've integrated into my website to answer questions you might have about me.");
        AddErinMessage("Alright - I gotta go! Aibba, can you take it from here?");

        AddAibbaMessage("Sure thing, Erin! Feel free to ask me if there's anything else you'd like to know about Erin!");
    }

    private void AddAibbaMessage(string message)
    {
        _messages.AddMessage(AuthorRole.Assistant, message);
        _queue.Enqueue(new("Aibba") { Content = message });
    }

    private void AddErinMessage(string message)
    {
        _messages.AddMessage(AuthorRole.System, message);
        _queue.Enqueue(new("Erin") { Content = message });
    }

    private void AddUserMessage(string message)
    {
        _messages.AddMessage(AuthorRole.User, message);
    }
}
