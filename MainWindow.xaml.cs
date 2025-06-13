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
            Title = "GlobalFoundries Setup Wizard";
            
            // Initialize step types
            InitializeStepViewTypes();
            
            // Navigate to first step
            NavigateToStep(0);
            
            // Set window size
            SetWindowSize(900, 700);
        }