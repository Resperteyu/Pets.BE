using Pets.API.Requests;
using System;

namespace Pets.API.Helpers
{
    public static class SearchHelper
    {
        public static double CalculateDistanceFromSearchLocation(double distanceInMeters, SearchRadiusType searchRadiusType)
        {
            double distance = searchRadiusType switch
            {
                SearchRadiusType.Miles => Math.Round(distanceInMeters * 0.000621371, 1),
                SearchRadiusType.Kilometers => Math.Round(distanceInMeters * 0.001, 1)
            };

            return distance;
        }
    }
}
