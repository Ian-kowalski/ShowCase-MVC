namespace PlaywrightTests;

[TestFixture]
public class HomePageTests : BasePageTest
{
    [Test]
    public async Task HomePage_LoadsSuccessfully()
    {
        await Page.GotoAsync(BaseUrl);
        await Expect(Page).ToHaveTitleAsync(new System.Text.RegularExpressions.Regex("ShowCase_MVC"));
    }

    [Test]
    public async Task HomePage_NavBar_IsVisible()
    {
        await Page.GotoAsync(BaseUrl);
        await Expect(Page.Locator("nav.navbar")).ToBeVisibleAsync();
    }

    [Test]
    public async Task HomePage_NavLinks_ArePresent()
    {
        await Page.GotoAsync(BaseUrl);

        var nav = Page.Locator("nav.navbar");
        await Expect(nav.Locator("a[href='/']")).ToBeVisibleAsync();
        await Expect(nav.Locator("a[href='/Home/Contact']")).ToBeVisibleAsync();
        await Expect(nav.Locator("a[href='/Zeeslag/Start']")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ClickingContactNav_NavigatesToContactPage()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.Locator("a[href='/Home/Contact']").ClickAsync();
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("/Home/Contact"));
    }
}
