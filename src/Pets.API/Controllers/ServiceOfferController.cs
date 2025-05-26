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
    public class ServiceOfferController(
        IServiceOfferService serviceOfferService,
        UserManager<ApplicationUser> userManager,
        ILogger<ServiceOfferController> logger)
        : ControllerBase
    {
        private readonly ILogger<ServiceOfferController> _logger = logger;

        [Authorize]
        [HttpGet("{serviceOfferId:Guid}")]
        public async Task<ActionResult<PetProfileDto>> Get(Guid serviceOfferId)
        {
            var serviceOffer = await serviceOfferService.Get(serviceOfferId);

            if (serviceOffer == null)
                return NotFound("Service not found");

            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));
            if (serviceOffer.User.Id != userId)
                return Unauthorized("You don't own this service");

            return Ok(serviceOffer);
        }

        [Authorize]
        [HttpGet("infos")]
        public async Task<ActionResult<List<ServiceOfferDto>>> GetServiceOfferInfos()
        {
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));
            var servicesOffer = await serviceOfferService.GetByOwnerId(userId);
            return Ok(servicesOffer);
        }

        [Authorize]
        [HttpGet("user/{userId:Guid}")]
        public async Task<ActionResult<List<ServiceOfferDto>>> GetServicesView(Guid userId)
        {
            var services = await serviceOfferService.GetServicesView(userId);

            //what to return if owner does not exist??

            return Ok(services);
        }

        [Authorize]
        [HttpGet("{serviceOfferId:Guid}/view")]
        public async Task<ActionResult<PetProfileDto>> GetServiceOfferView(Guid serviceOfferId)
        {
            var serviceOffer = await serviceOfferService.GetServiceOfferView(serviceOfferId);

            if (serviceOffer == null)
                return NotFound("Service not found");

            return Ok(serviceOffer);
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<ActionResult<List<ServiceOfferDto>>> Search([FromQuery] SearchServiceOfferParams searchParams)
        {
            //TODO: Location perimeter search
            //TODO: return image url
            searchParams.UserId = Guid.Parse(userManager.GetUserId(HttpContext.User));
            var results = await serviceOfferService.Search(searchParams);

            return Ok(results);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Guid>> Post(CreateServiceOfferRequest request)
        {
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User)); // TODO Insert in context?
            var serviceOfferId = await serviceOfferService.Create(request, userId);
            return Ok(serviceOfferId);
        }
        
        [Authorize]
        [HttpPut]
        public async Task<ActionResult> Put(UpdateServiceOfferRequest request)
        {
            var serviceOfferEntity = await serviceOfferService.GetEntityById(request.Id);
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));

            if (serviceOfferEntity == null)
                return NotFound("Service-offer not found");

            if (serviceOfferEntity.UserId != userId)
                return Unauthorized("You don't own this service-offer");

            await serviceOfferService.Update(request, serviceOfferEntity);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{serviceOfferId:Guid}")]
        public async Task<ActionResult> Delete(Guid serviceOfferId)
        {
            var serviceOfferEntity = await serviceOfferService.GetEntityById(serviceOfferId);
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));

            if (serviceOfferEntity == null)
                return NotFound("Service-offer not found");

            if (serviceOfferEntity.UserId != userId)
                return Unauthorized("You don't own this service-offer");

            await serviceOfferService.Delete(serviceOfferEntity);
            return Ok();
        }
    }
}
