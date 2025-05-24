using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Pets.API.Helpers
{
    public class StubbedHttpClientHandler : HttpClientHandler
    {
        private static readonly Random _random = new();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Geocoding API
            if (request.RequestUri.ToString().Contains("maps.googleapis.com/maps/api/geocode"))
            {
                var stubbedResponseContent = GenerateStubbedLocation();
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(stubbedResponseContent, Encoding.UTF8, "application/json")
                };
                return await Task.FromResult(response);
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Request not stubbed")
            };
        }

        // As we are just using the lat and long, just replace those within a range of them within
        // more or less London. We can adjust and use other areas even based on the request
        private string GenerateStubbedLocation()
        {
            var (lat, lng) = GenerateRandomCoordinates();
            return $@"
                {{
                    ""results"": [
                        {{
                            ""address_components"": [
                                {{""long_name"": ""60"", ""short_name"": ""60"", ""types"": [""street_number""]}},
                                {{""long_name"": ""Palermo Road"", ""short_name"": ""Palermo Rd"", ""types"": [""route""]}},
                                {{""long_name"": ""London"", ""short_name"": ""London"", ""types"": [""postal_town""]}},
                                {{""long_name"": ""Greater London"", ""short_name"": ""Greater London"", ""types"": [""administrative_area_level_2"", ""political""]}},
                                {{""long_name"": ""England"", ""short_name"": ""England"", ""types"": [""administrative_area_level_1"", ""political""]}},
                                {{""long_name"": ""United Kingdom"", ""short_name"": ""GB"", ""types"": [""country"", ""political""]}},
                                {{""long_name"": ""NW10 5YP"", ""short_name"": ""NW10 5YP"", ""types"": [""postal_code""]}}
                            ],
                            ""formatted_address"": ""60 Palermo Rd, London NW10 5YP, UK"",
                            ""geometry"": {{
                                ""bounds"": {{
                                    ""northeast"": {{""lat"": 51.5332961, ""lng"": -0.2369711}},
                                    ""southwest"": {{""lat"": 51.5331476, ""lng"": -0.2370498}}
                                }},
                                ""location"": {{
                                    ""lat"": {lat},
                                    ""lng"": {lng}
                                }},
                                ""location_type"": ""ROOFTOP"",
                                ""viewport"": {{
                                    ""northeast"": {{""lat"": 51.5346160802915, ""lng"": -0.235661469708498}},
                                    ""southwest"": {{""lat"": 51.53191811970851, ""lng"": -0.2383594302915021}}
                                }}
                            }},
                            ""place_id"": ""ChIJ03nO4coRdkgRctoRGDK5qtg"",
                            ""types"": [""premise""]
                        }}
                    ],
                    ""status"": ""OK""
                }}";
        }


        private (double latitude, double longitude) GenerateRandomCoordinates()
        {
            double minLatitude = 51.30;
            double maxLatitude = 51.70;
            double minLongitude = -0.50;
            double maxLongitude = 0.20;

            double latitude = minLatitude + (_random.NextDouble() * (maxLatitude - minLatitude));
            double longitude = minLongitude + (_random.NextDouble() * (maxLongitude - minLongitude));

            return (latitude, longitude);
        }
    }
}
