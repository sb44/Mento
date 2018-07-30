using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MentoratNetCore.Models;
using MentoratNetCore.Services;
using MentoratNetCore.Data;
using Microsoft.Extensions.Configuration;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using static MentoratNetCore.AuthorizeCustom.Authorize;
using Microsoft.AspNetCore.Http;
using MentoratNetCore.Extensions;
using System.Web;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.EntityFrameworkCore;
using MentoratNetCore.ViewModels.Account;

namespace MentoratNetCore.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        [TempData]
        public string StatusMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                //  var user = await _userManager.FindAsync(model.UserName, model.Password);
                var user = await _userManager.FindByNameAsync(model.UserName);
                Microsoft.AspNetCore.Identity.SignInResult result = null;

                if (user != null)
                {
                    result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result != null && result.Succeeded)
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mot de passe invalide.");
                    }
                    // await SignInAsync(user, model.RememberMe);
                    // return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Code d'utilisateur invalide.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }






        //
        // GET: /Account/Register
        [Authorize(Roles = "GererUtilisateur")]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [Authorize(Roles = "GererUtilisateur")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //donner les droits identique d'un autre user              

                var user = new ApplicationUser { UserName = model.UserName, PrenomUser = model.Prenom, NomUser = model.Nom, Email = model.Email, IdCategorieUtilisateur = model.idCategorieUtilisateur, ActifUser = true };


                IdentityResult result = IdentityResult.Failed();

                try
                {
                    result = await _userManager.CreateAsync(user, model.Password);
                }
                catch (Exception er)
                {
                    ModelState.AddModelError("", er.InnerException.InnerException.Message.ToString());
                }


                if (result.Succeeded)
                {
                    await DupliqerDroits(model, user);

                    //await SignInAsync(user, isPersistent: false);
                    await _signInManager.SignInAsync(user, isPersistent: false);


                    return RedirectToAction("Index", "Accueil");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> InscriptionPhyto(ViewModels.Inscriptions.InscriptionsMentoreViewModel mentore)
        {

            if (ModelState.IsValid)
            {
                var db = new ApplicationDbContext();
                var newUser = new ApplicationUser { UserName = mentore.CourrielMentore, PrenomUser = mentore.PrenomMentore, NomUser = mentore.NomMentore, Email = mentore.CourrielMentore, IdCategorieUtilisateur = 4, ActifUser = true };


                IdentityResult result = IdentityResult.Failed();

                try
                {
                    //var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();         
                    result = await _userManager.CreateAsync(newUser, mentore.Password);
                }
                catch (Exception er)
                {
                    ModelState.AddModelError("", er.InnerException.InnerException.Message.ToString());
                }


                if (result.Succeeded)
                {
                    //creer le mentoré
                    var entity = new Mentore
                    {
                        No_Mentore = newUser.Id,
                        Prenom_Mentore = mentore.PrenomMentore,
                        Nom_Mentore = mentore.NomMentore,
                        Organisme_Mentore = mentore.Organisme_Mentore,
                        Courriel_Mentore = mentore.CourrielMentore,
                        Telephone_Mentore = mentore.TelephoneMentore,
                        Cellulaire_Mentore = mentore.CellulaireMentore,
                        No_Expert_Mentore = 1,
                        No_Mentor_Mentore = "1",
                        Objectifs_Mentore = mentore.Objectifs_Mentore,
                        Paye_Mentore = false,
                        DateInscription_Mentore = DateTime.Now
                    };


                    if (mentore.MentoresExpertises != null)
                    {
                        foreach (int intNoExpertise in mentore.MentoresExpertises)
                        {
                            var entityExp = new Expertise { No_Expertise = intNoExpertise };
                            if (entityExp != null)
                            {
                                db.Expertises.Attach(entityExp);

                                //entity.Expertises.Add(entityExp); //SB: Enlève
                                //SB: Ajout
                                entity.MentoresExpertises.Add(
                                    new MentoresExpertises()
                                    {
                                        Expertise = entityExp,
                                        Mentore = entity
                                    });
                            }
                        }
                    }

                    db.Mentores.Attach(entity);
                    // db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Added;


                    // db.Mentores.Add(entity);

                    //créer l'inscription

                    var inscription = new MentoratInscription()
                    {
                        //Annee = int.Parse(System.Configuration.ConfigurationManager.AppSettings["AnneeMentorat"]),
                        Annee = int.Parse(_configuration["AppSettings:AnneeMentorat"]),
                        DateInscription = DateTime.Now,
                        DateDebut = DateTime.Now.Date,
                        DateFin = DateTime.Now.Date.AddYears(1).Date.AddDays(-1),
                        MentoratCategorie = db.MentoratCategorie.FirstOrDefault(w => w.Nom.ToLower() == "phyto"),
                        Mentor = db.Mentors.FirstOrDefault(w => w.NoMentor == "1"),
                        Mentore = entity,
                        APaye = false
                    };

                    db.MentoratInscription.Attach(inscription);
                    db.Entry(inscription).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    //  db.MentoratInscription.Add(inscription);


                    db.SaveChanges();

                    mentore.NoMentore = entity.No_Mentore;

                    // await SignInAsync(user, isPersistent: false);  connecter le nouvel utilisateur

                    string apiKey = Environment.GetEnvironmentVariable("SENDGRID_KEY");

                    /// Extensions.CscExtensionsMethodes.Envoyercourriel(entity, Extensions.CscExtensionsMethodes.EcrireMessage(entity),apiKey).Wait();

                    // await SignInAsync(newUser, isPersistent: false);
                    await _signInManager.SignInAsync(newUser, isPersistent: false);

                    return RedirectToAction("PayerAvecPaypal", "Assignation", new { invoice = inscription.Id, nomCompletMentore = entity.NomComplet_Mentore });

                    //return RedirectToAction("Index", "Accueil");



                }

            }

            return View(mentore);


        }


        [Authorize(Roles = "GererUtilisateurDroits")]
        private async Task DupliqerDroits(RegisterViewModel model, ApplicationUser user)
        {
            //aller chercher le user en cours
            // string idUserenCours = User.Identity.GetUserId();
            // ApplicationUser userEnCours = _userManager.FindById(idUserenCours);
            ApplicationUser userEnCours = await _userManager.GetUserAsync(User);

            var db = new ApplicationDbContext();

            RoleManager<ApplicationRole> monRoleManager = MentoratNetCore.Extensions.CscExtensionsMethodes.ObtenirRoleManager(this._serviceProvider);
            List<ApplicationRole> droitsACopier = (await ObtenirRolesParIDUserAsync(model.CopierDroitsDe, db, userEnCours.IdCategorieUtilisateur)).ToList();

            foreach (var roleUser in droitsACopier)
            {
                // _userManager.AddToRole(user.Id, roleUser.Name);
                await _userManager.AddToRoleAsync(user, roleUser.Name);

            }

        }

        [Authorize(Roles = "PageUtilisateurs")]
        public JsonResult ObtenirUtilisateurs()
        {
         
            // SB: Dans EFCore2.1 : OrderBy ne fonctionne pas par défaut ..
            var vm = _userManager.Users.ToList().Select(g => new { NoUser = g.Id, NomPrenomUser = g.NomUser + " " + g.PrenomUser });
            return Json(vm);
            //             return Json(UserManager.Users.OrderBy(o => new { o.NomUser, o.PrenomUser }).Select(g => new { NoUser = g.Id, NomPrenomUser = g.NomUser + " " + g.PrenomUser }), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "GererUtilisateur,CreerDroit")]
        public async Task<JsonResult> ObtenirCategorieUtisateur()
        {
            var db = new ApplicationDbContext();
            //ApplicationUser userEnCours = _userManager.FindById(User.Identity.GetUserId());
            var userEnCours = await _userManager.GetUserAsync(User);

            //string dump = Extensions.CscExtensionsMethodes.DumpToHtmlString(db.ApplicationCategorieUser.Where(w => w.Id >= userEnCours.IdCategorieUtilisateur).OrderByDescending(o => o.Id).Select(g => new { noCat = g.Id, nomCat = g.Description }));
            var vMCategs = db.ApplicationCategorieUser.Where(w => w.Id >= userEnCours.IdCategorieUtilisateur).OrderByDescending(o => o.Id).Select(g => new { noCat = g.Id, nomCat = g.Description });
            return Json(vMCategs);

        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            ManageMessageId? message = null;
            IdentityResult result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);

            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }


        [Authorize]
        //
        // GET: /Account/Manage
        public async Task<ActionResult> Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Votre mot de passe a été changé."
                : message == ManageMessageId.SetPasswordSuccess ? "Votre mot de passe a été défini."
                : message == ManageMessageId.RemoveLoginSuccess ? "La connexion externe a été retiré."
                : message == ManageMessageId.Error ? "Une erreur est survenue."
                : "";
            ViewBag.HasLocalPassword = await HasPasswordAsync();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [Authorize]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = await HasPasswordAsync();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    //IdentityResult result = await _userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                var state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    //IdentityResult result = await _userManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    var user = await _userManager.GetUserAsync(User);
                    IdentityResult result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Account/GestionDroits
        [Authorize(Roles = "ParametresDroits")]
        public ActionResult GestionDroits()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {

            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await _userManager.FindByLoginAsync(loginInfo.LoginProvider,loginInfo.ProviderKey);
            if (user != null)
            {
                //await SignInAsync(user, isPersistent: false);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.ProviderDisplayName });
            }

        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        //await SignInAsync(user, isPersistent: false);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            //AuthenticationManager.SignOut();
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Accueil");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        //[ChildActionOnly]
        //public ActionResult RemoveAccountList()
        //{
        //    var linkedAccounts = _userManager.GetLogins(User.Identity.GetUserId());
        //    ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
        //    return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
               // UserManager = null;
            }
            base.Dispose(disposing);
        }

        //#region Helpers
        //// Used for XSRF protection when adding external logins
        //private const string XsrfKey = "XsrfId";

        //private IAuthenticationManager AuthenticationManager {
        //    get {
        //        return HttpContext.GetOwinContext().Authentication;
        //    }
        //}

        //private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //    var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

        //    //Ajout de martin pour le nom complet dans les cookies
        //    identity.AddClaim(new Claim("NomCompletUser", user.NomCompletUser));

        //    AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        //}

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.ToString());
            }
        }

        private async Task<bool> HasPasswordAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var hasPassword = await _userManager.HasPasswordAsync(user);
            return hasPassword;
            //var user = _userManager.FindById(User.Identity.GetUserId());
            //if (user != null)
            //{
            //    return user.PasswordHash != null;
            //}
            //return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Accueil");
            }
        }

        //
        // GET: /Account/Utilisateurs
        [Authorize(Roles = "PageUtilisateurs")]
        public ActionResult Utilisateurs()
        {
            return View();
        }

        [Authorize(Roles = "PageUtilisateurs")]
        public ActionResult Utilisateurs_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<UtilisateursViewModel> lstUtilisateurs = _userManager.Users.Select(s => new Models.UtilisateursViewModel() { Id = s.Id, UserName = s.UserName, Nom = s.NomUser, Prenom = s.PrenomUser, ActifUser = s.ActifUser, Email = s.Email }).ToList();

            DataSourceResult results = lstUtilisateurs.ToDataSourceResult(request);

            return Json(results);
            //return Json(results, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Account/Utilisateur
        [CustomAuthorizePageUtilisateur]
        [ResponseCache(NoStore = true, Duration = 0)]        // [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Utilisateur(string utilisateur, string section)
        {
            ApplicationUser monUser = null;
            if (utilisateur != null && MentoratNetCore.Extensions.CscExtensionsMethodes.VerifierSiUserExiste(utilisateur, _userManager, out monUser))
            {
                bool boolPhyto = false;

                using (var db = new ApplicationDbContext())
                {
                    boolPhyto = db.MentoratInscription.Any(a => a.Mentore.No_Mentore == monUser.Id && a.MentoratCategorie.Nom.ToString().ToLower() == "phyto");
                }
                ViewData["AfficherSectionPhyto"] = boolPhyto;
                return View(new UtilisateurViewModel { userName = utilisateur, section = section, Id = monUser.Id, Nom = monUser.NomCompletUser });
            }
            else
                return RedirectToAction("Utilisateurs");
        }



        ////
        //// Post: /Account/UtilisateurInfo

        //[HttpPost]
        //public ActionResult UtilisateursModifier([DataSourceRequest] DataSourceRequest request, ApplicationUser user)
        //{
        //    if (user != null && ModelState.IsValid && user.UserName != "martin")
        //    {
        //        //var rUser = new UtilisateurInfoViewModel
        //        //{
        //        //    Id = user.Id,
        //        //    UserName = user.UserName,
        //        //    Nom = user.NomUser,
        //        //    Prenom = user.PrenomUser,
        //        //    Email = user.Email
        //        //};

        //        return RedirectToAction("Utilisateur","Account", new UtilisateurViewModel { Id=user.Id});
        //    }
        //    else
        //    {
        //        return View(user);
        //    }


        //}

        //
        // GET: /Account/UtilisateurInfo
        [Authorize(Roles = "PageUtilisateurs")]
        public ActionResult UtilisateurInfo(UtilisateurInfoViewModel model)
        {
            if (model != null)
            {
                return PartialView(model);
            }
            else
            {
                return RedirectToAction("Utilisateurs");
            }

        }

        // GET: /Account/UtilisateurInfo
        [Authorize(Roles = "PageUtilisateurs")]
        public ActionResult UtilisateurInfoOuvrir(string utilisateur)
        {

            List<ApplicationUser> lstUser = _userManager.Users.ToList();
            ApplicationUser monUser = lstUser.FirstOrDefault(f => f.UserName == utilisateur);

            if (monUser != null)
            {
                var rUser = new UtilisateurInfoViewModel
                {
                    Id = monUser.Id,
                    UserName = monUser.UserName,
                    Nom = monUser.NomUser,
                    Prenom = monUser.PrenomUser,
                    Email = monUser.Email
                };
               
                HttpContext.Session.SetString("InfoOuvrirUserID", rUser.Id); //this.Session["InfoOuvrirUserID"] = rUser.Id;

                return PartialView("UtilisateurInfo", rUser);
            }
            else
            {
                return null;
                //return PartialView("UtilisateurInfo", null);
            }


        }


        //
        // POST: /Account/UpdateUtilisateur
        [HttpPost]
        [Authorize(Roles = "GererUtilisateur")]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUtilisateur(UtilisateurInfoViewModel model)
        {

            //     Task.WaitAll(Task.Delay(5000));
            if (ModelState.IsValid)
            {
                //model.Id = "fkjdslkfjdslku543080432mfds";
                if(model.Id == HttpContext.Session.GetString("InfoOuvrirUserID")) //if (model.Id == this.Session["InfoOuvrirUserID"].ToString())
                {
                    var db = new ApplicationDbContext();
                    List<ApplicationUser> lstUser = db.Users.ToList();

                    //vérifier si le user existe déjà
                    ApplicationUser monUser = lstUser.FirstOrDefault(f => f.Id != model.Id && f.UserName == model.UserName);

                    if (monUser == null)
                    {
                        //Aller chercher l'enregistrement du user

                        ApplicationUser monUserUpdate = lstUser.FirstOrDefault(f => f.Id == model.Id);

                        if (monUserUpdate != null)
                        {
                            //mettre à jour le user
                            monUserUpdate.UserName = model.UserName;
                            monUserUpdate.PrenomUser = model.Prenom;
                            monUserUpdate.NomUser = model.Nom;
                            monUserUpdate.Email = model.Email;

                            //vérifier si on change le mot de passe
                            if (model.Password != null && model.ConfirmPassword != null)
                            {
                                if (model.Password == model.ConfirmPassword)
                                    monUserUpdate.PasswordHash = _userManager.PasswordHasher.HashPassword(monUserUpdate, model.Password);
                            }

                            //mettre à jour la bd
                            // await _userManager.UpdateAsync(monUserUpdate);                      
                            // db.Entry(monUserUpdate).State = Microsoft.EntityFrameworkCore.EntityState.Modified;


                            //Mettre à jour aussi les tables mentors et mentorés à corriger un jour.
                            Mentor mentor = db.Mentors.FirstOrDefault(w => w.NoMentor == monUserUpdate.Id);

                            if (mentor != null)
                            {
                                mentor.PrenomMentor = monUserUpdate.PrenomUser;
                                mentor.NomMentor = monUserUpdate.NomUser;
                                mentor.CourrielMentor = monUserUpdate.Email;
                            }

                            //db.Mentors.Attach(mentor);
                            //db.Entry(mentor).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                            Mentore mentore = db.Mentores.FirstOrDefault(w => w.No_Mentore == monUserUpdate.Id);

                            if (mentore != null)
                            {
                                mentore.Prenom_Mentore = monUserUpdate.PrenomUser;
                                mentore.Nom_Mentore = monUserUpdate.NomUser;
                                mentore.Courriel_Mentore = monUserUpdate.Email;
                            }

                            //db.Mentores.Attach(mentore);
                            //db.Entry(mentore).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                            db.SaveChanges();

                            //retourner l'information que l'update a eu lieu
                            return Json(new { success = true, });
                        }

                    }
                    else
                    {
                        //utilisateur déjà utilisé.                  
                        ModelState.AddModelError("UserName", "Code d'utilisateur existant.");
                    }
                }
                else
                {
                    //les id sont différents!.                  
                    ModelState.AddModelError("Id", "Une erreur c'est produite! Veuillez cliquer sur le bouton Annuler.");
                }


            }

            //On retourne les erreurs à la page
            return PartialView("UtilisateurInfo", model);

        }


        [Authorize(Roles = "GererUtilisateurDroits")]
        public ActionResult UtilisateurDroits(string utilisateur)
        {

            List<ApplicationUser> lstUser = _userManager.Users.ToList();
            ApplicationUser monUser = lstUser.FirstOrDefault(f => f.UserName == utilisateur);

            if (monUser != null)
            {
                var rUser = new UtilisateurDroitsViewModel
                {
                    Id = monUser.Id,
                    Nom = monUser.NomUser,
                    Prenom = monUser.PrenomUser
                };


                return PartialView(rUser);
            }
            else
            {

                return PartialView(null);
            }

        }


        [HttpPost]
        [Authorize(Roles = "GererUtilisateurSuppression")]
        public async Task<ActionResult> SupprimerUtilisateur(string utilisateur)
        {
            int nbInterventions = 0;
            string msg = "";
            IEnumerable<Mentore> lstMentores = null;
            IEnumerable<Mentor> lstMentors = null;


            if (utilisateur == null || utilisateur == "")
            {
                return Json(new { success = false, msg = "Utilisateur inexistant!" });
            }


            ApplicationUser user = await _userManager.FindByNameAsync(utilisateur);

            if (user == null)
            {
                return Json(new { success = false, msg = "Utilisateur inexistant!" });
            }

            using (var db = new ApplicationDbContext())
            {
                nbInterventions = db.Interventions.Count(c => c.Mentore.No_Mentore == user.Id || c.Mentor.NoMentor == user.Id);

                if (nbInterventions > 0)
                {
                    return Json(new { success = false, msg = "Vous ne pouvez supprimer un utilisateur qui a des interventions (" + nbInterventions + ")" });
                }

                lstMentores = db.Mentores.Where(w => w.No_Mentore == user.Id);
                lstMentors = db.Mentors.Where(w => w.NoMentor == user.Id);

                if (lstMentores != null)
                {
                    try
                    {
                        db.Mentores.RemoveRange(lstMentores);
                    }
                    catch (Exception e)
                    {
                        await HttpContext.Response.WriteAsync(e.Message.ToString()); //HttpContext.Response.Write(e.Message.ToString());
                    }

                }

                if (lstMentors != null)
                {
                    db.Mentors.RemoveRange(lstMentors);
                }

                if (lstMentores != null || lstMentors != null)
                {
                    await db.SaveChangesAsync();
                }
            }

            //ICollection<ApplicationUserLogin> logins = user.Logins;
            var logins = await _userManager.GetLoginsAsync(user);
            IList<string> rolesForUser = await _userManager.GetRolesAsync(user);

            using (var dbContext = new ApplicationDbContext())
            {


                using (var transaction = dbContext.Database.BeginTransaction())
                {




                    foreach (var login in logins.ToList())
                    {
                        var use = await _userManager.FindByLoginAsync(login.LoginProvider, login.ProviderKey);
                        //await _userManager.RemoveLoginAsync(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                        await _userManager.RemoveLoginAsync(use, login.LoginProvider, login.ProviderKey);
                    }

                    if (rolesForUser.Count() > 0)
                    {
                        foreach (var item in rolesForUser.ToList())
                        {
                            // item should be the name of the role
                            IdentityResult result = await _userManager.RemoveFromRoleAsync(user, item);
                        }
                    }

                    await _userManager.DeleteAsync(user);
                    transaction.Commit();
                }

            }





            return Json(new { success = true });



        }

        private async Task<IQueryable<ApplicationRole>> ObtenirRolesParIDUserAsync(string idUtilisateur, ApplicationDbContext context, int? iCategorieMinimum)
        {
            ApplicationDbContext monContext;

            if (context == null)
            {
                monContext = new ApplicationDbContext();
            }
            else
            {
                monContext = context;
            }

            if (iCategorieMinimum == null)
            {
                iCategorieMinimum = 0;
            }

            RoleManager<ApplicationRole> roleMgr = CscExtensionsMethodes.ObtenirRoleManager(this._serviceProvider);

            var user = await _userManager.FindByIdAsync(idUtilisateur);
            IList<string> lst = await _userManager.GetRolesAsync(user);

            IQueryable<ApplicationRole> result = from tRole in roleMgr.Roles
                                                 where lst.Contains(tRole.Name) && tRole.IdCategorieUtilisateur >= iCategorieMinimum
                                                 select tRole;


            return result;
        }

        [Authorize(Roles = "GererUtilisateurDroits")]
        //[AcceptVerbs(HttpVerbs.Post)]
        public async Task<JsonResult> UtilisateurDroitsTree_Read(string id, string utilisateurEnCours, string utilisateurACopier)
        {
            var monContext = new ApplicationDbContext();

            //Quand on copie les droits d'un utilisateur vers un autre, l'application perd le id de l'utilisateur à copier lors des appels récursifs du treeview
            if (id == null)
            {
                if (utilisateurACopier != null)
                    HttpContext.Session.SetString("UserACopier", utilisateurACopier); //this.Session["UserACopier"] = utilisateurACopier;
                else
                    HttpContext.Session.Remove("UserACopier"); //this.Session["UserACopier"] = null;
            }
            else
            {
                if (HttpContext.Session.GetString("UserACopier") != null) // if (this.Session["UserACopier"] != null)
                    utilisateurACopier = HttpContext.Session.GetString("UserACopier"); // utilisateurACopier = this.Session["UserACopier"].ToString();
            }


            // string idUserenCours = User.Identity.GetUserId();
            // ApplicationUser userEnCours = _userManager.FindById(idUserenCours);
            ApplicationUser userEnCours = await _userManager.GetUserAsync(User);

            ApplicationUser userAModifierOuCopier = await _userManager.FindByIdAsync((utilisateurACopier == null ? utilisateurEnCours : utilisateurACopier));

            var monRoleManager = MentoratNetCore.Extensions.CscExtensionsMethodes.ObtenirRoleManager(this._serviceProvider);


            //On va chercher les drois de l'utilisateur à modifier ou de l'utilisateur à qui on veut copier ses droits.
            List<ApplicationUserRole> sr = userAModifierOuCopier.UserRoles.ToList(); // List<ApplicationUserRole> sr = userAModifierOuCopier.Roles.ToList();


            //On va chercher les droits que l'utilisateur en cours à le droit de modifier
            List<ApplicationRole> rolesUserEnCours = monRoleManager.Roles.Where(w => w.IdCategorieUtilisateur >= userEnCours.IdCategorieUtilisateur).Select(s => s).ToList();

            var droits = from roles in rolesUserEnCours
                         join uRoles in sr on roles.Id equals uRoles.RoleId into uRolesGrp
                         from uRoles in uRolesGrp.DefaultIfEmpty()
                         where (id == "-1" ? roles.IdParent == null : roles.IdParent == id)
                         orderby roles.NomLong ascending
                         select new
                         {
                             id = roles.Id,
                             Name = roles.Name,
                             text = roles.NomLong,
                             hasChildren = (roles.RolesEnfants.Any()),
                             @checked = uRoles != null
                         };

            return Json(droits);
        }


        [HttpPost]
        [Authorize(Roles = "GererUtilisateurDroits")]
        public JsonResult UtilisateurDroitsTree_Save(string utilisateurId, Array lesDroits)
        {
            string strMsg = "";

            if (ModelState.IsValid)
            {
                List<string> lstLesDroits = lesDroits.OfType<string>().ToList();

                bool result = EnregistrerUtilisateurDroitsTree_Save(utilisateurId, lstLesDroits).Result;
                // ViewBag.strMsg 
                if (result)
                { }

            }


            //  Task.WaitAll(Task.Delay(5000));
            return Json(new { success = true, msg = strMsg });
        }


        private async Task<bool> EnregistrerUtilisateurDroitsTree_Save(string utilisateurId, List<string> lesDroits)
        {
            var monContext = new ApplicationDbContext();
            
            string idUserenCours = this.User.FindFirstValue(ClaimTypes.NameIdentifier); //string idUserenCours = User.Identity.GetUserId();
            var lstLesDroits = new List<string>();


            // lesDroits.Remove("05370da0-d92f-4cfe-9dc1-6d4d90d7b09c");   //Pour faire un test pour m'assurer qu'il ne me manque pas de parent.

            lstLesDroits.AddRange(lesDroits);

            ApplicationUser userEnCours = _userManager.Users.FirstOrDefault(u => u.Id == idUserenCours); // ApplicationUser userEnCours = _userManager.FindById(idUserenCours);
            ApplicationUser userAModifier = _userManager.Users.FirstOrDefault(u => u.Id == utilisateurId); // ApplicationUser userAModifier = _userManager.FindById(utilisateurId);


            RoleManager<ApplicationRole> monRoleManager = MentoratNetCore.Extensions.CscExtensionsMethodes.ObtenirRoleManager(this._serviceProvider);

            //On va chercher les droits que l'utilisateur en cours à le droit de modifier
            List<ApplicationRole> rolesPossibleUserEnCours = monRoleManager.Roles.Where(w => w.IdCategorieUtilisateur >= userEnCours.IdCategorieUtilisateur).ToList();

            //On va chercher les drois de l'utilisateur à modifier 
            //List<ApplicationRole> rolesUserAModifier = await (ObtenirRolesParIDUserAsync(userAModifier.Id, monContext, null)).ToList(); //.ToList();
            IQueryable<ApplicationRole> rl = await ObtenirRolesParIDUserAsync(userAModifier.Id, monContext, null);
            List<ApplicationRole> rolesUserAModifier = rl.ToList();

            List<ApplicationRole> rolesChecked = (from rRoles in rolesPossibleUserEnCours
                                                  where lesDroits.Contains(rRoles.Id)
                                                  select rRoles).ToList();


            //on s'assure qu'il ne manque pas de droits parents qui doivent être checked
            foreach (var roleAAjouter in new List<ApplicationRole>(rolesChecked))
            {
                ObtenirRolesParent(ref rolesChecked, roleAAjouter, rolesPossibleUserEnCours);
            }

            List<ApplicationRole> rolesToAdd = rolesPossibleUserEnCours.Intersect(rolesChecked).Except(rolesUserAModifier).ToList();
            List<ApplicationRole> rolesASupprimer = rolesPossibleUserEnCours.Except(rolesChecked).Intersect(rolesUserAModifier).ToList();
            List<ApplicationRole> rolesRestants = rolesUserAModifier.Except(rolesToAdd).Except(rolesASupprimer).ToList();


            //on s'assure que des rôles plus elevé ne se trouvent pas sans rôles parents
            //ont fait la vérification pour les ajouter. Il se peut qu'un rôle qui a été supprimé soit de retour.
            var rolesParents = new List<ApplicationRole>();
            foreach (var item in rolesRestants)
            {
                ObtenirRolesParent(ref rolesParents, item, monRoleManager.Roles.ToList());
            }

            var iRoleASup = rolesASupprimer.Count; //garde en mémoire le nombre avant la condition suivante.

            if (rolesParents.Count > 0 && rolesASupprimer.Count > 0)
            {
                //on retire des rôles a supprimer les rôles parents de rôles de niveau suppérieur(l'utilisateur qui retire le droit ne peut retirer un rôle parent d'un autre rôle d'un niveau plus important).
                rolesASupprimer = rolesASupprimer.Except(rolesParents).ToList();
            }

            if (iRoleASup > 0 && iRoleASup > rolesASupprimer.Count)
            {
                //Nous avons retirer des rôles à supprimer pour les garder.
                ViewBag.strMsg = "Vous ne pouvez retirer certains droits.";
            }

            foreach (var item in rolesASupprimer)
            {
                IdentityResult iRest = await _userManager.RemoveFromRoleAsync(userAModifier, item.Name);
            }

            foreach (var item in rolesToAdd)
            {
                IdentityResult iRest = await _userManager.AddToRoleAsync(userAModifier, item.Name);
            }




            return true;
        }

        private void ObtenirRolesParent(ref List<ApplicationRole> rolesToAdd, ApplicationRole roleAVerifier, List<ApplicationRole> lesroles)
        {
            //Pour vérifier qu'il nous manque pas de parentRole
            if (roleAVerifier.RoleParent != null)
            {
                if (!rolesToAdd.Contains(roleAVerifier.RoleParent) && lesroles.Contains(roleAVerifier.RoleParent))
                {
                    rolesToAdd.Add(roleAVerifier.RoleParent);
                }
                ObtenirRolesParent(ref rolesToAdd, roleAVerifier.RoleParent, lesroles);
            }
        }


        //
        // GET: /Account/Utilisateurs
        [Authorize(Roles = "ParametresDroits")]
        public ActionResult ParametresDroits()
        {
            return View(new ParametresDroitsViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "ParametresDroits")]
        [ValidateAntiForgeryToken]
        public ActionResult ParametresDroits(ParametresDroitsViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool result = ModifierRoleAsync(model).Result;
            }

            return Json(CscExtensionsMethodes.ModelStateAllErrors(ModelState));
        }


        private async Task<bool> ModifierRoleAsync(ParametresDroitsViewModel model)
        {
            var monContext = new ApplicationDbContext();
            string idUserenCours = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool boolValide = false;

            ApplicationUser userEnCours = await _userManager.FindByIdAsync(idUserenCours);

            RoleManager<ApplicationRole> monRoleManager = MentoratNetCore.Extensions.CscExtensionsMethodes.ObtenirRoleManager(this._serviceProvider);

            if (ValiderRolesUpdate(model, monRoleManager, userEnCours))
            {
                ApplicationRole roleToUpdate = monRoleManager.Roles.FirstOrDefault(w => w.IdCategorieUtilisateur >= userEnCours.IdCategorieUtilisateur && w.Name == model.NomHidden && w.Id == model.Id);

                if (roleToUpdate != null)
                {
                    if (User.IsInRole("CreerDroit"))
                    {
                        roleToUpdate.Name = model.Nom;
                    }

                    roleToUpdate.NomLong = model.NomLong;
                    roleToUpdate.IdCategorieUtilisateur = model.IdCategorie;
                    roleToUpdate.IdParent = model.IdParent;

                    IdentityResult res = await monRoleManager.UpdateAsync(roleToUpdate);

                    if (res == IdentityResult.Success)
                    {
                        boolValide = true;
                    }
                    else
                    {
                        ModelState.AddModelError("Nom", "Impossible d'enregistrer les modifications de la page.");
                    }
                }
                else
                {
                    ModelState.AddModelError("Nom", "Vous ne pouvez modifier un rôle qui n'existe pas.");
                }
            }


            return boolValide;
        }

        private bool ValiderRolesUpdate(ParametresDroitsViewModel model, RoleManager<ApplicationRole> monRoleManager, ApplicationUser userEnCours)
        {
            bool valide = true;

            if (model.NomHidden != model.Nom)
            {
                //on change de nom
                if (monRoleManager.Roles.Any(w => w.Name == model.Nom && w.Id != model.Id))
                {
                    valide = false;
                    ModelState.AddModelError("Nom", model.Nom + " est déjà existant.");
                }
            }


            if (monRoleManager.Roles.Any(w => w.NomLong == model.NomLong && w.Id != model.Id))
            {
                valide = false;
                ModelState.AddModelError("NomLong", model.NomLong + " est déjà utilisé.");
            }


            //valider le droit parent
            if (model.IdParent != null && model.IdParent != "")
            {
                List<ApplicationRole> rolesPossibleUserEnCours = monRoleManager.Roles.Where(w => w.IdCategorieUtilisateur >= userEnCours.IdCategorieUtilisateur).ToList();

                //Utilisateur a le droit d'utiliser ce droit?
                if (!rolesPossibleUserEnCours.Any(a => a.Id == model.IdParent))
                {
                    valide = false;
                    ModelState.AddModelError("IdParent", "Vous ne pouvez utiliser ce droit.");
                }
                else
                {
                    ApplicationRole roleParent = rolesPossibleUserEnCours.FirstOrDefault(f => f.Id == model.IdParent);
                    //role parent identique?
                    if (roleParent == null)
                    {
                        valide = false;
                        ModelState.AddModelError("IdParent", "Vous ne pouvez utiliser ce droit.");
                    }
                    else
                    {
                        if (roleParent.Id == model.Id)
                        {
                            valide = false;
                            ModelState.AddModelError("IdParent", "Vous ne pouvez utiliser ce droit.");
                        }
                        else
                        {
                            if (roleParent.IdCategorieUtilisateur < model.IdCategorie)
                            {
                                valide = false;
                                ModelState.AddModelError("IdParent", "Le droit parent ne doit pas être d'un niveau supérieur.");
                            }
                        }
                    }
                }

            }




            return valide;
        }

        [HttpPost]
        [Authorize(Roles = "CreerDroit")]
        [ValidateAntiForgeryToken]
        public ActionResult ParametresDroits_Add(ParametresDroitsViewModel model)
        {

            if (ModelState.IsValid)
            {
                bool result = AjouterRoleAsync(model).Result;
                return Json(CscExtensionsMethodes.ModelStateAllErrors(ModelState));
            }
            return Json(CscExtensionsMethodes.ModelStateAllErrors(ModelState));
        }

        [Authorize(Roles = "CreerDroit")]
        private async Task<bool> AjouterRoleAsync(ParametresDroitsViewModel model)
        {
           // var monContext = new ApplicationDbContext();
            string idUserenCours = this.User.FindFirstValue(ClaimTypes.NameIdentifier);  //User.Identity.GetUserId();
            bool valide = false;

            ApplicationUser userEnCours = await _userManager.FindByIdAsync(idUserenCours);

            RoleManager<ApplicationRole> monRoleManager = MentoratNetCore.Extensions.CscExtensionsMethodes.ObtenirRoleManager(this._serviceProvider);

            //model.Id = Guid.NewGuid().ToString();

            if (ValiderRolesUpdate(model, monRoleManager, userEnCours))
            {
                var roleToAdd = new ApplicationRole() { Id = Guid.NewGuid().ToString(), IdCategorieUtilisateur = model.IdCategorie, IdParent = model.IdParent, Name = model.Nom, NomLong = model.NomLong };
                IdentityResult res = await monRoleManager.CreateAsync(roleToAdd);

                if (res == IdentityResult.Success)
                {
                    valide = true;
                }
            }
            return valide;
        }

        [Authorize(Roles = "ParametresDroits")]
        public async Task<ActionResult> ParametresDroits_Read([DataSourceRequest] DataSourceRequest request)
        {

           // var monContext = new ApplicationDbContext();
            string idUserenCours = this.User.FindFirstValue(ClaimTypes.NameIdentifier);  //User.Identity.GetUserId();

            ApplicationUser userEnCours = await _userManager.FindByIdAsync(idUserenCours);

            RoleManager<ApplicationRole> monRoleManager = MentoratNetCore.Extensions.CscExtensionsMethodes.ObtenirRoleManager(this._serviceProvider);

            List<Models.ParametresDroitsViewModel> rolesPossibleUserEnCours = monRoleManager.Roles.Where(w => w.IdCategorieUtilisateur >= userEnCours.IdCategorieUtilisateur).Select(s => new Models.ParametresDroitsViewModel() { Id = s.Id, Nom = s.Name, NomHidden = s.Name, NomLong = s.NomLong, IdCategorie = s.IdCategorieUtilisateur, IdParent = s.IdParent }).ToList();

            DataSourceResult results = rolesPossibleUserEnCours.ToDataSourceResult(request);

            return Json(results);
        }

        [Authorize(Roles = "ParametresDroits")]
        public async Task<JsonResult> ParametresDroitsRoles_Read()
        {
          //  var monContext = new ApplicationDbContext();
            string idUserenCours = this.User.FindFirstValue(ClaimTypes.NameIdentifier);  //User.Identity.GetUserId();

            ApplicationUser userEnCours = await _userManager.FindByIdAsync(idUserenCours);

            RoleManager<ApplicationRole> monRoleManager = MentoratNetCore.Extensions.CscExtensionsMethodes.ObtenirRoleManager(this._serviceProvider);

            List<Models.ParametresDroitsViewModel> rolesPossibleUserEnCours = monRoleManager.Roles.Where(w => w.IdCategorieUtilisateur >= userEnCours.IdCategorieUtilisateur).OrderBy(o => o.NomLong).Select(s => new Models.ParametresDroitsViewModel() { Id = s.Id, NomLong = s.NomLong, IdCategorie = s.IdCategorieUtilisateur }).ToList();

            return Json(rolesPossibleUserEnCours);

        }


        [Authorize(Roles = "ParametresDroits")]
        public ActionResult UtilisateursParametresDroits_Read([DataSourceRequest] DataSourceRequest request, string nomRole)
        {
            //List<UtilisateursViewModel> lstUtilisateurs = _userManager.Users.Select(s => new Models.UtilisateursViewModel() { Id = s.Id, UserName = s.UserName, Nom = s.NomUser, Prenom = s.PrenomUser, ActifUser = s.ActifUser, Email = s.Email }).ToList();
            DataSourceResult results = null;

            //if (nomRole != null && nomRole != string.Empty)
            //{
                RoleManager<ApplicationRole> monRoleManager = CscExtensionsMethodes.ObtenirRoleManager(this._serviceProvider);
                List<UtilisateursViewModel> lstUtilisateurs = CscExtensionsMethodes.ObtenirUsersInRole(nomRole, monRoleManager, _userManager).Select(s => new Models.UtilisateursViewModel() { Id = s.Id, UserName = s.UserName, Nom = s.NomUser, Prenom = s.PrenomUser, ActifUser = s.ActifUser, Email = s.Email }).ToList();

                results = lstUtilisateurs.ToDataSourceResult(request);
            //}

            return Json(results);
        }

        [Authorize(Roles = "ParametresDroits")]
        [HttpPost]
        public async Task<JsonResult> UserToRoles_Add(string nomRole, List<ApplicationUser> userToAdd)
        {
          //  var monContext = new ApplicationDbContext();
            string idUserenCours = this.User.FindFirstValue(ClaimTypes.NameIdentifier);// User.Identity.GetUserId();
            bool boolAdd = false;

            ApplicationUser userEnCours = await _userManager.FindByIdAsync(idUserenCours);

            RoleManager<ApplicationRole> monRoleManager = MentoratNetCore.Extensions.CscExtensionsMethodes.ObtenirRoleManager(this._serviceProvider);

            //Vérifier si l'utilisateur pour modifier ce role
            bool boolRoleExist = monRoleManager.Roles.Any(w => w.IdCategorieUtilisateur >= userEnCours.IdCategorieUtilisateur && w.Name == nomRole);


            if (boolRoleExist)
            {
                //retrouver les rôles parents
                List<string> lstNomRoles = ObtenirNomRolesParents(nomRole, monRoleManager, userEnCours);

                //on ajoute aux parent le nom du role.
                lstNomRoles.Add(nomRole);

                foreach (var monUser in userToAdd)
                {
                    ApplicationUser userValide = await _userManager.FindByIdAsync(monUser.Id);

                    foreach (var monRole in lstNomRoles)
                    {
                        if (userValide != null && !await _userManager.IsInRoleAsync(monUser, monRole))
                        {
                            await _userManager.AddToRoleAsync(monUser, monRole);
                            boolAdd = true;
                        }
                    }
                }
            }

            return Json(new { success = boolAdd });
        }

        private List<string> ObtenirNomRolesParents(string roleName, RoleManager<ApplicationRole> monRoleManager, ApplicationUser userEnCours)
        {

            ApplicationRole appRole = monRoleManager.Roles.FirstOrDefault(w => w.IdCategorieUtilisateur >= userEnCours.IdCategorieUtilisateur && w.Name == roleName);

            var lstParents = new List<string>();

            if (appRole != null && appRole.RoleParent != null)
            {
                ParcourirParentRole(appRole, ref lstParents);
            }

            return lstParents;
        }

        private void ParcourirParentRole(ApplicationRole appRole, ref List<string> lstParents)
        {
            if (appRole.RoleParent != null)
            {
                lstParents.Add(appRole.RoleParent.Name);
                ParcourirParentRole(appRole.RoleParent, ref lstParents);
            }
        }

        private void AffecterIdRolesEnfants(ApplicationRole leRole, int? iNiveau, ref List<string> lstIdEnfants, ref List<string> lstNomEnfants)
        {
            //Sert à trouver tous les rôles enfants ou les rôles enfants d'un niveau supérieur. 

            if (leRole.RolesEnfants != null)
            {
                foreach (var roleEnfant in leRole.RolesEnfants)
                {
                    if (iNiveau == null || roleEnfant.IdCategorieUtilisateur < iNiveau)
                    {
                        lstIdEnfants.Add(roleEnfant.Id);

                        if (iNiveau == null)
                        {
                            //pour obtenir la liste des noms à supprimer
                            lstNomEnfants.Add(roleEnfant.Name);
                        }
                    }

                    if (roleEnfant.RolesEnfants != null)
                    {
                        AffecterIdRolesEnfants(roleEnfant, iNiveau, ref lstIdEnfants, ref lstNomEnfants);
                    }
                }
            }

        }

        private bool VerifierSiRoleEnfantAvecDroitsSuperieur(ApplicationRole leRole, ApplicationUser leUser, List<string> lstIdEnfantsNiveauSup)
        {
            bool boolEnfantSup = false;

            if (lstIdEnfantsNiveauSup.Count > 0)
            {
                boolEnfantSup = leUser.UserRoles.Any(a => lstIdEnfantsNiveauSup.Contains(a.RoleId));  //leUser.Roles.Any(a => lstIdEnfantsNiveauSup.Contains(a.RoleId));
            }

            return boolEnfantSup;
        }

        [Authorize(Roles = "ParametresDroits")]
        [HttpPost]
        //public async Task<JsonResult> UserToRoles_Remove(string nomRole, List<ApplicationUser> userToRemove)
        public async Task<JsonResult> UserToRoles_Remove([FromBody] RemoveUserViewModel vm)
        {
            string nomRole = vm.nomRole;
            List<ApplicationUser> userToRemove = vm.userToRemove;

            var monContext = new ApplicationDbContext();
            string idUserenCours = this.User.FindFirstValue(ClaimTypes.NameIdentifier); //User.Identity.GetUserId();
            bool boolRemove = false;
            string strMsg = "";

            ApplicationUser userEnCours = await _userManager.FindByIdAsync(idUserenCours);

            RoleManager<ApplicationRole> monRoleManager = MentoratNetCore.Extensions.CscExtensionsMethodes.ObtenirRoleManager(this._serviceProvider);

            //Vérifier si l'utilisateur pour modifier ce role
            //  bool boolRoleExist = monRoleManager.Roles.Any(w => w.IdCategorieUtilisateur >= userEnCours.IdCategorieUtilisateur && w.Name == nomRole);

            ApplicationRole leRole = monRoleManager.Roles.FirstOrDefault(w => w.IdCategorieUtilisateur >= userEnCours.IdCategorieUtilisateur && w.Name == nomRole);

            var lstIdEnfantsNiveauSup = new List<string>();
            var lstIdEnfants = new List<string>();
            var lstNomEnfants = new List<string>();
            AffecterIdRolesEnfants(leRole, userEnCours.IdCategorieUtilisateur, ref lstIdEnfantsNiveauSup, ref lstNomEnfants);
            AffecterIdRolesEnfants(leRole, null, ref lstIdEnfants, ref lstNomEnfants);

            //bloquer la suppression d'un rôle s'il a un rôle enfant d'un niveau supérieur.
            //supprimer tous les rôles enfants sauf ceux d'un niveaux supérieurs

            if (leRole != null)
            {
                foreach (var monUser in userToRemove)
                {
                    ApplicationUser userValide = await _userManager.FindByIdAsync(monUser.Id);

                    if (userValide != null && await _userManager.IsInRoleAsync(userValide, nomRole))
                    {
                        if (!VerifierSiRoleEnfantAvecDroitsSuperieur(leRole, userValide, lstIdEnfantsNiveauSup))
                        {
                            try
                            {
                                if (await _userManager.RemoveFromRoleAsync(userValide, nomRole) == IdentityResult.Success)
                                {
                                    boolRemove = true;

                                    //Supprimer les rôles enfants
                                    foreach (var nomEnfant in lstNomEnfants)
                                    {
                                        if (await _userManager.IsInRoleAsync(userValide, nomEnfant))
                                        {
                                            await _userManager.RemoveFromRoleAsync(userValide, nomEnfant);
                                        }
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                string w2 = ex.Message;
                                throw;
                            }
                        }
                        else
                        {
                            strMsg = "Vous n'avez pas l'autorisation de retirer certains utilisateurs de ce droit.";
                        }


                    }
                }
            }



            return Json(new { success = boolRemove, msg = strMsg });
        }

        [Authorize]
        [CustomAuthorizeMentoreEtMentorsReadOnlyAttribute]
        public ActionResult GridUtilisateurInscriptions_Read([DataSourceRequest]DataSourceRequest request, string noMentore)
        {

            var db = new ApplicationDbContext();

            string idMentor = "";
            bool boolDroitGerer = User.IsInRole("GererMentores");
            bool boolEstLeMentore = false;

            if (User.IsInRole("Mentors"))
            {
                idMentor = this.User.FindFirstValue(ClaimTypes.NameIdentifier); // User.Identity.GetUserId();
            }

            if (this.User.FindFirstValue(ClaimTypes.NameIdentifier) == noMentore)
            {
                boolEstLeMentore = true;
            }


            List<Models.MentoratInscription> mentoresInsc;

            mentoresInsc = db.MentoratInscription.Where(w => w.Mentore.No_Mentore == noMentore).OrderByDescending(o => o.Annee).ToList();

            List<ApplicationUser> lesUsers = db.Users.ToList();

            DataSourceResult test = mentoresInsc.ToDataSourceResult(request, m => new MentoratNetCore.ViewModels.AssignationViewModel()
            {
                NoInscription = m.Id,

                NomUtilisateur = lesUsers.FirstOrDefault(u => u.Id == m.Mentore.No_Mentore).UserName.ToString(),
                Annee = m.Annee,
                //PrenomMentore = m.Mentore.Prenom_Mentore,
                //NomMentore = m.Mentore.Nom_Mentore,
                //Organisme_Mentore = m.Mentore.Organisme_Mentore,
                Mentore = new ViewModels.Assignation.AssignationMentoreViewModel()
                {
                    NoMentore = m.Mentore.No_Mentore,
                    PrenomMentore = m.Mentore.Prenom_Mentore,
                    NomMentore = m.Mentore.Nom_Mentore,
                    CellulaireMentore = m.Mentore.Cellulaire_Mentore,
                    CourrielMentore = m.Mentore.Courriel_Mentore,
                    //Expertises = m.Mentore.Expertises.Select(t => new Expertise
                    //{
                    //    Nom_Expertise = t.Nom_Expertise,
                    //    No_Expertise = t.No_Expertise
                    //}).ToList(),
                    Objectifs_Mentore = m.Mentore.Objectifs_Mentore

                },
                Mentor = new ViewModels.Assignation.AssignationMentorViewModel()
                {
                    NoMentor = m.Mentor.NoMentor,
                    PrenomMentor = m.Mentor.PrenomMentor,
                    NomMentor = m.Mentor.NomMentor
                },
                //Objectifs_Mentore = m.Objectifs_Mentore,
                APaye = m.APaye,
                DateInscription = m.DateInscription,
                DateDebut = m.DateDebut,
                DateFin = m.DateFin,
                //Mentore = m.Mentore,
                // Mentor = m.Mentor,
                MentoratCategorie = new MentoratNetCore.ModelsViews.MentoratCategorieViewModel() { Id = m.MentoratCategorie.Id, Nom = m.MentoratCategorie.Nom, Description = m.MentoratCategorie.Description },
                AfficherBoutonPlan = boolEstLeMentore || (idMentor != "" ? (m.Mentor.NoMentor == idMentor ? true : false) : boolDroitGerer)

            });

            return Json(test);

        }

        [Authorize(Roles = "GererMentores")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GridUtilisateurInscriptions_Update([DataSourceRequest]DataSourceRequest request, MentoratNetCore.ViewModels.AssignationViewModel inscription)
        {
            var db = new ApplicationDbContext();

            if (inscription != null && ModelState.IsValid)
            {

                var entity = db.MentoratInscription.FirstOrDefault(f => f.Id == inscription.NoInscription);

                if (entity != null)
                {
                    entity.Annee = inscription.Annee;
                    entity.DateDebut = inscription.DateDebut;
                    entity.DateFin = inscription.DateFin;
                    entity.APaye = inscription.APaye;
                    entity.Mentor = db.Mentors.First(f => f.NoMentor == inscription.Mentor.NoMentor);
                    db.MentoratInscription.Attach(entity);
                    db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                    db.SaveChanges();
                }

            }


            return Json(new[] { inscription }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "AssignationSuppression")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GridUtilisateurInscriptions_Destroy([DataSourceRequest]DataSourceRequest request, MentoratNetCore.ViewModels.AssignationViewModel inscription)
        {
            var db = new ApplicationDbContext();

            if (inscription != null && ModelState.IsValid)
            {

                var entity = db.MentoratInscription.FirstOrDefault(f => f.Id == inscription.NoInscription);

                if (entity != null)
                {

                    db.MentoratInscription.Attach(entity);
                    db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;

                    db.SaveChanges();
                }

            }


            return Json(new[] { inscription }.ToDataSourceResult(request, ModelState));
        }

        //
        // GET: /Account/Utilisateur
        // [Authorize(Roles = "GererUtilisateur,GererUtilisateurDroits")]
        [CustomAuthorizeMentoresAttribute]
        public async Task<ActionResult> MonDossier()
        {

            string idUserenCours = this.User.FindFirstValue(ClaimTypes.NameIdentifier);  //User.Identity.GetUserId();
            ApplicationUser userEnCours = await _userManager.FindByIdAsync(idUserenCours);


            if (userEnCours != null)
                return RedirectToAction("Utilisateur", "Account", new { utilisateur = userEnCours.UserName });
            else
                return RedirectToAction("index", "Accueil");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult MotDePasseOublie()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MotDePasseOublie(ReinitialiserMotDePasseViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    //ne pas informer que l'utilisateur n'existe pas
                }
                else
                {
                    string code = await _userManager.GeneratePasswordResetTokenAsync(user);


                    code = HttpUtility.UrlEncode(code);

                    string callbackUrl = Url.Action("ReinitialiserMotDePasse", "Account", new ReinitialiserMotDePasseViewModel { UserId = user.Id, Code = code, Email = user.Email, Utilisateur = user.UserName }, protocol: Request.Scheme); // protocol: Request.Url.Scheme);

                    string apiKey = Environment.GetEnvironmentVariable("SENDGRID_KEY");

                    dynamic sg = new SendGridAPIClient(apiKey);

                    //Mon object de courriel
                    var monCourriel = new Mail();
                    var mesInformations = new Personalization();

                    //De:
                    var contactEmail = new Email();
                    contactEmail.Address = "coordination@coordination-sc.org";
                    contactEmail.Name = "Mentorat en phytoprotection";
                    monCourriel.From = contactEmail;

                    contactEmail = new Email();
                    contactEmail.Address = user.Email;
                    mesInformations.AddTo(contactEmail);

                    mesInformations.Subject = "Réinitialisation de votre mot de passe.";

                    monCourriel.AddPersonalization(mesInformations);

                    string message = "Bonjour " + user.NomCompletUser + ",\n\n Nous avons reçu une demande de réinitialisation de votre mot de passe. Veuillez cliquer sur ce lien pour inscrire votre nouveau mot de passe  : <a href=\"" + callbackUrl + "\">Modifier votre mot de passe</a>. " +
                                     "\n\nSi vous n'êtes pas l'auteur de cette requête, veuillez ignorer ce courriel.";

                    string strMsg = HttpUtility.HtmlDecode(message);
                    var content = new Content();
                    content.Type = "text/html";
                    content.Value = strMsg;
                    monCourriel.AddContent(content);

                    dynamic response = await sg.client.mail.send.post(requestBody: monCourriel.Get());

                    // await _userManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");


                }

                model.Message = "Nous vous avons envoyé un courriel avec un lien à utiliser pour réinitialiser votre mot de passe. Vous devriez recevoir le courriel dans les 5 prochaines minutes.";
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ReinitialiserMotDePasse(ReinitialiserMotDePasseViewModel model)
        {
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(AccueilController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }



        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ReinitialiserMotDePassePost(ReinitialiserMotDePasseViewModel model)
        {


            if (ModelState.IsValid)
            {
                string courriel = model.Email;
                ApplicationUser user = await _userManager.FindByEmailAsync(courriel);

                if (user != null && user.Id == model.UserId)
                {
                    string code = HttpUtility.UrlDecode(model.Code);

                    IdentityResult result = await _userManager.ResetPasswordAsync(user, code, model.Password);

                    if (result == IdentityResult.Success)
                    {
                        //on login      
                        // await SignInAsync(user, isPersistent: false);
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        using (var db = new ApplicationDbContext())
                        {
                            bool boolInsc = db.MentoratInscription.Any(a => a.Mentore.No_Mentore == user.Id && a.MentoratCategorie.Nom.ToLower() == "phyto");

                            if (boolInsc)
                            {
                                return RedirectToAction("Utilisateur", "Account", new { utilisateur = user.UserName, section = "phyto" });
                            }
                        }

                        return RedirectToAction("Index", "Accueil");
                    }
                    else
                    {
                        ModelState.AddModelError("Message", string.Concat("\n", result.Errors.ToArray()));
                    }
                }

            }

            return View("ReinitialiserMotDePasse", model);
        }
    }
}

/*
namespace MentoratNetCore.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
*/
