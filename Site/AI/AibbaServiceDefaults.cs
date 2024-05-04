using Site.AI;

namespace Microsoft.Extensions.DependencyInjection;

public static class AibbaServiceDefaults
{
    public static void AddAibbaDefaults(this IHostApplicationBuilder builder, string configurationSectionName = "AI")
    {
        builder.Services.Configure<AIOptions>(builder.Configuration.GetRequiredSection(configurationSectionName));
        builder.Services.AddScoped<Aibba>();
        builder.Services.AddSingleton<AibbaKnowledge>();
        builder.Services.AddScoped<AibbaNavigationPlugin>();
    }

    public static async Task UseAibba<T>(this T app) where T : IHost, IEndpointRouteBuilder
    {
        app.MapHub<AibbaHub>("/aibba");

        using var scope = app.Services.CreateScope();
        var knowledge = scope.ServiceProvider.GetRequiredService<AibbaKnowledge>();
        await knowledge.AddMemoriesAsync();
    }
}
