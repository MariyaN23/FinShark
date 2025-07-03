using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Account;

public class RegisterDto
{
    [Required] 
    public string? Username { get; set; }
    
    [Required] 
    [EmailAddress] public string? Email { get; set; }
    
    [Required] 
    // add regex for password validation
    public string? Password { get; set; }
}