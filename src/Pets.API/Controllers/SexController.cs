using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Pets.API.Responses.Dtos;
using Pets.API.Services;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SexController(ISexService sexService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<SexDto>>> GetAll()
        {

            var sexes = await sexService.GetAll();
            return Ok(sexes);
        }
    }
}
