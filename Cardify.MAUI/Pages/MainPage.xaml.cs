namespace Cardify.MAUI.Pages
{
    public partial class MainPage : ContentPage
    {
        private string _currentActiveSection = "overview";

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Ensure the initial section is visible and sidebar is updated
            ShowSection(_currentActiveSection);
            SidebarNav.SetActiveSection(_currentActiveSection);
        }


        private void OnSectionSelected(object sender, string sectionName)
        {
            ShowSection(sectionName);
            // Update the sidebar's active section directly
            SidebarNav.SetActiveSection(sectionName);
            _currentActiveSection = sectionName; // Update internal state
        }

        /// <summary>
        /// Hides all content sections and then shows the specified section.
        /// </summary>
        /// <param name="sectionName">The name of the section to show.</param>
        private void ShowSection(string sectionName)
        {
            // Hide all sections first
            OverviewSection.IsVisible = false;
            CardsSection.IsVisible = false;
            //TransactionsSection.IsVisible = false;
            //AccountsSection.IsVisible = false;
            //AnalyticsSection.IsVisible = false;
            InvestmentsSection.IsVisible = false;
            SettingsSection.IsVisible = false;

            // Show the selected section
            switch (sectionName)
            {
                case "overview":
                    OverviewSection.IsVisible = true;
                    break;
                case "cards":
                    CardsSection.IsVisible = true;
                    break;
                //case "transactions":
                //    TransactionsSection.IsVisible = true;
                //    break;
                //case "accounts":
                //    AccountsSection.IsVisible = true;
                //    break;
                //case "analytics":
                //    AnalyticsSection.IsVisible = true;
                //    break;
                //case "investments":
                //    InvestmentsSection.IsVisible = true;
                //    break;
                //case "settings":
                //    SettingsSection.IsVisible = true;
                //    break;
                default:
                    // Fallback to overview if an unknown section is requested
                    OverviewSection.IsVisible = true;
                    break;
            }
        }
    }
}
