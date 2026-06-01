namespace BiteRecord.Services;

public static class SpeechService
{
    private static CancellationTokenSource? _cancellationTokenSource;

    public static async Task SpeakAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        Stop();

        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await TextToSpeech.Default.SpeakAsync(text);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Speech error: {ex.Message}");
        }
    }

    public static void Stop()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }
}