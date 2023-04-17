using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pets.Db.Models;

namespace Pets.API.Controllers
{
    [Controller]
  public abstract class BaseController : ControllerBase
  {
    public Account Account => (Account)HttpContext.Items["Account"];
  }
}