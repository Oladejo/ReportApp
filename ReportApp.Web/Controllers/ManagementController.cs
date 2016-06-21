using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using ReportApp.Core.Abstract;
using ReportApp.Core.Concrete;
using ReportApp.Core.Entities;
using ReportApp.Core.Repository;
using ReportApp.Web.Models;

namespace ReportApp.Web.Controllers
{
    public class ManagementController : Controller
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IDepartment _departmentRepository;
        private readonly IUnit _unitRepository;
        private ApplicationUserManager _userManager;

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

        public ManagementController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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

        // GET: management
        public ActionResult Index()
        {
            return View(_staffRepository.GetProfile.ToList());
        }

        public ActionResult CreateAccount()
        {
            ViewBag.Department = new SelectList(_departmentRepository.GetDepartments(), "DepartmentId", "DepartmentName");
            return View();
        }

        //[HttpPost, ActionName("CreateAccount")]
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
    }
}