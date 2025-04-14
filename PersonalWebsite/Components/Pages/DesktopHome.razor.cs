using Microsoft.AspNetCore.Components;

namespace PersonalWebsite.Components.Pages;

public sealed partial class DesktopHome
{
    private Guid ActiveWindowId { get; set; }
    private List<WindowInfo> ActiveWindows { get; set; } = new();
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
    
    private void MinimizeWindow(Guid windowId)
    {
        var window = ActiveWindows.FirstOrDefault(w => w.Id == windowId);
        if (window != null)
        {
            window.IsMinimized = true;
            
            // Set the next active window
            if (ActiveWindows.Count > 0)
            {
                var nextActive = ActiveWindows.Where(w => !w.IsMinimized)
                    .OrderByDescending(w => w.ZIndex)
                    .FirstOrDefault();
                    
                if (nextActive != null)
                {
                    ActivateWindow(nextActive.Id);
                }
            }
            
            StateHasChanged();
        }
    }
    
    private void MaximizeWindow(Guid windowId)
    {
        var window = ActiveWindows.FirstOrDefault(w => w.Id == windowId);
        if (window != null)
        {
            if (window.IsMaximized)
            {
                // Restore to previous size
                window.X = window.PrevX;
                window.Y = window.PrevY;
                window.Width = window.PrevWidth;
                window.Height = window.PrevHeight;
                window.IsMaximized = false;
            }
            else
            {
                // Store current size
                window.PrevX = window.X;
                window.PrevY = window.Y;
                window.PrevWidth = window.Width;
                window.PrevHeight = window.Height;
                
                // Maximize - these values will be overridden by CSS when maximized class is applied
                window.X = 0;
                window.Y = 0;
                window.Width = window.PrevWidth; // Keep width value for restoring
                window.Height = window.PrevHeight; // Keep height value for restoring
                window.IsMaximized = true;
            }
            
            StateHasChanged();
        }
    }
    
    private void ActivateWindow(Guid windowId)
    {
        ActiveWindowId = windowId;
        
        var window = ActiveWindows.FirstOrDefault(w => w.Id == windowId);
        if (window != null)
        {
            // Bring to front
            window.ZIndex = NextZIndex++;
            window.IsMinimized = false;
            
            StateHasChanged();
        }
    }
    
    private void UpdateWindowPosition(Guid windowId, ResizableWindow.WindowPosition position)
    {
        var window = ActiveWindows.FirstOrDefault(w => w.Id == windowId);
        if (window != null && !window.IsMaximized)
        {
            window.X = position.X;
            window.Y = position.Y;
            window.Width = position.Width;
            window.Height = position.Height;
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
        public int PrevX { get; set; }
        public int PrevY { get; set; }
        public int PrevWidth { get; set; }
        public int PrevHeight { get; set; }
        public RenderFragment? Content { get; set; }
    }
}