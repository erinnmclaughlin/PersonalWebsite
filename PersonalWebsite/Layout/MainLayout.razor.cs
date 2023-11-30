using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace PersonalWebsite.Layout;

public sealed partial class MainLayout
{
    private bool IsHomePage => NavigationManager.Uri == NavigationManager.BaseUri;

    public void Dispose()
    {
        NavigationManager.LocationChanged -= HandleLocationChanged;
    }

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += HandleLocationChanged;
    }

    private void HandleLocationChanged(object? o, LocationChangedEventArgs e)
    {
        StateHasChanged();
    }
}