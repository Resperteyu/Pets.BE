using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Movrr.API.ComponentTest
{
  public class UnitTest1
  {
    [Fact]
    public async Task Test1()
    {
      var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

      var response = await httpClient.GetAsync("/weatherforecast");
      var jsonResponse = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
      var dates = JsonSerializer.Deserialize<List<dynamic>>(jsonResponse);

      Assert.Equal(7, dates.Count);

    }
  }
}
