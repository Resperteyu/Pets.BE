using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Pets.API.Responses.Dtos;
using Pets.API.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Identity;
using Pets.Db.Models;
using Pets.API.Helpers;
using Pets.API.Requests.MateRequest;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("mate-request")]
    public class MateRequestController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMateRequestService _mateRequestService;
        private readonly IPetProfileService _petProfileService;
        private readonly IMateRequestStateChangeValidator _mateRequestStateChangeValidator;
        public MateRequestController(UserManager<ApplicationUser> userManager, 
            IMateRequestService mateRequestService, 
            IPetProfileService petProfileService,
            IMateRequestStateChangeValidator mateRequestStateChangeValidator)
        {
            _userManager = userManager;
            _mateRequestService = mateRequestService;
            _petProfileService = petProfileService;
            _mateRequestStateChangeValidator = mateRequestStateChangeValidator;
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
        [HttpGet("filter")]
        public async Task<ActionResult<List<MateRequestDto>>> Filter([FromQuery] MateRequestSearchParams mateRequestSearchParams)
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var mateRequests = await _mateRequestService.Filter(userId, mateRequestSearchParams);
            return Ok(mateRequests);
        }

        [Authorize]
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<MateRequestDto>> GetById(Guid id)
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var mateRequest = await _mateRequestService.GetById(id, userId);
            if (mateRequest == null)
            {
                return BadRequest("Mate request not found");
            }

            if(mateRequest.PetMateProfile.Owner.Id != userId 
                && mateRequest.PetProfile.Owner.Id != userId) 
            {
                return Unauthorized("You are not authorised to see this request");
            }

            return Ok(mateRequest);
        }

        [Authorize]
        [HttpPost("reply")]
        public async Task<ActionResult> Reply(PetMateRequestReplyRequest request)
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            
            var mateRequest = await _mateRequestService.GetById(request.MateRequestId, userId);
            if (mateRequest == null)
            {
                return NotFound("Mate-request not found");
            }
            
            if (!mateRequest.IsReceiver)
            {
                return Unauthorized("You are not authorised to complete this operation");
            }            

            if (string.IsNullOrEmpty(request.Response))
            {
                return BadRequest("Response comment cannot be empty");
            }

            var validatorResult = _mateRequestStateChangeValidator.ValidateReply(mateRequest, request.MateRequestStateId);
            if (!validatorResult.Result)
            {
                return BadRequest(validatorResult.Message);
            }

            await _mateRequestService.UpdateReply(request);

            //TO DO: send email notification
            return Ok();
        }

        [Authorize]
        [HttpPost("transition")]
        public async Task<ActionResult> Transition(PetMateRequestTransitionRequest request)
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            
            var mateRequest = await _mateRequestService.GetById(request.MateRequestId, userId);
            if (mateRequest == null)
            {
                return NotFound("Mate-request not found");
            }
            
            if (mateRequest.IsReceiver == false && mateRequest.IsRequester == false)
            {
                return Unauthorized("You are not authorised to complete this operation");
            }            

            if (string.IsNullOrEmpty(request.Comment))
            {
                return BadRequest("Transition comment cannot be empty");
            }

            var validatorResult = _mateRequestStateChangeValidator.ValidateTransition(mateRequest, request.MateRequestStateId);
            if(!validatorResult.Result)
            {
                return BadRequest(validatorResult.Message);
            }    

            await _mateRequestService.UpdateTransition(request);

            //TO DO: send email notification
            return Ok();
        }

        [Authorize]
        [HttpPatch]
        public async Task<ActionResult> Patch(PetMateRequestUpdateRequest request)
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));

            var mateRequest = await _mateRequestService.GetById(request.MateRequestId, userId);
            if (mateRequest == null)
            {
                return NotFound("Mate-request not found");
            }

            if (mateRequest.IsRequester == false)
            {
                return Unauthorized("You are not authorised to complete this operation");
            }

            if (mateRequest.MateRequestState.Id != MateRequestStateConsts.CHANGES_REQUESTED)
            {
                return BadRequest("Current state of mate-request does not allow this operation " + mateRequest.MateRequestState.Title);
            }

            if (string.IsNullOrEmpty(request.Description)
                || string.IsNullOrEmpty(request.AmountAgreement)
                || string.IsNullOrEmpty(request.LitterSplitAgreement)
                || string.IsNullOrEmpty(request.BreedingPlaceAgreement))
            {
                return BadRequest();
            }

            await _mateRequestService.UpdateDetails(request);

            //TO DO: send email notification
            return Ok();
        }
    }
}
