using System.ComponentModel.DataAnnotations;

namespace MessangerBackend.Requests;

public class LoginRequest
{
    [Required(ErrorMessage = "Nickname is required.")]
    public string Nickname { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }
}