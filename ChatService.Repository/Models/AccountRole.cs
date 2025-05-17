using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Repository.Models
{
    [Table("AccountRoles")]
    public class AccountRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid AccountId { get; set; }
        public required Account Account { get; set; }

        public int RoleId { get; set; }
        public required Role Role { get; set; }

    }
}
