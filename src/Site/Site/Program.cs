using Microsoft.AspNetCore.ResponseCompression;
using Site.AI;
using Site.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.Configure<AIOptions>(builder.Configuration.GetSection("AI"));
builder.Services.AddScoped<Aibba>();
builder.Services.AddSingleton<AibbaKnowledge>();
builder.Services.AddScoped<AibbaNavigationPlugin>();

builder.Services.AddResponseCompression(o =>
{
    o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]);
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseResponseCompression();
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

app.MapHub<AibbaHub>("/aibba");

using (var scope = app.Services.CreateScope())
{
    var knowledge = scope.ServiceProvider.GetRequiredService<AibbaKnowledge>();
    await knowledge.AddMemoriesAsync();
}

app.Run();
