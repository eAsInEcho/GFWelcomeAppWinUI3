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
        // Use the static MainWindow property from App to close the window
        if (App.MainWindow != null)
        {
            App.MainWindow.Close();
        }
        else
        {
            // Fallback if MainWindow is not available
            Application.Current.Exit();
        }
    }
    }
}
