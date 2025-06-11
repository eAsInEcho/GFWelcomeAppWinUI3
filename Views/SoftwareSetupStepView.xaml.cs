using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using GFSetupWizard.App.WinUI3.SystemIntegration;

namespace GFSetupWizard.App.WinUI3.Views
{
    public sealed partial class SoftwareSetupStepView : UserControl
    {
        public SoftwareSetupStepView()
        {
            this.InitializeComponent();
        }
        
        private async void LaunchSoftwareCenterButton_Click(object sender, RoutedEventArgs e)
        {
            bool success = SystemApplicationLauncher.LaunchSoftwareCenter();
            
            if (!success)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Launch Failed",
                    Content = "Unable to launch Software Center. It may not be installed on this computer.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                
                await dialog.ShowAsync();
            }
        }

        private async void OpenServicePortalButton_Click(object sender, RoutedEventArgs e)
        {
            bool success = SystemApplicationLauncher.OpenSoftwareRequestPortal();
            
            if (!success)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Launch Failed",
                    Content = "Unable to open the service portal. Please try again later.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                
                await dialog.ShowAsync();
            }
        }
    }
}
