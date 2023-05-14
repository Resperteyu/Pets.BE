using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pets.API.Requests;
using Pets.API.Responses.Dtos;
using Pets.API.Services;
using Pets.Db.Models;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public PetsController(IPetProfileService petProfileService, UserManager<ApplicationUser> userManager)
        {
            _petProfileService = petProfileService;
            _userManager = userManager;
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
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User)); // TODO Insert in context?
            var petId = await _petProfileService.CreatePet(request, userId);
            return Ok(petId);
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> Put(UpdatePetRequest request)
        {
            var petEntity = await _petProfileService.GetEntityByPetId(request.Id);
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));

            if (petEntity == null)
                return NotFound("Pet not found");

            if (petEntity.OwnerId != userId)
                return Unauthorized("You don't own this pet");

            await _petProfileService.UpdatePet(request, petEntity);
            return Ok();
        }
    }
}
