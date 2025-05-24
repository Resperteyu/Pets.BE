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
    public class PetTypeController : ControllerBase
    {
        private readonly IPetTypeService _petTypeService;
        public PetTypeController(IPetTypeService petTypeService)
        {
            _petTypeService = petTypeService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Gets a list of all pet types.", Description = "Retrieves a comprehensive list of all pet types available in the system.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PetTypeDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<List<PetTypeDto>>> GetAll()
        {

            var petTypes = await _petTypeService.GetAll();
            return Ok(petTypes);
        }
    }
}
