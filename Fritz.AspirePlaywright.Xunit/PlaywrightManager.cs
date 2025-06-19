using System.Diagnostics;


namespace Fritz.AspirePlaywright.Xunit;

/// <summary>
/// Configure Playwright for interacting with the browser in tests.
/// </summary>
internal class PlaywrightManager : IAsyncLifetime
{
	private static bool IsDebugging => Debugger.IsAttached;
	private static bool IsHeadless => IsDebugging ? false : true;

	private IPlaywright? _playwright;

	internal IBrowser Browser { get; set; } = null!;

	public async Task InitializeAsync()
	{
		Assertions.SetDefaultExpectTimeout(10_000);

		_playwright = await Playwright.CreateAsync();

		var options = new BrowserTypeLaunchOptions
		{
			Headless = IsHeadless
		};

		Console.WriteLine($"Launching browser in {(IsHeadless ? "headless" : "headed")} mode...");
		Browser = await _playwright.Chromium.LaunchAsync(options).ConfigureAwait(false);
	}

	public async Task DisposeAsync()
	{
		await Browser.CloseAsync();

		_playwright?.Dispose();
	}
}
