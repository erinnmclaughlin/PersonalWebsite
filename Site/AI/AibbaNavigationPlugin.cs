using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Site.AI;

public sealed class AibbaNavigationPlugin(NavigationManager navigationManager)
{
    private readonly NavigationManager _navigationManager = navigationManager;

    internal void ApplyToKernel(Kernel kernel)
    {
        kernel.Plugins.AddFromObject(this);
    }

    [KernelFunction, Description("Navigate to Erin's GitHub profile.")]
    public void NavigateToGitHub()
    {
        _navigationManager.NavigateTo(AibbaKnowledge.GetGitHubUrl());
    }

    [KernelFunction, Description("Navigate to Erin's LinkedIn profile.")]
    public void NavigateToLinkedIn()
    {
        _navigationManager.NavigateTo(AibbaKnowledge.GetLinkedInUrl());
    }

    [KernelFunction, Description("Navigate to Erin's resume.")]
    public void NavigateToResume()
    {
        _navigationManager.NavigateTo(AibbaKnowledge.GetResume());
    }
}