using Microsoft.Extensions.AI;

namespace PersonalWebsite.Components.Pages;

public sealed partial class Home
{
    private readonly IChatClient _chatClient;
    private readonly List<ChatMessage> _chatMessages = [];
    private List<string> StreamingMessageParts { get; set; } = [];
    
    private string InputField { get; set; } = string.Empty;

    public Home(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    protected override void OnInitialized()
    {
        _chatMessages.Add(new ChatMessage(ChatRole.System, """
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

    private async Task StreamResponse()
    {
        await foreach (var item in _chatClient.GetStreamingResponseAsync(_chatMessages))
        {
            StreamingMessageParts.Add(item.Text);
            StateHasChanged();
        }

        var fullMessage = string.Join("", StreamingMessageParts);
        _chatMessages.Add(new ChatMessage(ChatRole.Assistant, fullMessage));
        StreamingMessageParts.Clear();
        StateHasChanged();
    }

    private async Task Submit()
    {
        if (string.IsNullOrWhiteSpace(InputField))
            return;
        
        var input = InputField.Trim();
        InputField = string.Empty;
        StateHasChanged();

        _chatMessages.Add(new ChatMessage(ChatRole.User, input));
        await InvokeAsync(StreamResponse);
    }
}