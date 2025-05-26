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
    public class MateRequestController(
        UserManager<ApplicationUser> userManager,
        EmailService emailService,
        IMateRequestService mateRequestService,
        IPetProfileService petProfileService,
        IMateRequestStateChangeValidator mateRequestStateChangeValidator,
        IUserProfileService userProfileService)
        : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Guid>> Post(CreateMateRequestRequest request)
        {
            var pet = await petProfileService.GetByPetId(request.PetProfileId);
            if (pet == null)
            {
                return BadRequest("Pet not found");
            }
            if (!pet.AvailableForBreeding)
            {
                return BadRequest("Pet is not available for breeding");
            }

            var mate = await petProfileService.GetByPetId(request.PetMateProfileId);
            if (mate == null)
            {
                return BadRequest("Pet mate not found");
            }
            if (!mate.AvailableForBreeding)
            {
                return BadRequest("Pet mate not available for breeding");
            }
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));
            if (mate.Owner.Id != userId)
            {
                return BadRequest("You don't own the pet mate");
            }

            //TO DO: understand when the pets become unavailable? we can just leave
            //this to the owner control without manipulating it during the mating process

            var mateRequestId = await mateRequestService.CreateMateRequest(request, pet.Owner.Id, userId);

            //get email by userid.
            var profile = await userProfileService.GetUserProfile(pet.Owner.Id.ToString());
            emailService.SendMateRequestEmailAsync(profile.Email, mateRequestId);

            return Ok(mateRequestId);
        }

        [Authorize]
        [HttpGet("filter")]
        public async Task<ActionResult<List<MateRequestDto>>> Filter([FromQuery] MateRequestSearchParams mateRequestSearchParams)
        {
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));
            var mateRequests = await mateRequestService.Filter(userId, mateRequestSearchParams);
            return Ok(mateRequests);
        }

        [Authorize]
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<MateRequestDto>> GetById(Guid id)
        {
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));
            var mateRequest = await mateRequestService.GetById(id, userId);
            if (mateRequest == null)
            {
                return BadRequest("Mate request not found");
            }

            if(mateRequest.PetMateOwnerId != userId 
                && mateRequest.PetOwnerId != userId) 
            {
                return Unauthorized("You are not authorised to see this request");
            }

            return Ok(mateRequest);
        }

        [Authorize]
        [HttpPost("reply")]
        public async Task<ActionResult> Reply(PetMateRequestReplyRequest request)
        {
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));
            
            var mateRequest = await mateRequestService.GetById(request.MateRequestId, userId);
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

            var validatorResult = mateRequestStateChangeValidator.ValidateReply(mateRequest, request.MateRequestStateId);
            if (!validatorResult.Result)
            {
                return BadRequest(validatorResult.Message);
            }

            await mateRequestService.UpdateReply(request);

            var profile = await userProfileService.GetUserProfile(mateRequest.PetMateOwnerId.ToString());
            emailService.SendMateRequestStatusChangeEmailAsync(profile.Email, mateRequest.Id);

            return Ok();
        }

        [Authorize]
        [HttpPost("transition")]
        public async Task<ActionResult> Transition(PetMateRequestTransitionRequest request)
        {
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));
            
            var mateRequest = await mateRequestService.GetById(request.MateRequestId, userId);
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

            var validatorResult = mateRequestStateChangeValidator.ValidateTransition(mateRequest, request.MateRequestStateId);
            if(!validatorResult.Result)
            {
                return BadRequest(validatorResult.Message);
            }    

            await mateRequestService.UpdateTransition(request);

            //get email by userid.
            var sendEmailTo = mateRequest.IsRequester ? mateRequest.PetOwnerId : mateRequest.PetMateOwnerId;
            var profile = await userProfileService.GetUserProfile(sendEmailTo.ToString());
            emailService.SendMateRequestStatusChangeEmailAsync(profile.Email, mateRequest.Id);

            return Ok();
        }

        [Authorize]
        [HttpPatch]
        public async Task<ActionResult> Patch(PetMateRequestUpdateRequest request)
        {
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));

            var mateRequest = await mateRequestService.GetById(request.MateRequestId, userId);
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

            await mateRequestService.UpdateDetails(request);

            //get email by userid.
            var profile = await userProfileService.GetUserProfile(mateRequest.PetOwnerId.ToString());
            emailService.SendMateRequestStatusChangeEmailAsync(profile.Email, mateRequest.Id);

            return Ok();
        }
    }
}
