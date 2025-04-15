using Microsoft.AspNetCore.Components;

namespace PersonalWebsite.Client.Pages;

public sealed partial class DesktopHome
{
    private Guid ActiveWindowId { get; set; }
    private List<WindowInfo> ActiveWindows { get; } = [];
    private int NextZIndex { get; set; } = 1;
    
    private void LaunchApp(AppInfo app)
    {
        // Check if app is already open
        var existingWindow = ActiveWindows.FirstOrDefault(w => w.AppId == app.Id);
        
        if (existingWindow != null)
        {
            // If minimized, restore it
            if (existingWindow.IsMinimized)
            {
                existingWindow.IsMinimized = false;
            }
            
            // Make it the active window
            ActivateWindow(existingWindow.Id);
            return;
        }
        
        // Create a new window
        var windowId = Guid.NewGuid();
        var window = new WindowInfo
        {
            Id = windowId,
            AppId = app.Id,
            Title = app.Name,
            X = new Random().Next(50, 150),
            Y = new Random().Next(50, 150),
            Width = app.DefaultWidth,
            Height = app.DefaultHeight,
            ZIndex = NextZIndex++,
            Content = app.Content
        };
        
        ActiveWindows.Add(window);
        ActivateWindow(windowId);
    }
    
    private void CloseWindow(Guid windowId)
    {
        var windowIndex = ActiveWindows.FindIndex(w => w.Id == windowId);
        if (windowIndex >= 0)
        {
            ActiveWindows.RemoveAt(windowIndex);
            
            // Set the next active window
            if (ActiveWindows.Count > 0)
            {
                ActivateWindow(ActiveWindows.OrderByDescending(w => w.ZIndex).First().Id);
            }
            
            StateHasChanged();
        }
    }
    
    private void ActivateWindow(Guid windowId)
    {
        // Only process if this is a different window than the currently active one
        if (ActiveWindowId != windowId)
        {
            ActiveWindowId = windowId;
            
            var window = ActiveWindows.FirstOrDefault(w => w.Id == windowId);
            if (window != null)
            {
                // Bring to front by setting z-index higher than any other window
                var highestZIndex = ActiveWindows.Max(w => w.ZIndex);
                window.ZIndex = highestZIndex + 1;
                NextZIndex = window.ZIndex + 1;
                
                // Ensure window is not minimized
                window.IsMinimized = false;
                
                // Always trigger a UI update
                StateHasChanged();
            }
        }
    }
    
    public sealed class AppInfo
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "";
        public int DefaultWidth { get; set; } = 800;
        public int DefaultHeight { get; set; } = 600;
        public RenderFragment? Content { get; set; }
    }
    
    public sealed class WindowInfo
    {
        public Guid Id { get; set; }
        public string AppId { get; set; } = "";
        public string Title { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ZIndex { get; set; }
        public bool IsMinimized { get; set; }
        public bool IsMaximized { get; set; }
        public RenderFragment? Content { get; set; }
    }
}