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
			// Your test logic here, e.g., interacting with the page
			await page.GotoAsync("/");
			// Add more interactions as needed
		});

	}

}
