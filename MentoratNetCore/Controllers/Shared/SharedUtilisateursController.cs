using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MentoratNetCore.Models;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Mvc;
//using Mentorat.Extensions;
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MentoratNetCore.Extensions;
using Microsoft.AspNetCore.Identity;
using MentoratNetCore.Data;

namespace Mentorat.Controllers
{

    public class SharedUtilisateursController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IServiceProvider _serviceProvider;

        public SharedUtilisateursController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _serviceProvider = serviceProvider;

        }
    public ActionResult Index()
        {
            return View("erreur!");
        }

        //
        // GET: /Account/Register
      
   
        public ActionResult RechercherUtilisateurs(string nomParent,string parentComplement)
        {       
              switch(nomParent)
            {
                case "ParametresDroits":
                    return PartialView("~/Views/Shared/Utilisateurs/_Rechercherutilisateurs.cshtml", new MentoratNetCore.ViewModels._RechercherUtilisateursViewModels() { RoleExclus=parentComplement,TitreFenetre="Ajouter des utilisateurs",TexteBoutonEnregistrer="Ajouter"});
                default:
                    return PartialView("~/Views/Shared/Utilisateurs/_Rechercherutilisateurs.cshtml",new MentoratNetCore.ViewModels._RechercherUtilisateursViewModels() );
            }
           
         //   return PartialView("~/Views/Shared/Utilisateurs/_Rechercherutilisateurs.cshtml",new Mentorat.ViewModels._RechercherUtilisateursViewModels() { UtisateursExclus=saufUtilisateurs.ToList() });
        }

       
        [Authorize(Roles = "ParametresDroits")]
        public ActionResult Utilisateurs_Read([DataSourceRequest]DataSourceRequest request, string dansRole, string pasDansRole )
        {
            //ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            List<ApplicationUser> lesUsers;

            List<ApplicationUser> lesUsersExclus;

            RoleManager<ApplicationRole> monRoleManager = CscExtensionsMethodes.ObtenirRoleManager(this._serviceProvider);


            if (dansRole!= null && dansRole!=string.Empty)
            {
                //On va chercher seulement les user qui sont dans un role    
                
                lesUsers = CscExtensionsMethodes.ObtenirUsersInRole(dansRole, monRoleManager, _userManager).ToList();
            }
            else
            {
                lesUsers = this._userManager.Users.ToList();
            }
           
            if(pasDansRole!=null && pasDansRole!=string.Empty)
            {
                lesUsersExclus = CscExtensionsMethodes.ObtenirUsersInRole(pasDansRole, monRoleManager, this._userManager).ToList();
                lesUsers=  lesUsers.Except(lesUsersExclus).ToList();
            }

            //if(utilisateursExclus!=null && utilisateursExclus != string.Empty)
            //{
                
            //}

           

            List<MentoratNetCore.ViewModels.RechercherUtilisateursViewModels> lstUtilisateurs =lesUsers.OrderBy(o=> o.PrenomUser).ThenBy(o=>o.NomUser).Select(s => new MentoratNetCore.ViewModels.RechercherUtilisateursViewModels{ Id = s.Id, UserName = s.UserName, Nom = s.NomUser, Prenom = s.PrenomUser, ActifUser = s.ActifUser, Email = s.Email,CategorieUser = s.CategorieUtilisateur.Description }).ToList();

            DataSourceResult results = lstUtilisateurs.ToDataSourceResult(request);

            return Json(results);
        }

        public JsonResult ObtenirUtilisateursDUnRole(string nomRole)
        {
            List<ApplicationUser> lesUsers=null;

            RoleManager<ApplicationRole> monRoleManager = CscExtensionsMethodes.ObtenirRoleManager(this._serviceProvider);

            if (nomRole != null && nomRole != string.Empty)
            {
                //On va chercher seulement les user qui sont dans un role    
                //ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                lesUsers = CscExtensionsMethodes.ObtenirUsersInRole(nomRole, monRoleManager, this._userManager).ToList();
            }

            return Json(lesUsers.Select(s => new MentoratNetCore.ViewModels.ObjBaseVM { Id = s.Id, Nom = s.NomCompletUser }).ToList());
        }

   
        [Authorize(Roles = "ParametresDroits")]
        public async System.Threading.Tasks.Task<JsonResult> Roles_ReadAsync()
        {
            var monContext = new ApplicationDbContext();
            //ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            //string idUserenCours = User.Identity.GetUserId();
            var userEnCours = await _userManager.GetUserAsync(User);

            RoleManager<ApplicationRole> monRoleManager = CscExtensionsMethodes.ObtenirRoleManager(this._serviceProvider);

            List<MentoratNetCore.Models.ParametresDroitsViewModel> rolesPossibleUserEnCours = monRoleManager.Roles.Where(w => w.IdCategorieUtilisateur >= userEnCours.IdCategorieUtilisateur).Select(s => new MentoratNetCore.Models.ParametresDroitsViewModel() { Id = s.Id,Nom=s.Name, NomLong = s.NomLong, IdCategorie = s.IdCategorieUtilisateur }).ToList();

            return Json(rolesPossibleUserEnCours);
        }

        


    }
}