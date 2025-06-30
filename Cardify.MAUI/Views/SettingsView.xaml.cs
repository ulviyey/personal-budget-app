using Cardify.MAUI.Models;
using Cardify.MAUI.Services;

namespace Cardify.MAUI.Views;

public partial class SettingsView : ContentView
{
    private readonly ApiUserService _apiUserService;

	public SettingsView()
	{
		InitializeComponent();
        _apiUserService = new ApiUserService(); // This should be injected
	}

    private async void OnChangePasswordClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        SuccessLabel.IsVisible = false;

        string currentPassword = CurrentPasswordEntry.Text;
        string newPassword = NewPasswordEntry.Text;
        string confirmPassword = ConfirmPasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(currentPassword) || 
            string.IsNullOrWhiteSpace(newPassword) || 
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            ErrorLabel.Text = "All password fields are required.";
            ErrorLabel.IsVisible = true;
            return;
        }

        if (newPassword != confirmPassword)
        {
            ErrorLabel.Text = "New passwords do not match.";
            ErrorLabel.IsVisible = true;
            return;
        }

        var userId = ApiLoginService.CurrentUserId;
        if (userId == null || userId == 0)
        {
            ErrorLabel.Text = "User not identified. Please re-login.";
            ErrorLabel.IsVisible = true;
            return;
        }

        var passwordChangeDto = new PasswordChangeDto
        {
            CurrentPassword = currentPassword,
            NewPassword = newPassword
        };
        
        var success = await _apiUserService.ChangePasswordAsync(userId.Value, passwordChangeDto);
        
        if(success)
        {
            SuccessLabel.IsVisible = true;
            CurrentPasswordEntry.Text = string.Empty;
            NewPasswordEntry.Text = string.Empty;
            ConfirmPasswordEntry.Text = string.Empty;
        }
        else
        {
            ErrorLabel.Text = "Failed to change password. Please check your current password.";
            ErrorLabel.IsVisible = true;
        }
    }
} 