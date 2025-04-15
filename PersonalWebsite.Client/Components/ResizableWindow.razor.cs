using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PersonalWebsite.Client.Components;

public partial class ResizableWindow : IAsyncDisposable
{
    private ElementReference _element;
    private DotNetObjectReference<ResizableWindow>? _objRef;
    private IJSObjectReference? _jsRef;

    [Parameter] public string Title { get; set; } = "Window";

    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter] public int DefaultX { get; set; } = 50;

    [Parameter] public int DefaultY { get; set; } = 50;

    [Parameter] public int DefaultWidth { get; set; } = 600;

    [Parameter] public int DefaultHeight { get; set; } = 400;

    [Parameter] public int ZIndex { get; set; } = 1;

    [Parameter] public EventCallback OnMinimize { get; set; }

    [Parameter] public EventCallback OnMaximize { get; set; }

    [Parameter] public EventCallback OnClose { get; set; }

    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _objRef = DotNetObjectReference.Create(this);
            await using var module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Components/ResizableWindow.razor.js");
            _jsRef = await module.InvokeAsync<IJSObjectReference>("initializeWindow", _element, _objRef);
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_jsRef is not null)
            {
                await _jsRef.DisposeAsync();
            }

            _objRef?.Dispose();
        }
        catch (Exception)
        {
            // Ignore disposal exceptions
        }
    }
}