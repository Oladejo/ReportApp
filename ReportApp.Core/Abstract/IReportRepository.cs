using System;
using System.Collections.Generic;
using System.Web;
using ReportApp.Core.Entities;

namespace ReportApp.Core.Abstract
{
    public interface IReportRepository : IDisposable
    {
        IEnumerable<Report> GetReport();
        Report GetReportById(int? reportId);
        void InsertReport(Report report);
        void DeleteReport(int reportId);
        void UpdateReport(Report report);
        void Save();
        AttachedFile GetAttachedFile(int id); //Get AttachedFile
        void InsertAttachedFile(HttpPostedFileBase fileBase, int reportId);
    }
}
