using Newtonsoft.Json.Linq;
using Pets.API.Responses.Dtos;
using System.Net.Http;
using System;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Microsoft.FeatureManagement;
using Pets.API.Config;
using Location = Pets.Db.Models.Location;

namespace Pets.API.Services
{
    public interface IGeocodingService
    {
        Task<Location> CalculateLocationAsync(AddressDto addressDto);
    }

    public class GoogleGeocodingService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GoogleGeocodingService> logger,
        IFeatureManager featureManager)
        : IGeocodingService
    {
        private readonly string _apiKey = configuration["GoogleGeocodingApiKey"];

        public async Task<Location> CalculateLocationAsync(AddressDto addressDto)
        {
            // Check if geocoding API is enabled via feature flag
            var useGeocodingApi = await featureManager.IsEnabledAsync(FeatureFlags.UseGeocodingApi);
            
            if (!useGeocodingApi)
            {
                logger.LogInformation("Geocoding API disabled via feature flag. Returning random London coordinates.");
                return GetRandomLondonLocation();
            }

            try
            {
                var address =
                    $"{addressDto.Line1}, {(string.IsNullOrEmpty(addressDto.Line2) ? "" : addressDto.Line2 + ", ")} {addressDto.Postcode} {addressDto.City}, {addressDto.Country.Code}";

                var targetUrl = $"https://maps.googleapis.com/maps/api/geocode/json" +
                                $"?address={Uri.EscapeDataString(address)}" +
                                $"&inputtype=textquery&fields=geometry" +
                                $"&key={_apiKey}";

                var response = await httpClient.GetAsync(targetUrl);

                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JObject.Parse(jsonString);

                var results = responseObject["results"];

                double? lat = results[0]?["geometry"]?["location"]?["lat"];
                double? lng = results[0]?["geometry"]?["location"]?["lng"];

                var result = new Location
                {
                    Latitude = (decimal)lat.Value,
                    Longitude = (decimal)lng.Value,
                    GeoLocation = new Point(lat.Value, lng.Value) { SRID = 4326 }
                };

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ERROR USING Geocoding Service, falling back to random London coordinates");
                return GetRandomLondonLocation();
            }
        }

        private Location GetRandomLondonLocation()
        {
            // London approximate bounds: 51.2868 to 51.6919 (lat), -0.5104 to 0.3340 (lng)
            var random = new Random();
            var lat = (decimal)(51.2868 + random.NextDouble() * (51.6919 - 51.2868));
            var lng = (decimal)(-0.5104 + random.NextDouble() * (0.3340 - (-0.5104)));

            return new Location
            {
                Latitude = lat,
                Longitude = lng,
                GeoLocation = new Point((double)lng, (double)lat) { SRID = 4326 }
            };
        }
    }
}
