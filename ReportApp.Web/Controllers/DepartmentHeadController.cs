using System;
using System.Collections.Generic;
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
    //[Authorize(Roles = "Department")]
    public class DepartmentHeadController : Controller
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IDepartment _departmentRepository;
        private readonly IUnit _unitRepository;

        public DepartmentHeadController()
        {
            _staffRepository = new StaffRepository(new EfDbContext());
            _reportRepository = new ReportRepository(new EfDbContext());
            _departmentRepository = new DepartmentRepository(new EfDbContext());
            _unitRepository = new UnitRepository(new EfDbContext());
        }

        public DepartmentHeadController(IStaffRepository staffRepository, IReportRepository reportRepository, IDepartment department, IUnit unit)
        {
            _staffRepository = staffRepository;
            _reportRepository = reportRepository;
            _departmentRepository = department;
            _unitRepository = unit;
        }

        // GET: DepartmentHead
        public ActionResult Index()
        {
            var profile = GetProfile();
            var staff = _staffRepository.GetProfile.Where(x => x.Unit.DepartmentId == profile.Unit.DepartmentId).ToList();
            return View(staff);
        }

        private Profile GetProfile()
        {
            string profileId = User.Identity.GetUserId();
            var profile = _staffRepository.GetProfileById(profileId);
            return profile;
        }

        // GET: DepartmentHead/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DepartmentHead/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DepartmentHead/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: DepartmentHead/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DepartmentHead/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: DepartmentHead/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DepartmentHead/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
