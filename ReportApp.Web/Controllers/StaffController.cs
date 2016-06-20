using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        public ActionResult ReportDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = _reportRepository.GetReportById(id);
            if (report == null)
            {
                return HttpNotFound();
            }
            return View(report);
        }

        public ActionResult DownloadAttachedFile(int id)
        {
            var fileToRetrieve = _reportRepository.GetAttachedFile(id);
            return File(fileToRetrieve.Content, fileToRetrieve.MimeType, fileToRetrieve.FileName);
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
        

        //under Thinking if I will allow a staff to edit, delete his/her report
        public ActionResult EditReport(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = _reportRepository.GetReportById(id);
            if (report == null)
            {
                return HttpNotFound();
            }
            return View(report);
        }

        [HttpPost, ActionName("EditReport")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,ReportDate,SubmissionDate,ProfileId,ReportType")] Report report)
        {
            if (ModelState.IsValid)
            {
                _reportRepository.UpdateReport(report);
                _reportRepository.Save();
                return RedirectToAction("Index");
            }
            return View(report);
        }


        public ActionResult DeleteReport(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = _reportRepository.GetReportById(id);
            if (report == null)
            {
                return HttpNotFound();
            }
            return View(report);
        }

        [HttpPost, ActionName("DeleteReport")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _reportRepository.DeleteReport(id);
            _reportRepository.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _reportRepository.Dispose();
            _staffRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}
