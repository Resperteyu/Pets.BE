using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pets.API.Requests;
using Pets.API.Requests.Search;
using Pets.API.Requests.ServiceOffer;
using Pets.API.Responses.Dtos;
using Pets.API.Services;
using Pets.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("service-offer")]
    public class ServiceOfferController : ControllerBase
    {
        private readonly IServiceOfferService _serviceOfferService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ServiceOfferController> _logger;

        public ServiceOfferController(IServiceOfferService serviceOfferService,
            UserManager<ApplicationUser> userManager,
            ILogger<ServiceOfferController> logger)
        {
            _serviceOfferService = serviceOfferService;
            _userManager = userManager;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("{serviceOfferId:Guid}")]
        public async Task<ActionResult<PetProfileDto>> Get(Guid serviceOfferId)
        {
            var serviceOffer = await _serviceOfferService.Get(serviceOfferId);

            if (serviceOffer == null)
                return NotFound("Service not found");

            return Ok(serviceOffer);
        }

        [Authorize]
        [HttpGet("infos")]
        public async Task<ActionResult<List<ServiceOfferDto>>> GetServiceOfferInfos()
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var servicesOffer = await _serviceOfferService.GetByOwnerId(userId);
            return Ok(servicesOffer);
        }

        /*
        [Authorize]
        [HttpGet("user/{userId:Guid}")]
        public async Task<ActionResult<List<PetProfileDto>>> GetPetsView(Guid userId)
        {
            var petProfiles = await _petProfileService.GetPetsView(userId);

            //what to return if owner does not exist??

            return Ok(petProfiles);
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<ActionResult<List<PetProfileDto>>> Search([FromQuery] SearchParams searchParams)
        {
            //TODO: Location perimeter search
            //TODO: return image url
            searchParams.UserId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
            var petProfiles = await _petProfileService.Search(searchParams);

            return Ok(petProfiles);
        }
        */

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Guid>> Post(CreateServiceOfferRequest request)
        {
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User)); // TODO Insert in context?
            var serviceOfferId = await _serviceOfferService.Create(request, userId);
            return Ok(serviceOfferId);
        }
        
        [Authorize]
        [HttpPut]
        public async Task<ActionResult> Put(UpdateServiceOfferRequest request)
        {
            var serviceOfferEntity = await _serviceOfferService.GetEntityById(request.Id);
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));

            if (serviceOfferEntity == null)
                return NotFound("Service-offer not found");

            if (serviceOfferEntity.UserId != userId)
                return Unauthorized("You don't own this service-offer");

            await _serviceOfferService.Update(request, serviceOfferEntity);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{serviceOfferId:Guid}")]
        public async Task<ActionResult> Delete(Guid serviceOfferId)
        {
            var serviceOfferEntity = await _serviceOfferService.GetEntityById(serviceOfferId);
            var userId = Guid.Parse(_userManager.GetUserId(HttpContext.User));

            if (serviceOfferEntity == null)
                return NotFound("Service-offer not found");

            if (serviceOfferEntity.UserId != userId)
                return Unauthorized("You don't own this service-offer");

            await _serviceOfferService.Delete(serviceOfferEntity);
            return Ok();
        }
    }
}
