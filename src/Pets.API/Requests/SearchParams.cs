using System;
namespace Pets.API.Requests
{
	public class SearchParams
	{
        public bool? AvailableForBreeding { get; set; }
        public byte? SexId { get; set; }
        public int? Age { get; set; }
        public byte? TypeId { get; set; }
        public int? BreedId { get; set; }
        public Guid UserId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public SearchRadiusType? SearchRadiusType { get; set; }
        public int? SearchRadius { get; set; }
        public bool? ForSale { get; set; }
        public bool? ForAdoption { get; set; }
        public bool? Missing { get; set; }
    }
}

