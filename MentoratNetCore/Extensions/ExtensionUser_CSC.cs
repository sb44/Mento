using MentoratNetCore.Models;
using MentoratNetCore.Data;
using MentoratNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MentoratNetCore.Extensions
{
    public static class ExtensionUserCsc
    {
        public static bool ADesInscriptionsMentorat(this System.Security.Principal.IPrincipal principal)
        {
            if (principal.Identity.IsAuthenticated)
            {
                using (var db = new ApplicationDbContext())
                {
                    ApplicationUser monUser = db.Users.Where(w => w.UserName == principal.Identity.Name.ToString()).FirstOrDefault();

                    if (monUser != null)
                    {
                        MentoratInscription inscription = db.MentoratInscription.Where(w => w.Mentore.No_Mentore.Contains(monUser.Id)).FirstOrDefault();
                        if (inscription != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

        }

        public static bool EstCurrentMentore(this System.Security.Principal.IPrincipal principal)
        {

           // var test = HttpActionContext.ActionArguments[""];
            //var test2 = test.Values["id"];
            if (principal.Identity.IsAuthenticated)
            {
                using (var db = new ApplicationDbContext())
                {
                    //ApplicationUser monUser = db.Users.Where(w => w.UserName == principal.Identity.Name.ToString()).FirstOrDefault();

                    //if (monUser != null)
                    //{
                    //    MentoratInscription inscription = db.MentoratInscription.Where(w => w.Mentore.No_Mentore.Contains(monUser.Id)).FirstOrDefault();
                    //    if (inscription != null)
                    //    {
                    //        return true;
                    //    }
                    //    else
                    //    {
                    //        return false;
                    //    }
                    //}
                    //else
                    //{
                    //    return false;
                    //}
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        public static string Id(this System.Security.Principal.IPrincipal principal)
        {
            if (principal.Identity.IsAuthenticated)
            {
                using (var db = new ApplicationDbContext())
                {
                    ApplicationUser monUser = db.Users.Where(w => w.UserName == principal.Identity.Name.ToString()).FirstOrDefault();

                    if (monUser != null)
                    {
                        return monUser.Id;
                    }
                    
                }
            }
            return "";

        }
    }
}