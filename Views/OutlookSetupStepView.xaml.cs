using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using GFSetupWizard.App.WinUI3.SystemIntegration;

namespace GFSetupWizard.App.WinUI3.Views
{
    public sealed partial class OutlookSetupStepView : UserControl
    {
        public OutlookSetupStepView()
        {
            this.InitializeComponent();
        }

        private void LaunchOutlook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemApplicationLauncher.LaunchOutlook();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Failed to launch Outlook: {ex.Message}");
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
