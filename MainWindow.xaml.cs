using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using GFSetupWizard.App.WinUI3.Views;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;

namespace GFSetupWizard.App.WinUI3
{
    /// <summary>
    /// Main window of the application.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        // UI elements
        private Grid mainGrid;
        private Grid navGrid;
        private ContentControl stepContentControl;
        private Button backButton;
        private Button nextButton;
        private ProgressBar stepProgressBar;
        private CheckBox autoRunCheckbox;
        
        // Navigation state
        private int currentStepIndex = 0;
        private List<Type> stepViewTypes;
        
        public MainWindow()
        {
            Title = "GlobalFoundries Setup Wizard";
            
            // Initialize step types
            InitializeStepViewTypes();
            
            // Create UI
            CreateMainLayout();
            
            // Show first step
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
        
        private void CreateMainLayout()
        {
            try
            {
                // Main grid with rows for navigation and content
                mainGrid = new Grid();
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                mainGrid.Background = new SolidColorBrush(Colors.White);
                
                // Content area for step views
                stepContentControl = new ContentControl
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch
                };
                
                Grid.SetRow(stepContentControl, 0);
                mainGrid.Children.Add(stepContentControl);
                
                // Navigation bar at bottom
                navGrid = new Grid
                {
                    Padding = new Thickness(20, 10, 20, 10),
                    Background = new SolidColorBrush(Colors.WhiteSmoke),
                    BorderThickness = new Thickness(0, 1, 0, 0),
                    BorderBrush = new SolidColorBrush(Colors.LightGray)
                };
                
                navGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                navGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                navGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                
                // Back button
                backButton = new Button
                {
                    Content = "Back",
                    Margin = new Thickness(0, 0, 10, 0),
                    IsEnabled = false
                };
                backButton.Click += BackButton_Click;
                Grid.SetColumn(backButton, 0);
                navGrid.Children.Add(backButton);
                
                // Progress bar
                stepProgressBar = new ProgressBar
                {
                    Minimum = 0,
                    Maximum = stepViewTypes.Count - 1,
                    Value = 0,
                    Margin = new Thickness(10, 0, 10, 0),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(stepProgressBar, 1);
                navGrid.Children.Add(stepProgressBar);
                
                // Next button
                nextButton = new Button
                {
                    Content = "Next",
                    Margin = new Thickness(10, 0, 0, 0)
                };
                nextButton.Click += NextButton_Click;
                Grid.SetColumn(nextButton, 2);
                navGrid.Children.Add(nextButton);
                
                // Auto-run checkbox
                autoRunCheckbox = new CheckBox
                {
                    Content = "Run this wizard at startup",
                    Margin = new Thickness(20, 10, 0, 0),
                    IsChecked = false
                };
                autoRunCheckbox.Checked += AutoRunCheckbox_Checked;
                autoRunCheckbox.Unchecked += AutoRunCheckbox_Unchecked;
                
                // Add a separate row for the checkbox
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                Grid.SetRow(autoRunCheckbox, 2);
                mainGrid.Children.Add(autoRunCheckbox);
                
                // Add navigation bar to main grid
                Grid.SetRow(navGrid, 1);
                mainGrid.Children.Add(navGrid);
                
                // Set main grid as window content
                Content = mainGrid;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating main layout: {ex.Message}");
            }
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
                stepContentControl.Content = stepView;
                
                // Update navigation state
                currentStepIndex = index;
                stepProgressBar.Value = index;
                
                // Configure back button
                backButton.IsEnabled = index > 0;
                
                // Configure next button
                if (index == stepViewTypes.Count - 1)
                {
                    nextButton.Content = "Finish";
                }
                else
                {
                    nextButton.Content = "Next";
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
