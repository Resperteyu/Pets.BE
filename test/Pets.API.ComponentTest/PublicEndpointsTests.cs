using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Pets.API.ComponentTest;

public class PublicEndpointsTests : IClassFixture<TestWebAppFactory>
{
    private readonly TestWebAppFactory _factory;
    public PublicEndpointsTests(TestWebAppFactory factory) => _factory = factory;

    [Theory]
    [InlineData("/Country")]
    [InlineData("/Sex")]
    [InlineData("/PetType")]
    [InlineData("/service-type")]
    public async Task Get_Endpoints_WithAuth_Return_200(string path)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(TestAuthHandler.Scheme);
        var res = await client.GetAsync(path);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}
