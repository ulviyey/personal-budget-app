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

        // Validation (meeting course requirement for data validation)
        if (string.IsNullOrWhiteSpace(email))
        {
            GeneralErrorLabel.Text = "Email is required.";
            GeneralErrorLabel.IsVisible = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            GeneralErrorLabel.Text = "Password is required.";
            GeneralErrorLabel.IsVisible = true;
            return;
        }

        if (email.Length > 100)
        {
            GeneralErrorLabel.Text = "Email cannot exceed 100 characters.";
            GeneralErrorLabel.IsVisible = true;
            return;
        }

        if (password.Length > 100)
        {
            GeneralErrorLabel.Text = "Password cannot exceed 100 characters.";
            GeneralErrorLabel.IsVisible = true;
            return;
        }

        // Basic email format validation
        if (!email.Contains("@") || !email.Contains("."))
        {
            GeneralErrorLabel.Text = "Please enter a valid email address.";
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

    private async void OnRegisterTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SignUpPage));
    }
}
