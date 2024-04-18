using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Site.AI;

public class WebsitePlugin(NavigationManager navigationManager)
{
    [KernelFunction, Description("Get a link to Erin's GitHub profile.")]
    public static string GetGitHubUrl() => "https://github.com/erinnmclaughlin";

    [KernelFunction, Description("Get a link to Erin's LinkedIn profile.")]
    public static string GetLinkedInUrl() => "https://www.linkedin.com/in/e1mclaughlin/";

    [KernelFunction, Description("Get a link to Erin's resume.")]
    public static string GetResume() => "https://erinnmclaughlin.github.io/Resume/";

    [KernelFunction, Description("Navigate to Erin's GitHub profile.")]
    public void NavigateToGitHub()
    {
        navigationManager.NavigateTo(GetGitHubUrl());
    }

    [KernelFunction, Description("Navigate to Erin's LinkedIn profile.")]
    public void NavigateToLinkedIn()
    {
        navigationManager.NavigateTo(GetLinkedInUrl());
    }

    [KernelFunction, Description("Navigate to Erin's resume.")]
    public void NavigateToResume()
    {
       navigationManager.NavigateTo(GetResume());
    }
}
