namespace Pets.API.Responses.Dtos.Search
{
    public class PetSearchResultDto : PetProfileDto
    {
        public double? DistanceFromSearchLocation { get; set; }
    }
}

