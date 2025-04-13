using Microsoft.Extensions.AI;
using OpenAI;
using PersonalWebsite.Components;
using PersonalWebsite.Storage;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddChatClient(BuildChatClient()).UseFunctionInvocation().UseLogging();

builder.Services.AddTransient<IStorageProvider, LocalStorage>();

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

IChatClient BuildChatClient()
{
    var key = builder.Configuration["OpenAI:Key"];
    return new OpenAIClient(key).GetChatClient("gpt-4o-mini").AsIChatClient();
}
