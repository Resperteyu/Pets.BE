using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Movrr.API.Authentication.Service;
using Movrr.API.Authentication.Service.Models;
using System.Threading.Tasks;

public class PasswordController : Controller
{
  private readonly IAccountService _accountService;
  private readonly ILogger<PasswordController> _logger;

  public PasswordController(IAccountService accountService, ILogger<PasswordController> logger)
  {
    _accountService = accountService;
    _logger = logger;
  }

  public IActionResult Reset(string token)
  {
    if (string.IsNullOrEmpty(token))
    {
      return BadRequest();
    };

    return View(new ResetPasswordRequest { Token = token });
  }

  [HttpGet]
  public IActionResult Login()
  {
    return View();
  }
  
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Reset(ResetPasswordRequest request)
  {
    try
    {
      if (ModelState.IsValid)
      {
       await  _accountService.ResetPasswordAsync(request);
        ViewBag.result = true;
        return View(new ResetPasswordRequest());
      }
    }
    catch (Exception e)
    {
      _logger.LogError(new EventId(), e, "Reset Password exception");
    }

    ViewBag.result = false;
    return View(new ResetPasswordRequest());
  }

  public async Task<IActionResult> Verify(string token)
  {
    if (string.IsNullOrEmpty(token))
    {
      return BadRequest();
    }

    string message;
    string cssClass;
    try
    {
      await _accountService.VerifyEmailAsync(token);
      message = "Email verified, you can log in now!";
      cssClass = "display-success";
    }
    catch (Exception e)
    {
      _logger.LogError(new EventId(), e, "Verify email exception");
      message = "Sorry, there's been an error";
      cssClass = "display-error";
    }

    var model = new VerifyEmail
    {
      Message = message,
      CssClass = cssClass
    };

    return View(model);
  }
}
