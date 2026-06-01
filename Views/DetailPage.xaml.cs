using BiteRecord.Models;
using BiteRecord.Services;

namespace BiteRecord.Views;

public partial class DetailPage : ContentPage
{
    private BiteRecordModel? _currentRecord;

    public DetailPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Get id from query string
        var queryString = Shell.Current.CurrentState?.Location.Query;
        if (!string.IsNullOrEmpty(queryString) && queryString.Contains("id="))
        {
            var idPart = queryString.Replace("?id=", "");
            if (int.TryParse(idPart, out int id))
            {
                await LoadRecordById(id);
            }
        }
        else if (BindingContext is BiteRecordModel record)
        {
            _currentRecord = record;
            DisplayRecord();
        }
        else
        {
            await DisplayAlert("Error", "Unable to load record", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        ApplyAccessibility();
    }

    private async Task LoadRecordById(int id)
    {
        try
        {
            _currentRecord = await App.Database.GetRecordByIdAsync(id);
            if (_currentRecord != null)
            {
                DisplayRecord();
            }
            else
            {
                await DisplayAlert("Error", "Record not found", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load: {ex.Message}", "OK");
        }
    }

    private void DisplayRecord()
    {
        if (_currentRecord == null) return;

        RestaurantLabel.Text = _currentRecord.RestaurantName;
        DishLabel.Text = _currentRecord.DishName;
        RatingLabel.Text = _currentRecord.RatingDisplay;
        PriceLabel.Text = _currentRecord.PriceDisplay;
        DateLabel.Text = $"Visited: {_currentRecord.DateDisplay}";

        // Location
        if (!string.IsNullOrEmpty(_currentRecord.LocationAddress))
        {
            LocationLabel.Text = _currentRecord.LocationAddress;
        }
        else if (!string.IsNullOrEmpty(_currentRecord.Latitude) && !string.IsNullOrEmpty(_currentRecord.Longitude))
        {
            LocationLabel.Text = $"Lat: {_currentRecord.Latitude}, Lon: {_currentRecord.Longitude}";
        }
        else
        {
            LocationLabel.Text = "No location recorded";
        }

        // Photo
        if (!string.IsNullOrEmpty(_currentRecord.PhotoPath) && File.Exists(_currentRecord.PhotoPath))
        {
            PhotoImage.Source = ImageSource.FromFile(_currentRecord.PhotoPath);
            PhotoImage.IsVisible = true;
            PhotoTitleLabel.IsVisible = true;
        }
        else
        {
            PhotoImage.IsVisible = false;
            PhotoTitleLabel.IsVisible = false;
        }

        // Review
        if (!string.IsNullOrEmpty(_currentRecord.ReviewText))
        {
            ReviewLabel.Text = _currentRecord.ReviewText;
        }
        else
        {
            ReviewLabel.Text = "No review written.";
            ReviewLabel.TextColor = Colors.Gray;
        }
    }

    private async void OnSpeakClicked(object sender, EventArgs e)
    {
        if (_currentRecord == null) return;

        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            string textToSpeak = $"{_currentRecord.RestaurantName}. {_currentRecord.DishName}. Rating {_currentRecord.Rating} stars. {_currentRecord.ReviewText}";
            await SpeechService.SpeakAsync(textToSpeak);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Speech Error", ex.Message, "OK");
        }
    }

    private void OnStopSpeakClicked(object sender, EventArgs e)
    {
        SpeechService.Stop();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_currentRecord == null) return;

        bool confirm = await DisplayAlert("Delete", $"Delete {_currentRecord.RestaurantName}?", "Yes", "No");

        if (confirm)
        {
            try
            {
                SpeechService.Stop();
                await App.Database.DeleteRecordAsync(_currentRecord);
                await Shell.Current.GoToAsync("//mainpage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Delete Failed", ex.Message, "OK");
            }
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        SpeechService.Stop();
    }

    private void ApplyAccessibility()
    {
        if (AccessibilityService.LargeTextEnabled)
        {
            AccessibilityService.ApplyFontScale(this);
        }
    }
}