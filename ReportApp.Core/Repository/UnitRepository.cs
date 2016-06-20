using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ReportApp.Core.Abstract;
using ReportApp.Core.Concrete;
using ReportApp.Core.Entities;

namespace ReportApp.Core.Repository
{
    public class UnitRepository : IUnit
    {
        private readonly EfDbContext _context;

        public UnitRepository(EfDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Unit> GetUnits()
        {
            return _context.Units.Include(x => x.Department).ToList();
        }
        
        public IQueryable<Unit> GetUnitListByDepartmentId(int departmentId)
        {
            var entity = (from x in _context.Units where x.DepartmentId == departmentId select x);
            return entity;
        }

        public Unit GetUnitById(int unitId)
        {
            return _context.Units.Find(unitId);
        }

        public void InsertUnit(Unit unit)
        {
            _context.Units.Add(unit);
        }

        public void DeleteUnit(int unitId)
        {
            Unit unit = GetUnitById(unitId);
            _context.Units.Remove(unit);
        }

        public void UpdateUnit(Unit unit)
        {
            _context.Entry(unit).State = EntityState.Modified;
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