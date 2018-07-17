using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MentoratNetCore.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MentoratNetCore.Data
{
    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>
    , ApplicationUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// SB: Hack "Poor Man's DI" pour adapté code legacy MVC5 à MVCCore
        /// </summary>
        public ApplicationDbContext() : this(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(Startup.ConnectionString).Options)
        {
        }

        /// <summary>
        ///  Usage du "Fluent API" pour configurer le model en overridant OnModelCreating
        /// "Fluent API configuration has the highest precedence and will override conventions and data annotations." - https://docs.microsoft.com/en-us/ef/core/modeling/?view=aspnetcore-2.1
        ///  Code adapté de https://docs.microsoft.com/en-us/aspnet/core/migration/1x-to-2x/identity-2x#add-identityuser-poco-navigation-properties ET https://stackoverflow.com/questions/45863522/ef-core-2-0-identity-adding-navigation-properties
        /// </summary>
        /// <param name="builder"></param>
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

            // SB : Enlèvement pour EFCore. 
            //      Remplacé par la création de la classe MentoresExpertises Et le code ci-dessous pour permettre relation Plusieurs à Plusieurs
            //builder.Entity<Expertise>()
            //    .HasMany(e => e.Mentores)
            //    .WithMany(e => e.Expertises)
            //    .Map(m => m.ToTable("MentoresExpertises").MapLeftKey("No_Expertise_ME").MapRightKey("No_Mentore_ME"));

            // Configur. table de jonction "MentoresExpertises" pour relation plus. à plus.
            builder.Entity<MentoresExpertises>()
                .HasKey(t => new { t.No_Expertise, t.No_Mentore });

            // Configur. table de jonction "MentoratCategorieMentors" pour relation plus. à plus.
            builder.Entity<MentoratCategorieMentors>()
                 .HasKey(t => new { t.NoMentor, t.MentoratCategorieId });


            builder.Entity<Mentore>()
                .Property(e => e.upsize_ts);
                //.IsFixedLength(); //SB: enlever pour EF Core

            builder.Entity<Mentore>()
                .HasMany(e => e.Interventions)
                .WithOne(e => e.Mentore) //SB: remplacement de .WithOptional par .WithOne pour EFCore
                .HasForeignKey(e => e.No_Mentore_Intervention);

            builder.Entity<Mentor>()
                .HasMany(e => e.Interventions)
                .WithOne(e => e.Mentor) //SB: remplacement de .WithOptional par .WithOne pour EFCore
                .HasForeignKey(e => e.No_Mentor_Intervention);


            builder.Entity<ApplicationCategorieUser>()
                //.HasKey(k => k.Id)
                .ToTable("AspNetCategorieUser")
                .Property(p => p.Id);
                //.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);


            builder.Entity<ApplicationUser>()
                .HasMany(e => e.UserRoles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUserRole>()
                .HasOne(e => e.User)
                .WithMany(e => e.UserRoles)
                .HasForeignKey(e => e.UserId);

            builder.Entity<ApplicationUserRole>()
                .HasOne(e => e.Role)
                .WithMany(e => e.UserRoles)
                .HasForeignKey(e => e.RoleId);


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

        public virtual DbSet<MentoresExpertises> MentoresExpertises { get; set; }
    }
}
