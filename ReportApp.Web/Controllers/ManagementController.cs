using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ReportApp.Core.Abstract;
using ReportApp.Core.Concrete;
using ReportApp.Core.Entities;
using ReportApp.Core.Repository;
using ReportApp.Web.CustomAuthorization;
using ReportApp.Web.Models;

namespace ReportApp.Web.Controllers
{
    [CustomAuthorize]
    public class ManagementController : Controller
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IDepartment _departmentRepository;
        private readonly IUnit _unitRepository;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public ManagementController()
        {
            _staffRepository = new StaffRepository(new EfDbContext());
            _reportRepository = new ReportRepository(new EfDbContext());
            _departmentRepository = new DepartmentRepository(new EfDbContext());
            _unitRepository = new UnitRepository(new EfDbContext());
        }

        public ManagementController(IStaffRepository staffRepository, IReportRepository reportRepository, IDepartment department, IUnit unit)
        {
            _staffRepository = staffRepository;
            _reportRepository = reportRepository;
            _departmentRepository = department;
            _unitRepository = unit;
        }

        public ManagementController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        // GET: Staff List
        public ActionResult Index()
        {
            return View(_staffRepository.GetProfile.ToList());
        }

        public ActionResult CreateAccount()
        {
            ViewBag.Department = new SelectList(_departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAccount(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                if (IsUnitSelectedUnderDepartment(register.DepartmentId, register.UnitId))
                {
                     var user = new Staff
                    {
                        UserName = register.Email,
                        Email = register.Email,
                        PhoneNumber = register.PhoneNumber
                    };

                    var result = await UserManager.CreateAsync(user, register.Password);
                    if (result.Succeeded)
                    {
                        var newProfile = new Profile
                        {
                            Staff = _staffRepository.GetStaff(user.Id),
                            FullName = register.FullName,
                            UnitId = register.UnitId,
                            Gender = register.Gender
                        };
                        _staffRepository.InsertProfile(newProfile);
                        _staffRepository.Save();
                        return RedirectToAction("Index");
                    }
                }
            }
            ViewBag.Department = new SelectList(_departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName");
            return View();
        }

        public bool IsUnitSelectedUnderDepartment(int departmentId, int unitId)
        {
            var checkUnit = _unitRepository.GetUnits().FirstOrDefault(d => d.DepartmentId == departmentId && d.UnitId == unitId);
            if (checkUnit == null)
            {
                return false;
            }
            return true;
        }

        public JsonResult UnitListByDepartmentId(int id)
        {
            return Json(new SelectList(_unitRepository.GetUnitListByDepartmentId(id).ToArray(), "UnitId", "UnitName"), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> AssignRole(string id)
        {
            var profile = _staffRepository.GetProfileById(id);
            if (profile != null)
            {
                var userRoles = await UserManager.GetRolesAsync(id);
                return View(new AssignRoleModel
                {
                    Id = profile.Staff.Id,
                    Name = profile.FullName,
                    Department = profile.Unit.Department.DepartmentName,
                    Unit = profile.Unit.UnitName,
                    RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                    {
                        Selected = userRoles.Contains(x.Name),
                        Text = x.Name,
                        Value = x.Name
                    })
                });
            }
            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignRole([Bind(Include = "Id")] AssignRoleModel roleModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var profile = _staffRepository.GetProfileById(roleModel.Id);
                if (profile != null)
                {
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(roleModel.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            return View();
                        }
                    }
                }
                return HttpNotFound();
            }
            ModelState.AddModelError("", "Something failed.");
            return RedirectToAction("Index");
        }


        public ActionResult Roles()
        {
            return View(RoleManager.Roles);
        }
        
        public ActionResult EditAccount(int id)
        {
            Profile profile = _staffRepository.GetProfileById(id);
            if (profile != null)
            {
                ViewBag.Department = new SelectList(_departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName");
                return View(new RegisterViewModel()
                {
                    Id = profile.Staff.Id,
                    FullName = profile.FullName,
                    PhoneNumber = profile.Staff.PhoneNumber,
                    Email = profile.Staff.Email,
                    DepartmentId = profile.Unit.DepartmentId,
                    UnitId = profile.UnitId,
                    Gender = profile.Gender
                });
            }
            return HttpNotFound();
        }

        [HttpPost, ActionName("EditAccount")]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount(RegisterViewModel staffProfile)
        {
            if (ModelState.IsValid)
            {
                if (IsUnitSelectedUnderDepartment(staffProfile.DepartmentId, staffProfile.UnitId))
                {
                    Profile profile = _staffRepository.GetProfileById(staffProfile.Id);
                    profile.FullName = staffProfile.FullName;
                    profile.Staff.Email = staffProfile.Email;
                    profile.Staff.UserName = staffProfile.Email;
                    profile.Gender = staffProfile.Gender;
                    profile.UnitId = staffProfile.UnitId;
                    profile.Staff.PhoneNumber = staffProfile.PhoneNumber;
                    _staffRepository.UpdateProfile(profile);
                    _staffRepository.Save();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Department = new SelectList(_departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName");
            return View();
        }

        public ActionResult Details(string id)
        {
            Profile profile = _staffRepository.GetProfileById(id);
            if (profile != null)
            {
                return View(profile);
            }
            return HttpNotFound();
        }

        
        public ActionResult DeleteAccount(string id)
        {
            Profile profile = _staffRepository.GetProfileById(id);
            if (profile != null)
            {
                return View(profile);
            }
            return HttpNotFound();
        }

        [HttpPost, ActionName("DeleteAccount")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccount(int id)
        {
            _staffRepository.DeleteProfile(id);
            _staffRepository.Save();
            return RedirectToAction("Index");
        }

        public ActionResult Reports(string id)
        {
            return View();
        }
    }
}