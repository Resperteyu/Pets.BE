using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Pets.API.ComponentTest;

public class AccountControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly TestWebAppFactory _factory;
    public AccountControllerTests(TestWebAppFactory factory) => _factory = factory;

    private static StringContent Json(object o) =>
        new StringContent(JsonSerializer.Serialize(o), Encoding.UTF8, "application/json");

    private static string RandomEmail() => $"test_{System.Guid.NewGuid():N}@example.com";
    private static string RandomUser() => $"user_{System.Guid.NewGuid():N}";

    [Fact]
    public async Task Register_Should_Return_2xx()
    {
        var client = _factory.CreateClient();
        var payload = new
        {
            email = RandomEmail(),
            username = RandomUser(),
            password = "Password123!",
            role = "PetOwner",
            firstName = "Test",
            lastName = "User",
            address = new
            {
                line1 = "123 Test St",
                city = "Testville",
                postcode = "T35 7AA",
                country = new { code = "GB", name = "United Kingdom" }
            }
        };

        var res = await client.PostAsync("/Account/register", Json(payload));
        Assert.True((int)res.StatusCode >= 200 && (int)res.StatusCode < 300,
            $"Expected 2xx but got {(int)res.StatusCode} {res.StatusCode}: {await res.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task Register_Then_Login_Should_Return_2xx()
    {
        var client = _factory.CreateClient();
        var email = RandomEmail();
        var username = RandomUser();
        var password = "Password123!";

        var reg = new
        {
            email,
            username,
            password,
            role = "PetOwner",
            firstName = "Test",
            lastName = "User",
            address = new
            {
                line1 = "123 Test St",
                city = "Testville",
                postcode = "T35 7AA",
                country = new { code = "GB", name = "United Kingdom" }
            }
        };
        var regRes = await client.PostAsync("/Account/register", Json(reg));
        Assert.True((int)regRes.StatusCode >= 200 && (int)regRes.StatusCode < 300,
            $"Register failed: {(int)regRes.StatusCode} {regRes.StatusCode} {await regRes.Content.ReadAsStringAsync()}");

        var login = new { email, password };
        var loginRes = await client.PostAsync("/Account/login", Json(login));
        Assert.True((int)loginRes.StatusCode >= 200 && (int)loginRes.StatusCode < 300,
            $"Login failed: {(int)loginRes.StatusCode} {loginRes.StatusCode} {await loginRes.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task Login_With_Invalid_Password_Should_Return_400()
    {
        var client = _factory.CreateClient();

        var email = RandomEmail();
        var username = RandomUser();
        var password = "Password123!";

        var reg = new
        {
            email,
            username,
            password,
            role = "PetOwner",
            firstName = "Test",
            lastName = "User",
            address = new
            {
                line1 = "123 Test St",
                city = "Testville",
                postcode = "T35 7AA",
                country = new { code = "GB", name = "United Kingdom" }
            }
        };

        var regRes = await client.PostAsync("/Account/register", Json(reg));
        Assert.True((int)regRes.StatusCode >= 200 && (int)regRes.StatusCode < 300,
            $"Register failed: {(int)regRes.StatusCode} {regRes.StatusCode} {await regRes.Content.ReadAsStringAsync()}");

        var badLogin = new { email, password = "WrongPass!123" };
        var loginRes = await client.PostAsync("/Account/login", Json(badLogin));
        Assert.Equal(HttpStatusCode.BadRequest, loginRes.StatusCode);
        var body = await loginRes.Content.ReadAsStringAsync();
        Assert.Contains("Invalid authentication request", body);
    }

    [Fact]
    public async Task ForgotPassword_Should_Return_2xx()
    {
        var client = _factory.CreateClient();
        // Use a random email; API may return 200 even if not found (to avoid user enumeration)
        var res = await client.PostAsync("/Account/forgot-password", Json(new { email = RandomEmail() }));
        Assert.True((int)res.StatusCode >= 200 && (int)res.StatusCode < 300,
            $"Expected 2xx but got {(int)res.StatusCode} {res.StatusCode}: {await res.Content.ReadAsStringAsync()}");
    }
}
