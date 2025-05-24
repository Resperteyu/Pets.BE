using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Pets.API.Responses.Dtos;
using Pets.API.Services;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("mate-request-state")]
    public class MateRequestStateController : ControllerBase
    {
        private readonly IMateRequestStateService _mateRequestStateService;
        public MateRequestStateController(IMateRequestStateService mateRequestStateService)
        {
            _mateRequestStateService = mateRequestStateService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Gets a list of all possible mate request states.", Description = "Retrieves a list of all defined states that a mate request can be in.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MateRequestStateDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<List<MateRequestStateDto>>> GetAll()
        {

            var mateRequestStates = await _mateRequestStateService.GetAll();
            return Ok(mateRequestStates);
        }
    }
}
