using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Site.Components;

public sealed partial class TerminalChat
{
    private readonly CancellationTokenSource _ctSource = new();

    [Inject]
    private Kernel Kernel { get; set; } = default!;

    private ChatHistory ChatHistory { get; } = [];
    private List<(string AuthorName, string Message)> Messages { get; } = [];
    private ChatState State { get; set; } = ChatState.Loading;

    protected override void OnInitialized()
    {
        ChatHistory.AddSystemMessage("You are an AI assistant on Erin McLaughlin's personal website & portfolio. Erin is the software engineer that programmed you. Talk to users about her.");
        ChatHistory.AddAssistantMessage("Oh hey, you found my website! What would you like to know?");
        Messages.Add(("AI", ChatHistory.Last().Content ?? "No comment."));
        StateHasChanged();

        State = ChatState.WaitingForUser;
    }

    private async Task RenderUserMessage(string message)
    {
        ChatHistory.AddUserMessage(message);
        Messages.Add(("User", message));
        State = ChatState.WaitingForAssistant;
        StateHasChanged();

        await RenderAssistantMessage();
    }

    private async Task RenderAssistantMessage()
    {
        var chatService = Kernel.GetRequiredService<IChatCompletionService>();
        var response = await chatService.GetChatMessageContentAsync(ChatHistory, null, Kernel, _ctSource.Token);

        var content = response.Content ?? "Hrm. Looks like I've encountered an error. Sorry about that! Maybe try back later?";
        Messages.Add(("User", content));
        ChatHistory.AddAssistantMessage(content);

        State = ChatState.WaitingForUser;
        StateHasChanged();
    }

    private IEnumerable<(string AuthorName, string Message)> GetChatHistory()
    {
        foreach (var message in ChatHistory)
        {
            if (message.AuthorName is not null && message.Content is not null)
            yield return (message.AuthorName, message.Content);
        }
    }

    private enum ChatState
    {
        Loading,
        WaitingForAssistant,
        StreamingAssistantMessage,
        WaitingForUser
    }
}