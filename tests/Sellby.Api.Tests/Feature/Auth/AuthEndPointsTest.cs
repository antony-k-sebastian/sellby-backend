using Sellby.Api.Features.Auth;
using NUnit.Framework;


namespace Sellby.Api.Tests.Feature.Auth;

[TestFixture]
public class AuthLogicTests
{
    private readonly string[] AllowedDomains = ["mumail.ie"];

    [TestCase("studentemail@mumail.ie", true)]  // valid domain
    [TestCase("studentemail@gmail.com", false)] // different domain
    [TestCase("studentemail@MUMAIL.IE", false)] // case sensitivity
    [TestCase("studentemail", false)]  // not an email

    public void isAllowedDomain_ValidatesEmailDomain(String email, bool expected)
    {
        var result =  AuthLogic.IsAllowedDomain(email, AllowedDomains);
        Assert.That(expected, Is.EqualTo(result));
    }

}
