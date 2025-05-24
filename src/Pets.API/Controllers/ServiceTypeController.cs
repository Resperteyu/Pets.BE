using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Pets.API.Responses.Dtos;
using Pets.API.Services;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("service-type")]
    public class ServiceTypeController : ControllerBase
    {
        private readonly IServiceTypeService _serviceTypeService;
        public ServiceTypeController(IServiceTypeService serviceTypeService)
        {
            _serviceTypeService = serviceTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ServiceTypeDto>>> GetAll()
        {
            var serviceTypes = await _serviceTypeService.GetAll();
            return Ok(serviceTypes);
        }
    }
}
