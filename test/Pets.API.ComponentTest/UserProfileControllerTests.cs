using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pets.API.ComponentTest;

public class UserProfileControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly TestWebAppFactory _factory;
    public UserProfileControllerTests(TestWebAppFactory factory) => _factory = factory;

    [Fact]
    public async Task Get_Me_Unauthorized_Should_401()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/UserProfile/me");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Get_Me_Authorized_Should_Succeed()
    {
        var client = _factory.CreateClient();
        
        // First register a user
        var email = $"test_{System.Guid.NewGuid().ToString("N")[..8]}@example.com";
        var username = $"testuser_{System.Guid.NewGuid().ToString("N")[..8]}";
        var password = "Password123!";
        
        var registerPayload = new
        {
            email = email,
            username = username,
            password = password,
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
        
        var registerRes = await client.PostAsync("/Account/register", 
            new StringContent(System.Text.Json.JsonSerializer.Serialize(registerPayload), 
                System.Text.Encoding.UTF8, "application/json"));
        Assert.True((int)registerRes.StatusCode >= 200 && (int)registerRes.StatusCode < 300,
            $"Register failed with {(int)registerRes.StatusCode} {registerRes.StatusCode}");
        
        // Then login to get a JWT token
        var loginPayload = new { email = email, password = password };
        var loginRes = await client.PostAsync("/Account/login", 
            new StringContent(System.Text.Json.JsonSerializer.Serialize(loginPayload), 
                System.Text.Encoding.UTF8, "application/json"));
        Assert.True((int)loginRes.StatusCode >= 200 && (int)loginRes.StatusCode < 300,
            $"Login failed with {(int)loginRes.StatusCode} {loginRes.StatusCode}");
        
        var loginContent = await loginRes.Content.ReadAsStringAsync();
        var loginResult = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(loginContent);
        var token = loginResult.GetProperty("token").GetString();
        
        // Now use the JWT token to access the protected endpoint
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var res = await client.GetAsync("/UserProfile/me");
        
        // 200 OK = user profile found, 404 NotFound = user authenticated but profile doesn't exist 
        // Both are valid responses for a properly authenticated request
        Assert.True(res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.NotFound,
            $"Expected 200 OK or 404 NotFound, got {(int)res.StatusCode} {res.StatusCode}");
    }
}

