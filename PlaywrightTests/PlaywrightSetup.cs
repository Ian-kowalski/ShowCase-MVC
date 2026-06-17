using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

// Base URL can be overridden with the BASE_URL environment variable.
// Defaults to the local dev server.
public abstract class BasePageTest : PageTest
{
    protected string BaseUrl =>
        Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5285";
}
