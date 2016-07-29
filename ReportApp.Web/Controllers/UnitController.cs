using System.Web.Mvc;
using ReportApp.Core.Abstract;
using ReportApp.Core.Concrete;
using ReportApp.Core.Entities;
using ReportApp.Core.Repository;
using ReportApp.Web.CustomAuthorization;

namespace ReportApp.Web.Controllers
{
    [CustomAuthorize]
    public class UnitController : Controller
    {
        private readonly IUnit _unit;
        private readonly IDepartment _departmentRepository;

        public UnitController()
        {
            _unit = new UnitRepository(new EfDbContext());
            _departmentRepository = new DepartmentRepository(new EfDbContext());
        }

        public UnitController(IUnit unit, IDepartment department)
        {
            _unit = unit;
            _departmentRepository = department;
        }

        // GET: Unit
        public ActionResult Index()
        {
            return View(_unit.GetUnits());
        }

        // GET: Unit/Details/5
        public ActionResult Details(int? id)
        {
            Unit unit = _unit.GetUnitById((int) id);
            if (unit != null)
            {
                return View(unit);
            }
            return HttpNotFound();
        }

        // GET: Unit/Create
        public ActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }

        // POST: Unit/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UnitName,DepartmentId")] Unit unit)
        {
            if (ModelState.IsValid)
            {
                _unit.InsertUnit(unit);
                _unit.Save();
                return RedirectToAction("Index");
            }
            PopulateDepartmentsDropDownList();
            return View(unit);
        }

        // GET: Unit/Edit/5
        public ActionResult Edit(int id)
        {
            Unit unit = _unit.GetUnitById(id);
            if (unit != null)
            {
                PopulateDepartmentsDropDownList(unit.DepartmentId);
                return View(unit);
            }
            return HttpNotFound();
        }

        // POST: Unit/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UnitId,UnitName,DepartmentId")] Unit unit)
        {
            if (ModelState.IsValid)
            {
                _unit.UpdateUnit(unit);
                _unit.Save();
                return RedirectToAction("Index");
            }
            PopulateDepartmentsDropDownList(unit.DepartmentId);
            return View(unit);
        }

        private void PopulateDepartmentsDropDownList( object selectedDepartment = null)
        {
            var departmentsQuery = _departmentRepository.GetDepartments();
            ViewBag.DepartmentId = new SelectList(departmentsQuery, "DepartmentId", "DepartmentName", selectedDepartment);
        }
        
        // GET: Unit/Delete/5
        public ActionResult Delete(int id)
        {
            Unit unit = _unit.GetUnitById((int) id);
            if (unit != null)
            {
                return View(unit);
            }
            return HttpNotFound();
        }

        // POST: Unit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _unit.DeleteUnit(id);
            _unit.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _unit.Dispose();
            _departmentRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}
