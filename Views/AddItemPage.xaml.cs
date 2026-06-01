using BiteRecord.Models;
using BiteRecord.Services;

namespace BiteRecord.Views;

public partial class AddItemPage : ContentPage
{
    private string? _currentPhotoPath;
    private string? _currentLatitude;
    private string? _currentLongitude;
    private string? _currentAddress;

    public AddItemPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyAccessibility();
    }

    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo != null)
            {
                _currentPhotoPath = await SavePhotoToLocalAsync(photo);
                PhotoPreview.Source = ImageSource.FromFile(_currentPhotoPath);
                PhotoPreview.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Camera Error", ex.Message, "OK");
        }
    }

    private async void OnPickPhotoClicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo != null)
            {
                _currentPhotoPath = await SavePhotoToLocalAsync(photo);
                PhotoPreview.Source = ImageSource.FromFile(_currentPhotoPath);
                PhotoPreview.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Gallery Error", ex.Message, "OK");
        }
    }

    private async Task<string> SavePhotoToLocalAsync(FileResult photo)
    {
        var localFolder = Path.Combine(FileSystem.AppDataDirectory, "photos");
        if (!Directory.Exists(localFolder))
            Directory.CreateDirectory(localFolder);

        var fileName = $"bite_{DateTime.Now.Ticks}.jpg";
        var localPath = Path.Combine(localFolder, fileName);

        using var sourceStream = await photo.OpenReadAsync();
        using var fileStream = File.Create(localPath);
        await sourceStream.CopyToAsync(fileStream);

        return localPath;
    }

    private async void OnGetLocationClicked(object sender, EventArgs e)
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permission Denied", "Location permission is required to get your location.", "OK");
                return;
            }

            var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            });

            if (location != null)
            {
                _currentLatitude = location.Latitude.ToString();
                _currentLongitude = location.Longitude.ToString();
                LocationLabel.Text = $"Lat: {location.Latitude:F4}, Lon: {location.Longitude:F4}";

                var placemarks = await Geocoding.Default.GetPlacemarksAsync(location.Latitude, location.Longitude);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    var addressParts = new[] { placemark.CountryName, placemark.AdminArea, placemark.Locality };
                    _currentAddress = string.Join(", ", addressParts.Where(p => !string.IsNullOrEmpty(p)));
                    LocationLabel.Text += $"\n{_currentAddress}";
                }
            }
            else
            {
                await DisplayAlert("Location Failed", "Unable to get location. Please try again.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Location Error", ex.Message, "OK");
        }
    }

    private bool ValidateForm(out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(RestaurantEntry.Text))
        {
            errorMessage = "Restaurant name is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(DishEntry.Text))
        {
            errorMessage = "Dish name is required.";
            return false;
        }

        if (RatingPicker.SelectedIndex == -1)
        {
            errorMessage = "Please select a rating.";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(PriceEntry.Text) && !decimal.TryParse(PriceEntry.Text, out _))
        {
            errorMessage = "Price must be a valid number.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    private void ShowError(string message)
    {
        ErrorMessageLabel.Text = message;
        ErrorPanel.IsVisible = true;

        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200));
        }
        catch { }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (!ValidateForm(out string errorMessage))
        {
            ShowError(errorMessage);
            return;
        }

        ErrorPanel.IsVisible = false;

        var record = new BiteRecordModel
        {
            RestaurantName = RestaurantEntry.Text!.Trim(),
            DishName = DishEntry.Text!.Trim(),
            Rating = RatingPicker.SelectedIndex + 1,
            PricePerPerson = decimal.TryParse(PriceEntry.Text, out var price) ? price : 0,
            ReviewText = ReviewEditor.Text ?? string.Empty,
            PhotoPath = _currentPhotoPath ?? string.Empty,
            Latitude = _currentLatitude ?? string.Empty,
            Longitude = _currentLongitude ?? string.Empty,
            LocationAddress = _currentAddress ?? string.Empty
        };

        try
        {
            await App.Database.SaveRecordAsync(record);

            try
            {
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100));
            }
            catch { }

            await DisplayAlert("Success", "Your bite record has been saved!", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Save Failed", ex.Message, "OK");
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