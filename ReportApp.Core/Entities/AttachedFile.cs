using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportApp.Core.Entities
{
    public class AttachedFile
    {
        [Key]
        public int Id { get; set; }

        public string FileName { get; set; }

        public Byte[] Content { get; set; }

        public string MimeType { get; set; }

        [ForeignKey("Report")]
        public int ReportId { get; set; }

        public virtual Report Report { get; set; }
    }
}
