using Microsoft.AspNetCore.Mvc;
using Pets.API.Responses.Dtos;
using Pets.API.Services;
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
    }
}
