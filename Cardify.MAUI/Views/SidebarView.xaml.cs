using Cardify.MAUI.Pages;

namespace Cardify.MAUI.Views
{
    // Define a delegate for the custom event
    public delegate void SectionSelectedEventHandler(object sender, string sectionName);

    public partial class SidebarView : ContentView
    {
        // Event declared to communicate selection back to MainPage
        public event SectionSelectedEventHandler SectionSelected = delegate { }; // Initialize with an empty delegate to avoid null

        // Store references to the Border and Button elements for direct manipulation
        private Border _lastActiveBorder = new Border(); // Initialize with a new instance
        private Button _lastActiveButton = new Button(); // Initialize with a new instance

        public SidebarView()
        {
            InitializeComponent();
            // Initial styling for the default active section (Overview)
            SetActiveSection("overview");
        }

        /// <summary>
        /// Event handler for all section buttons in the sidebar.
        /// Raises the SectionSelected event with the CommandParameter as the section name.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSectionButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string sectionName)
            {
                // Update the visual state of the sidebar itself
                SetActiveSection(sectionName);

                // Raise the custom event to notify the parent (MainPage)
                SectionSelected?.Invoke(this, sectionName);
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");

        /// <summary>
        /// Updates the visual appearance of the sidebar to highlight the active section.
        /// </summary>
        /// <param name="sectionName">The name of the section that should be highlighted.</param>
        public void SetActiveSection(string sectionName)
        {
            // Reset previous active section's styling
            if (_lastActiveBorder != null)
            {
                _lastActiveBorder.BackgroundColor = Colors.Transparent;
            }
            if (_lastActiveButton != null)
            {
                _lastActiveButton.TextColor = Color.FromArgb("#4B5563"); // Default gray text
            }

            // Apply new active section's styling
            switch (sectionName)
            {
                case "overview":
                    OverviewBorder.BackgroundColor = Color.FromArgb("#EEF2FF"); // Light blue for active background
                    OverviewButton.TextColor = Color.FromArgb("#4338CA"); // Dark blue for active text
                    _lastActiveBorder = OverviewBorder;
                    _lastActiveButton = OverviewButton;
                    break;
                case "cards":
                    CardsBorder.BackgroundColor = Color.FromArgb("#EEF2FF");
                    CardsButton.TextColor = Color.FromArgb("#4338CA");
                    _lastActiveBorder = CardsBorder;
                    _lastActiveButton = CardsButton;
                    break;
                    //case "transactions":
                    //    TransactionsBorder.BackgroundColor = Color.FromArgb("#EEF2FF");
                    //    TransactionsButton.TextColor = Color.FromArgb("#4338CA");
                    //    _lastActiveBorder = TransactionsBorder;
                    //    _lastActiveButton = TransactionsButton;
                    //    break;
                    //case "accounts":
                    //    AccountsBorder.BackgroundColor = Color.FromArgb("#EEF2FF");
                    //    AccountsButton.TextColor = Color.FromArgb("#4338CA");
                    //    _lastActiveBorder = AccountsBorder;
                    //    _lastActiveButton = AccountsButton;
                    //    break;
                    //case "analytics":
                    //    AnalyticsBorder.BackgroundColor = Color.FromArgb("#EEF2FF");
                    //    AnalyticsButton.TextColor = Color.FromArgb("#4338CA");
                    //    _lastActiveBorder = AnalyticsBorder;
                    //    _lastActiveButton = AnalyticsButton;
                    //    break;
                    //case "investments":
                    //    InvestmentsBorder.BackgroundColor = Color.FromArgb("#EEF2FF");
                    //    InvestmentsButton.TextColor = Color.FromArgb("#4338CA");
                    //    _lastActiveBorder = InvestmentsBorder;
                    //    _lastActiveButton = InvestmentsButton;
                    //    break;
                    //case "settings":
                    //    SettingsBorder.BackgroundColor = Color.FromArgb("#EEF2FF");
                    //    SettingsButton.TextColor = Color.FromArgb("#4338CA");
                    //    _lastActiveBorder = SettingsBorder;
                    //    _lastActiveButton = SettingsButton;
                    //    break;
            }
        }
    }
}
