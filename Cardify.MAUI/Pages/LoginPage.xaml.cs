using Cardify.Core.Services;

namespace Cardify.MAUI.Pages;

public partial class LoginPage : ContentPage
{
    private readonly ILoginService _loginService;
    private bool _isLoading = false;

    public LoginPage(ILoginService loginService)
    {
        InitializeComponent();
        _loginService = loginService;
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (_isLoading) return;

        GeneralErrorLabel.IsVisible = false;
        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            GeneralErrorLabel.Text = "Email and password are required.";
            GeneralErrorLabel.IsVisible = true;
            return;
        }

        _isLoading = true;
        LoginButton.Text = "Signing in...";
        LoginButton.IsEnabled = false;

        try
        {
            var success = await _loginService.LoginAsync(email, password);
            if (success)
            {
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
            else
            {
                GeneralErrorLabel.Text = "Invalid email or password";
                GeneralErrorLabel.IsVisible = true;
            }
        }
        catch
        {
            GeneralErrorLabel.Text = "An unexpected error occurred.";
            GeneralErrorLabel.IsVisible = true;
        }
        finally
        {
            _isLoading = false;
            LoginButton.Text = "Sign In";
            LoginButton.IsEnabled = true;
        }
    }
}
