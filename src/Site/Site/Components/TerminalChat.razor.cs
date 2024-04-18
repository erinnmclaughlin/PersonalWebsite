using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Site.Components;

public class ChatMessage(string author)
{
    public string Author { get; } = author;
    public string? Content { get; set; }
}

public sealed partial class TerminalChat
{
    private readonly CancellationTokenSource _ctSource = new();
    private readonly OpenAIPromptExecutionSettings _promptExecutionSettings = new() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };
    
    [Parameter, EditorRequired]
    public required Kernel Kernel { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private ChatHistory ChatHistory { get; } = [];
    private List<ChatMessage> Messages { get; } = [];
    private ChatState State { get; set; } = ChatState.StreamingMessage;

    protected override void OnInitialized()
    {
        ChatHistory.AddSystemMessage("""
            You are an AI assistant named Aibba and you are running on Erin McLaughlin's personal website. 
            Erin is the software engineer that programmed you. Talk to users about her.
            """);

        InvokeAsync(RenderGreeting);
    }

    private async Task RenderGreeting()
    {
        foreach (var message in WelcomeMessages)
        {
            await RenderGreeting(message);
            await Task.Delay(300);
        }

        await RenderAssistantMessage("Sure thing! Feel free to ask me if there's anything else you'd like to know about Erin!");
    }

    private async Task RenderGreeting(string message)
    {
        var chatMessage = new ChatMessage("Erin") { Content = string.Empty };
        Messages.Add(chatMessage);

        foreach (var part in message)
        {
            chatMessage.Content += part;
            await Task.Delay(30);
            StateHasChanged();
        }

        ChatHistory.AddSystemMessage(message);
    }

    private async Task RenderUserMessage(string message)
    {
        ChatHistory.AddUserMessage(message);
        Messages.Add(new ("User") { Content = message });

        await RenderAssistantMessage();
    }

    private async Task RenderAssistantMessage(string? message = null)
    {
        State = ChatState.WaitingForAssistant;
        StateHasChanged();

        if (message is null)
        {
            var chatService = Kernel.GetRequiredService<IChatCompletionService>();
            var chatMessageContent = await chatService.GetChatMessageContentAsync(ChatHistory, _promptExecutionSettings, Kernel, _ctSource.Token);
            message = chatMessageContent.Content ?? string.Empty;
        }

        var chatMessage = new ChatMessage("Aibba") { Content = string.Empty };
        Messages.Add(chatMessage);

        State = ChatState.StreamingMessage;

        foreach (var part in message)
        {
            chatMessage.Content += part;
            await Task.Delay(30);
            StateHasChanged();
        }

        ChatHistory.AddAssistantMessage(chatMessage.Content);
        State = ChatState.WaitingForUser;
        StateHasChanged();
    }

    private enum ChatState
    {
        Loading,
        WaitingForAssistant,
        StreamingMessage,
        WaitingForUser
    }

    private readonly List<string> WelcomeMessages = [
        "Hi! Welcome to my website. I'm a software engineer with a passion for building context-driven systems.",
        "You can check out my work on [GitHub](https://github.com/erinnmclaughlin), or ask my AI friend Aibba about me!",
        "Aibba is a large language model I've integrated into my website to answer questions you might have about me.",
        "Alright - I gotta go! Aibba, can you take it from here?"
        ];
}