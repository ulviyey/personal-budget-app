using Cardify.MAUI.Services;
using Cardify.MAUI.Views;

namespace Cardify.MAUI.Pages
{
    public partial class MainPage : ContentPage
    {
        private string _currentSection = "dashboard";

        // Event that fires when a tab/section changes
        public event EventHandler<string>? TabChanged;

        public MainPage()
        {
            InitializeComponent();
            SetupNavigation();
        }

        private void SetupNavigation()
        {
            // Wire up sidebar events
            Sidebar.SectionSelected += OnSectionSelected;
            Sidebar.LogoutRequested += OnLogoutRequested;
            
            // Wire up tab change events to notify views
            TabChanged += DashboardView.OnTabChanged;
            TabChanged += CardsView.OnTabChanged;
            TabChanged += TransactionsView.OnTabChanged;
            
            // Wire up transaction modification events to refresh dashboard
            TransactionsView.TransactionModified += OnTransactionModified;
            
            // Wire up card modification events to refresh dashboard
            CardsView.CardModified += OnCardModified;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            UpdateUsername();
            await LoadDashboardData();
        }

        private void UpdateUsername()
        {
            var username = ApiLoginService.CurrentUsername ?? "User";
            Sidebar.UpdateUsername(username);
        }

        private async Task LoadDashboardData()
        {
            await DashboardView.LoadData();
        }

        public async Task RefreshDashboard()
        {
            await DashboardView.LoadData();
        }

        private void OnSectionSelected(object? sender, string section)
        {
            _currentSection = section;
            Sidebar.SetActiveSection(section);
            ShowSection(section);
            
            // Fire the TabChanged event to notify views
            TabChanged?.Invoke(this, section);
        }

        private void ShowSection(string section)
        {
            // Hide all views
            DashboardView.IsVisible = false;
            CardsView.IsVisible = false;
            TransactionsView.IsVisible = false;
            SettingsComingSoonView.IsVisible = false;

            // Show the selected view
            switch (section)
            {
                case "dashboard":
                    DashboardView.IsVisible = true;
                    break;
                case "cards":
                    CardsView.IsVisible = true;
                    break;
                case "transactions":
                    TransactionsView.IsVisible = true;
                    break;
                case "settings":
                    SettingsComingSoonView.IsVisible = true;
                    SettingsComingSoonView.SetContent("Settings", "Manage your account settings", "Settings will be implemented next!");
                    break;
            }
        }

        private async void OnLogoutRequested(object? sender, EventArgs e)
        {
            // Clear user data
            ApiLoginService.CurrentUserId = null;
            ApiLoginService.CurrentUsername = null;
            
            // Navigate back to login
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        private async void OnTransactionModified(object? sender, EventArgs e)
        {
            // Refresh dashboard when transactions are modified
            await RefreshDashboard();
        }

        private async void OnCardModified(object? sender, EventArgs e)
        {
            // Refresh dashboard when cards are modified
            await RefreshDashboard();
        }
    }
}
