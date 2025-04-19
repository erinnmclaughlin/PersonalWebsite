using Microsoft.Extensions.AI;
using Microsoft.JSInterop;

namespace Site.Components.Pages;

public sealed partial class Home
{
    private readonly IChatClient _chatClient;
    private readonly List<ChatMessage> _chatMessages = [];
    private readonly IJSRuntime _jsRuntime;

    private Dictionary<string, string> NavLinks = new()
    {
        ["github"] = "https://github.com/erinnmclaughlin",
        ["linked in"] = "https://www.linkedin.com/in/e1mclaughlin",
        ["resume"] = "https://erinnmclaughlin.github.io/Resume/"
    };

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
            $"""
               You are a friendly AI assistant who represents Erin McLaughlin, the software engineer who programmed you.
               You are chatting with users on Erin's website. Your job is to answer questions about Erin.
               
               You currently know the following about Erin:
               {string.Join(Environment.NewLine, ErinFacts.Select(f => $"- {f}"))}
               
               Start by saying hello to the user and introducing yourself!
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

    private static readonly string[] ErinFacts =
    [
        "Erin is a full stack software engineer that specializes in C#, .NET, and Blazor.",
        "Erin received her Master's degree in Software Engineering from Penn State University in 2023.",
        "Erin received her Bachelor's degree in Biology from Bridgewater State University in 2016.",
        "Erin is passionate about continuous learning and development.",
        "Erin strongly believes that context is key for making technical decisions. She believes there is never a 'one-size-fits-all' solution in software.",
        "Erin has mainly focused on building internal web applications for small companies, helping them to increase process efficiency and cross-departmental collaboration.",
        "Erin has two cats: Mia and Jax.",
        "Erin currently works as a Senior Software Engineer / Platform Analyst at Cobalt Benefits Group, LLC. She started there in August 2023.",
        "Erin previously worked as a Software Engineer at Lighthouse Instruments. She started there as a laboratory technician in 2016 and transitioned to software engineer by 2019. Erin left Lighthouse in 2023.",
        "Erin's favorite color is yellow."
    ];
}
