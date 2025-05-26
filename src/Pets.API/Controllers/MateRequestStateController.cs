using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Pets.API.Responses.Dtos;
using Pets.API.Services;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("mate-request-state")]
    public class MateRequestStateController(IMateRequestStateService mateRequestStateService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<MateRequestStateDto>>> GetAll()
        {

            var mateRequestStates = await mateRequestStateService.GetAll();
            return Ok(mateRequestStates);
        }
    }
}
