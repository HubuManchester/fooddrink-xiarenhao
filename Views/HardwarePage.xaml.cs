using BiteRecord.Services;

namespace BiteRecord.Views;

public partial class HardwarePage : ContentPage
{
    private int _hapticCount = 0;

    public HardwarePage()
    {
        InitializeComponent();
    }

    private async void OnCameraClicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo != null)
            {
                var localFolder = Path.Combine(FileSystem.AppDataDirectory, "demo_photos");
                if (!Directory.Exists(localFolder))
                    Directory.CreateDirectory(localFolder);

                var fileName = $"demo_{DateTime.Now.Ticks}.jpg";
                var localPath = Path.Combine(localFolder, fileName);

                using var sourceStream = await photo.OpenReadAsync();
                using var fileStream = File.Create(localPath);
                await sourceStream.CopyToAsync(fileStream);

                CameraPreview.Source = ImageSource.FromFile(localPath);
                CameraPreview.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Camera Error", ex.Message, "OK");
        }
    }

    private async void OnLocationClicked(object sender, EventArgs e)
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                LocationDisplay.Text = "Location permission denied.";
                return;
            }

            var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            });

            if (location != null)
            {
                LocationDisplay.Text = $"Lat: {location.Latitude:F5}, Lon: {location.Longitude:F5}";

                var placemarks = await Geocoding.Default.GetPlacemarksAsync(location.Latitude, location.Longitude);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    var addressParts = new[] { placemark.CountryName, placemark.AdminArea, placemark.Locality, placemark.Thoroughfare };
                    var address = string.Join(", ", addressParts.Where(p => !string.IsNullOrEmpty(p)));
                    AddressDisplay.Text = address;
                }
                else
                {
                    AddressDisplay.Text = "Address not available";
                }
            }
            else
            {
                LocationDisplay.Text = "Unable to get location.";
            }
        }
        catch (Exception ex)
        {
            LocationDisplay.Text = $"Error: {ex.Message}";
        }
    }

    private async void OnSpeakClicked(object sender, EventArgs e)
    {
        string text = SpeechEntry.Text;
        if (string.IsNullOrWhiteSpace(text))
        {
            await DisplayAlert("Info", "Please enter some text to speak.", "OK");
            return;
        }

        await SpeechService.SpeakAsync(text);
    }

    private void OnStopClicked(object sender, EventArgs e)
    {
        SpeechService.Stop();
    }

    private void OnVibrateClicked(object sender, EventArgs e)
    {
        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(300));
        }
        catch (FeatureNotSupportedException)
        {
            DisplayAlert("Not Supported", "Vibration is not supported on this device/emulator.", "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void OnHapticClicked(object sender, EventArgs e)
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            _hapticCount++;
            HapticCountLabel.Text = $"Haptic test count: {_hapticCount}";
        }
        catch (FeatureNotSupportedException)
        {
            DisplayAlert("Not Supported", "Haptic feedback is not supported on this device/emulator.", "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }
}