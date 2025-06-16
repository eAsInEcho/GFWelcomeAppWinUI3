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
            }
            this.Activated += MainWindow_Activated;

            // Initialize step types
            InitializeStepViewTypes();

            // Navigate to first step
            NavigateToStep(0);

            // Set window size and title
            SetWindowSize(900, 700);
            SetWindowTitle("GF Setup Wizard");
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
                typeof(FinalSummaryStepView)
            };
        }

        private void NavigateToStep(int index)
        {
            try
            {
                if (index < 0 || index >= stepViewTypes.Count)
                    return;

                // Create instance of the step view
                UserControl stepView = (UserControl)Activator.CreateInstance(stepViewTypes[index]);

                // Set as content
                StepContent.Content = stepView;

                // Update navigation state
                currentStepIndex = index;
                StepProgress.Value = index;
                StepProgress.Maximum = stepViewTypes.Count - 1;

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
                Close();
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
            var dragRect = new Windows.Graphics.RectInt32
            {
                X = 0,
                Y = 0,
                Width = (int)(this.Bounds.Width - 10),
                Height = 80
            };
            this.AppWindow.TitleBar.SetDragRectangles(new[] { dragRect });
        }
    }
}
