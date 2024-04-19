using Microsoft.AspNetCore.SignalR;
using System.Diagnostics.CodeAnalysis;

namespace Site.AI;

public sealed class AibbaHub(ILogger<AibbaHub> logger) : Hub
{
    private readonly Dictionary<string, Aibba> _aibbas = [];
    private readonly ILogger _logger = logger;

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("User connected: {ConnectionId}", Context.ConnectionId);

        if (TryGetAibbaInstance(out var aibba))
            await SendNewMessagesAsync(aibba);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("User disconnected: {ConnectionId}", Context.ConnectionId);
        _aibbas.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string message)
    {
        _logger.LogInformation("User says {message}", message);

        if (TryGetAibbaInstance(out var aibba))
        {
            await aibba.TriggerResponse(message, Context.ConnectionAborted);
            await SendNewMessagesAsync(aibba);
        }
    }

    private bool TryGetAibbaInstance([NotNullWhen(true)] out Aibba? aibba)
    {
        aibba = _aibbas.GetValueOrDefault(Context.ConnectionId);

        if (aibba is null)
        {
            aibba = Context.GetHttpContext()?.RequestServices.GetRequiredService<Aibba>();

            if (aibba is null)
            {
                _logger.LogError("Failed to create Aibba instance for connection {ConnectionId}.", Context.ConnectionId);
                return false;
            }

            _aibbas.Add(Context.ConnectionId, aibba);
        }

        return true;
    }

    private async Task SendNewMessagesAsync(Aibba aibba)
    {
        var messages = aibba.GetNextMessages().ToList();

        if (messages.Count != 0)
        {
            foreach (var message in messages)
                _logger.LogInformation("Sending message {message} from {author}", message.Message, message.Author);

            await Clients.Caller.SendAsync("ReceiveMessage", messages, Context.ConnectionAborted);
        }
    }
}
