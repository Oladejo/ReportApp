﻿using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using ReportApp.Core.Entities;

namespace ReportApp.Web.Models
{
    public class InitializedDbContext : IdentityDbContext<Staff>
    {
        public InitializedDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        static InitializedDbContext()
        {
            Database.SetInitializer(new ApplicationDbInitializer());
        }

        public static InitializedDbContext Create()
        {
            return new InitializedDbContext();
        }

        public System.Data.Entity.DbSet<ReportApp.Core.Entities.Report> Reports { get; set; }

        public System.Data.Entity.DbSet<ReportApp.Core.Entities.Profile> Profiles { get; set; }
    }
}