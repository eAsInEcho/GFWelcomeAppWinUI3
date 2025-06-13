using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using GFSetupWizard.App.WinUI3.SystemIntegration;
using H.InputSimulator;
using H.InputSimulator.Core.Native;

namespace GFSetupWizard.App.WinUI3.Views
{
    public sealed partial class EdgeSetupStepView : UserControl
    {
        public EdgeSetupStepView()
        {
            this.InitializeComponent();
            System.Diagnostics.Debug.WriteLine("EdgeSetupStepView constructor completed");
        }
        
        private void LaunchEdgeButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("LaunchEdgeButton clicked");
            
            try
            {
                // Kill all running Edge processes
                Process.Start("taskkill", "/F /IM msedge.exe");
                
                // Small delay for good measure
                Thread.Sleep(1000);
                
                // Launch Edge using shell execution (so Windows can find Edge without a full path)
                Process.Start(new ProcessStartInfo
                {
                    FileName = "msedge.exe",
                    Arguments = "--profile-directory=\"Default\"",
                    UseShellExecute = true
                });
                
                System.Diagnostics.Debug.WriteLine("Edge launched successfully");
                
                // Give Edge time to fully initialize before simulating typing
                Thread.Sleep(2000);
                
                // Create input simulator
                var simulator = new InputSimulator();
                
                // Focus the address bar using Ctrl+L
                System.Diagnostics.Debug.WriteLine("Sending Ctrl+L to focus address bar");
                simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_L);
                
                // Small delay to ensure address bar is focused
                Thread.Sleep(500);
                
                // Type the profile settings URL
                System.Diagnostics.Debug.WriteLine("Typing URL");
                SystemApplicationLauncher.SimulateTyping("edge://settings/profiles/sync");
                
                // Small delay before pressing Enter
                Thread.Sleep(500);
                
                // Press Enter to navigate to the URL
                System.Diagnostics.Debug.WriteLine("Pressing Enter");
                SystemApplicationLauncher.SimulatePressEnter();
                
                System.Diagnostics.Debug.WriteLine("Typed URL and pressed Enter");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error launching Edge: {ex.Message}");
                // Show error dialog asynchronously
                _ = ShowErrorDialogAsync("Edge Launch Error", $"Failed to launch Edge: {ex.Message}");
            }
        }

        private async Task ShowErrorDialogAsync(string title, string message)
        {
            try
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = title,
                    Content = message,
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing dialog: {ex.Message}");
            }
        }
        
        private async void CheckStatusButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Copy Edge sync URL to clipboard button clicked");
            
            // Directly copy the URL to clipboard without trying to launch Edge
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
