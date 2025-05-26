using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Pets.API.Responses.Dtos;
using Pets.API.Services;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("service-type")]
    public class ServiceTypeController(IServiceTypeService serviceTypeService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<ServiceTypeDto>>> GetAll()
        {
            var serviceTypes = await serviceTypeService.GetAll();
            return Ok(serviceTypes);
        }
    }
}
