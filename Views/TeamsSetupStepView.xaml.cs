using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using GFSetupWizard.App.WinUI3.SystemIntegration;

namespace GFSetupWizard.App.WinUI3.Views
{
    public sealed partial class TeamsSetupStepView : UserControl
    {
        public TeamsSetupStepView()
        {
            this.InitializeComponent();
        }
        
        private async void LaunchTeamsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool success = SystemApplicationLauncher.LaunchTeams();
                
                if (!success)
                {
                    ContentDialog dialog = new ContentDialog
                    {
                        Title = "Launch Failed",
                        Content = "Unable to launch Teams automatically. Please open it manually from the Start menu.",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    
                    await dialog.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to launch Teams: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                
                await dialog.ShowAsync();
            }
        }
    }
}
