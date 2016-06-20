using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportApp.Core.Entities
{
    public class Unit
    {
        [Key]
        public int UnitId { get; set; }

        [Required]
        [Display(Name = "Unit")]
        public string UnitName { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public virtual ICollection<Profile> Profile { get; set; }
    }
}