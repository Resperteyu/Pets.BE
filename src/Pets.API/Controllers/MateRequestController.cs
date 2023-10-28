using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Pets.API.Responses.Dtos;
using Pets.API.Services;
using Microsoft.AspNetCore.Authorization;
using Pets.API.Requests;
using System;
using Microsoft.AspNetCore.Identity;
using Pets.Db.Models;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("mate-request")]
    public class MateRequestController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMateRequestService _mateRequestService;
        private readonly IPetProfileService _petProfileService;
        public MateRequestController(UserManager<ApplicationUser> userManager, 
            IMateRequestService mateRequestService, 
            IPetProfileService petProfileService)
        {
            _userManager = userManager;
            _mateRequestService = mateRequestService;
            _petProfileService = petProfileService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Guid>> Post(CreateMateRequestRequest request)
        {
            var pet = await _petProfileService.GetByPetId(request.PetProfileId);
            if(pet == null)
            {
                return BadRequest("Pet not found");
            }
            if(!pet.AvailableForBreeding)
            {
                return BadRequest("Pet is not available for breeding");
            }

            var mate = await _petProfileService.GetByPetId(request.PetMateProfileId);
            if(mate == null)
            {
                return BadRequest("Pet mate not found");
            }
            if (!mate.AvailableForBreeding)
            {
                return BadRequest("Pet mate not available for breeding");
            }
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            if(mate.Owner.Id != userId) 
            {
                return BadRequest("You don't own the pet mate");
            }

            //TO DO: understand when the pets become unavailable? we can just leave
            //this to the owner control without manipulating it during the mating process

            var mateRequestId = await _mateRequestService.CreateMateRequest(request);
            return Ok(mateRequestId);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<MateRequestDto>>> GetBy([FromQuery] MateRequestSearchParams mateRequestSearchParams)
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var mateRequests = await _mateRequestService.GetBy(userId, mateRequestSearchParams);
            return Ok(mateRequests);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<MateRequestDto>> GetById(Guid id)
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var mateRequest = await _mateRequestService.GetById(id);
            if (mateRequest == null)
            {
                return BadRequest("Mate request not found");
            }

            if(mateRequest.PetMateProfile.Owner.Id != userId 
                || mateRequest.PetProfile.Owner.Id != userId) 
            {
                return Unauthorized("You are not authorised to see this request");
            }

            return Ok(mateRequest);
        }
    }
}
