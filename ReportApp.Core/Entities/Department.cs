using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReportApp.Core.Entities
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

        public virtual ICollection<Unit> Unit { get; set; }

    }
}
