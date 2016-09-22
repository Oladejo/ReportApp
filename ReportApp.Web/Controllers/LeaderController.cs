using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ReportApp.Core.Abstract;
using ReportApp.Core.Concrete;
using ReportApp.Core.Entities;
using ReportApp.Core.Repository;
using ReportApp.Web.CustomAuthorization;

namespace ReportApp.Web.Controllers
{
    [ClaimAuthorization]
    public class LeaderController : Controller
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IReportRepository _reportRepository;

        public LeaderController()
        {
            _staffRepository = new StaffRepository(new EfDbContext());
            _reportRepository = new ReportRepository(new EfDbContext());
        }

        public LeaderController(IStaffRepository staffRepository, IReportRepository reportRepository)
        {
            _staffRepository = staffRepository;
            _reportRepository = reportRepository;
        }

        // GET staffs based on the role assign to the user
        public ActionResult Index()
        {
            var profile = GetProfile();
            List<Profile> staff = null;
            string role = GetUserRole();
            switch (role)
            {
                case "Department":
                    staff = _staffRepository.GetProfile.Where(x => x.Unit.DepartmentId == profile.Unit.DepartmentId).ToList();
                    break;
                case "Unit":
                    staff = _staffRepository.GetProfile.Where(x => x.UnitId == profile.UnitId).ToList();
                    break;
            }
            return View(staff);
        }

        private Profile GetProfile()
        {
            string profileId = User.Identity.GetUserId();
            var profile = _staffRepository.GetProfileById(profileId);
            return profile;
        }

        private string GetUserRole()
        {
            string roleName = null;
            if (HttpContext.User.IsInRole("Department"))
            {
                roleName = "Department";
            }
            else if (HttpContext.User.IsInRole("Unit"))
            {
                roleName = "Unit";
            }
            return roleName;
        }

        //Staff Details by Id
        public ActionResult Details(string id)
        {
            Profile profile = _staffRepository.GetProfileById(id);
            if (profile != null)
            {
                return View(profile);
            }
            return HttpNotFound();
        }

        //Staff reports list
        public ActionResult Reports()
        {
            var profile = GetProfile();
            List<Report> reports = null;
            string role = GetUserRole();
            switch (role)
            {
                case "Department":
                    reports = _reportRepository.GetReport().Where(x => x.Profile.Unit.DepartmentId == profile.Unit.DepartmentId).ToList();
                    break;
                case "Unit":
                    reports = _reportRepository.GetReport().Where(x => x.Profile.UnitId == profile.UnitId).ToList();
                    break;
            }
            return View(reports);
        }

        

        //staff report details
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
            _reportRepository.Dispose();
            _staffRepository.Dispose();
            base.Dispose(disposing);
        }

    }
}
