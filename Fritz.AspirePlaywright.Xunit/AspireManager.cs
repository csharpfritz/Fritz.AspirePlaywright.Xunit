namespace Fritz.AspirePlaywright.Xunit;

/// <summary>
/// Startup and configure the Aspire application for testing.
/// </summary>
public class AspireManager : IAsyncLifetime
{

	internal PlaywrightManager PlaywrightManager { get; } = new();

	internal DistributedApplication? App { get; private set; }

	/// <summary>
	/// Configure the Aspire application for testing with Playwright.
	/// </summary>
	public async Task<DistributedApplication> ConfigureAsync<TEntryPoint>(
			string[]? args = null,
			Action<IDistributedApplicationTestingBuilder>? configureBuilder = null) where TEntryPoint : class
	{

		if (App is not null) return App;

		var builder = await DistributedApplicationTestingBuilder.CreateAsync<TEntryPoint>(
				args: args ?? [],
				configureBuilder: static (options, _) =>
				{
					options.DisableDashboard = false;
				});

		builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";

		configureBuilder?.Invoke(builder);

		App = await builder.BuildAsync();

		await App.StartAsync();

		return App;
	}

	/// <summary>
	/// Initialize the Aspire application and Playwright for testing.
	/// This method should be called before running any tests to ensure the application is ready.
	/// </summary>
	/// <returns></returns>
	public async Task InitializeAsync()
	{
		// Initialization logic here
		await PlaywrightManager.InitializeAsync();
	}

	public async Task DisposeAsync()
	{
		await PlaywrightManager.DisposeAsync();

		await (App?.DisposeAsync() ?? ValueTask.CompletedTask);
	}
	
}
