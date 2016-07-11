using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using ReportApp.Core.Abstract;
using ReportApp.Core.Concrete;
using ReportApp.Core.Entities;
using System.Web;

namespace ReportApp.Core.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly EfDbContext _context;

        public ReportRepository(EfDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Report> GetReport()
        {
            return _context.Reports.ToList();
        }

        public Report GetReportById(int? reportId)
        {
            return _context.Reports.Find(reportId);
        }

        public void InsertReport(Report report)
        {
            _context.Reports.Add(report);
        }

        public void DeleteReport(int reportId)
        {
            Report report = GetReportById(reportId);
            _context.Reports.Remove(report);
        }

        public void UpdateReport(Report report)
        {
            _context.Entry(report).State = EntityState.Modified;
            RemoveAttachedFile(report.Id);
        }

        public AttachedFile GetAttachedFile(int id)
        {
            var attachFile = _context.AttachedFiles.Find(id);
            return attachFile;
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

        //upload attached file
        public void InsertAttachedFile(HttpPostedFileBase file, int reportId)
        {
            AttachedFile newAttachedFile = new AttachedFile
            {
                MimeType = file.ContentType,
                FileName = file.FileName,
                Content = ConvertToBytes(file),
                ReportId = reportId
            };
            _context.AttachedFiles.Add(newAttachedFile);
            Save();
        }

        private static byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            BinaryReader reader = new BinaryReader(file.InputStream);
            var fileBytes = reader.ReadBytes(file.ContentLength);
            return fileBytes;
        }

        public void RemoveAttachedFile(int? reportId)
        {
            AttachedFile file = _context.AttachedFiles.FirstOrDefault(x => x.ReportId == reportId);
            if (file != null)
            {
                var fileToDelete = _context.AttachedFiles.Where(x => x.ReportId == reportId);
                _context.AttachedFiles.RemoveRange(fileToDelete);
            }
        }

    }
}
