﻿using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
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

        public ActionResult Report(string sortOrder, string searchString, string currentFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.TitleSort = sortOrder == "title" ? "title_desc" : "title";
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
            var profile = GetProfile();
            var reports = _reportRepository.GetReport().Where(x => x.Profile.Id == profile.Id);

            //use for searching
            if (!String.IsNullOrEmpty(searchString))
            {
                //search only by title or report content
                reports = reports.Where(s => s.ReportContent.Contains(searchString) || s.Title.Contains(searchString)).OrderByDescending(s => s.SubmissionDate);
            }

            //use for sorting
            switch (sortOrder)
            {
                case "title":
                    reports = reports.OrderBy(s => s.Title);
                    break;
                case "title_desc":
                    reports = reports.OrderByDescending(s => s.Title);
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

            const int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(reports.ToPagedList(pageNumber, pageSize));

            //var profile = GetProfile();
            //var reports = _reportRepository.GetReport().Where(x => x.Profile.Id == profile.Id).ToList();
            //return View(reports);
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

        // GET: Staff/Create
        public ActionResult CreateReport()
        {
            return View();
        }

        [HttpPost, ActionName("CreateReport")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,ReportContent,ReportDate,ReportType")] Report report, HttpPostedFileBase[] attachedFile)
        {
            if (ModelState.IsValid)
            {
                Report newReport = new Report
                {
                    ProfileId = GetProfile().Id,
                    EncryptedId = Guid.NewGuid().ToString(),
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

        public ActionResult EditReport(string id)
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
        public ActionResult Edit([Bind(Include = "Id,EncryptedId,Title,ReportContent,ReportDate,ReportType")] Report report, HttpPostedFileBase[] attachedFile)
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

        public ActionResult DeleteReport(string id)
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
        public ActionResult DeleteConfirmed(string id)
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
