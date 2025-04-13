using Microsoft.Extensions.AI;
using OpenAI;
using PersonalWebsite.Components;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var chatClient = new OpenAIClient(builder.Configuration["OpenAI:Key"]).GetChatClient("gpt-4o-mini");
builder.Services.AddChatClient(chatClient.AsIChatClient()).UseFunctionInvocation().UseLogging();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
