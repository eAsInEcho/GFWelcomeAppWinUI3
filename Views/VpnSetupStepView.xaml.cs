using GFSetupWizard.App.WinUI3.SystemIntegration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GFSetupWizard.App.WinUI3.Views
{
    public sealed partial class VpnSetupStepView : UserControl
    {
        public VpnSetupStepView()
        {
            this.InitializeComponent();
        }
        
        private async void OpenServicePortalButton_Click(object sender, RoutedEventArgs e)
        {
            bool success = SystemApplicationLauncher.OpenRsaTokenRequestForVpn();
            
            if (!success)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Launch Failed",
                    Content = "Unable to open the service portal. Please contact IT support for assistance.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                
                await dialog.ShowAsync();
            }
        }
    }
}
