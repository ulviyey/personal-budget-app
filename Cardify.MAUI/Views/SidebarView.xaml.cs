namespace Cardify.MAUI.Views
{
    public partial class SidebarView : ContentView
    {
        public event EventHandler<string>? SectionSelected;
        public event EventHandler? LogoutRequested;

        public SidebarView()
        {
            InitializeComponent();
            SetupNavigation();
        }

        private void SetupNavigation()
        {
            // Add tap gestures to sidebar buttons
            var dashboardTap = new TapGestureRecognizer();
            dashboardTap.Tapped += (s, e) => OnSectionSelected("dashboard");
            DashboardButton.GestureRecognizers.Add(dashboardTap);

            var cardsTap = new TapGestureRecognizer();
            cardsTap.Tapped += (s, e) => OnSectionSelected("cards");
            CardsButton.GestureRecognizers.Add(cardsTap);

            var transactionsTap = new TapGestureRecognizer();
            transactionsTap.Tapped += (s, e) => OnSectionSelected("transactions");
            TransactionsButton.GestureRecognizers.Add(transactionsTap);

            var settingsTap = new TapGestureRecognizer();
            settingsTap.Tapped += (s, e) => OnSectionSelected("settings");
            SettingsButton.GestureRecognizers.Add(settingsTap);

            var logoutTap = new TapGestureRecognizer();
            logoutTap.Tapped += (s, e) => OnLogoutRequested();
            LogoutButton.GestureRecognizers.Add(logoutTap);
        }

        private void OnSectionSelected(string section)
        {
            SectionSelected?.Invoke(this, section);
        }

        private void OnLogoutRequested()
        {
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }

        public void SetActiveSection(string section)
        {
            // Reset all buttons
            DashboardButton.BackgroundColor = Colors.Transparent;
            CardsButton.BackgroundColor = Colors.Transparent;
            TransactionsButton.BackgroundColor = Colors.Transparent;
            SettingsButton.BackgroundColor = Colors.Transparent;

            // Reset all button text colors to default
            UpdateButtonTextColor(DashboardButton, Color.FromArgb("#6B7280"), false);
            UpdateButtonTextColor(CardsButton, Color.FromArgb("#6B7280"), false);
            UpdateButtonTextColor(TransactionsButton, Color.FromArgb("#6B7280"), false);
            UpdateButtonTextColor(SettingsButton, Color.FromArgb("#6B7280"), false);

            // Update button colors and text colors based on selection
            switch (section)
            {
                case "dashboard":
                    DashboardButton.BackgroundColor = Color.FromArgb("#EFF6FF");
                    UpdateButtonTextColor(DashboardButton, Color.FromArgb("#2563EB"), true);
                    break;
                case "cards":
                    CardsButton.BackgroundColor = Color.FromArgb("#EFF6FF");
                    UpdateButtonTextColor(CardsButton, Color.FromArgb("#2563EB"), true);
                    break;
                case "transactions":
                    TransactionsButton.BackgroundColor = Color.FromArgb("#EFF6FF");
                    UpdateButtonTextColor(TransactionsButton, Color.FromArgb("#2563EB"), true);
                    break;
                case "settings":
                    SettingsButton.BackgroundColor = Color.FromArgb("#EFF6FF");
                    UpdateButtonTextColor(SettingsButton, Color.FromArgb("#2563EB"), true);
                    break;
            }
        }

        private void UpdateButtonTextColor(Border button, Color color, bool isBold)
        {
            if (button.Content is HorizontalStackLayout layout)
            {
                foreach (var child in layout.Children)
                {
                    if (child is Label label && label.Text != "📊" && label.Text != "💳" && label.Text != "📝" && label.Text != "⚙️")
                    {
                        label.TextColor = color;
                        label.FontAttributes = isBold ? FontAttributes.Bold : FontAttributes.None;
                    }
                }
            }
        }

        public void UpdateUsername(string username)
        {
            UsernameLabel.Text = $"Welcome, {username}!";
        }
    }
} 