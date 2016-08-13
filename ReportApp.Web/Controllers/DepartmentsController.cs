using System.Net;
using System.Web.Mvc;
using ReportApp.Core.Abstract;
using ReportApp.Core.Concrete;
using ReportApp.Core.Entities;
using ReportApp.Core.Repository;
using ReportApp.Web.CustomAuthorization;

namespace ReportApp.Web.Controllers
{
    [CustomAuthorize]
    public class DepartmentsController : Controller
    {
        private readonly IDepartment _departmentRepository;

        public DepartmentsController()
        {
            _departmentRepository = new DepartmentRepository(new EfDbContext());
        }

        public DepartmentsController(IDepartment department)
        {
            _departmentRepository = department;
        }

        // GET: Departments
        public ActionResult Index()
        {
            return View(_departmentRepository.GetDepartments());
        }

        // GET: Departments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = _departmentRepository.GetDepartmentById((int) id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentName")] Department department)
        {
            if (ModelState.IsValid)
            {
                _departmentRepository.InsertDepartment(department);
                _departmentRepository.Save();
                return RedirectToAction("Index");
            }
            return View(department);
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = _departmentRepository.GetDepartmentById((int) id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentId,DepartmentName")] Department department)
        {
            if (ModelState.IsValid)
            {
                _departmentRepository.UpdateDepartment(department);
                _departmentRepository.Save();
                return RedirectToAction("Index");
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = _departmentRepository.GetDepartmentById((int) id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _departmentRepository.DeleteDepartment(id);
            _departmentRepository.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _departmentRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}
