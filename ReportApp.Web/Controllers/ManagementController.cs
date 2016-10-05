using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
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
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.GenderSort = sortOrder == "gender" ? "gender_desc" : "gender";
            ViewBag.UnitSort = sortOrder == "unit" ? "unit_desc" : "unit";
            ViewBag.DepartmentSort = sortOrder == "department" ? "department_desc" : "department";
            ViewBag.EmailSort = sortOrder == "email" ? "email_desc" : "email";
            
            //use in pagination process
            if (searchString != null){
                    page = 1;
            }
            else {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var staffs = _staffRepository.GetProfile;

            if (staffs != null)
            {
                //use for searching
                if (!String.IsNullOrEmpty(searchString))
                {
                    staffs = staffs.Where(s => s.FullName.Contains(searchString) || s.Unit.UnitName.Contains(searchString)
                        || s.Unit.Department.DepartmentName.Contains(searchString) || s.Staff.Email.Contains(searchString));
                }

                //use for sorting
                switch (sortOrder)
                {
                    case "name_desc":
                        staffs = staffs.OrderByDescending(s => s.FullName);
                        break;
                    case "gender":
                        staffs = staffs.OrderBy(s => s.Gender);
                        break;
                    case "gender_desc":
                        staffs = staffs.OrderByDescending(s => s.Gender);
                        break;
                    case "unit_desc":
                        staffs = staffs.OrderByDescending(s => s.Unit.UnitName);
                        break;
                    case "unit":
                        staffs = staffs.OrderBy(s => s.Unit.UnitName);
                        break;
                    case "department_desc":
                        staffs = staffs.OrderByDescending(s => s.Unit.Department.DepartmentName);
                        break;
                    case "department":
                        staffs = staffs.OrderBy(s => s.Unit.Department.DepartmentName);
                        break;
                    case "email_desc":
                        staffs = staffs.OrderByDescending(s => s.Staff.Email);
                        break;
                    case "email":
                        staffs = staffs.OrderBy(s => s.Staff.Email);
                        break;
                    default:
                        staffs = staffs.OrderBy(s => s.FullName);
                        break;
                }
            }

            const int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(staffs.ToPagedList(pageNumber, pageSize));
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
                    //Get previous role assign to the user
                    var userRoles = await UserManager.GetRolesAsync(profile.Staff.Id);
                    selectedRoles = selectedRoles ?? new string[] { }; 

                    //add new roles to the user
                    var result = await UserManager.AddToRolesAsync(profile.Staff.Id, selectedRoles.Except(userRoles).ToArray());
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }

                    //remove previous role from the user
                    result = await UserManager.RemoveFromRolesAsync(profile.Staff.Id, userRoles.Except(selectedRoles).ToArray());
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }
                    return RedirectToAction("Index");

                    //if (selectedRoles != null)
                    //{
                    //    var result = await UserManager.AddToRolesAsync(roleModel.Id, selectedRoles);
                    //    if (!result.Succeeded)
                    //    {
                    //        ModelState.AddModelError("", result.Errors.First());
                    //        return View();
                    //    }
                    //    return RedirectToAction("Index");
                    //}
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
        
        public ActionResult EditAccount(string id)
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
        public async Task<ActionResult> EditAccount(RegisterViewModel staffProfile)
        {
            if (ModelState.IsValid)
            {
                if (IsUnitSelectedUnderDepartment(staffProfile.DepartmentId, staffProfile.UnitId))
                {
                    Profile profile = _staffRepository.GetProfileById(staffProfile.Id);
                    Staff staff = await UserManager.FindByIdAsync(profile.Staff.Id);
                    staff.Email = staffProfile.Email;
                    staff.UserName = staffProfile.Email;
                    staff.PhoneNumber = staffProfile.PhoneNumber;
                    await UserManager.UpdateAsync(staff);
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
        public ActionResult Delete(string id)
        {
            _staffRepository.DeleteProfile(id);
            _staffRepository.Save();
            return RedirectToAction("Index");
        }

        //Get All reports
        public ActionResult Reports(string sortOrder, string searchString, string currentFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSort = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.reportDateSort = sortOrder == "reportDate" ? "reportDate_desc" : "reportDate";
            ViewBag.submissionDateSort = String.IsNullOrEmpty(sortOrder) ? "submissionDate_desc" : "";
            ViewBag.typeSort = sortOrder == "type" ? "type_desc" : "type";

            //use in pagination process
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var reports = _reportRepository.GetReport();

            if (reports != null)
            {
                //use for searching
                if (!String.IsNullOrEmpty(searchString))
                {
                    //search only by FullName
                    reports = reports.Where(s => s.Profile.FullName.Contains(searchString)).OrderByDescending(s => s.SubmissionDate);
                }

                //use for sorting
                switch (sortOrder)
                {
                    case "name":
                        reports = reports.OrderBy(s => s.Profile.FullName);
                        break;
                    case "name_desc":
                        reports = reports.OrderByDescending(s => s.Profile.FullName);
                        break;
                    case "reportDate":
                        reports = reports.OrderBy(s => s.ReportDate);
                        break;
                    case "reportDate_desc":
                        reports = reports.OrderByDescending(s => s.ReportDate);
                        break;
                    case "submissionDate_desc":
                        reports = reports.OrderBy(s => s.SubmissionDate);
                        break;
                    case "type_desc":
                        reports = reports.OrderByDescending(s => s.ReportType);
                        break;
                    case "type":
                        reports = reports.OrderBy(s => s.ReportType);
                        break;
                    default:
                        reports = reports.OrderByDescending(s => s.SubmissionDate);
                        break;
                }
            }

            const int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(reports.ToPagedList(pageNumber, pageSize));

            //old code before sorting and searching added
            //var reports = _reportRepository.GetReport().OrderByDescending(x => x.SubmissionDate).ToList();
            //return View(reports);
        }

        //Get list of reports of a particular staff
        public ActionResult StaffReports(string id)
        {
            Profile profile = _staffRepository.GetProfileById(id);
            ViewBag.User = profile.FullName;

            var reports = _reportRepository.GetReport().Where(x => x.Profile.Staff.Id == id).ToList();
            return View(reports);
        }

        public ActionResult ReportDetails(string id)
        {
            Report report = _reportRepository.GetReportById(id);
            if (report != null)
            {
                return View(report);
            }
            return HttpNotFound();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                    _roleManager = null;
                }
                _departmentRepository.Dispose();
                _reportRepository.Dispose();
                _staffRepository.Dispose();
                _unitRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}