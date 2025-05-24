using System;

namespace Pets.API.Requests.Search
{
    public class SearchServiceOfferParams
    {
        public byte? TypeId { get; set; }
        public Guid UserId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool ForCats { get; set; }
        public bool ForDogs { get; set; }
        public SearchRadiusType? SearchRadiusType { get; set; }
        public int? SearchRadius { get; set; }
    }
}

