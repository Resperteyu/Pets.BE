using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Pets.API.Responses.Dtos;
using Pets.API.Services;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SexController : ControllerBase
    {
        private readonly ISexService _sexService;
        public SexController(ISexService sexService)
        {
            _sexService = sexService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<SexDto>>> GetAll()
        {

            var sexes = await _sexService.GetAll();
            return Ok(sexes);
        }
    }
}
