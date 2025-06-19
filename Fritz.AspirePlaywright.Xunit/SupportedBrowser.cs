namespace Fritz.AspirePlaywright.Xunit;

/// <summary>
/// Supported web browsers for testing.
/// </summary>
public enum SupportedBrowser
{
    Chromium,
    Firefox,
    Webkit
}

public static class BrowserTypeExtensions
{

		/// <summary>
		/// Converts the supported browser enum to its Playwright name.
		/// </summary>
		/// <param name="browser"></param>
		/// <returns></returns>
    public static string ToPlaywrightName(this SupportedBrowser browser)
    {
        return browser switch
        {
            SupportedBrowser.Chromium => "chromium",
            SupportedBrowser.Firefox => "firefox",
            SupportedBrowser.Webkit => "webkit",
            _ => throw new ArgumentOutOfRangeException(nameof(browser))
        };
    }
}