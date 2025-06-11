using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using GFSetupWizard.App.WinUI3.SystemIntegration;

namespace GFSetupWizard.App.WinUI3
{
    public sealed partial class App : Application
    {
        private Window m_window;
        
        // Static property to access the main window from other parts of the app
        public static Window MainWindow { get; private set; }
        
        public App()
        {
            // In WinUI3, we don't need to call InitializeComponent explicitly
            // when the XAML compilation system isn't fully set up
            Debug.WriteLine("App constructor called");
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Set up the SystemApplicationLauncher message dialog handler
            SystemIntegration.SystemApplicationLauncher.ShowMessageDialogAsync = ShowAppMessageDialogAsync;
            
            try
            {
                // Check if this is the first time the app has been run
                if (!InstallationManager.HasAppRunBefore())
                {
                    // Set up auto-launch registry entry
                    Debug.WriteLine("First run detected, setting up auto-launch registry entry");
                    InstallationManager.SetupRegistryEntry();
                    
                    try
                    {
                        string logDirectory = @"C:\GFApps";
                        string logPath = Path.Combine(logDirectory, "app_log.txt");
                        
                        if (!Directory.Exists(logDirectory))
                        {
                            Directory.CreateDirectory(logDirectory);
                        }
                        
                        File.AppendAllText(logPath, $"{DateTime.Now}: First run detected, auto-launch registry entry created\n");
                    }
                    catch
                    {
                        // Continue even if logging fails
                    }
                }
                
                // Mark the app as having been run
                InstallationManager.SetAppHasRun();
                
                // Initialize font system explicitly
                InitializeFontSystem();
                
                // Add exception handling
                UnhandledException += (sender, args) =>
                {
                    LogException(args.Exception, "Application.UnhandledException");
                    ShowErrorMessage($"An unhandled exception occurred: {args.Exception.Message}");
                    args.Handled = true;
                };
                
                // Log startup
                try
                {
                    string logDirectory = @"C:\GFApps";
                    string logPath = Path.Combine(logDirectory, "app_log.txt");
                    
                    if (!Directory.Exists(logDirectory))
                    {
                        Directory.CreateDirectory(logDirectory);
                    }
                    
                    File.AppendAllText(logPath, $"{DateTime.Now}: Application startup initiated\n");
                }
                catch
                {
                    // Continue even if logging fails
                    Debug.WriteLine("Unable to write to app_log.txt, but continuing startup");
                }
                
                // Create and show the main window
                try
                {
                    Debug.WriteLine("Creating MainWindow instance");
                    m_window = new MainWindow();
                    MainWindow = m_window; // Set the static property
                    m_window.Activate();
                    
                    // Log successful window creation
                    try
                    {
                        string logDirectory = @"C:\GFApps";
                        string logPath = Path.Combine(logDirectory, "app_log.txt");
                        
                        if (!Directory.Exists(logDirectory))
                        {
                            Directory.CreateDirectory(logDirectory);
                        }
                        
                        File.AppendAllText(logPath, $"{DateTime.Now}: MainWindow created and shown successfully\n");
                    }
                    catch
                    {
                        // Continue even if logging fails
                        Debug.WriteLine("Unable to write success log, but window was created successfully");
                    }
                }
                catch (Exception windowEx)
                {
                    LogException(windowEx, "MainWindow Creation");
                    ShowErrorMessage($"Failed to create or show main window: {windowEx.Message}");
                    throw; // Re-throw to be caught by the outer try-catch
                }
            }
            catch (Exception ex)
            {
                LogException(ex, "OnLaunched");
                ShowErrorMessage($"Failed to start application: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Implementation of the message dialog handler for SystemApplicationLauncher
        /// </summary>
        private async Task<object> ShowAppMessageDialogAsync(string message, string title)
        {
            try
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = title,
                    Content = message,
                    CloseButtonText = "OK"
                };
                
                // Set XamlRoot if the window is available
                if (m_window != null && m_window.Content != null)
                {
                    dialog.XamlRoot = m_window.Content.XamlRoot;
                    return await dialog.ShowAsync();
                }
                else
                {
                    // Log but don't throw if window isn't available
                    Debug.WriteLine($"Window not available for dialog: {title} - {message}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing message dialog: {ex.Message}");
                return null;
            }
        }
        
        private void LogException(Exception ex, string source)
        {
            try
            {
                // Use C:\GFApps directory for logs since it's guaranteed to be user-accessible
                string logDirectory = @"C:\GFApps";
                string logPath = Path.Combine(logDirectory, "app_error_log.txt");
                
                string logMessage = $"{DateTime.Now}: {source} - {ex?.GetType().Name}: {ex?.Message}\n{ex?.StackTrace}\n\n";
                
                // Ensure the directory exists
                try
                {
                    if (!Directory.Exists(logDirectory))
                    {
                        Directory.CreateDirectory(logDirectory);
                    }
                    
                    File.AppendAllText(logPath, logMessage);
                }
                catch
                {
                    // Silently continue if file operations fail
                    // The important thing is the app starts successfully
                }
                
                Debug.WriteLine($"Exception logged: {source} - {ex?.GetType().Name}: {ex?.Message}");
            }
            catch
            {
                // Suppress all exceptions in the logging method
                // We don't want logging failures to affect the application
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
                    CloseButtonText = "OK"
                };
                
                // In WinUI 3, ContentDialog needs an XamlRoot to be displayed
                if (m_window != null && m_window.Content != null)
                {
                    dialog.XamlRoot = m_window.Content.XamlRoot;
                    await dialog.ShowAsync();
                }
                else
                {
                    // Fallback if window isn't available yet
                    Debug.WriteLine($"Error dialog couldn't be shown (no window): {message}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to show error dialog: {ex.Message}. Original error: {message}");
            }
        }
        
        /// <summary>
        /// Explicitly initializes the font system to avoid issues in deployment.
        /// </summary>
        private void InitializeFontSystem()
        {
            try
            {
                // Log the start of font initialization
                Debug.WriteLine("Starting font system initialization");
                try
                {
                    string logDirectory = @"C:\GFApps";
                    string logPath = Path.Combine(logDirectory, "app_log.txt");
                    
                    if (!Directory.Exists(logDirectory))
                    {
                        Directory.CreateDirectory(logDirectory);
                    }
                    
                    File.AppendAllText(logPath, $"{DateTime.Now}: Starting font system initialization\n");
                }
                catch
                {
                    // Continue even if logging fails
                }
                
                // WinUI 3 has different font initialization than WPF
                // Create some UI elements to force font loading
                
                // Approach 1: Create a simple TextBlock with a generic font family
                try
                {
                    Debug.WriteLine("Approach 1: Creating TextBlock with generic font family");
                    var textBlock = new TextBlock
                    {
                        Text = "Font Init",
                        FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe UI, Arial")
                    };
                    
                    textBlock.Measure(new Windows.Foundation.Size(100, 100));
                    Debug.WriteLine("TextBlock created and measured successfully");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Approach 1 failed: {ex.Message}");
                    LogException(ex, "Font init approach 1");
                }
                
                // Approach 2: Create and measure a Button (which uses different font rendering path)
                try
                {
                    Debug.WriteLine("Approach 2: Creating and measuring a Button");
                    var button = new Button { Content = "Font Init" };
                    button.Measure(new Windows.Foundation.Size(100, 30));
                    Debug.WriteLine("Button created and measured successfully");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Approach 2 failed: {ex.Message}");
                    LogException(ex, "Font init approach 2");
                }
                
                Debug.WriteLine("Font system initialization completed");
                try
                {
                    string logDirectory = @"C:\GFApps";
                    string logPath = Path.Combine(logDirectory, "app_log.txt");
                    
                    if (!Directory.Exists(logDirectory))
                    {
                        Directory.CreateDirectory(logDirectory);
                    }
                    
                    File.AppendAllText(logPath, $"{DateTime.Now}: Font system initialization completed\n");
                }
                catch
                {
                    // Continue even if logging fails
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Font initialization error: {ex.Message}");
                LogException(ex, "Font initialization");
                
                // Continue even if font initialization fails
                // The application might still work with default fonts
            }
        }
    }
}
