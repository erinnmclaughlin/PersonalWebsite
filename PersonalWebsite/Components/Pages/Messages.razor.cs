using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.JSInterop;

namespace PersonalWebsite.Components.Pages;

public sealed partial class Messages
{
    private readonly IChatClient _chatClient;
    private readonly List<ChatMessage> _chatMessages = [];
    private readonly IJSRuntime _jsRuntime;

    private string InputField { get; set; } = string.Empty;
    private string StreamingMessage { get; set; } = "";
    
    public Messages(IChatClient chatClient, IJSRuntime jsRuntime)
    {
        _chatClient = chatClient;
        _jsRuntime = jsRuntime;
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        
        await LoadMessages();

        if (_chatMessages.Count == 0)
        {
            _chatMessages.Add(new ChatMessage(ChatRole.System,
                """
                You are a friendly AI assistant who represents Erin McLaughlin, the software engineer who programmed you.
                You are chatting with users on Erin's website. Your job is to answer questions about Erin.

                You currently know the following about Erin:
                - Erin is a full stack software engineer that specializes in C#, .NET, and Blazor.
                - Erin received her Master's degree in Software Engineering from Penn State University in 2023.
                - Erin received her Bachelor's degree in Biology from Bridgewater State University in 2016.
                - Erin is passionate about continuous learning and development.
                - Erin strongly believes that context is key for making technical decisions. She believes there is never a 'one-size-fits-all' solution in software.
                - Erin has two cats: Mia and Jax.
                - Erin currently works as a Platform Software Engineer at WillowTree.
                - Before working at WillowTree, Erin worked as a Senior Software Engineer / Platform Analyst at Cobalt Benefits Group, LLC.
                - Before working at Cobalt, Erin worked as a Software Engineer at Lighthouse Instruments. She started there as a laboratory technician in 2016 and transitioned to software engineer by 2019. Erin left Lighthouse in 2023.
                - Erin's favorite color is yellow.

                Start by saying hello to the user and introducing yourself!
                """));
            
            await SaveMessages();
        }
        else
        {
            await ScrollToBottom();
        }
        
        if (_chatMessages.LastOrDefault()?.Role != ChatRole.Assistant)
        {
            await StreamResponse();
        }
        
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task ScrollToBottom()
    {
        await _jsRuntime.InvokeVoidAsync("scrollElementToBottom", "messagesContainer");
    }

    private async Task StreamResponse()
    {
        await foreach (var item in _chatClient.GetStreamingResponseAsync(_chatMessages))
        {
            StreamingMessage += item.Text;
            StateHasChanged();
            await ScrollToBottom();
        }

        _chatMessages.Add(new ChatMessage(ChatRole.Assistant, StreamingMessage));
        StreamingMessage = "";
        StateHasChanged();
        
        await ScrollToBottom();
        await SaveMessages();
    }

    private async Task Submit()
    {
        if (string.IsNullOrWhiteSpace(InputField))
            return;

        var input = InputField.Trim();
        InputField = string.Empty;
        _chatMessages.Add(new ChatMessage(ChatRole.User, input));
        StateHasChanged();

        await SaveMessages();
        await Task.Delay(30);
        
        await ScrollToBottom();
        await InvokeAsync(StreamResponse);
    }

    private async Task LoadMessages()
    {
        var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "messages");

        if (json is null) return;
        
        foreach (var message in JsonSerializer.Deserialize<List<LocalChatMessage>>(json) ?? [])
        {
            _chatMessages.Add(new ChatMessage(message.Role, message.Content));
        }

        await InvokeAsync(StateHasChanged);
    }

    private async Task SaveMessages()
    {
        var messages = _chatMessages.Select(x => new LocalChatMessage(x.Role, x.Text)).ToList();
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "messages", JsonSerializer.Serialize(messages));
    }

    private sealed record LocalChatMessage(ChatRole Role, string Content);
}