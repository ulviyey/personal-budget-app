using Cardify.MAUI.Services;
using Cardify.MAUI.Views;

namespace Cardify.MAUI.Pages
{
    public partial class MainPage : ContentPage
    {
        private string _currentSection = "dashboard";

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

        private void OnSectionSelected(object? sender, string section)
        {
            _currentSection = section;
            Sidebar.SetActiveSection(section);
            ShowSection(section);
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
    }
}
