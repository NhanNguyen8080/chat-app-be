using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Service.DTOs
{
    public class AccountDTO
    {

        [Required(ErrorMessage = "Phone number is required!")]
        [Length(5, 50, ErrorMessage = "Full name must be between 5 and 50 characters long!")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Date of birth is required!")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format!")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Phone number is required!")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [Length(8, 20, ErrorMessage = "Password must be between 8 and 20 characters long!")]
        public string Password { get; set; }
    }

    public class AccountCM : AccountDTO
    {

        [Required(ErrorMessage = "Phone number is required!")]
        [RegularExpression(@"^\+?[0-9]{10}$", ErrorMessage = "Invalid phone number format!")]
        public string PhoneNumber { get; set; }
    }

    public class AccountUM : AccountDTO
    {

    }

    public class AccountVM : AccountDTO
    {
        public Guid AccountId { get; set; }
        public string Avatar { get; set; }
        public string CoverPhoto { get; set; }
        public string Bio { get; set; }
        public string PhoneNumber { get; set; }
    }

}
