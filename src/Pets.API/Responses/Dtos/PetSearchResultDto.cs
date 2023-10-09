namespace Pets.API.Responses.Dtos
{
	public class PetSearchResultDto : PetProfileDto
	{
        public double? DistanceFromSearchLocation { get; set; }
    }
}

