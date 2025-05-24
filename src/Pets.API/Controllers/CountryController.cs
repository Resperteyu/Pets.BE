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
    [Route("[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Gets a list of all countries.", Description = "Retrieves a comprehensive list of countries available in the system.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CountryDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<List<CountryDto>>> GetAll()
        {
            var countries = await _countryService.GetAll();
            return Ok(countries);
        }
    }
}
