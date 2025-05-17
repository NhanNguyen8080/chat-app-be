using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Service.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Phone number is required!")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; }
    }

    public class AuthenticationResult
    {
        public string Token { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Expires { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Password is required.")]
        [Length(8, 20, ErrorMessage = "Password must be between 8 and 20 characters long!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }


}
