using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ReportApp.Core.Abstract;
using ReportApp.Core.Concrete;
using ReportApp.Core.Entities;

namespace ReportApp.Core.Repository
{
    public class StaffRepository : IStaffRepository
    {
        private readonly EfDbContext _context;

        public StaffRepository(EfDbContext context)
        {
            _context = context;
        }


        public IEnumerable<Profile> GetProfile
        {
            get { return _context.Profiles.ToList(); }
        }
        
        public Staff GetStaff(string staffId)
        {
            var staff = _context.Users.Find(staffId);
            return staff;
        }

        public Profile GetProfileById(string id)
        {
            return _context.Profiles.FirstOrDefault(x => x.Staff.Id == id);
        }

        public Profile GetProfileById(int? id)
        {
            return _context.Profiles.FirstOrDefault(x => x.Id == id);
        }

        public void InsertProfile(Profile profile)
        {
            _context.Profiles.Add(profile);
        }

        public void DeleteProfile(int profileId)
        {
            Profile profile = GetProfileById(profileId);
            _context.Entry(profile.Staff).State = EntityState.Deleted;
            _context.Profiles.Remove(profile);
        }

        public void UpdateProfile(Profile profile)
        {
            _context.Entry(profile).State = EntityState.Modified;
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
