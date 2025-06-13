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
        // Navigation state
        private int currentStepIndex = 0;
        private List<Type> stepViewTypes;
        
        public MainWindow()
        {
            this.InitializeComponent();
            Title = "GlobalFoundries Setup Wizard";
            
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
        
        private ContentControl StepContent;
        private ProgressBar StepProgress;
        private Button BackButton;
        private Button NextButton;
        private CheckBox AutoRunCheckbox;
        
        private UIElement CreateMainLayout()
        {
            try
            {
                // Create main grid with three rows
                Grid mainGrid = new Grid();
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                mainGrid.Background = new SolidColorBrush(Colors.White);
                
                // Header with GF branding
                Border header = new Border
                {
                    Background = new SolidColorBrush(Colors.Orange),
                    Padding = new Thickness(20, 10, 20, 10)
                };
                
                TextBlock headerText = new TextBlock
                {
                    Text = "GlobalFoundries Setup Wizard",
                    FontSize = 20,
                    Foreground = new SolidColorBrush(Colors.White)
                };
                
                header.Child = headerText;
                Grid.SetRow(header, 0);
                mainGrid.Children.Add(header);
                
                // Content area
                StepContent = new ContentControl
                {
                    Margin = new Thickness(20),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch
                };
                
                Grid.SetRow(StepContent, 1);
                mainGrid.Children.Add(StepContent);
                
                // Navigation bar
                Grid navGrid = new Grid
                {
                    Padding = new Thickness(20, 10, 20, 10),
                    Background = new SolidColorBrush(Colors.LightGray)
                };
                
                navGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                navGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                navGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                
                // Back button
                BackButton = new Button
                {
                    Content = "Back",
                    Margin = new Thickness(0, 0, 10, 0),
                    IsEnabled = false
                };
                BackButton.Click += BackButton_Click;
                Grid.SetColumn(BackButton, 0);
                navGrid.Children.Add(BackButton);
                
                // Progress bar
                StepProgress = new ProgressBar
                {
                    Minimum = 0,
                    Maximum = stepViewTypes != null ? stepViewTypes.Count - 1 : 7,
                    Value = 0,
                    Margin = new Thickness(10, 0, 10, 0),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(StepProgress, 1);
                navGrid.Children.Add(StepProgress);
                
                // Next button
                NextButton = new Button
                {
                    Content = "Next",
                    Margin = new Thickness(10, 0, 0, 0)
                };
                NextButton.Click += NextButton_Click;
                Grid.SetColumn(NextButton, 2);
                navGrid.Children.Add(NextButton);
                
                Grid.SetRow(navGrid, 2);
                mainGrid.Children.Add(navGrid);
                
                // Auto-run checkbox
                AutoRunCheckbox = new CheckBox
                {
                    Content = "Run this wizard at startup",
                    Margin = new Thickness(20, 10, 20, 10),
                    IsChecked = false
                };
                AutoRunCheckbox.Checked += AutoRunCheckbox_Checked;
                AutoRunCheckbox.Unchecked += AutoRunCheckbox_Unchecked;
                
                // Add a separate row for the checkbox
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                Grid.SetRow(AutoRunCheckbox, 3);
                mainGrid.Children.Add(AutoRunCheckbox);
                
                return mainGrid;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating main layout: {ex.Message}");
                
                // Create a simple fallback UI with error message
                Grid errorGrid = new Grid();
                TextBlock errorText = new TextBlock
                {
                    Text = "Error loading UI. Please restart the application.",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                errorGrid.Children.Add(errorText);
                return errorGrid;
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
