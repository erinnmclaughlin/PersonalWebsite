using Microsoft.AspNetCore.Components;

namespace PersonalWebsite.Components.Pages.Apps.Photos;

public sealed partial class PhotoApp : ComponentBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly List<string> _photos = [];
    
    public PhotoApp(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    protected override void OnInitialized()
    {
        var basePath = Path.Combine("apps", "photos");
        var contents = _environment.WebRootFileProvider.GetDirectoryContents(basePath);

        if (contents.Exists)
        {
            foreach (var path in contents.Where(p => !p.IsDirectory).OrderBy(p => p.Name))
            {
                _photos.Add(Path.Combine(basePath, path.Name));
            }
        }
    }
}