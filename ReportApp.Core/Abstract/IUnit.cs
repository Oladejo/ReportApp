
using System;
using System.Collections.Generic;
using System.Linq;
using ReportApp.Core.Entities;

namespace ReportApp.Core.Abstract
{
    public interface IUnit : IDisposable
    {
        IEnumerable<Unit> GetUnits();
        IQueryable<Unit> GetUnitListByDepartmentId(int departmentId);
        Unit GetUnitById(int unitId);
        void InsertUnit(Unit unit);
        void DeleteUnit(int unitId);
        void UpdateUnit(Unit unit);
        void Save();
    }
}
