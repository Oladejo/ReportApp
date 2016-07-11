using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ReportApp.Web.Models
{
    public class CustomModel
    {
    }

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

}