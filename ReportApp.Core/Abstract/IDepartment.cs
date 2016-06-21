using System;
using System.Collections.Generic;
using ReportApp.Core.Entities;

namespace ReportApp.Core.Abstract
{
    public interface IDepartment : IDisposable
    {
        IEnumerable<Department> GetDepartments();
        Department GetDepartmentById(int departmentId);
        void InsertDepartment(Department department);
        void DeleteDepartment(int departmentId);
        void UpdateDepartment(Department department);
        void Save();
    }
}
