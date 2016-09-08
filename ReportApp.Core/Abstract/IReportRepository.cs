using System;
using System.Collections.Generic;
using System.Web;
using ReportApp.Core.Entities;

namespace ReportApp.Core.Abstract
{
    public interface IReportRepository : IDisposable
    {
        IEnumerable<Report> GetReport();
        Report GetReportById(string reportId);
        void InsertReport(Report report);
        void DeleteReport(string reportId);
        void UpdateReport(Report report);
        void Save();
        AttachedFile GetAttachedFile(int id);
        void InsertAttachedFile(HttpPostedFileBase fileBase, int reportId);
    }
}
