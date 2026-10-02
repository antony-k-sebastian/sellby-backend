using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Sellby.Api.Specs.StepDefinitions.Feature.Authentication;

[Binding]
[Scope(Tag = "api")]
public class APILoginSteps
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private HttpResponseMessage _response = null!;
    private string _email = null!;

    [Given(@"the user provides a valid university email")]
    public void GivenTheUserProvidesAValidUniversityEmail()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseContentRoot(Directory.GetCurrentDirectory()));
        _client = _factory.CreateClient();
        _email = $"student+{Guid.NewGuid():N}@mumail.ie";
    }

    [When(@"they request a login OTP")]
    public async Task WhenTheyRequestALoginOtp()
    {
        _response = await _client.PostAsJsonAsync("/auth/request-otp", new { Email = _email });

        // TODO: full login coverage (verify-otp + token assertion) needs a way to
        // read the OTP that AuthEndpoints.RequestOtp currently only Console.WriteLine's.
        // Once there's a test strategy for that (e.g. reading the OtpCode row via
        // AppDbContext, or a test-only hook), extend this with a "when they verify the
        // OTP" step that posts to /auth/verify-otp.
    }

    [Then(@"the OTP request should succeed")]
    public async Task ThenTheOtpRequestShouldSucceed()
    {
        Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var body = await _response.Content.ReadFromJsonAsync<OtpResponse>();
        Assert.That(body?.Message, Is.EqualTo("Otp is sent."));
    }

    private record OtpResponse(string Message);
}
