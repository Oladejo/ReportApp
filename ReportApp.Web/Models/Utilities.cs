using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ReportApp.Core.Abstract;
using ReportApp.Core.Concrete;
using ReportApp.Core.Repository;

namespace ReportApp.Web.Models
{
    public class Utilities : Controller
    {
        private readonly IReportRepository _reportRepository = new ReportRepository(new EfDbContext());

        public ActionResult DownloadAttachedFile(int id)
        {
            var fileToRetrieve = _reportRepository.GetAttachedFile(id);
            return File(fileToRetrieve.Content, fileToRetrieve.MimeType, fileToRetrieve.FileName);
        }
    }
}