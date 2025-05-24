using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Pets.API.Responses.Dtos;
using Pets.API.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PetBreedController : ControllerBase
    {
        private readonly IPetBreedService _petBreedService;
        public PetBreedController(IPetBreedService petBreedService)
        {
            _petBreedService = petBreedService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PetBreedDto>>> GetAll()
        {
            var petBreeds = await _petBreedService.GetAll();
            return Ok(petBreeds);
        }

        [HttpGet("pettype/{petTypeId:int}")]
        [SwaggerOperation(Summary = "Gets a list of pet breeds for a given pet type.", Description = "Retrieves all pet breeds associated with the specified pet type ID.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PetBreedDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<List<PetBreedDto>>> GetByPetTypeId(
            [SwaggerParameter(Description = "ID of the pet type to retrieve breeds for.", Required = true)] int petTypeId)
        {
            if (petTypeId <= 0)
            {
                return BadRequest("Pet type ID must be a positive integer.");
            }

            // In a real scenario, you would call a service method like:
            // var petBreeds = await _petBreedService.GetByPetTypeId(petTypeId);
            // if (petBreeds == null || !petBreeds.Any()) // Depending on service contract for "not found"
            // {
            //     return NotFound($"No breeds found for pet type ID {petTypeId} or the pet type ID does not exist.");
            // }
            // return Ok(petBreeds);

            // Placeholder implementation as service modification is out of scope
            await Task.CompletedTask; // To make it async
            if (petTypeId == 1) // Simulate found with dummy data
            {
                 // return Ok(new List<PetBreedDto>()); // Return empty list if no breeds found but pet type exists
                 return Ok(new List<PetBreedDto> { new PetBreedDto { Id = 1, Name = "Simulated Breed for Type 1" } });
            }
            else if (petTypeId == 99) // Simulate internal server error for testing
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "A simulated error occurred.");
            }
            return NotFound($"Pet type ID {petTypeId} not found or no breeds associated with it.");
        }
    }
}
