using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Pets.API.ComponentTest;

public class PetsAuthorizationTests : IClassFixture<TestWebAppFactory>
{
    private readonly TestWebAppFactory _factory;
    public PetsAuthorizationTests(TestWebAppFactory factory) => _factory = factory;

    private static StringContent Json(object o) =>
        new StringContent(JsonSerializer.Serialize(o), Encoding.UTF8, "application/json");

    private static string RandomEmail() => $"test_{System.Guid.NewGuid():N}@example.com";
    private static string RandomUser() => $"user_{System.Guid.NewGuid():N}";

    [Fact]
    public async Task Protected_Endpoints_Without_Token_Return_401()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/Pets/infos");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Protected_Endpoints_With_JWT_Succeed()
    {
        var client = _factory.CreateClient();
        var email = RandomEmail();
        var username = RandomUser();
        const string password = "Password123!";

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

        var loginRes = await client.PostAsync("/Account/login", Json(new { email, password }));
        Assert.InRange((int)loginRes.StatusCode, 200, 299);
        var loginJson = JsonDocument.Parse(await loginRes.Content.ReadAsStringAsync()).RootElement;
        var token = loginJson.GetProperty("token").GetString();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var res = await client.GetAsync("/Pets/infos");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}

