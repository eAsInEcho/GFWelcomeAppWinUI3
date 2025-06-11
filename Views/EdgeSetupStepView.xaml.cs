using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using GFSetupWizard.App.WinUI3.SystemIntegration;

namespace GFSetupWizard.App.WinUI3.Views
{
    public sealed partial class EdgeSetupStepView : UserControl
    {
        public EdgeSetupStepView()
        {
            this.InitializeComponent();
        }

        private async void LaunchEdgeButton_Click(object sender, RoutedEventArgs e)
        {
            bool success = SystemApplicationLauncher.LaunchEdgeWithInputSimulator();
            
            if (!success)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Launch Failed",
                    Content = "Unable to launch Edge with sync settings. Please try using the 'Copy Sync Settings URL' button instead.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                
                await dialog.ShowAsync();
            }
            // Success message removed to prevent interruption of auto-typing
        }
        
        private async void CheckStatusButton_Click(object sender, RoutedEventArgs e)
        {
            bool success = SystemApplicationLauncher.CopyEdgeSyncUrlToClipboard();
            
            if (success)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "URL Copied",
                    Content = "The sync settings URL has been copied to your clipboard. " +
                              "Please paste it into Edge's address bar to navigate to the sync settings page.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                
                await dialog.ShowAsync();
            }
            else
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Copy Failed",
                    Content = "Unable to copy the URL to clipboard. The URL is: edge://settings/profiles/sync",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                
                await dialog.ShowAsync();
            }
        }
    }
}
