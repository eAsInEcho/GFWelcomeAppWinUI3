using System;
using System.Diagnostics;
using GFSetupWizard.App.WinUI3.Views;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI;
using Windows.Foundation;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Markup;
using Windows.Graphics;
using Microsoft.UI.Xaml.Media.Animation;

namespace GFSetupWizard.App.WinUI3
{
    /// <summary>
    /// Main window of the application.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        // Navigation state
        private int currentStepIndex = 0;
        private List<Type> stepViewTypes;
        private bool isClosing = false; // Track if window is being closed

        public MainWindow()
        {
            this.InitializeComponent();

            // Extend into title bar
            ExtendsContentIntoTitleBar = true;
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                var titleBar = this.AppWindow.TitleBar;
                titleBar.BackgroundColor = Microsoft.UI.ColorHelper.FromArgb(255, 255, 96, 18);
                titleBar.InactiveBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(255, 255, 96, 18);
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(50, 255, 255, 255);
                this.AppWindow.SetIcon("GFSetupIcon.ico");
            }

            // Subscribe to events
            this.Activated += MainWindow_Activated;
            this.Closed += MainWindow_Closed;

            // Initialize step types
            InitializeStepViewTypes();

            // Navigate to first step
            NavigateToStep(0);

            // Set window size and title
            SetWindowSize(1000, 850);
            SetWindowTitle("GF Setup Wizard");
            SetMinimumWindowSize(1000, 700);
        }

        private void SetMinimumWindowSize(int minWidth, int minHeight)
        {
            try
            {
                var appWindow = this.AppWindow;
                if (appWindow != null)
                {
                    var minSize = new Windows.Graphics.SizeInt32
                    {
                        Width = minWidth,
                        Height = minHeight
                    };
                    // Note: WinUI3 doesn't have direct MinSize, but you can handle the SizeChanged event
                    // to enforce minimum sizes if needed
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting minimum window size: {ex.Message}");
            }
        }

        private void InitializeStepViewTypes()
        {
            stepViewTypes = new List<Type>
            {
                typeof(WelcomeStepView),
                typeof(OneDriveSetupStepView),
                typeof(OutlookSetupStepView),
                typeof(TeamsSetupStepView),
                typeof(EdgeSetupStepView),
                typeof(VpnSetupStepView),
                typeof(SoftwareSetupStepView),
                typeof(PrinterSetupStepView),
                typeof(FinalSummaryStepView)
            };
        }

        private void NavigateToStep(int index)
        {
            try
            {
                if (index < 0 || index >= stepViewTypes.Count)
                    return;

                UserControl stepView = index switch
                {
                    0 => new WelcomeStepView(),
                    1 => new OneDriveSetupStepView(),
                    2 => new OutlookSetupStepView(),
                    3 => new TeamsSetupStepView(),
                    4 => new EdgeSetupStepView(),
                    5 => new VpnSetupStepView(),
                    6 => new SoftwareSetupStepView(),
                    7 => new PrinterSetupStepView(),
                    8 => new FinalSummaryStepView(),
                    _ => throw new ArgumentOutOfRangeException(nameof(index))
                };

                // Set as content
                StepContent.Content = stepView;
                // Update navigation state
                currentStepIndex = index;

                // Get total steps and calculate progress ratio
                int totalSteps = stepViewTypes.Count;
                double progressRatio = (double)index / (double)(totalSteps - 1);

                // Update progress text
                ProgressText.Text = $"Step {index + 1} of {totalSteps}";

                try
                {
                    // Use ProgressContainer instead of ProgressBackground for width calculation
                    // If ActualWidth is 0, try to get Width property first, or use a default
                    var containerWidth = ProgressContainer.ActualWidth;
                    if (containerWidth <= 0)
                    {
                        containerWidth = 400; // Fallback default width
                    }

                    // Set the progress fill width - apply a minimum width for visibility
                    ProgressFill.Width = Math.Max(10, containerWidth * progressRatio);

                    Debug.WriteLine($"Progress: Step {index + 1} of {totalSteps}, Width: {ProgressFill.Width}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error updating progress bar: {ex.Message}");
                }
                // Configure back button
                BackButton.IsEnabled = index > 0;
                // Configure next button
                if (index == stepViewTypes.Count - 1)
                {
                    NextButton.Content = "Finish";
                }
                else
                {
                    NextButton.Content = "Next";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error navigating to step {index}: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentStepIndex > 0)
            {
                NavigateToStep(currentStepIndex - 1);
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentStepIndex < stepViewTypes.Count - 1)
            {
                NavigateToStep(currentStepIndex + 1);
            }
            else
            {
                // This is the last step, close the application
                CloseWindow();
            }
        }

        private void AutoRunCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Enable auto-run
                Debug.WriteLine("Auto-run enabled");
                GFSetupWizard.App.WinUI3.SystemIntegration.InstallationManager.SetupRegistryEntry();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting up auto-run: {ex.Message}");
            }
        }

        private void AutoRunCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Disable auto-run
                Debug.WriteLine("Auto-run disabled");
                GFSetupWizard.App.WinUI3.SystemIntegration.InstallationManager.RemoveRegistryEntry();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error removing auto-run: {ex.Message}");
            }
        }

        // Helper method to set window size
        private void SetWindowSize(int width, int height)
        {
            try
            {
                var appWindow = this.AppWindow;
                if (appWindow != null)
                {
                    var size = new Windows.Graphics.SizeInt32
                    {
                        Width = width,
                        Height = height
                    };
                    appWindow.Resize(size);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting window size: {ex.Message}");
            }
        }

        // Helper method to set window title
        private void SetWindowTitle(string title)
        {
            try
            {
                var appWindow = this.AppWindow;
                if (appWindow != null)
                {
                    appWindow.Title = title;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting window title: {ex.Message}");
            }
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            try
            {
                // Don't try to set drag rectangles if the window is closing or already closed
                if (isClosing || this.AppWindow == null || this.AppWindow.TitleBar == null)
                    return;

                var dragRect = new Windows.Graphics.RectInt32
                {
                    X = 0,
                    Y = 0,
                    Width = (int)(this.Bounds.Width - 10),
                    Height = 80
                };

                this.AppWindow.TitleBar.SetDragRectangles(new[] { dragRect });
            }
            catch (Exception ex)
            {
                // Silently handle the exception to prevent crashes during window disposal
                Debug.WriteLine($"Error in MainWindow_Activated: {ex.Message}");
            }
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            try
            {
                // Mark that we're closing to prevent further operations
                isClosing = true;

                // Unsubscribe from events to prevent them from firing during disposal
                this.Activated -= MainWindow_Activated;
                this.Closed -= MainWindow_Closed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in MainWindow_Closed: {ex.Message}");
            }
        }

        // Centralized method to close the window safely
        private void CloseWindow()
        {
            try
            {
                if (!isClosing)
                {
                    isClosing = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error closing window: {ex.Message}");
            }
        }

        // Public method that can be called from other views
        public void SafeClose()
        {
            CloseWindow();
        }
    }
}
