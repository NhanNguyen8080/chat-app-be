using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Repository.Models
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("Avatar", TypeName = "varchar(150)")]
        public string Avatar { get; set; }

        [Column("CoverPhoto", TypeName = "varchar(150)")]
        public string CoverPhoto { get; set; }

        [Column("Bio", TypeName = "nvarchar(500)")]
        public string Bio { get; set; }

        [Column("Password", TypeName = "varchar(256)")]
        public string PasswordHash { get; set; }

        [Column("FullName", TypeName = "nvarchar(30)")]
        public string FullName { get; set; }

        [Column("PhoneNumber", TypeName = "varchar(10)")]
        public string PhoneNumber { get; set; }

        [Column("DateOfBirth")]
        public DateOnly DateOfBirth { get; set; }

        [Column("Gender", TypeName = "varchar(6)")]
        public string Gender { get; set; }
    }
}
