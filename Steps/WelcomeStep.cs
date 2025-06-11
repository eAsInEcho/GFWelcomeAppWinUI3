using Microsoft.UI.Xaml.Controls;
using GFSetupWizard.App.WinUI3.Core;

namespace GFSetupWizard.App.WinUI3.Steps
{
    public class WelcomeStep : BaseSetupStep
    {
        public override string Title => "Welcome to Your New Laptop";
        public override string Description => "This wizard will guide you through the setup process of your new GlobalFoundries laptop.";

        public required Microsoft.UI.Xaml.Controls.UserControl View { get; init; }
    }
}
