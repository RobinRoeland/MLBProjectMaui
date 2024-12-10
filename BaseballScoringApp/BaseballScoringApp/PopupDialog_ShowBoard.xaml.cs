using CommunityToolkit.Maui.Views;

namespace BaseballScoringApp;

public partial class PopupDialog_ShowBoard : Popup
{
    private readonly int _duration;
    public string messagetoshow;
    public PopupDialog_ShowBoard(string message, int duration)
    {
        InitializeComponent();
        _duration = duration;
        messagetoshow = message; // Set the message content
        CanBeDismissedByTappingOutsideOfPopup = false; // Make sure it can't be dismissed by tapping outside
    }
    public async Task ShowAsync(Page parentPage)
    {
        // Show the popup
        MessageLabel.Text = messagetoshow;
        parentPage.ShowPopup(this); // This is an extension method from CommunityToolkit.Maui.Views

        // Wait for the specified duration
        await Task.Delay(_duration);

        // Close the popup
        Close();
    }
}