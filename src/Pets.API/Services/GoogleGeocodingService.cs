using Newtonsoft.Json.Linq;
using Pets.API.Responses.Dtos;
using System.Net.Http;
using System;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
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
        ILogger<GoogleGeocodingService> logger)
        : IGeocodingService
    {
        private readonly string _apiKey = configuration["GoogleGeocodingApiKey"];

        public async Task<Location> CalculateLocationAsync(AddressDto addressDto)
        {
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
                logger.LogError("ERROR USING Geocoding Service", ex);
                throw;
            }
        }
    }
}
