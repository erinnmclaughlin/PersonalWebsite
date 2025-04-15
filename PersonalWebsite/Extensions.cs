using Microsoft.Extensions.AI;
using OpenAI;

namespace PersonalWebsite;

public static class Extensions
{
    public static void AddBlazorComponents(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();
    }

    public static void AddChatClient(this WebApplicationBuilder builder)
    {
        var chatClient = new OpenAIClient(builder.Configuration["OpenAI:Key"]).GetChatClient("gpt-4o-mini");
        builder.Services.AddChatClient(chatClient.AsIChatClient()).UseFunctionInvocation().UseLogging();
    }
}
