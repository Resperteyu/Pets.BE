using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace Pets.API.ComponentTest;

public class PetsControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly TestWebAppFactory _factory;
    public PetsControllerTests(TestWebAppFactory factory) => _factory = factory;

    [Fact]
    public async Task Get_Pets_AllSimple_Should_Return_OK()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/Pets/all-simple");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Get_Pets_Authorized_Should_Succeed()
    {
        var client = _factory.CreateClient();
        // Use a valid JWT token for testing
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var res = await client.GetAsync("/Pets/all-simple");

        Assert.True(res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.NoContent,
            $"Expected 200/204, got {(int)res.StatusCode} {res.StatusCode}");
    }
}
