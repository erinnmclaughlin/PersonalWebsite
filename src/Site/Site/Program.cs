using Microsoft.SemanticKernel;
using Site.AI;
using Site.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

if (builder.Configuration.GetSection("AI").Get<AIOptions>() is { } aiOptions)
{
    builder.Services.AddKernel()
        .AddOpenAIChatCompletion(aiOptions.TextCompletionModel, aiOptions.ApiKey)
        .AddOpenAITextEmbeddingGeneration(aiOptions.TextEmbeddingModel, aiOptions.ApiKey);
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Site.Client._Imports).Assembly);

app.Run();
