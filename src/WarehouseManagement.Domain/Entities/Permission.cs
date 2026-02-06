using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Domain.Entities
{
    public class Permission
    {
        public Permission(string functionId, string action)
        {
            FunctionId = functionId;
            Action = action;
        }

        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string FunctionId { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string Action { get; set; } = string.Empty;
        // VIEW, CREATE, UPDATE, DELETE
    }
}
