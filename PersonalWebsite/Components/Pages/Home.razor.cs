using Microsoft.Extensions.AI;
using Microsoft.JSInterop;

namespace PersonalWebsite.Components.Pages;

public sealed partial class Home
{
    private readonly IChatClient _chatClient;
    private readonly List<ChatMessage> _chatMessages = [];
    private readonly IJSRuntime _jsRuntime;
    
    private string InputField { get; set; } = string.Empty;
    private string StreamingMessage { get; set; } = "";
    
    public Home(IChatClient chatClient, IJSRuntime jsRuntime)
    {
        _chatClient = chatClient;
        _jsRuntime = jsRuntime;
    }

    protected override void OnInitialized()
    {
        _chatMessages.Add(new ChatMessage(ChatRole.System, 
            """
               You are a friendly hiking enthusiast who helps people discover fun hikes in their area.
               You introduce yourself when first saying hello.
               When helping people out, you always ask them for this information
               to inform the hiking recommendation you provide:
           
               1. The location where they would like to hike
               2. What hiking intensity they are looking for
           
               You will then provide three suggestions for nearby hikes that vary in length
               after you get that information. You will also share an interesting fact about
               the local nature on the hikes when making a recommendation. At the end of your
               response, ask if there is anything else you can help with.
           """));

        InvokeAsync(StreamResponse);
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
    }

    private async Task Submit()
    {
        if (string.IsNullOrWhiteSpace(InputField))
            return;

        var input = InputField.Trim();
        InputField = string.Empty;
        _chatMessages.Add(new ChatMessage(ChatRole.User, input));
        StateHasChanged();

        await Task.Delay(30);
        
        await ScrollToBottom();
        await InvokeAsync(StreamResponse);
    }
}