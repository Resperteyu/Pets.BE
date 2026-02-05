using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Pets.API.ComponentTest;

public class AccountRefreshTokenTests : IClassFixture<TestWebAppFactory>
{
    private readonly TestWebAppFactory _factory;
    public AccountRefreshTokenTests(TestWebAppFactory factory) => _factory = factory;

    private static StringContent Json(object o) =>
        new StringContent(JsonSerializer.Serialize(o), Encoding.UTF8, "application/json");

    private static string RandomEmail() => $"test_{System.Guid.NewGuid():N}@example.com";
    private static string RandomUser() => $"user_{System.Guid.NewGuid():N}";

    [Fact]
    public async Task RefreshToken_HappyPath_Issues_NewTokens()
    {
        var client = _factory.CreateClient();

        var email = RandomEmail();
        var username = RandomUser();
        const string password = "Password123!";

        // Register
        var registerPayload = new
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
        var regRes = await client.PostAsync("/Account/register", Json(registerPayload));
        Assert.InRange((int)regRes.StatusCode, 200, 299);

        // Login
        var loginRes = await client.PostAsync("/Account/login", Json(new { email, password }));
        Assert.InRange((int)loginRes.StatusCode, 200, 299);
        var loginJson = JsonDocument.Parse(await loginRes.Content.ReadAsStringAsync()).RootElement;
        var token = loginJson.GetProperty("token").GetString();
        var refresh = loginJson.GetProperty("refreshToken").GetString();
        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.False(string.IsNullOrWhiteSpace(refresh));

        // Refresh
        var refreshRes = await client.PostAsync("/Account/refresh-token", Json(new { token, refreshToken = refresh }));
        Assert.InRange((int)refreshRes.StatusCode, 200, 299);
        var refreshJson = JsonDocument.Parse(await refreshRes.Content.ReadAsStringAsync()).RootElement;
        var newToken = refreshJson.GetProperty("token").GetString();
        var newRefresh = refreshJson.GetProperty("refreshToken").GetString();

        Assert.False(string.IsNullOrWhiteSpace(newToken));
        Assert.False(string.IsNullOrWhiteSpace(newRefresh));
        Assert.NotEqual(token, newToken);
        Assert.NotEqual(refresh, newRefresh);
    }

    [Fact]
    public async Task RefreshToken_ReusingSameRefreshToken_Fails()
    {
        var client = _factory.CreateClient();

        var email = RandomEmail();
        var username = RandomUser();
        const string password = "Password123!";

        // Register
        var registerPayload = new
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
        var regRes = await client.PostAsync("/Account/register", Json(registerPayload));
        Assert.InRange((int)regRes.StatusCode, 200, 299);

        // Login
        var loginRes = await client.PostAsync("/Account/login", Json(new { email, password }));
        Assert.InRange((int)loginRes.StatusCode, 200, 299);
        var loginJson = JsonDocument.Parse(await loginRes.Content.ReadAsStringAsync()).RootElement;
        var token = loginJson.GetProperty("token").GetString();
        var refresh = loginJson.GetProperty("refreshToken").GetString();

        // First refresh should succeed
        var refreshRes1 = await client.PostAsync("/Account/refresh-token", Json(new { token, refreshToken = refresh }));
        Assert.InRange((int)refreshRes1.StatusCode, 200, 299);

        // Second refresh with the same refresh token should fail (used token)
        var refreshRes2 = await client.PostAsync("/Account/refresh-token", Json(new { token, refreshToken = refresh }));
        Assert.Equal(HttpStatusCode.BadRequest, refreshRes2.StatusCode);
        var body = await refreshRes2.Content.ReadAsStringAsync();
        Assert.Contains("Refresh Token has been used", body);
    }
}

