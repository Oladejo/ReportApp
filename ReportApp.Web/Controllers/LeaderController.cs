using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
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
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            var profile = GetProfile();
            IEnumerable<Profile> staffs = null;
            string role = GetUserRole();
            
            switch (role)
            {
                case "Department":
                    staffs = _staffRepository.GetProfile.Where(x => x.Unit.DepartmentId == profile.Unit.DepartmentId);
                    ViewBag.role = "department";
                    break;
                case "Unit":
                    staffs = _staffRepository.GetProfile.Where(x => x.UnitId == profile.UnitId);
                    ViewBag.role = "unit";
                    break;
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.GenderSort = sortOrder == "gender" ? "gender_desc" : "gender";
            ViewBag.UnitSort = sortOrder == "unit" ? "unit_desc" : "unit";
            ViewBag.DepartmentSort = sortOrder == "department" ? "department_desc" : "department";
            ViewBag.EmailSort = sortOrder == "email" ? "email_desc" : "email";

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

            const int pageSize = 1;
            int pageNumber = (page ?? 1);
            return View(staffs.ToPagedList(pageNumber, pageSize));
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
        public ActionResult Reports(string sortOrder, string searchString, string currentFilter, int? page)
        {
            var profile = GetProfile();
            IEnumerable<Report> reports = null;
            string role = GetUserRole();
            switch (role)
            {
                case "Department":
                    reports =
                        _reportRepository.GetReport()
                            .Where(x => x.Profile.Unit.DepartmentId == profile.Unit.DepartmentId);
                    ViewBag.role = "department";
                    break;
                case "Unit":
                    reports = _reportRepository.GetReport().Where(x => x.Profile.UnitId == profile.UnitId);
                    ViewBag.role = "unit";
                    break;
            }

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
        }

        //Get list of reports of a particular staff
        public ActionResult StaffReports(string id)
        {
            Profile profile = _staffRepository.GetProfileById(id);
            ViewBag.User = profile.FullName;
            var reports = _reportRepository.GetReport().Where(x => x.Profile.Staff.Id == id).ToList();
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
