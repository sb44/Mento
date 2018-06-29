using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MentoratNetCore.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace MentoratNetCore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            if (builder == null)
            {
                throw new ArgumentNullException("ModelBuilder is NULL");
            }

            base.OnModelCreating(builder);

            // SB : Enlèvement pour EFCore et création de la classe MentoresExpertises
            //builder.Entity<Expertise>()
            //    .HasMany(e => e.Mentores)
            //    .WithMany(e => e.Expertises)
            //    .Map(m => m.ToTable("MentoresExpertises").MapLeftKey("No_Expertise_ME").MapRightKey("No_Mentore_ME"));
            builder.Entity<MentoresExpertises>()
                .HasKey(t => new { t.ExpertiseId, t.MentoreId });

            builder.Entity<Mentore>()
                .Property(e => e.upsize_ts);
                //.IsFixedLength(); //SB: enlever pour EF Core

            builder.Entity<Mentore>()
                .HasMany(e => e.Interventions)
                .WithOne(e => e.Mentore) //SB: remplacement de .WithOptional par .WithOne pour EFCore3
                .HasForeignKey(e => e.No_Mentore_Intervention);

            builder.Entity<Mentor>()
                .HasMany(e => e.Interventions)
                .WithOne(e => e.Mentor) //SB: remplacement de .WithOptional par .WithOne pour EFCore3
                .HasForeignKey(e => e.No_Mentor_Intervention);

            builder.Entity<ApplicationCategorieUser>()
                .HasKey(k => k.Id)
                .ToTable("AspNetCategorieUser")
                .Property(p => p.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);


            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());

        }

        public virtual DbSet<Expertise> Expertises { get; set; }
        public virtual DbSet<Expert> Experts { get; set; }
        public virtual DbSet<Intervention> Interventions { get; set; }
        public virtual DbSet<Mentore> Mentores { get; set; }
        public virtual DbSet<Mentor> Mentors { get; set; }
        public virtual DbSet<ApplicationCategorieUser> ApplicationCategorieUser { get; set; }
        public virtual DbSet<MentoratCategorie> MentoratCategorie { get; set; }
        public virtual DbSet<MentoratInscription> MentoratInscription { get; set; }

        public virtual DbSet<PlanAction> PlanAction { get; set; }
    }
}
