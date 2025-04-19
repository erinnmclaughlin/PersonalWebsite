using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.AI;
using OpenAI;
using Site.Components;

var builder = WebApplication.CreateBuilder(args);

var chatClient = new OpenAIClient(builder.Configuration["AI:ApiKey"]).GetChatClient("gpt-4o-mini");
builder.Services.AddChatClient(chatClient.AsIChatClient()).UseFunctionInvocation().UseLogging();

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddResponseCompression(o =>
{
    o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseResponseCompression();
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

await app.RunAsync();
