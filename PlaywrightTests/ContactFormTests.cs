namespace PlaywrightTests;

[TestFixture]
public class ContactFormTests : BasePageTest
{
    [SetUp]
    public async Task GoToContactPage()
    {
        await Page.GotoAsync($"{BaseUrl}/Home/Contact");
    }

    [Test]
    public async Task ContactPage_FormIsVisible()
    {
        await Expect(Page.Locator("#contact-form")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ContactPage_AllFieldsArePresent()
    {
        await Expect(Page.Locator("#firstname")).ToBeVisibleAsync();
        await Expect(Page.Locator("#lastname")).ToBeVisibleAsync();
        await Expect(Page.Locator("#phone")).ToBeVisibleAsync();
        await Expect(Page.Locator("#email")).ToBeVisibleAsync();
        await Expect(Page.Locator("#subject")).ToBeVisibleAsync();
        await Expect(Page.Locator("#massege")).ToBeVisibleAsync();
        await Expect(Page.Locator("#sendButton")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ContactPage_EmailField_RejectsInvalidEmail()
    {
        await Page.FillAsync("#email", "not-an-email");
        await Page.ClickAsync("#sendButton");

        var errorSpan = Page.Locator("#email + span.error");
        await Expect(errorSpan).Not.ToBeEmptyAsync();
    }

    [Test]
    public async Task ContactPage_FillingValidEmail_ClearsEmailError()
    {
        // Trigger error first
        await Page.FillAsync("#email", "bad");
        await Page.DispatchEventAsync("#email", "input");

        // Fix it
        await Page.FillAsync("#email", "valid@example.com");
        await Page.DispatchEventAsync("#email", "input");

        var errorSpan = Page.Locator("#email + span.error");
        await Expect(errorSpan).ToBeEmptyAsync();
    }

    [Test]
    public async Task ContactPage_SubjectField_MaxLength_Is200()
    {
        var maxLength = await Page.Locator("#subject").GetAttributeAsync("maxlength");
        Assert.That(maxLength, Is.EqualTo("200"));
    }

    [Test]
    public async Task ContactPage_MessageField_MaxLength_Is600()
    {
        var maxLength = await Page.Locator("#massege").GetAttributeAsync("maxlength");
        Assert.That(maxLength, Is.EqualTo("600"));
    }

    [Test]
    public async Task ContactPage_SendButton_HasCorrectInitialText()
    {
        var text = await Page.Locator("#sendButton .text").InnerTextAsync();
        Assert.That(text.Trim(), Is.EqualTo("verzend"));
    }

    [Test]
    public async Task ContactPage_PhoneField_HasPatternValidation()
    {
        var pattern = await Page.Locator("#phone").GetAttributeAsync("pattern");
        Assert.That(pattern, Is.Not.Null.And.Not.Empty);
    }
}
