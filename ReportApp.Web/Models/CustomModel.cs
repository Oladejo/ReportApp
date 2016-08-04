using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ReportApp.Core.Entities;

namespace ReportApp.Web.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }
    }

    public class AssignRoleModel
    {
        public string Id { get; set; }

        [Display(Name = "Full name")]
        public string Name { get; set; }

        public string Unit { get; set; }

        public string Department { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }
    }

    public class StaffProfile
    {
        [Required]
        [Display(Name = "Full Name")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Required]
        [Display(Name = "Unit")]
        public int UnitId { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public Gender Gender { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }
    }

}