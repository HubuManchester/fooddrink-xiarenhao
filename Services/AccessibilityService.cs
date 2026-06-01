namespace BiteRecord.Services;

public static class AccessibilityService
{
    private const string LARGE_TEXT_KEY = "LargeTextEnabled";
    private const float NORMAL_FONT_SIZE = 14f;
    private const float LARGE_FONT_SIZE = 20f;

    public static bool LargeTextEnabled
    {
        get => Preferences.Default.Get(LARGE_TEXT_KEY, false);
        set => Preferences.Default.Set(LARGE_TEXT_KEY, value);
    }

    public static void ApplyFontScale(Page page)
    {
        if (page == null) return;

        ApplyFontScaleToElement(page, LargeTextEnabled);
    }

    private static void ApplyFontScaleToElement(object? element, bool isLarge)
    {
        if (element == null) return;

        if (element is Label label)
        {
            if (isLarge && label.FontSize == NORMAL_FONT_SIZE)
            {
                label.FontSize = LARGE_FONT_SIZE;
            }
            else if (!isLarge && label.FontSize == LARGE_FONT_SIZE)
            {
                label.FontSize = NORMAL_FONT_SIZE;
            }
        }

        if (element is Layout layout)
        {
            foreach (var child in layout.Children)
            {
                ApplyFontScaleToElement(child, isLarge);
            }
        }

        if (element is ContentView contentView && contentView.Content != null)
        {
            ApplyFontScaleToElement(contentView.Content, isLarge);
        }

        if (element is ScrollView scrollView && scrollView.Content != null)
        {
            ApplyFontScaleToElement(scrollView.Content, isLarge);
        }

        if (element is ContentPage contentPage && contentPage.Content != null)
        {
            ApplyFontScaleToElement(contentPage.Content, isLarge);
        }
    }
}