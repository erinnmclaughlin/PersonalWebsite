using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics.CodeAnalysis;

namespace Site.Client.Components;

public sealed partial class TerminalChat : IAsyncDisposable
{
    private HubConnection? _hubConnection;

    [Inject, NotNull]
    private NavigationManager? NavigationManager { get; set; }

    private List<TerminalChatMessage> Messages { get; } = [];

    private ChatState State { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
            await _hubConnection.DisposeAsync();
    }

    protected override void OnInitialized()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/aibba"))
            .Build();

        _hubConnection.On<List<TerminalChatMessage>>("ReceiveMessage", (chatMessages) =>
        {
            InvokeAsync(async () => await RenderReceivedMessagesAsync(chatMessages));
        });

        InvokeAsync(() => _hubConnection.StartAsync());
    }

    private async Task RenderUserMessage(string message)
    {
        State = ChatState.WaitingForAssistant;
        Messages.Add(new TerminalChatMessage { Author = "User", Message = message });
        StateHasChanged();

        if (_hubConnection is not null)
            await _hubConnection.SendAsync("SendMessage", message);
    }

    private async Task RenderReceivedMessagesAsync(List<TerminalChatMessage> messages)
    {
        State = ChatState.Rendering;
        StateHasChanged();

        for (int i = 0; i < messages.Count; i++)
        {
            await RenderReceivedMessageAsync(messages[i]);
            
            if (i != messages.Count - 1)
                await Task.Delay(300);
        }

        State = ChatState.WaitingForUser;
        StateHasChanged();
    }

    private async Task RenderReceivedMessageAsync(TerminalChatMessage chatContent)
    {
        var message = new TerminalChatMessage
        {
            Author = chatContent.Author
        };

        Messages.Add(message);

        foreach (var c in chatContent.Message)
        {
            message.Message += c;
            await Task.Delay(30);
            StateHasChanged();
        }
    }

    enum ChatState
    {
        Rendering,
        WaitingForAssistant,
        WaitingForUser
    }
}