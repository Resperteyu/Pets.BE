using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
using Swashbuckle.AspNetCore.Annotations;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LitterController : ControllerBase
    {
        private readonly ILitterService _litterService;
        private readonly IPetProfileService _petprofileService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LitterController> _logger;

        public LitterController(ILitterService litterService,
            IPetProfileService petProfileService,
            UserManager<ApplicationUser> userManager,
            ILogger<LitterController> logger)
        {
            _litterService = litterService;
            _petprofileService = petProfileService;
            _userManager = userManager;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("{id:Guid}")]
        [SwaggerOperation(Summary = "Gets a specific litter by ID.", Description = "Retrieves a litter by its unique ID. Ensures the current user owns the litter.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LitterDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult<LitterDto>> GetById([SwaggerParameter(Description = "ID of the litter to retrieve.", Required = true)] Guid id)
        {            
            var litter = await _litterService.GetById(id);

            if (litter == null)
                return NotFound("Litter not found");

            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            if (litter.Owner.Id != userId)
                return Unauthorized("You don't own this litter");

            return Ok(litter);
        }

        [Authorize]
        [HttpGet("{id:Guid}/view")]
        [SwaggerOperation(Summary = "Gets a specific litter by ID (public view).", Description = "Retrieves a litter by its unique ID. Publicly accessible.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LitterDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult<LitterDto>> GetByIdView([SwaggerParameter(Description = "ID of the litter to retrieve.", Required = true)] Guid id)
        {
            var litter = await _litterService.GetById(id);

            if (litter == null)
                return NotFound("Litter not found");

            return Ok(litter);
        }

        [Authorize]
        [HttpGet("infos")]
        [SwaggerOperation(Summary = "Gets litters for the current user.", Description = "Retrieves a list of litters associated with the currently authenticated user.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PetProfileDto>))] // Should likely be LitterDto
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<PetProfileDto>>> GetLitterInfos()
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var litters = await _litterService.GetLittersView(userId);
            return Ok(litters);
        }

        [Authorize]
        [HttpGet("user/{userId:Guid}")]
        [SwaggerOperation(Summary = "Gets litters for a specific user (public view).", Description = "Retrieves a list of litters for the specified user ID.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PetProfileDto>))] // Should likely be LitterDto
        public async Task<ActionResult<List<PetProfileDto>>> GetLittersView([SwaggerParameter(Description = "ID of the user whose litters to retrieve.", Required = true)] Guid userId)
        {
            var litters = await _litterService.GetLittersView(userId);

            //what to return if owner does not exist??

            return Ok(litters);
        }

        [Authorize]
        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new litter.", Description = "Adds a new litter record. Validates ownership of parent pets.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        public async Task<ActionResult<Guid>> Post([FromBody][SwaggerParameter(Description = "Litter creation details, including MotherPetId and FatherPetId.", Required = true)] CreateLitterRequest request)
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));

            var pet = await _petprofileService.GetByPetId(request.MotherPetId);
            if (pet == null)
            {
                return BadRequest("Mother pet does not exisit");
            }

            if (pet.Owner.Id != userId)
            {
                return Unauthorized("You do not own Mother pet");
            }

            pet = await _petprofileService.GetByPetId(request.FatherPetId);
            if (pet == null)
            {
                return BadRequest("Father pet does not exisit");
            }

            if (pet.Owner.Id != userId)
            {
                return Unauthorized("You do not own Father pet");
            }

            var litterId = await _litterService.CreateLitter(request, userId);
            return Ok(litterId);
        }

        [Authorize]
        [HttpPut]
        [SwaggerOperation(Summary = "Updates an existing litter.", Description = "Modifies the details of an existing litter. Ensures the current user owns the litter.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult> Put([FromBody][SwaggerParameter(Description = "Litter update details.", Required = true)] UpdateLitterRequest request)
        {
            var petEntity = await _litterService.GetEntityById(request.Id);
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));

            if (petEntity == null)
                return NotFound("Litter not found");

            if (petEntity.OwnerId != userId)
                return Unauthorized("You don't own this litter");

            await _litterService.UpdateLitter(request, petEntity);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id:Guid}")]
        [SwaggerOperation(Summary = "Deletes a litter by ID.", Description = "Removes the specified litter record. Ensures the current user owns the litter.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult> Delete([SwaggerParameter(Description = "ID of the litter to delete.", Required = true)] Guid id)
        {
            var litterEntity = await _litterService.GetEntityById(id);
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));

            if (litterEntity == null)
                return NotFound("Litter not found");

            if (litterEntity.OwnerId != userId)
                return Unauthorized("You don't own this litter");

            await _litterService.DeleteLitter(litterEntity);
            return Ok();
        }

        [Authorize]
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Searches for litters.", Description = "Finds litters based on search criteria. Includes user ID for context.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LitterDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<LitterDto>>> Search([FromQuery][SwaggerParameter(Description = "Search criteria for litters.")] SearchLitterParams searchParams)
        {
            //TODO: Location perimeter search
            //TODO: return image url
            searchParams.UserId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var petProfiles = await _litterService.Search(searchParams);

            return Ok(petProfiles);
        }
    }
}
