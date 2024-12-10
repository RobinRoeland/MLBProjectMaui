using System;
using System.Globalization;
using System.Collections.Generic;
using BaseballScoringApp.Models;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;


namespace BaseballScoringApp
{
    public partial class DynamicButtonActionList : Popup
    {
        List<IGameAction> the_actions;
        public double MaxPopupHeight { get; set; } // Bindable property for maximum height
        public double CalculatedHeight
        {
            get
            {
                if (the_actions != null)
                {
                    MaxPopupHeight = DeviceDisplay.MainDisplayInfo.Height * 0.75 / DeviceDisplay.MainDisplayInfo.Density;
                    double itemHeight = 50; // Example item height
                    double wantedMinHeight = Math.Max(the_actions.Count * itemHeight + 300, 300);
                    double height =  Math.Min(wantedMinHeight, MaxPopupHeight);
                    return height;
                }
                return 0;
            }
        }
        public DynamicButtonActionList(List<IGameAction> actions)
        {
            the_actions = actions;
            InitializeComponent();
            
            ActionsCollection.ItemsSource = actions;
            BindingContext = this; // Ensure the BindingContext includes ActionSelectedCommand

            // Set the maximum height dynamically
            MaxPopupHeight = DeviceDisplay.MainDisplayInfo.Height * 0.75 / DeviceDisplay.MainDisplayInfo.Density;
            
            IGameAction action = actions[0];
            TitleText.Text = "Action for " + action.playerInvolved.GetShortDisplayString();

        }
        public Command<IGameAction> ActionSelectedCommand => new Command<IGameAction>(OnActionSelected);

        private void OnActionSelected(IGameAction selectedAction)
        {
            Close(selectedAction); // Close the popup and return the selected action
        }
    };

}
