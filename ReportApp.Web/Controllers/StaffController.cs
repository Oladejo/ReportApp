using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ReportApp.Core.Abstract;
using ReportApp.Core.Concrete;
using ReportApp.Core.Entities;
using ReportApp.Core.Repository;

namespace ReportApp.Web.Controllers
{
    [Authorize]
    public class StaffController : Controller
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IReportRepository _reportRepository;

        public StaffController()
        {
            _staffRepository = new StaffRepository(new EfDbContext());
            _reportRepository = new ReportRepository(new EfDbContext());
        }

        public StaffController(IStaffRepository staffRepository, IReportRepository reportRepository)
        {
            _staffRepository = staffRepository;
            _reportRepository = reportRepository;
        }


        // GET: Staff
        public ActionResult Index()
        {
            var profile = GetProfile();
            return View(profile);
        }

        private Profile GetProfile()
        {
            string profileId = User.Identity.GetUserId();
            var profile = _staffRepository.GetProfileById(profileId);
            return profile;
        }

        public ActionResult Report()
        {
            var profile = GetProfile();
            var reports = _reportRepository.GetReport().Where(x => x.Profile.Id == profile.Id).ToList();
            return View(reports);
        }

        public ActionResult ReportDetails(int id)
        {
            Report report = _reportRepository.GetReportById(id);
            if (report != null)
            {
                return View(report);
            }
            return HttpNotFound();
        }

        // GET: Staff/Create
        public ActionResult CreateReport()
        {
            return View();
        }

        [HttpPost, ActionName("CreateReport")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,ReportContent,ReportDate,ReportType")] Report report, HttpPostedFileBase[] attachedFile)
        {
            if (ModelState.IsValid)
            {
                Report newReport = new Report
                {
                    ProfileId = GetProfile().Id,
                    Title = report.Title,
                    ReportContent = report.ReportContent,
                    ReportDate = report.ReportDate,
                    ReportType = report.ReportType,
                    SubmissionDate = DateTime.Now
                };
                _reportRepository.InsertReport(newReport);
                _reportRepository.Save();

                //if there is attached document with the report
                foreach (var file in attachedFile)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        _reportRepository.InsertAttachedFile(file, newReport.Id);
                    }
                }
                return RedirectToAction("Report");
            }
            return View(report);
        }

        public ActionResult EditReport(int id)
        {
            Report report = _reportRepository.GetReportById(id);
            if (report != null)
            {
                return View(report);
            }
            return HttpNotFound();
        }

        [HttpPost, ActionName("EditReport")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,ReportContent,ReportDate,ReportType")] Report report, HttpPostedFileBase[] attachedFile)
        {
            if (ModelState.IsValid)
            {
                report.ProfileId = GetProfile().Id;
                report.SubmissionDate = DateTime.Now;
                _reportRepository.UpdateReport(report);
                _reportRepository.Save();

                foreach (var file in attachedFile)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        _reportRepository.InsertAttachedFile(file, report.Id);
                    }
                }
                return RedirectToAction("Report");
            }
            return View(report);
        }


        public ActionResult DeleteReport(int id)
        {
            Report report = _reportRepository.GetReportById(id);
            if (report != null)
            {
                return View(report);
            }
            return HttpNotFound();
        }

        [HttpPost, ActionName("DeleteReport")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _reportRepository.DeleteReport(id);
            _reportRepository.Save();
            return RedirectToAction("Report");
        }

        protected override void Dispose(bool disposing)
        {
            _reportRepository.Dispose();
            _staffRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}
