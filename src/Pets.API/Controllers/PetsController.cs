using Microsoft.AspNetCore.Mvc;
using Pets.API.Responses.Dtos;
using Pets.API.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly IPetProfileService _petProfileService;
        public PetsController(IPetProfileService petProfileService)
        {
            _petProfileService = petProfileService;
        }

        [HttpGet("{petId:Guid}")]
        public async Task<ActionResult<PetProfileDto>> GetByPetId(Guid petId)
        {
            var petProfile = await _petProfileService.GetByPetId(petId);

            if (petProfile == null)
                return NotFound();

            return Ok(petProfile);
        }

        [HttpGet("owner/{ownerId:Guid}")]
        public async Task<ActionResult<List<PetProfileDto>>> GetByOwnerId(Guid ownerId)
        {
            var petProfiles = await _petProfileService.GetByOwnerId(ownerId);

            //what to return if owner does not exist??

            if (petProfiles == null)
                return NotFound();

            return Ok(petProfiles);
        }
    }
}
