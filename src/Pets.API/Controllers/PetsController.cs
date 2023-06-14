using Microsoft.AspNetCore.Mvc;
using Pets.API.Authentication.Service;
using Pets.API.Requests;
using Pets.API.Responses.Dtos;
using Pets.API.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PetsController : BaseController
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
                return NotFound("Pet not found");

            return Ok(petProfile);
        }

        [HttpGet("owner/{ownerId:Guid}")]
        public async Task<ActionResult<List<PetProfileDto>>> GetByOwnerId(Guid ownerId)
        {
            var petProfiles = await _petProfileService.GetByOwnerId(ownerId);

            //what to return if owner does not exist??

            return Ok(petProfiles);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Guid>> Post(CreatePetRequest request)
        {
            try
            {
                var petId = await _petProfileService.CreatePet(request, Account.Id);
                return Ok(petId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Oops, something went wrong...");
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> Put(UpdatePetRequest request)
        {
            var petEntity = await _petProfileService.GetEntityByPetId(request.Id);

            if (petEntity == null)
                return NotFound("Pet not found");

            if (petEntity.OwnerId != Account.Id)
                return Unauthorized("You don't own this pet");

            await _petProfileService.UpdatePet(request, petEntity);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{petId:Guid}")]
        public async Task<ActionResult> Delete(Guid petId)
        {
            var petEntity = await _petProfileService.GetEntityByPetId(petId);

            if (petEntity == null)
                return NotFound("Pet not found");

            if (petEntity.OwnerId != Account.Id)
                return Unauthorized("You don't own this pet");

            await _petProfileService.DeletePet(petEntity);
            return Ok();
        }
    }
}
