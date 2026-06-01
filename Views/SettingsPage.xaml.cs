using BiteRecord.Services;

namespace BiteRecord.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        LoadCurrentSettings();
    }

    private void LoadCurrentSettings()
    {
        var theme = Preferences.Default.Get("AppTheme", "system");
        ThemePicker.SelectedIndex = theme switch
        {
            "light" => 0,
            "dark" => 1,
            _ => 2
        };

        LargeTextSwitch.IsToggled = AccessibilityService.LargeTextEnabled;
    }

    private void OnThemeChanged(object sender, EventArgs e)
    {
        var selectedTheme = ThemePicker.SelectedIndex switch
        {
            0 => "light",
            1 => "dark",
            _ => "system"
        };

        Preferences.Default.Set("AppTheme", selectedTheme);
        ApplyTheme(selectedTheme);
    }

    private void ApplyTheme(string theme)
    {
        var userAppTheme = theme switch
        {
            "light" => AppTheme.Light,
            "dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };

        Application.Current.UserAppTheme = userAppTheme;
    }

    private async void OnLargeTextToggled(object sender, ToggledEventArgs e)
    {
        AccessibilityService.LargeTextEnabled = e.Value;

        if (e.Value)
        {
            await DisplayAlert("Large Text Mode", "Large text mode enabled. Text size will increase on all pages.", "OK");
        }

        AccessibilityService.ApplyFontScale(this);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
    }
}