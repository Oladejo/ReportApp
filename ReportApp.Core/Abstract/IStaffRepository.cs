using System;
using System.Collections.Generic;
using ReportApp.Core.Entities;

namespace ReportApp.Core.Abstract
{
    public interface IStaffRepository : IDisposable
    {
        IEnumerable<Profile> GetProfile { get; }
        Staff GetStaff(string staffId);
        Profile GetProfileById(string id);
        Profile GetProfileById(int id);
        void InsertProfile(Profile profile);
        void DeleteProfile(int profileId);
        void UpdateProfile(Profile profile);
        void Save();
    }
}
