using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MentoratNetCore.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using MentoratNetCore.Extensions;
using static MentoratNetCore.AuthorizeCustom.Authorize;
using Microsoft.AspNetCore.Mvc;
using MentoratNetCore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MentoratNetCore.Views
{
    public class InscriptionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public InscriptionsController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public ActionResult Index()
        {
           //ViewBag.Message = "Remplissez le formulaire.";           
            var entity = new ViewModels.Inscriptions.InscriptionsMentoreViewModel();
            return View(entity);
        }

        


        [AllowAnonymous]
        [HttpPost]
        public ActionResult VerifierCourrielExiste(string CourrielMentore) // the error i think from email paramater, cause the video said to make the paramater exactly the same name...
        {
         bool existe = false;

            using (var db = new ApplicationDbContext())
            {
                existe = db.Users.Any(a => a.Email == CourrielMentore || a.UserName ==CourrielMentore);
            }

            return Json(!existe);
        }




        public ActionResult Confirmation(Mentore mentore)
        {
            ViewBag.Message = "Détails de la transaction.";
            
            return View(mentore);
        }

        public JsonResult ObtenirExperts()
        {
            return Json(db.Experts.Select(g => new { Nom_Expert = g.Nom_Expert, No_Expert = g.No_Expert }));
        }

        public JsonResult ObtenirExpertises()
        {
            return Json(db.Expertises.Select(g => new { Nom_Expertise = g.Nom_Expertise, No_Expertise = g.No_Expertise }));
        }

        public JsonResult ObtenirMentors()
        {
            return Json(db.Mentors.Select(g => new { NomMentor = g.PrenomMentor + " " + g.NomMentor, NoMentor = g.NoMentor }));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [HttpGet]
        //[Authorize(Roles = "Mentors,GererUtilisateur")]
        [CustomAuthorizeMentoresDossierAttribute]
        public ActionResult InformationsMentore(string utilisateur)
        {
            ApplicationUser monUser = null;

            MentoratNetCore.Extensions.CscExtensionsMethodes.VerifierSiUserExiste(utilisateur, this._userManager, out monUser);

            if(monUser!=null)
            {
                var db = new ApplicationDbContext();
                List<Mentore> lstMentores = db.Mentores.ToList();

                MentoratNetCore.ViewModels.Inscriptions.InformationsMentoreViewModel monMentore = lstMentores.Where(w => w.No_Mentore == monUser.Id).Select(s => new MentoratNetCore.ViewModels.Inscriptions.InformationsMentoreViewModel()
                {
                    //Mentore = new Mentorat.ModelsViews.MentoreViewModel()
                    //{
                    //    NoMentore = s.No_Mentore,
                    //    NomMentore = s.Nom_Mentore,
                    //    PrenomMentore = s.Prenom_Mentore,
                    //    CourrielMentore = s.Courriel_Mentore,
                    //    Organisme_Mentore = s.Organisme_Mentore,
                    //    CellulaireMentore = s.Cellulaire_Mentore,
                    //   TelephoneMentore = s.Telephone_Mentore

                    //},                   
                    NoMentore = s.No_Mentore,
                    NomMentore = s.Nom_Mentore,
                    PrenomMentore = s.Prenom_Mentore,
                    CourrielMentore = s.Courriel_Mentore,
                    Organisme_Mentore = s.Organisme_Mentore,
                    CellulaireMentore = s.Cellulaire_Mentore,
                    TelephoneMentore = s.Telephone_Mentore,
                    UserName = monUser.UserName,
                    UserNameHidden = monUser.UserName,
                    CourrielHidden = s.Courriel_Mentore
                }).FirstOrDefault();



                if (monMentore!=null)
                    return PartialView(monMentore);
              
            }

            //erreur on retourne à la page d'Accueil
            return RedirectToAction("index", "Accueil");
        }

        [HttpPost]
        //[Authorize(Roles = "Mentors,GererUtilisateur")]
        [CustomAuthorizeMentoresDossierAttribute]
        public async Task<ActionResult> InformationsMentore( MentoratNetCore.ViewModels.Inscriptions.InformationsMentoreViewModel model)
        {

            if(ModelState.IsValid)
            {
                var db = new ApplicationDbContext();
                bool boolChangerUtilisateur = false;
                bool boolChangerCourriel = false;
             
                List<Mentore> lstMentores = db.Mentores.ToList();

                Mentore monMentore = lstMentores.FirstOrDefault(f => f.No_Mentore == model.NoMentore);
               

                if (monMentore != null)
                {
                    List<ApplicationUser> lstUsers = db.Users.ToList();
                    ApplicationUser monUser = null;
                    ApplicationUser userVerif = null;

                    monUser = lstUsers.FirstOrDefault(f => f.Id == monMentore.No_Mentore);

                    if (monUser != null)
                    {
                        //s'assurer que le courriel et l'username est en minuscule
                        model.CourrielMentore = model.CourrielMentore.ToLower();
                        model.UserName = model.UserName.ToLower();

                        if ( model.UserName != monUser.UserName)
                        {
                            if (monUser.Email != monUser.UserName)
                            {
                                userVerif = lstUsers.Where(w => w.Id != monUser.Id).FirstOrDefault(f => f.UserName == model.UserName || f.Email == model.UserName);

                                if (userVerif != null)
                                {
                                    //L'user name est déjà présent comme courriel ou utilisateur.
                                    ModelState.AddModelError("UserName", "Ce nom d'utilisateur est déjà utilisé.");
                                }
                                else
                                {
                                    boolChangerUtilisateur = true;
                                }
                            }
                            else
                            {
                                if(!User.IsInRole("GererUtilisateur"))
                                {
                                    //Il est seulement possible d'avoir un nom d'utilisateur différent du courriel si un admin a déjà fait la modification
                                    ModelState.AddModelError("UserName", "Votre nom d'utilisateur ne peut être différent de votre adresse courriel.");
                                }
                                else
                                {
                                    boolChangerUtilisateur = true;
                                }
                               
                            }

                           
                        }

                        if (model.CourrielMentore != monUser.Email)
                        {
                            userVerif = lstUsers.Where(w => w.Id != monUser.Id).FirstOrDefault(f => f.UserName == model.CourrielMentore || f.Email == model.CourrielMentore);

                            if (userVerif != null)
                            {
                                //Le courriel est déjà présent comme courriel ou utilisateur.
                                ModelState.AddModelError("CourrielMentore", "Ce courriel est déjà utilisé.");
                            }
                            else
                            {
                                boolChangerCourriel = true;

                                if(!boolChangerUtilisateur && monUser.Email == monUser.UserName)
                                {
                                    model.UserName = model.CourrielMentore;                                    
                                    boolChangerUtilisateur = true;
                                }
                            }
                        }                                        
                       
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "Erreur!"); //l'utilisateur n'a pas été reconnu
                    }                   


                   if (ModelState.IsValid && monUser !=null)
                    {
                        monMentore.Prenom_Mentore = model.PrenomMentore;
                        monMentore.Nom_Mentore = model.NomMentore;
                        monMentore.Organisme_Mentore = model.Organisme_Mentore;
                        monMentore.Telephone_Mentore = model.TelephoneMentore;
                        monMentore.Cellulaire_Mentore = model.CellulaireMentore;

                        monUser.PrenomUser = model.PrenomMentore;
                        monUser.NomUser = model.NomMentore;

                        if(boolChangerUtilisateur)
                        {                           
                            monUser.UserName = model.UserName;                            
                        }

                        if(boolChangerCourriel)
                        {
                            monMentore.Courriel_Mentore = model.CourrielMentore;
                            monUser.Email = model.CourrielMentore;
                        }
                        db.SaveChanges();


                        if(boolChangerUtilisateur && !User.IsInRole("GererUtilisateur"))
                        {
                            //on déconnecte l'utilisateur.
                            //Microsoft.Owin.IOwinContext ow = Request.GetOwinContext();
                            //Microsoft.Owin.Security.IAuthenticationManager auth = ow.Authentication;
                            //auth.SignOut(DefaultAuthenticationTypes.ApplicationCookie);      
                            await _signInManager.SignOutAsync();
                        }

                        return Json(new { success = true, refresh=boolChangerUtilisateur,userName=monUser.UserName });
                    }                  
                }

            }

            return PartialView(model);
        }


    }
}
