using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Movrr.API.Authentication.Service;
using Movrr.API.Authentication.Service.Models;

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

  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult Reset(ResetPasswordRequest request)
  {
    try
    {
      if (ModelState.IsValid)
      {
        _accountService.ResetPassword(request);
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

  public IActionResult Verify(string token)
  {
    if (string.IsNullOrEmpty(token))
    {
      return BadRequest();
    }

    string message;
    string cssClass;
    try
    {
      _accountService.VerifyEmail(token);
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
