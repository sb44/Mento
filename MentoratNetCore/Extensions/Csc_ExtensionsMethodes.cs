using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Principal;
using System.Security.Claims;
using System.Diagnostics;
using MentoratNetCore.Models;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using SendGrid;
using MentoratNetCore.ViewModels.Inscriptions;
using MentoratNetCore.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MentoratNetCore.Extensions
{
    public static class CscExtensionsMethodes
    {
        public static MvcHtmlString Validation_Error_Convertion_Kendo_Template(this HtmlHelper helper)
        {
            //Partie 1 de la méthode

            var sb = new StringBuilder();

            string strCode = "<script type='text/kendo-template' id='message'>" + ((char) 13) +
                        "<span class='k-widget k-tooltip k-tooltip-validation k-invalid-msg field-validation-error' data-for='#=field#' data-valmsg-for='#=field#' id='#=field#_validationMessage' role='alert'>" + ((char) 13) +
                        "<span class='k-icon k-i-warning'> </span> #=message#" + ((char) 13) +
                        "</span>" + ((char) 13) +
                     "</script>";

            sb.Append(strCode);

            return MvcHtmlString.Create(sb.ToString());

        }

        public static MvcHtmlString Validation_Error_Convertion_Kendo_Script(this HtmlHelper helper, string nom)
        {
            //Partie 2 de la méthode
            var sb = new StringBuilder();

            string strCode = "var validationMessageTmpl = kendo.template($('#message').html());" + ((char) 13) +
                             "$('#" + nom + "').kendoValidator();" + ((char) 13) +
                             "$('#" + nom + "').find('.field-validation-error').replaceWith(function (e) {" + ((char) 13) +
                             "var element = $(this);" + ((char) 13) +
                             "return validationMessageTmpl({ field: element.attr('data-valmsg-for'), message: element.text() });})";

            sb.Append(strCode);

            return MvcHtmlString.Create(sb.ToString());

        }

        public static MvcHtmlString ValidationMessageForKendoCsc(this HtmlHelper helper, string nom)
        {
            //Partie 2 de la méthode
            var sb = new StringBuilder();

            string strCode = "<span class='k-invalid-msg' data-for="+nom+"></span>";

            sb.Append(strCode);

            return MvcHtmlString.Create(sb.ToString());

        }

        public static string NomCompletUtilisateur(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                var claimsIdentity = user.Identity as ClaimsIdentity;
                foreach (var claim in claimsIdentity.Claims)
                {
                    if (claim.Type == "NomCompletUser")
                        return claim.Value;
                }
                return "";
            }
            else
                return "";
        }

        public static string DumpToHtmlString<T>(this T objectToSerialize)
        {
            string strHTML = "";
            try
            {
                var writer = LINQPad.Util.CreateXhtmlWriter(true);
                writer.Write(objectToSerialize);
                strHTML = writer.ToString();
            }
            catch (Exception exc)
            {
                Debug.Assert(false, "Investigate why ?" + exc);
            }
            return strHTML;
        }

        public static RoleManager<ApplicationRole> ObtenirRoleManager(IServiceProvider serviceProvider)
        {
            //  var roleStore = new ApplicationRoleStore(context);
            //  var roleMgr = new RoleManager<ApplicationRole>(roleStore);
            var roleMgr = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            return roleMgr;
        }

        public static IQueryable<ApplicationUser> ObtenirUsersInRole(string roleName,RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {

            //List<ApplicationUserRole> lstUserRoles = roleManager.FindByName(roleName).Users.ToList();

            return (from u in userManager.Users
                        where u.UserRoles.Any(a => a.Role.Name == roleName) // where u.Roles.Any(a => a.Role.Name == roleName)
                        select u);
        }


        public class Error
        {
            public Error(string key, string message)
            {
                Key = key;
                Message = message;
            }

            public string Key { get; set; }
            public string Message { get; set; }
        }

        public static IEnumerable<Error> ModelStateAllErrors(ModelStateDictionary modelState)
        {
            IEnumerable<Error> result = from ms in modelState
                                        where ms.Value.Errors.Any()
                                        let fieldKey = ms.Key
                                        let errors = ms.Value.Errors
                                        from error in errors
                                        select new Error(fieldKey, error.ErrorMessage);

            return result;
        }

        public static void AttribuerDateDebutDateFinAnneeFinanciereProjet(string annee,out Nullable<DateTime> dateDebut, out Nullable<DateTime> dateFin)
        {
            int iAnnee;
            string strAnneeFin =annee;
            string moisJourDebut = "-01-01";
            string moisJourFin = "-02-15";

            dateDebut = null;
            dateFin = null;

            if(int.TryParse(annee,out iAnnee) && (iAnnee>=2016 && iAnnee <= 2100))
            {
                switch (iAnnee)
                {
                    case 2016:
                        //la première année le projet a commence plus tôt;
                        moisJourDebut = "-12-17"; //premier jour.
                        annee = "2015";
                        moisJourFin = "-12-31";
                        strAnneeFin = "2016";
                        break;
                    case 2017:
                        moisJourDebut = "-01-01"; //premier jour.
                        annee = "2017";
                        moisJourFin = "-02-28";
                        strAnneeFin = "2018";
                        break;

                    default:
                        moisJourDebut = "-03-01"; //premier jour.
                        annee = iAnnee.ToString();
                        moisJourFin = "-02-28";
                        strAnneeFin = iAnnee + 1 + "";
                        break;
                }

                
                dateDebut =   DateTime.ParseExact(annee + moisJourDebut,"yyyy-MM-dd",CultureInfo.CurrentCulture);
                dateFin = DateTime.ParseExact(strAnneeFin + moisJourFin, "yyyy-MM-dd", CultureInfo.CurrentCulture).AddDays(1).AddTicks(-1);               
            }
        }

        [Authorize]
        public static Boolean VerifierSiUserExiste(string strUser, out ApplicationUser monUserOut)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            List<ApplicationUser> lstUser = userManager.Users.ToList();
            ApplicationUser monUser = lstUser.FirstOrDefault(f => f.UserName == strUser);

            if (monUser != null)
            {
                monUserOut = monUser;
                return true;
            }

            monUserOut = null;

            return false;
        }

        public static string EcrireMessage(Mentore mentore)
        {
            var myClient = new WebClient();
            //Il doit y avoir sur la première ligne des informations sur les images.
            //string strHTML = myClient.DownloadString(HttpContext.Server.MapPath("~/Content/Inscriptions/Courriel_Inscription.html"));
            string strHTML = myClient.DownloadString(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Inscriptions/Courriel_Inscription.html"));

            strHTML = strHTML.Replace("<#:Nom>", mentore.NomComplet_Mentore);
            strHTML = strHTML.Replace("<#:Organisme>", mentore.Organisme_Mentore);
            strHTML = strHTML.Replace("<#:Courriel>", mentore.Courriel_Mentore);
            strHTML = strHTML.Replace("<#:Telephone>", mentore.Telephone_Mentore);
            strHTML = strHTML.Replace("<#:Objectifs>", mentore.Objectifs_Mentore);

            //strHTML=  HttpUtility.HtmlEncode(strHTML);

            return strHTML;
        }

        public static async Task Envoyercourriel(Mentore mentore, string message, string apiKey)
        {

            dynamic sg = new SendGridAPIClient(apiKey);



            //Mon object de courriel
            var monCourriel = new Mail();
            var mesInformations = new Personalization();

            //De:
            var contactEmail = new Email();
            contactEmail.Address = "coordination@coordination-sc.org";
            contactEmail.Name = "Inscription Mentorat";
            monCourriel.From = contactEmail;

            ////À:
            //for (int i = 0; i <= monMail.LesDestinataires.Count - 1; i++)
            //{
            //    contactEmail = new Email();
            //    contactEmail.Address = monMail.LesDestinataires[i];
            //    mesInformations.AddTo(contactEmail);
            //}

            //À:

            ///      contactEmail = new Email();
            ///      contactEmail.Address = "mdupuis@coordination-sc.org";
            ///      mesInformations.AddTo(contactEmail);
            ///
            ///      contactEmail = new Email();
            ///      contactEmail.Address = "mbrisebois@coordination-sc.org";
            ///      mesInformations.AddCc(contactEmail);

            contactEmail = new Email();
            contactEmail.Address = "sasha.bouchard@coordination-sc.org";
            mesInformations.AddTo(contactEmail);


            //if (monMail.LesCC != null)
            //{
            //    for (int i = 0; i <= monMail.LesCC.Count - 1; i++)
            //    {
            //        contactEmail = new Email();
            //        contactEmail.Address = monMail.LesCC[i];
            //        mesInformations.AddCc(contactEmail);
            //    }
            //}

            //contactEmail = new Email();
            //contactEmail.Address = "mbrisebois@upa.qc.ca";
            //mesInformations.AddCc(contactEmail);


            //CC:


            ////CCI:
            //for (int i = 0; i <= monMail.LesCCI.Count - 1; i++)
            //{
            //    contactEmail = new Email();
            //    contactEmail.Address = monMail.LesCCI[i];              
            //    mesInformations.AddBcc(contactEmail);
            //}


            //Objet
            //monCourriel.Subject = "Ha oui!";
            mesInformations.Subject = "Nouvelle inscription au service de mentorat";

            monCourriel.AddPersonalization(mesInformations);

            //Message
            string strMsg = HttpUtility.HtmlDecode(message);
            var content = new Content();
            content.Type = "text/html";
            content.Value = strMsg;
            monCourriel.AddContent(content);

            //for (int i = 0; i <= lstLiensImg.Count - 1; i++)
            //{
            //    //je permet seulement les .png
            //    if (lstLiensImg[i].IndexOf(".png") > 0)
            //    {
            //        string headerPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Signatures/" + lstLiensImg[i]);
            //        var attch = new Attachment();
            //        byte[] imageArray = System.IO.File.ReadAllBytes(headerPath);
            //        string strAttachement = Convert.ToBase64String(imageArray);

            //        attch.Content = strAttachement;
            //        attch.Type = "image/png";
            //        attch.ContentId = lstLiensImg[i].Replace(".png", "");
            //        attch.Filename = lstLiensImg[i];
            //        attch.Disposition = "inline";
            //        monCourriel.AddAttachment(attch);
            //    }
            //}

            //désactivé pour ne pas envoyer de courriel
            dynamic response = await sg.client.mail.send.post(requestBody: monCourriel.Get());
        }



    }
}