using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace ReportApp.Core.Entities
{
    public class Report
    {
        [Key]
        public int Id { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        [StringLength(100, MinimumLength = 3)]
        [Index]
        public string Title { get; set; }

        [Display(Name = "Details")]
        [AllowHtml]
        public string ReportContent { get; set; }

        [Required]
        [Display(Name = "Report Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReportDate { get; set; }

        [Display(Name = "Submission Date")]
        public DateTime SubmissionDate { get; set; }

        [ForeignKey("Profile")]
        public int ProfileId { get; set; }

        public ReportType ReportType { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual ICollection<AttachedFile> AttachedFile { get; set; }
    }
}