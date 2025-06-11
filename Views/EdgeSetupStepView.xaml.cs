using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.Win32;
using GFSetupWizard.App.WinUI3.SystemIntegration;
using WindowsInput;
using WindowsInput.Native;

namespace GFSetupWizard.App.WinUI3.Views
{
    public sealed partial class EdgeSetupStepView : UserControl
    {
        private InputSimulator _inputSimulator;
        private WebView2 _webView;
        
        public EdgeSetupStepView()
        {
            this.InitializeComponent();
            
            // Initialize the input simulator
            _inputSimulator = new InputSimulator();
            
            // Wait until the control is loaded before initializing WebView2
            this.Loaded += OnLoaded;
            
            System.Diagnostics.Debug.WriteLine("EdgeSetupStepView constructor completed");
        }
        
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("EdgeSetupStepView loaded event fired");
            
            // Find the WebView2 control
            _webView = FindName("EdgeWebView") as WebView2;
            
            if (_webView != null)
            {
                System.Diagnostics.Debug.WriteLine("WebView2 control found after loading");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERROR: WebView2 control not found after loading");
            }
        }
        
        /// <summary>
        /// Checks if the WebView2 Runtime is installed on the system
        /// </summary>
        private bool IsWebView2RuntimeInstalled()
        {
            try
            {
                // Method 1: Check for WebView2 fixed version runtime files
                string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                string webView2RuntimePath = Path.Combine(programFilesPath, "Microsoft", "EdgeWebView", "Application");
                
                if (Directory.Exists(webView2RuntimePath))
                {
                    System.Diagnostics.Debug.WriteLine("WebView2 Runtime found in Program Files");
                    return true;
                }
                
                // Method 2: Check for WebView2 Evergreen runtime
                string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string webView2EvergreenPath = Path.Combine(localAppDataPath, "Microsoft", "EdgeWebView", "Application");
                
                if (Directory.Exists(webView2EvergreenPath))
                {
                    System.Diagnostics.Debug.WriteLine("WebView2 Evergreen Runtime found in LocalAppData");
                    return true;
                }
                
                // Method 3: Check registry (simplified approach)
                try
                {
                    var regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                        @"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}");
                    
                    if (regKey != null)
                    {
                        System.Diagnostics.Debug.WriteLine("WebView2 Runtime found in registry");
                        return true;
                    }
                }
                catch (Exception regEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error checking registry: {regEx.Message}");
                }
                
                System.Diagnostics.Debug.WriteLine("WebView2 Runtime not found");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking WebView2 Runtime: {ex.Message}");
                return false;
            }
        }
        
        private async void LaunchEdgeButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("LaunchEdgeButton clicked");
            
            // Check if WebView2 Runtime is installed
            if (!IsWebView2RuntimeInstalled())
            {
                System.Diagnostics.Debug.WriteLine("WebView2 Runtime not installed");
                
                ContentDialog dialog = new ContentDialog
                {
                    Title = "WebView2 Runtime Required",
                    Content = "Microsoft Edge WebView2 Runtime is required to embed Edge in this app. Would you like to download and install it?",
                    PrimaryButtonText = "Download",
                    CloseButtonText = "Use Alternative Method",
                    XamlRoot = this.XamlRoot
                };
                
                var result = await dialog.ShowAsync();
                
                if (result == ContentDialogResult.Primary)
                {
                    // Launch browser to WebView2 download page
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Opening WebView2 download page");
                        
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "https://developer.microsoft.com/microsoft-edge/webview2/",
                            UseShellExecute = true
                        });
                    }
                    catch (Exception webEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error opening download page: {webEx.Message}");
                    }
                }
                
                // Fall back to direct Edge launch
                LaunchEdgeDirectly();
                return;
            }
            
            try
            {
                // Check if we have the WebView2 control
                if (_webView == null)
                {
                    // Try to find it again if it wasn't found in OnLoaded
                    _webView = FindName("EdgeWebView") as WebView2;
                    
                    if (_webView == null)
                    {
                        System.Diagnostics.Debug.WriteLine("WebView2 control not found in click handler");
                        throw new InvalidOperationException("WebView2 control not found");
                    }
                }
                
                // Make the WebView2 control visible
                _webView.Visibility = Visibility.Visible;
                System.Diagnostics.Debug.WriteLine("WebView2 made visible");
                
                // Try to initialize the WebView2 control if not already initialized
                try 
                {
                    // Set up navigation event before navigating
                    _webView.NavigationCompleted += OnNavigationCompleted;
                    
                    await _webView.EnsureCoreWebView2Async();
                    System.Diagnostics.Debug.WriteLine("WebView2 initialization completed");
                    
                    // Navigate to a blank page first
                    System.Diagnostics.Debug.WriteLine("Navigating to about:blank");
                    _webView.Source = new Uri("about:blank");
                    System.Diagnostics.Debug.WriteLine("Navigation initiated");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"WebView2 initialization/navigation error: {ex.Message}");
                    await ShowErrorDialogAsync("WebView2 Error", $"Failed to initialize WebView2: {ex.Message}. Using alternative method.");
                    
                    // Fall back to direct Edge launch
                    LaunchEdgeDirectly();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LaunchEdgeButton_Click: {ex.Message}");
                
                // Handle any exceptions
                await ShowErrorDialogAsync("Launch Failed", 
                    $"Unable to launch Edge with sync settings: {ex.Message}. Using alternative method.");
                
                // Hide WebView if there was an error and it exists
                if (_webView != null)
                {
                    _webView.Visibility = Visibility.Collapsed;
                }
                
                // Fall back to direct Edge launch
                LaunchEdgeDirectly();
            }
        }
        
        private void LaunchEdgeDirectly()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Launching Edge directly");
                
                // Try to launch Edge with sync settings URL directly
                Process.Start(new ProcessStartInfo
                {
                    FileName = "microsoft-edge:edge://settings/profiles/sync",
                    UseShellExecute = true
                });
                
                System.Diagnostics.Debug.WriteLine("Edge launched directly");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error launching Edge directly: {ex.Message}");
                
                // If direct launch fails, copy URL to clipboard as last resort
                SystemApplicationLauncher.CopyEdgeSyncUrlToClipboard();
            }
        }
        
        private void OnNavigationCompleted(object sender, object args)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Navigation completed event fired");
                
                if (sender is WebView2 senderWebView)
                {
                    string url = senderWebView.Source?.ToString() ?? "null";
                    System.Diagnostics.Debug.WriteLine($"Navigation URL: {url}");
                    
                    if (url == "about:blank")
                    {
                        System.Diagnostics.Debug.WriteLine("Blank page loaded, preparing to simulate typing");
                        // After the blank page is loaded, send the edge://settings/profiles/sync URL
                        SimulateTypingEdgeSyncUrl();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in navigation completed handler: {ex.Message}");
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
        
        private async void SimulateTypingEdgeSyncUrl()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Starting to simulate typing");
                
                // Focus the WebView2 control to ensure it receives keyboard input
                if (_webView != null)
                {
                    _webView.Focus(FocusState.Programmatic);
                    System.Diagnostics.Debug.WriteLine("WebView2 focused");
                    
                    // Wait a moment for the focus to take effect
                    await Task.Delay(1000);
                    
                    // Type the Edge sync settings URL using the SystemApplicationLauncher's SimulateTyping method
                    string syncUrl = "edge://settings/profiles/sync";
                    System.Diagnostics.Debug.WriteLine($"Typing URL: {syncUrl}");
                    
                    bool typingSuccess = SystemApplicationLauncher.SimulateTyping(syncUrl);
                    System.Diagnostics.Debug.WriteLine($"Typing success: {typingSuccess}");
                    
                    // Press Enter to navigate
                    await Task.Delay(500);
                    System.Diagnostics.Debug.WriteLine("Pressing Enter key");
                    bool enterSuccess = SystemApplicationLauncher.SimulatePressEnter();
                    System.Diagnostics.Debug.WriteLine($"Enter key press success: {enterSuccess}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("WebView2 is null when trying to simulate typing");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error simulating typing: {ex.Message}");
            }
        }
        
        private async void CheckStatusButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Launching Edge directly with protocol handler");
                
                // Try to launch Edge with sync settings URL directly using protocol handler
                Process.Start(new ProcessStartInfo
                {
                    FileName = "microsoft-edge:edge://settings/profiles/sync",
                    UseShellExecute = true
                });
                
                System.Diagnostics.Debug.WriteLine("Edge launched directly via protocol handler");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error launching Edge via protocol: {ex.Message}");
                
                // Fall back to copying URL to clipboard
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
}
