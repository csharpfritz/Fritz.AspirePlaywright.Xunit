namespace Fritz.AspirePlaywright.Xunit;

/// <summary>
/// Base class for Playwright tests, providing common functionality and setup for Playwright testing with ASP.NET Core.
/// </summary>
/// <param name="aspireManager"></param>
public abstract class BasePlaywrightTests : IClassFixture<AspireManager>, IAsyncDisposable
{
	private AspireManager AspireManager { get; }
	private PlaywrightManager PlaywrightManager => AspireManager.PlaywrightManager;


	protected BasePlaywrightTests(AspireManager aspireManager) 
	{
		Console.WriteLine("BasePlaywrightTests constructor called");
		AspireManager = aspireManager ?? throw new ArgumentNullException(nameof(aspireManager));
	}

	public string? DashboardUrl { get; private set; }
	public string DashboardLoginToken { get; private set; } = "";

	/// <summary>
	/// The default timeout for Playwright operations, such as waiting for a page to load or an element to appear.
	/// </summary>
	protected TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);
	private IBrowserContext? _context;

/// <summary>
/// Configure the Aspire application for testing with Playwright.  Run this at the beginning of your tests.
/// </summary>
/// /// <typeparam name="TEntryPoint">The AppHost class for the Aspire application.  Should have a syntax like 'Projects.AppHost'</typeparam>
	public Task<DistributedApplication> ConfigureAsync<TEntryPoint>(
			string[]? args = null,
			Action<IDistributedApplicationTestingBuilder>? configureBuilder = null) where TEntryPoint : class =>
			AspireManager.ConfigureAsync<TEntryPoint>(args, builder =>
			{
				var aspNetCoreUrls = builder.Configuration["ASPNETCORE_URLS"];
				var urls = aspNetCoreUrls is not null ? aspNetCoreUrls.Split(";") : [];

				DashboardUrl = urls.FirstOrDefault();
				DashboardLoginToken = builder.Configuration["AppHost:BrowserToken"] ?? "";

				configureBuilder?.Invoke(builder);
			});

	public async Task InteractWithPageAsync(string serviceName,
		Func<IPage, Task> test,
		ViewportSize? size = null)
	{

		if (AspireManager.App is null)
		{
			throw new InvalidOperationException("Aspire application is not configured. Call ConfigureAsync<TEntryPoint>() before running tests.");
		}

		Uri urlSought;
		var cancellationToken = new CancellationTokenSource(DefaultTimeout).Token;

		// Empty string means the dashboard URL
		if (!string.IsNullOrEmpty(serviceName))
		{
			if (AspireManager.App?.GetEndpoint(serviceName) is null)
			{
				throw new InvalidOperationException($"Service '{serviceName}' not found in the application endpoints");
			}

			urlSought = AspireManager.App.GetEndpoint(serviceName);

		}
		else
		{
			urlSought = new Uri(DashboardUrl!);
		}

		// Waits for the specified service to become healthy.
		await AspireManager.App.ResourceNotifications
			.WaitForResourceHealthyAsync(serviceName, cancellationToken)
			.WaitAsync(DefaultTimeout, cancellationToken);

		var page = await CreateNewPageAsync(urlSought, size);

		try
		{
			await test(page);
		}
		finally
		{
			await page.CloseAsync();
		}

	}

	private async Task<IPage> CreateNewPageAsync(Uri uri, ViewportSize? size = null)
	{

		Console.WriteLine($"Creating new page for {uri} with size {size}");

		_context = await PlaywrightManager.Browser
			.NewContextAsync(new BrowserNewContextOptions
			{
				
				IgnoreHTTPSErrors = true,
				ColorScheme = ColorScheme.Dark,
				ViewportSize = size,
				BaseURL = uri.ToString()
			});

		return await _context.NewPageAsync();

	}


	public async ValueTask DisposeAsync()
	{
		GC.SuppressFinalize(this);

		if (_context is not null)
		{
			await _context.DisposeAsync();
		}
	}
}
