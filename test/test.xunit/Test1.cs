using Fritz.AspirePlaywright.Xunit;
using Microsoft.Extensions.Logging;

namespace test.xunit.Tests;

public class Test1(AspireManager aspireManager) : BasePlaywrightTests(aspireManager)
{

	[Fact(DisplayName = "First Test")]
	public async Task FirstTest()
	{
		// Arrange
		var serviceName = "webfrontend"; // Replace with your actual service name
		await ConfigureAsync<Projects.test_AppHost>();

		// Act
		await InteractWithPageAsync(serviceName, async page =>
		{

			Console.WriteLine("Running FirstTest...");
			// Your test logic here, e.g., interacting with the page
			await page.GotoAsync("/");
			// Add more interactions as needed
		});

	}

}

public class Test2(AspireManager aspireManager) : BasePlaywrightTests(aspireManager)
{

	[Fact(DisplayName = "Second Test")]
	public async Task SecondTest()
	{
		// Arrange
		Console.WriteLine("Running SecondTest...");
		var serviceName = "webfrontend"; // Replace with your actual service name
		await ConfigureAsync<Projects.test_AppHost>();

		// Act
		await InteractWithPageAsync(serviceName, async page =>
		{
			// Your test logic here, e.g., interacting with the page
			await page.GotoAsync("/");
			// Add more interactions as needed
		});

	}

}
