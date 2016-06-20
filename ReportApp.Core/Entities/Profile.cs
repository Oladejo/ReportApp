using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportApp.Core.Entities
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name="Full Name")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public string FullName { get; set; }

        public Gender Gender { get; set; }
        
        [ForeignKey("Unit")]
        public int UnitId { get; set; }
      
        public virtual Unit Unit { get; set; }

        [Required]
        public virtual Staff Staff { get; set; }

        public virtual ICollection<Report> Reports { get; set; }
    }
}