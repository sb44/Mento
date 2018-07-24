using System;
using MentoratNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using MentoratNetCore.Models;
using System.Linq;
using System.Security.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using MentoratNetCore.Data;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace MentoratNetCore.AuthorizeCustom
{
    public class Authorize
    {

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class CustomAuthorizeMentoresAttribute : AuthorizeAttribute, IAuthorizationFilter
        {
            /// <summary>
            /// Méthode pour remplacer AuthorizeCore de .net mvc5
            /// </summary>
            /// <param name="context"></param>
            public void OnAuthorization(AuthorizationFilterContext context) 
            {
                var user = context.HttpContext.User;

                if (!(user.ADesInscriptionsMentorat()))
                    return;


            }
            /*
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                bool isAuthorized = base.AuthorizeCore(httpContext);

                if (!isAuthorized)
                {
                    return false;
                }


                return httpContext.User.ADesInscriptionsMentorat();

            }
            */
        }

        public class CustomAuthorizePageUtilisateurAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext actionContext)
            {
                bool valide = false;

                var user = actionContext.HttpContext.User;

                if (user.Identity.IsAuthenticated)    //if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    //doit être connecté
                    if (user.IsInRole("PageUtilisateurs") || user.IsInRole("Mentors")) //  if (HttpContext.Current.User.IsInRole("PageUtilisateurs") || HttpContext.Current.User.IsInRole("Mentors")) 
                    {
                        //Les membres de ces rôles peuvent accèder à la page
                        valide = true;
                    }
                    else
                    {           
                        
                        //seul le mentoré peut accèder à son dossier alors on compare les information du mentoré et de l'utilisateur
                        if (ValiderSiUserEstMentore(actionContext))
                        {
                            valide = true;
                        }
                        
                    }
                }


                if (valide)
                {
                    base.OnActionExecuting(actionContext);
                }
                else
                {
                    actionContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Rediriger", controller = "Commun", pAction = "Index", pController = "Accueil" }));
                }

            }


        }//fin class


        public class CustomAuthorizeMentoresDossierAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext actionContext)
            {
                bool valide = false;
                var user = actionContext.HttpContext.User;

                if (user.Identity.IsAuthenticated)    //if (HttpContext.Current.User.Identity.IsAuthenticated)
                {


                    //doit être connecté
                    if (user.IsInRole("GererUtilisateur"))
                    {
                        //Les membres de ce rôles peuvent accèder au get et au POST
                        valide = true;
                    }
                    else
                    {
                        if (actionContext.HttpContext.Request.Method == "POST")  //if (actionContext.HttpContext.Request.HttpMethod == "POST")
                        {
                            //Seul le mentoré peut modifier son dossier.(à part du groupe GererUtilisateur)
                            if (!user.IsInRole("Mentors"))
                            {
                                //on compare les information du mentoré et de l'utilisateur
                                if (ValiderSiUserEstMentore(actionContext))
                                {
                                    valide = true;
                                }
                            }
                        }
                        else
                        {
                            if (user.IsInRole("Mentors"))
                            {
                                //Les mentors peuvent accèder au "GET"
                                valide = true;
                            }
                            else
                            {
                                //seul le mentoré peut accèder à son dossier alors on compare les information du mentoré et de l'utilisateur
                                if (ValiderSiUserEstMentore(actionContext))
                                {
                                    valide = true;
                                }
                            }
                        }
                    }
                }


                if (valide)
                {
                    base.OnActionExecuting(actionContext);
                }
                else
                {
                    actionContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Rediriger", controller = "Commun", pAction = "Index", pController = "Accueil", pLayout = false }));
                }

            }




        }//fin class


        public class CustomAuthorizeMentoresDossierPhytoAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext actionContext)
            {
                bool valide = false;
                var user = actionContext.HttpContext.User;

                if (user.Identity.IsAuthenticated)
                {
                    //doit être connecté
                    if (user.IsInRole("GererMentores"))
                    {
                        //Les membres de ce rôles peuvent accèder au get et au POST
                        valide = true;
                    }
                    else
                    {
                        if (actionContext.HttpContext.Request.Method == "GET")                       
                        {
                            if (user.IsInRole("MentorsPhyto"))
                            {
                                //Les mentors peuvent accèder au "GET"
                                valide = true;
                            }
                            else
                            {
                                //seul le mentoré peut accèder à son dossier alors on compare les information du mentoré et de l'utilisateur
                                if (ValiderSiUserEstMentore(actionContext))
                                {
                                    valide = true;
                                }
                            }
                        }
                    }
                }

                if (valide)
                {
                    base.OnActionExecuting(actionContext);
                }
                else
                {
                    actionContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Rediriger", controller = "Commun", pAction = "Index", pController = "Accueil", pLayout = false }));
                }

            }




        }//fin class


        public class CustomAuthorizePageModifierPlanActionsAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext actionContext)
            {
                bool valide = false;
                var user = actionContext.HttpContext.User;
                //string noMentore, int noAnnee
                if (user.Identity.IsAuthenticated)
                {
                    //doit être connecté
                    if (user.IsInRole("GererMentores"))
                    {
                        //Les membres de ce rôles peuvent accèder au get et au POST
                        valide = true;
                    }
                    else
                    {                       
                        if (user.IsInRole("MentorsPhyto"))
                        {
                            if(ValiderSiEstLeMentor(actionContext))
                            {
                                valide = true;
                            }                            
                        }                                        
                    }
                }

                if (valide)
                {
                    base.OnActionExecuting(actionContext);
                }
                else
                {
                    actionContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Rediriger", controller = "Commun", pAction = "Index", pController = "Accueil", pLayout = false }));
                }

            }

        }//fin class

        public class CustomAuthorizePageAfficherPlanActionsAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext actionContext)
            {
                bool valide = false;
                var user = actionContext.HttpContext.User;
                //string noMentore, int noAnnee
                if (user.Identity.IsAuthenticated)
                {
                    //doit être connecté
                    if (user.IsInRole("GererMentores"))
                    {
                        //Les membres de ce rôles peuvent accèder au get et au POST
                        valide = true;
                    }
                    else
                    {
                        if ((user.IsInRole("MentorsPhyto") && ValiderSiEstLeMentor(actionContext)) || ValiderSiUserEstMentore(actionContext))
                        {
                            valide = true;
                        }
                    }
                }

                if (valide)
                {
                    base.OnActionExecuting(actionContext);
                }
                else
                {
                    actionContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Rediriger", controller = "Commun", pAction = "Index", pController = "Accueil", pLayout = false }));
                }

            }

        }//fin class


        public class CustomAuthorizeMentoreEtMentorsReadOnlyAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext actionContext)
            {
                bool valide = false;
                var user = actionContext.HttpContext.User;
                if (user.Identity.IsAuthenticated)
                {
                    //doit être connecté
                    if (user.IsInRole("GererMentores"))
                    {
                        //Les membres de ce rôles peuvent accèder au get et au POST
                        valide = true;
                    }
                    else
                    {
                       
                            if (user.IsInRole("MentorsPhyto") || ValiderSiUserEstMentore(actionContext))
                            {
                                //Les mentors peuvent accèder au "GET"
                                valide = true;
                            }                            
                       
                    }
                }

                if (valide)
                {
                    base.OnActionExecuting(actionContext);
                }
                else
                {
                    actionContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Rediriger", controller = "Commun", pAction = "Index", pController = "Accueil", pLayout = false }));
                }

            }




        }//fin class


        public class CustomAuthorizUserEstLeMentoreOnlyAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext actionContext)
            {
                bool valide = false;
                var user = actionContext.HttpContext.User;
                if (user.Identity.IsAuthenticated)
                {

                    //seul le mentoré peut accèder à son dossier alors on compare les information du mentoré et de l'utilisateur
                    if (ValiderSiUserEstMentore(actionContext))
                    {
                        valide = true;
                    }
                }

                if (valide)
                {
                    base.OnActionExecuting(actionContext);
                }
                else
                {
                    actionContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Rediriger", controller = "Commun", pAction = "Index", pController = "Accueil", pLayout = true }));
                }

            }




        }//fin class

        private static bool ValiderSiEstLeMentor(ActionExecutingContext actionContext)
        {
            string noMentore = "";
            string noAnnee = "";

            foreach (var key in actionContext.ActionArguments.Keys)
            {
                if (actionContext.ActionArguments[key] != null)
                {
                    switch (key.ToLower())
                    {
                        case "nomentore":
                            noMentore = actionContext.ActionArguments[key].ToString();
                            break;
                        case "noannee":
                            noAnnee = actionContext.ActionArguments[key].ToString();                           
                            break;
                        default:
                            break;
                    }
                }

            }

            if(noMentore !="" && noAnnee != "")
            {
                using (var db = new ApplicationDbContext())
                {
                    string userId = actionContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier); // string  userId = HttpContext.Current.User.Id();

                    int annee = int.Parse(noAnnee);

                    MentoratInscription inscription=  db.MentoratInscription.FirstOrDefault(f => f.Mentore.No_Mentore == noMentore && f.Annee == annee);
                    
                    if(inscription!= null)
                    {
                        if(inscription.Mentor != null)
                        {
                            if(inscription.Mentor.NoMentor == userId)
                            {
                                return true;
                            }
                        }
                    }
                }
             }

            return false;
        }



        private static bool ValiderSiUserEstMentore(ActionExecutingContext actionContext)
        {
            string utilisateur = "";
            string noMentore = "";
            MentoratNetCore.ViewModels.Inscriptions.InformationsMentoreViewModel model = null;


            foreach (var key in actionContext.ActionArguments.Keys) //foreach (var key in actionContext.ActionParameters.Keys)
            {
                if(actionContext.ActionArguments[key] !=null)
                {
                    switch (key.ToLower())
                    {
                        case "utilisateur":
                            utilisateur = actionContext.ActionArguments[key].ToString();
                            break;
                        case "nomentore":
                            noMentore = actionContext.ActionArguments[key].ToString();
                            break;
                        case "model":
                            if(noMentore=="")
                            {
                                model = (MentoratNetCore.ViewModels.Inscriptions.InformationsMentoreViewModel)actionContext.ActionArguments[key];
                                if (model != null) //on donne priorité au noMentore seul.
                                    noMentore = model.NoMentore;
                            }
                            
                            break;
                        default:
                            break;
                    }
                }
               
            }

            using (var db = new ApplicationDbContext())
            {
                ApplicationUser monUser = null;

                if (utilisateur != "")
                {
                    monUser = db.Users.Where(w => w.UserName == utilisateur).FirstOrDefault();
                }
                else if (noMentore != "")
                {
                    monUser = db.Users.Where(w => w.Id == noMentore).FirstOrDefault();
                }

                if (monUser != null)
                {
                    if (actionContext.HttpContext.User.Identity.Name == monUser.UserName)//if (HttpContext.Current.User.Identity.Name == monUser.UserName)
                    {
                        return true;
                    }
                }
            }




            return false;
        }//fin valider


        //private static string ObtenirIdMentore(ActionExecutingContext actionContext)
        //{
        //    string utilisateur = "";
        //    string noMentore = "";
        //    Mentorat.ViewModels.Inscriptions.InformationsMentoreViewModel model = null;


        //    foreach (var key in actionContext.ActionParameters.Keys)
        //    {
        //        if (actionContext.ActionParameters[key] != null)
        //        {
        //            switch (key.ToLower())
        //            {
        //                case "utilisateur":
        //                    utilisateur = actionContext.ActionParameters[key].ToString();
        //                    break;
        //                case "nomentore":
        //                    noMentore = actionContext.ActionParameters[key].ToString();
        //                    break;
        //                case "model":
        //                    if (noMentore == "")
        //                    {
        //                        model = (Mentorat.ViewModels.Inscriptions.InformationsMentoreViewModel)actionContext.ActionParameters[key];
        //                        if (model != null) //on donne priorité au noMentore seul.
        //                            noMentore = model.NoMentore;
        //                    }

        //                    break;
        //                default:
        //                    break;
        //            }
        //        }

        //    }

        //    using (var db = new ApplicationDbContext())
        //    {
        //        ApplicationUser monUser = null;

        //        if (utilisateur != "")
        //        {
        //            monUser = db.Users.Where(w => w.UserName == utilisateur).FirstOrDefault();
        //        }
        //        else if (noMentore != "")
        //        {
        //            monUser = db.Users.Where(w => w.Id == noMentore).FirstOrDefault();
        //        }

        //        if (monUser != null)
        //        {
        //            if (HttpContext.Current.User.Identity.Name == monUser.UserName)
        //            {
        //              return  monUser.Id;
        //            }
        //        }
        //    }

        //    return "";
        //}//fin Obtenir

        //private static bool ADesInscriptions(string idUser)
        //{
        //    bool aDesInsc = false;

        //    using (var db = new ApplicationDbContext())
        //    {
        //        aDesInsc = db.MentoratInscription.Any(a => a.Mentore.No_Mentore == idUser);                
        //    }

        //    return aDesInsc;
        //}

    }

}//fin namespace