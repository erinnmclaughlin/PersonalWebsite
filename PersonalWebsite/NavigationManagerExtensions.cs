using Microsoft.AspNetCore.Components;

namespace PersonalWebsite;

public static class NavigationManagerExtensions
{
    public static bool IsHomePage(this NavigationManager navManager)
    {
        return navManager.Uri.Equals(navManager.BaseUri, StringComparison.OrdinalIgnoreCase);
    }
}
