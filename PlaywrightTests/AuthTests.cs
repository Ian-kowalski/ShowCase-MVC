namespace PlaywrightTests;

[TestFixture]
public class AuthTests : BasePageTest
{
    [Test]
    public async Task ZeeslagStart_WhenNotLoggedIn_RedirectsToLogin()
    {
        await Page.GotoAsync($"{BaseUrl}/Zeeslag/Start");
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("/Identity/Account/Login"));
    }

    [Test]
    public async Task AdminPage_WhenNotLoggedIn_RedirectsToLogin()
    {
        await Page.GotoAsync($"{BaseUrl}/Home/Admin");
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("/Identity/Account/Login"));
    }

    [Test]
    public async Task LoginPage_HasEmailAndPasswordFields()
    {
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");
        await Expect(Page.Locator("input[type='email'], input[name='Input.Email']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[type='password']")).ToBeVisibleAsync();
    }

    [Test]
    public async Task RegisterPage_IsAccessible()
    {
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Register");
        await Expect(Page).ToHaveTitleAsync(new System.Text.RegularExpressions.Regex("Register"));
    }

    [Test]
    public async Task ContactPage_IsAccessibleWithoutLogin()
    {
        var response = await Page.GotoAsync($"{BaseUrl}/Home/Contact");
        Assert.That(response?.Status, Is.EqualTo(200));
    }

    [Test]
    public async Task HomePage_IsAccessibleWithoutLogin()
    {
        var response = await Page.GotoAsync(BaseUrl);
        Assert.That(response?.Status, Is.EqualTo(200));
    }
}
