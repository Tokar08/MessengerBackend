using System.ComponentModel.DataAnnotations;

namespace MessangerBackend.Requests;

public class CreateUserRequest
{
    [Required(ErrorMessage = "Nickname is required!")]
    [StringLength(25, MinimumLength = 3, ErrorMessage = "Nickname must be between 3 and 25 characters!")]
    public string Nickname { get; set; }

    [Required(ErrorMessage = "Password is required!")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters!")]
    public string Password { get; set; }
}