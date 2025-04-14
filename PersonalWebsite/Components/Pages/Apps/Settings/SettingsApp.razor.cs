using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace PersonalWebsite.Components.Pages.Apps.Settings
{
    public partial class SettingsApp : ComponentBase
    {
        [Parameter]
        public bool IsInWindow { get; set; }

        private string ActiveCategory { get; set; } = "appearance";
        private string CurrentTheme { get; set; } = "system";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadCurrentTheme();
            }
        }

        private async Task LoadCurrentTheme()
        {
            // Get the current theme from localStorage using app.js function
            var theme = await JSRuntime.InvokeAsync<string>("window.getCurrentTheme");
            CurrentTheme = theme;
        }

        private void SelectCategory(string category)
        {
            ActiveCategory = category;
        }

        private async Task SetTheme(string theme)
        {
            CurrentTheme = theme;
            
            // Use the setTheme function from app.js
            await JSRuntime.InvokeVoidAsync("window.setTheme", theme);
        }

        private async Task ApplyTheme(string theme)
        {
            if (theme == "system")
            {
                // Use the applySystemTheme function from app.js
                await JSRuntime.InvokeVoidAsync("window.applySystemTheme");
            }
            else
            {
                // Apply the theme to the document
                await JSRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", theme);
            }
        }
    }
} 