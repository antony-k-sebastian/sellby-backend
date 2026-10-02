using Microsoft.Playwright;
using TechTalk.SpecFlow;

[Binding]
public class Hooks
{
    private static IPlaywright? _playwright;
    private static IBrowser? _browser;
    private IBrowserContext _context = null!;

    [BeforeScenario("ui")]
    public async Task BeforeScenario(ScenarioContext scenarioContext)
    {
        _playwright ??= await Playwright.CreateAsync();
        _browser ??= await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });

        _context = await _browser.NewContextAsync();
        var page = await _context.NewPageAsync();
        scenarioContext.ScenarioContainer.RegisterInstanceAs<IPage>(page);
    }

    [AfterScenario("ui")]
    public async Task AfterScenario()
    {
        await _context.CloseAsync();
    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        if (_browser is not null)
        {
            await _browser.CloseAsync();
        }

        _playwright?.Dispose();
    }
}
