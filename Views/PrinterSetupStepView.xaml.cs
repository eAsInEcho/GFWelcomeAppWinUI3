using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using GFSetupWizard.App.WinUI3.SystemIntegration;

namespace GFSetupWizard.App.WinUI3.Views
{
    public sealed partial class PrinterSetupStepView : UserControl
    {
        // Printer server paths
        private const string MaltaAustinPath = @"\\vlfcprints01";
        private const string BurlingtonEastFishkillPath = @"\\vlfcprints02";
        private const string SantaClaraPath = @"\\scgprns01";

        public PrinterSetupStepView()
        {
            this.InitializeComponent();
        }
        
        private async void MaltaAustinButton_Click(object sender, RoutedEventArgs e)
        {
            await HandlePrinterLocationClick("Malta or Austin", MaltaAustinPath);
        }

        private async void BurlingtonEastFishkillButton_Click(object sender, RoutedEventArgs e)
        {
            await HandlePrinterLocationClick("Burlington or East Fishkill", BurlingtonEastFishkillPath);
        }

        private async void SantaClaraButton_Click(object sender, RoutedEventArgs e)
        {
            await HandlePrinterLocationClick("Santa Clara", SantaClaraPath);
        }

        private async Task HandlePrinterLocationClick(string locationName, string printerPath)
        {
            try
            {
                // Create shortcut if it doesn't exist
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string shortcutPath = Path.Combine(desktopPath, $"{locationName} Printers.lnk");
                
                bool shortcutCreated = false;

                // Check if shortcut exists in standard Desktop
                if (!File.Exists(shortcutPath))
                {
                    // Check OneDrive Desktop if OneDrive is configured
                    if (SystemApplicationLauncher.IsOneDriveConfigured())
                    {
                        string oneDrivePath = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                            "OneDrive - GlobalFoundries");
                        
                        string oneDriveDesktopPath = Path.Combine(oneDrivePath, "Desktop");
                        
                        if (Directory.Exists(oneDriveDesktopPath))
                        {
                            desktopPath = oneDriveDesktopPath;
                            shortcutPath = Path.Combine(desktopPath, $"{locationName} Printers.lnk");
                        }
                    }
                    
                    // Create the shortcut
                    if (!File.Exists(shortcutPath))
                    {
                        CreateShortcut(shortcutPath, printerPath, $"{locationName} Printers");
                        shortcutCreated = true;
                    }
                }

                // Show dialog to inform user
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Printer Access",
                    Content = shortcutCreated ? 
                        $"A shortcut to the {locationName} printers has been created on your desktop. File Explorer will now open to allow you to install a printer." :
                        $"File Explorer will now open to allow you to install a printer. A shortcut is already available on your desktop for future access.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                
                await dialog.ShowAsync();
                
                // Open Explorer to the printer path
                OpenExplorerToPath(printerPath);
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Unable to access printer location: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                
                await errorDialog.ShowAsync();
            }
        }

        private void CreateShortcut(string shortcutPath, string targetPath, string description)
        {
            // Create shortcut using Windows Script Host
            using (StreamWriter writer = new StreamWriter(Path.ChangeExtension(shortcutPath, ".vbs")))
            {
                writer.WriteLine("Set WshShell = WScript.CreateObject(\"WScript.Shell\")");
                writer.WriteLine($"Set shortcut = WshShell.CreateShortcut(\"{shortcutPath}\")");
                writer.WriteLine($"shortcut.TargetPath = \"{targetPath}\"");
                writer.WriteLine($"shortcut.Description = \"{description}\"");
                writer.WriteLine("shortcut.Save");
            }
            
            // Execute the VBS script
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "wscript.exe",
                Arguments = $"\"{Path.ChangeExtension(shortcutPath, ".vbs")}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            }).WaitForExit();
            
            // Delete the temporary VBS script
            File.Delete(Path.ChangeExtension(shortcutPath, ".vbs"));
        }

        private void OpenExplorerToPath(string path)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"\"{path}\"",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening Explorer: {ex.Message}");
            }
        }
    }
}
