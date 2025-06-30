using Cardify.MAUI.Models;
using Cardify.MAUI.Services;

namespace Cardify.MAUI.Pages;

public partial class SignUpPage : ContentPage
{
    private readonly ApiUserService _apiUserService;

	public SignUpPage()
	{
		InitializeComponent();
        _apiUserService = new ApiUserService();
	}

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;
        string confirmPassword = ConfirmPasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            await DisplayAlert("Error", "All fields are required.", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }

        var userCreateDto = new UserCreateDto
        {
            Username = username,
            Email = email,
            Password = password
        };

        try
        {
            var success = await _apiUserService.CreateUserAsync(userCreateDto);
            if (success)
            {
                await DisplayAlert("Success", "Account created successfully!", "OK");
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            else
            {
                await DisplayAlert("Error", "Failed to create account.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void OnLoginTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
} 