using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Sellby.Api.Specs.StepDefinitions.Listings;

[Binding]
public class CreateListingSteps
{
    private readonly ScenarioContext _scenarioContext;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private HttpResponseMessage _response = null!;
    private Guid _categoryId;

    public CreateListingSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(@"I am authenticated as a seller")]
    public void GivenIAmAuthenticatedAsASeller()
    {
        // TODO: decide on a test database strategy (e.g. a dedicated test Postgres
        // instance or an EF Core provider swap in a WebApplicationFactory override)
        // before wiring this up, so scenarios don't write into the real dev database.
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();

        // TODO: seed a User row and mint a JWT (reusing the signing logic in
        // AuthEndpoints.GenerateJwt) instead of going through the OTP flow.
        var token = "TODO-generate-test-jwt";
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Given(@"the category ""(.*)"" exists")]
    public void GivenTheCategoryExists(string categoryName)
    {
        // TODO: seed (or look up) a Category row via AppDbContext and capture its Id.
        _categoryId = Guid.NewGuid();
    }

    [When(@"I create a listing with title ""(.*)"", price (.*) and category ""(.*)""")]
    public async Task WhenICreateAListingWithTitlePriceAndCategory(string title, decimal price, string categoryName)
    {
        var dto = new { Title = title, Description = "", Price = price, CategoryId = _categoryId };
        _response = await _client.PostAsJsonAsync("/listings", dto);
    }

    [When(@"I create a listing with title ""(.*)"", price (.*) and an unknown category")]
    public async Task WhenICreateAListingWithTitlePriceAndAnUnknownCategory(string title, decimal price)
    {
        var dto = new { Title = title, Description = "", Price = price, CategoryId = Guid.NewGuid() };
        _response = await _client.PostAsJsonAsync("/listings", dto);
    }

    [Then(@"the response status should be (\d+)")]
    public void ThenTheResponseStatusShouldBe(int statusCode)
    {
        Assert.That(_response.StatusCode, Is.EqualTo((HttpStatusCode)statusCode));
    }

    [Then(@"the created listing should have title ""(.*)""")]
    public async Task ThenTheCreatedListingShouldHaveTitle(string expectedTitle)
    {
        var body = await _response.Content.ReadFromJsonAsync<ListingResponse>();
        Assert.That(body?.Title, Is.EqualTo(expectedTitle));
    }

    [Then(@"the response should contain the message ""(.*)""")]
    public async Task ThenTheResponseShouldContainTheMessage(string expectedMessage)
    {
        var body = await _response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.That(body?.Message, Is.EqualTo(expectedMessage));
    }

    private record ListingResponse(Guid Id, string Title);
    private record ErrorResponse(string Message);
}
