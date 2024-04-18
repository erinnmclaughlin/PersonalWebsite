using Microsoft.AspNetCore.Components;
using Site.AI;

namespace Site.Components;

public class ChatMessage(string author)
{
    public string Author { get; } = author;
    public string? Content { get; set; }
}

public sealed partial class TerminalChat
{
    private readonly CancellationTokenSource _ctSource = new();
    
    [Inject]
    private Aibba Aibba { get; set; } = default!;

    private List<ChatMessage> Messages { get; } = [];
    private ChatState State { get; set; } = ChatState.StreamingMessage;

    protected override void OnInitialized()
    {
        InvokeAsync(RenderQueuedMessages);
    }

    private async Task RenderQueuedMessages()
    {
        State = ChatState.StreamingMessage;
        StateHasChanged();

        while (Aibba.TryGetNextMessage(out var message))
        {
            await RenderMessage(message);
            await Task.Delay(300, _ctSource.Token);
        }

        State = ChatState.WaitingForUser;
        StateHasChanged();
    }

    private async Task RenderUserMessage(string message)
    {
        Messages.Add(new ChatMessage("User") { Content = message });
        State = ChatState.WaitingForAssistant;
        StateHasChanged();

        await Aibba.TriggerResponse(message, _ctSource.Token);
        await RenderQueuedMessages();
    }

    private async Task RenderMessage(ChatMessage message)
    {
        var chatMessage = new ChatMessage(message.Author) { Content = string.Empty };
        Messages.Add(chatMessage);

        foreach (var part in message.Content ?? "")
        {
            chatMessage.Content += part;
            await Task.Delay(30);
            StateHasChanged();
        }
    }

    private enum ChatState
    {
        Loading,
        WaitingForAssistant,
        StreamingMessage,
        WaitingForUser
    }
}