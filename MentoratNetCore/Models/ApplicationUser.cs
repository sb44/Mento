﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MentoratNetCore.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public override string Id { get; set; }

        public string NomUser { get; set; }
        public string PrenomUser { get; set; }

        public bool ActifUser { get; set; }

        public int? IdCategorieUtilisateur { get; set; }

        [ForeignKey("IdCategorieUtilisateur")]
        public virtual ApplicationCategorieUser CategorieUtilisateur { get; set; }

        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string StatutUser {
            get {
                if (ActifUser) return "Actif";
                return "Inactif";
            }
        }

        public string NomCompletUser {
            get {
                return PrenomUser + " " + NomUser;
            }

        }




        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        [ForeignKey("UserId")]
        public ICollection<ApplicationUserRole> UserRoles { get; set; }



    }

    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    {

        public string NomLong { get; set; }
        public virtual string IdParent { get; set; }

        [ForeignKey("IdParent")]
        public virtual ICollection<ApplicationRole> RolesEnfants { get; set; }

        public virtual ApplicationRole RoleParent { get; set; }

        public int? IdCategorieUtilisateur { get; set; } = 0;

        [ForeignKey("IdCategorieUtilisateur")]
        public virtual ApplicationCategorieUser CategorieUtilisateur { get; set; }
    }

    public class ApplicationCategorieUser
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Description { get; set; }

        public virtual ICollection<ApplicationUser> LstUtilisateurs { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }

    public class ApplicationUserClaim : IdentityUserClaim { }

    public class ApplicationUserLogin : IdentityUserLogin { }

    public class ApplicationUserRole : IdentityUserRole
    {
        [ForeignKey("RoleId")]
        public ApplicationRole Role { get; set; }
    }

    public class ApplicationUserStore :
    UserStore<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>,
    IUserStore<ApplicationUser>,
    IDisposable
    {
        public ApplicationUserStore(ApplicationDbContext context) : base(context) { }
    }

    public class ApplicationRoleStore : RoleStore<ApplicationRole, string, ApplicationUserRole>, IDisposable
    {
        public ApplicationRoleStore(ApplicationDbContext context) : base(context) { }
    }


}
