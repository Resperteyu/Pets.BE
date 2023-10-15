using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Pets.API.Responses.Dtos;
using Pets.API.Services;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MateRequestStateController : ControllerBase
    {
        private readonly IMateRequestStateService _mateRequestStateService;
        public MateRequestStateController(IMateRequestStateService mateRequestStateService)
        {
            _mateRequestStateService = mateRequestStateService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MateRequestStateDto>>> GetAll()
        {

            var mateRequestStates = await _mateRequestStateService.GetAll();
            return Ok(mateRequestStates);
        }
    }
}
