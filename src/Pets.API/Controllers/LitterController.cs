using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pets.API.Requests.Litter;
using Pets.API.Requests.Search;
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
    public class LitterController(
        ILitterService litterService,
        IPetProfileService petProfileService,
        UserManager<ApplicationUser> userManager,
        ILogger<LitterController> logger)
        : ControllerBase
    {
        private readonly ILogger<LitterController> _logger = logger;

        [Authorize]
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<LitterDto>> GetById(Guid id)
        {            
            var litter = await litterService.GetById(id);

            if (litter == null)
                return NotFound("Litter not found");

            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));
            if (litter.Owner.Id != userId)
                return Unauthorized("You don't own this litter");

            return Ok(litter);
        }

        [Authorize]
        [HttpGet("{id:Guid}/view")]
        public async Task<ActionResult<LitterDto>> GetByIdView(Guid id)
        {
            var litter = await litterService.GetById(id);

            if (litter == null)
                return NotFound("Litter not found");

            return Ok(litter);
        }

        [Authorize]
        [HttpGet("infos")]
        public async Task<ActionResult<List<PetProfileDto>>> GetLitterInfos()
        {
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));
            var litters = await litterService.GetLittersView(userId);
            return Ok(litters);
        }

        [Authorize]
        [HttpGet("user/{userId:Guid}")]
        public async Task<ActionResult<List<PetProfileDto>>> GetLittersView(Guid userId)
        {
            var litters = await litterService.GetLittersView(userId);

            //what to return if owner does not exist??

            return Ok(litters);
        }

        [Authorize]
        [HttpGet("user/me")]
        public async Task<ActionResult<List<PetProfileDto>>> GetLittersViewMe()
        {
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));
            var litters = await litterService.GetLittersView(userId);
            return Ok(litters);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Guid>> Post(CreateLitterRequest request)
        {
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));

            var pet = await petProfileService.GetByPetId(request.MotherPetId);
            if (pet == null)
            {
                return BadRequest("Mother pet does not exisit");
            }

            if (pet.Owner.Id != userId)
            {
                return Unauthorized("You do not own Mother pet");
            }

            pet = await petProfileService.GetByPetId(request.FatherPetId);
            if (pet == null)
            {
                return BadRequest("Father pet does not exisit");
            }

            if (pet.Owner.Id != userId)
            {
                return Unauthorized("You do not own Father pet");
            }

            var litterId = await litterService.CreateLitter(request, userId);
            return Ok(litterId);
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> Put(UpdateLitterRequest request)
        {
            var petEntity = await litterService.GetEntityById(request.Id);
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));

            if (petEntity == null)
                return NotFound("Litter not found");

            if (petEntity.OwnerId != userId)
                return Unauthorized("You don't own this litter");

            await litterService.UpdateLitter(request, petEntity);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var litterEntity = await litterService.GetEntityById(id);
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));

            if (litterEntity == null)
                return NotFound("Litter not found");

            if (litterEntity.OwnerId != userId)
                return Unauthorized("You don't own this litter");

            await litterService.DeleteLitter(litterEntity);
            return Ok();
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<ActionResult<List<LitterDto>>> Search([FromQuery] SearchLitterParams searchParams)
        {
            //TODO: Location perimeter search
            //TODO: return image url
            searchParams.UserId = Guid.Parse(userManager.GetUserId(HttpContext.User));
            var petProfiles = await litterService.Search(searchParams);

            return Ok(petProfiles);
        }
    }
}
