using Microsoft.Playwright;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Sellby.Api.Specs.StepDefinitions.Feature.Authentication;

[Binding]
[Scope(Tag = "ui")]
public class UILoginSteps
{
    private readonly IPage _page;
    private string _email;
    private string _password;

    private const string BaseUrl = "http://sellby.app";

    public UILoginSteps(IPage page)
    {
        _page = page;
    }

    [Given(@"the user is on login page")]
    public async Task GivenUserIsOnLoginPage()
    {
        await _page.GotoAsync("https://sellby-frontend.vercel.app/login");
        await _page.WaitForSelectorAsync("#email");
    }

    [When(@"they enter a valid email and password")]
    public async Task WhenValidCredentialsEntered()
    {
        _email = "student@test.com";
        _password = "ValidPass123";

        await _page.FillAsync("#email", _email);
        await _page.FillAsync("#password", _password);
    }

    [When(@"they click the login button")]
    public async Task WhenClickLogin()
    {
        await _page.ClickAsync("#login-button");
    }

    [Then(@"they should be redirected to the marketplace homepage.")]
    public async Task ThenRedirectedToHomepage()
    {
        await _page.WaitForURLAsync("**/marketplace**");

        Assert.That(_page.Url, Does.Contain("/marketplace"));
        Assert.That(await _page.IsVisibleAsync("[data-testid='marketplace-page']"), Is.True);
    }
}