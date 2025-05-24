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
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("mate-request")]
    public class MateRequestController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailService _emailService;
        private readonly IMateRequestService _mateRequestService;
        private readonly IPetProfileService _petProfileService;
        private readonly IMateRequestStateChangeValidator _mateRequestStateChangeValidator;
        private readonly IUserProfileService _userProfileService;
        public MateRequestController(UserManager<ApplicationUser> userManager,
            EmailService emailService,
            IMateRequestService mateRequestService, 
            IPetProfileService petProfileService,
            IMateRequestStateChangeValidator mateRequestStateChangeValidator,
            IUserProfileService userProfileService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _mateRequestService = mateRequestService;
            _petProfileService = petProfileService;
            _mateRequestStateChangeValidator = mateRequestStateChangeValidator;
            _userProfileService = userProfileService;
        }

        [Authorize]
        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new mate request.", Description = "Initiates a request for mating between two pets. Validates pet existence, breeding availability, and ownership.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Guid>> Post([FromBody][SwaggerParameter(Description = "Details for creating the mate request.", Required = true)] CreateMateRequestRequest request)
        {
            var pet = await _petProfileService.GetByPetId(request.PetProfileId);
            if (pet == null)
            {
                return BadRequest("Pet not found");
            }
            if (!pet.AvailableForBreeding)
            {
                return BadRequest("Pet is not available for breeding");
            }

            var mate = await _petProfileService.GetByPetId(request.PetMateProfileId);
            if (mate == null)
            {
                return BadRequest("Pet mate not found");
            }
            if (!mate.AvailableForBreeding)
            {
                return BadRequest("Pet mate not available for breeding");
            }
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            if (mate.Owner.Id != userId)
            {
                return BadRequest("You don't own the pet mate");
            }

            //TO DO: understand when the pets become unavailable? we can just leave
            //this to the owner control without manipulating it during the mating process

            var mateRequestId = await _mateRequestService.CreateMateRequest(request, pet.Owner.Id, userId);

            //get email by userid.
            var profile = await _userProfileService.GetUserProfile(pet.Owner.Id.ToString());
            _emailService.SendMateRequestEmailAsync(profile.Email, mateRequestId);

            return Ok(mateRequestId);
        }

        [Authorize]
        [HttpGet("filter")]
        [SwaggerOperation(Summary = "Filters mate requests for the current user.", Description = "Retrieves a list of mate requests based on specified search parameters, scoped to the authenticated user.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MateRequestDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<MateRequestDto>>> Filter([FromQuery][SwaggerParameter(Description = "Search parameters for filtering mate requests.")] MateRequestSearchParams mateRequestSearchParams)
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var mateRequests = await _mateRequestService.Filter(userId, mateRequestSearchParams);
            return Ok(mateRequests);
        }

        [Authorize]
        [HttpGet("{id:Guid}")]
        [SwaggerOperation(Summary = "Gets a specific mate request by its ID.", Description = "Retrieves the details of a mate request if the authenticated user is involved (either as pet owner or mate pet owner).")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MateRequestDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))] // For "Mate request not found" before Unauthorized check
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        public async Task<ActionResult<MateRequestDto>> GetById([SwaggerParameter(Description = "ID of the mate request to retrieve.", Required = true)] Guid id)
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var mateRequest = await _mateRequestService.GetById(id, userId);
            if (mateRequest == null)
            {
                return BadRequest("Mate request not found"); // Changed to BadRequest as per original code logic before Unauthorized
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
        [SwaggerOperation(Summary = "Submits a reply to a mate request.", Description = "Allows the receiver of a mate request to reply (e.g., accept, decline) with a comment and change its state.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult> Reply([FromBody][SwaggerParameter(Description = "Reply details for the mate request.", Required = true)] PetMateRequestReplyRequest request)
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

            var profile = await _userProfileService.GetUserProfile(mateRequest.PetMateOwnerId.ToString());
            _emailService.SendMateRequestStatusChangeEmailAsync(profile.Email, mateRequest.Id);

            return Ok();
        }

        [Authorize]
        [HttpPost("transition")]
        [SwaggerOperation(Summary = "Transitions a mate request to a new state.", Description = "Allows either party involved in a mate request to transition it to a new state (e.g., cancel, confirm breeding) with a comment.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult> Transition([FromBody][SwaggerParameter(Description = "Transition details for the mate request.", Required = true)] PetMateRequestTransitionRequest request)
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

            //get email by userid.
            var sendEmailTo = mateRequest.IsRequester ? mateRequest.PetOwnerId : mateRequest.PetMateOwnerId;
            var profile = await _userProfileService.GetUserProfile(sendEmailTo.ToString());
            _emailService.SendMateRequestStatusChangeEmailAsync(profile.Email, mateRequest.Id);

            return Ok();
        }

        [Authorize]
        [HttpPatch]
        [SwaggerOperation(Summary = "Updates details of a mate request.", Description = "Allows the requester to update the details of a mate request if it's in the 'changes requested' state.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult> Patch([FromBody][SwaggerParameter(Description = "Details to update for the mate request.", Required = true)] PetMateRequestUpdateRequest request)
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

            //get email by userid.
            var profile = await _userProfileService.GetUserProfile(mateRequest.PetOwnerId.ToString());
            _emailService.SendMateRequestStatusChangeEmailAsync(profile.Email, mateRequest.Id);

            return Ok();
        }
    }
}
