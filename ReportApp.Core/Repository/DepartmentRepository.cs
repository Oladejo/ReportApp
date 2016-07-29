using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ReportApp.Core.Abstract;
using ReportApp.Core.Concrete;
using ReportApp.Core.Entities;

namespace ReportApp.Core.Repository
{
    public class DepartmentRepository : IDepartment
    {
        private readonly EfDbContext _context;

        public DepartmentRepository(EfDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Department> GetDepartments()
        {
            return _context.Departments.ToList();
        }

        public Department GetDepartmentById(int departmentId)
        {
            return _context.Departments.Find(departmentId);
        }

        public void InsertDepartment(Department department)
        {
            _context.Departments.Add(department);
        }

        public void DeleteDepartment(int departmentId)
        {
            Department department = GetDepartmentById(departmentId); 
            _context.Departments.Remove(department);
        }

        public void UpdateDepartment(Department department)
        {
            _context.Entry(department).State = EntityState.Modified;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool _disposed;

        protected virtual void Disposed(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Disposed(true);
            GC.SuppressFinalize(this);
        }
    }
}
