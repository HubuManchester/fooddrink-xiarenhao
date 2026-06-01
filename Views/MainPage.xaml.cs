using BiteRecord.Models;
using BiteRecord.Services;

namespace BiteRecord.Views;

public partial class MainPage : ContentPage
{
    private List<BiteRecordModel> _allRecords = new();

    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadRecords();
        ApplyAccessibility();
    }

    private async Task LoadRecords(string? searchQuery = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                _allRecords = await App.Database.GetAllRecordsAsync();
            }
            else
            {
                _allRecords = await App.Database.SearchRecordsAsync(searchQuery);
            }

            RecordsCollection.ItemsSource = _allRecords;
            EmptyLabel.IsVisible = _allRecords.Count == 0;
            RecordsCollection.IsVisible = _allRecords.Count > 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load records: {ex.Message}", "OK");
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        _ = LoadRecords(e.NewTextValue);
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddItemPage));
    }

    private async void OnRecordSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is BiteRecordModel selectedRecord)
        {
            // Clear selection immediately
            RecordsCollection.SelectedItem = null;

            // Show action sheet with options
            string action = await DisplayActionSheet(selectedRecord.RestaurantName,
                "Cancel",
                "Delete",
                new string[] { "View Details" });

            if (action == "Delete")
            {
                // Confirm deletion
                bool confirm = await DisplayAlert("Confirm Delete",
                    $"Are you sure you want to delete {selectedRecord.RestaurantName}?",
                    "Yes", "No");

                if (confirm)
                {
                    try
                    {
                        await App.Database.DeleteRecordAsync(selectedRecord);
                        await LoadRecords();  // Refresh the list
                        await DisplayAlert("Success", "Record deleted successfully", "OK");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"Delete failed: {ex.Message}", "OK");
                    }
                }
            }
            else if (action == "View Details")
            {
                // Display record details
                await DisplayAlert("Record Details",
                    $"Restaurant: {selectedRecord.RestaurantName}\n\n" +
                    $"Dish: {selectedRecord.DishName}\n\n" +
                    $"Rating: {selectedRecord.RatingDisplay}\n\n" +
                    $"Price: {selectedRecord.PriceDisplay}\n\n" +
                    $"Date: {selectedRecord.DateDisplay}\n\n" +
                    $"Review: {(string.IsNullOrEmpty(selectedRecord.ReviewText) ? "No review" : selectedRecord.ReviewText)}\n\n" +
                    $"Location: {(string.IsNullOrEmpty(selectedRecord.LocationAddress) ? "No location" : selectedRecord.LocationAddress)}",
                    "OK");
            }
        }
    }

    private void ApplyAccessibility()
    {
        if (AccessibilityService.LargeTextEnabled)
        {
            AccessibilityService.ApplyFontScale(this);
        }
    }
}