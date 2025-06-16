using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using GFSetupWizard.App.WinUI3.SystemIntegration;

namespace GFSetupWizard.App.WinUI3.Views
{
    public sealed partial class FinalSummaryStepView : UserControl
    {
        public FinalSummaryStepView()
        {
            this.InitializeComponent();
        }

        private async void OpenServicePortalButton_Click(object sender, RoutedEventArgs e)
        {
            bool success = SystemApplicationLauncher.OpenServicePortal();

            if (!success)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Launch Failed",
                    Content = "Unable to open the Service Portal automatically. Please navigate to https://globalfoundries.service-now.com/esc manually.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };

                await dialog.ShowAsync();
            }
        }

        private async void OpenRsaRequestButton_Click(object sender, RoutedEventArgs e)
        {
            bool success = SystemApplicationLauncher.OpenRsaTokenRequestForVpn();

            if (!success)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Launch Failed",
                    Content = "Unable to open the RSA token request page automatically. Please navigate to the Service Portal and search for 'RSA token' manually.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };

                await dialog.ShowAsync();
            }
        }

        private async void OpenSoftwareRequestButton_Click(object sender, RoutedEventArgs e)
        {
            bool success = SystemApplicationLauncher.OpenSoftwareRequestPortal();

            if (!success)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Launch Failed",
                    Content = "Unable to open the software request page automatically. Please navigate to the Service Portal and search for 'software request' manually.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };

                await dialog.ShowAsync();
            }
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Use the static MainWindow property from App to close the window safely
                if (App.MainWindow != null && App.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.SafeClose();
                }
                else
                {
                    // Fallback if MainWindow is not available
                    Application.Current.Exit();
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in FinishButton_Click: {ex.Message}");
                // Try fallback exit
                try
                {
                    Application.Current.Exit();
                }
                catch
                {
                    // If all else fails, do nothing - the user can close manually
                }
            }
        }
    }
}