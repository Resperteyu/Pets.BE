using System.ComponentModel.DataAnnotations;

namespace Pets.API.Requests;

public class ForgotPasswordModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}