using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using GFSetupWizard.App.WinUI3.SystemIntegration;

namespace GFSetupWizard.App.WinUI3.Views
{
    public sealed partial class OneDriveSetupStepView : UserControl
    {
        public OneDriveSetupStepView()
        {
            this.InitializeComponent();
        }

        private void LaunchOneDrive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemApplicationLauncher.LaunchOneDrive();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Failed to launch OneDrive: {ex.Message}");
            }
        }

        private async void ShowErrorMessage(string message)
        {
            try
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = message,
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                // If we can't show the dialog, at least output to debug
                System.Diagnostics.Debug.WriteLine($"Error: {message}. Dialog error: {ex.Message}");
            }
        }
    }
}
