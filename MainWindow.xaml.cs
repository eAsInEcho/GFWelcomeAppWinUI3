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
            
            // Initialize step types
            InitializeStepViewTypes();

            // Navigate to first step
            NavigateToStep(0);

            // Set window size
            SetWindowSize(900, 700);
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
    } 
}
