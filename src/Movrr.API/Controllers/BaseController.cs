using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetDb.Models;

namespace movrr.Controllers
{
  [Controller]
  public abstract class BaseController : ControllerBase
  {
    public Account Account => (Account)HttpContext.Items["Account"];
  }
}