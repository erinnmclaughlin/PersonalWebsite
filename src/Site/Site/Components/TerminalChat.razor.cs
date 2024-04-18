using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Site.Components;

public class ChatMessage(string author)
{
    public string Author { get; } = author;
    public string Message { get; set; } = string.Empty;
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
    private ChatState State { get; set; } = ChatState.Loading;

    protected override void OnInitialized()
    {
        ChatHistory.AddSystemMessage("You are an AI assistant on Erin McLaughlin's personal website & portfolio. Erin is the software engineer that programmed you. Talk to users about her.");
        ChatHistory.AddAssistantMessage("Oh hey, you found Erin's website! What would you like to know?");
        Messages.Add(new ChatMessage("AI")
        {
            Message = ChatHistory.Last().Content ?? ""
        });
        StateHasChanged();

        State = ChatState.WaitingForUser;
    }

    private async Task RenderUserMessage(string message)
    {
        ChatHistory.AddUserMessage(message);
        Messages.Add(new ("User") { Message = message });
        State = ChatState.WaitingForAssistant;
        StateHasChanged();

        await RenderAssistantMessage();
    }

    private async Task RenderAssistantMessage()
    {
        State = ChatState.StreamingAssistantMessage;

        var message = new ChatMessage("AI");
        Messages.Add(message);
        var chatService = Kernel.GetRequiredService<IChatCompletionService>();
        var response = chatService.GetStreamingChatMessageContentsAsync(ChatHistory, _promptExecutionSettings, Kernel, _ctSource.Token);

        await foreach(var part in response)
        {
            message.Message += part.Content ?? "";
            await Task.Delay(50);
            StateHasChanged();
        }

        ChatHistory.AddAssistantMessage(message.Message);

        State = ChatState.WaitingForUser;
        StateHasChanged();
    }

    private enum ChatState
    {
        Loading,
        WaitingForAssistant,
        StreamingAssistantMessage,
        WaitingForUser
    }
}