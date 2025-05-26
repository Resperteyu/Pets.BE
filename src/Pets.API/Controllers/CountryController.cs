using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Pets.API.Responses.Dtos;
using Pets.API.Services;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountryController(ICountryService countryService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<CountryDto>>> GetAll()
        {
            var countries = await countryService.GetAll();
            return Ok(countries);
        }
    }
}
