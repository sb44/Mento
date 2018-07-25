using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MentoratNetCore.Models;
using Kendo.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.IO;
using MentoratNetCore.Extensions;
using System.Text;
using MentoratNetCore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MentoratNetCore.Views
{
    public class AssignationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext db;

        public AssignationController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            db = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        //private ApplicationDbContext db = new ApplicationDbContext();
        private static List<string> lstLiensImg = new List<string>() ;
        private string strPath { get; set; }

        [Authorize(Roles = "PageMentores")]
        public ActionResult Index()
        {
            ViewBag.Message = "";
            //var values = new ForeignKeyValues
            //{
            //    //Experts = db.Experts,
            //    Mentors = db.Mentors
            //};

            ViewData["listeMentor"] = db.Mentors.ToList().OrderBy(c => c.NomMentor).ThenBy(c => c.PrenomMentor);

            return View();
        }

        //[Authorize(Roles = "PageMentores")]
        //public ActionResult Mentores_Read([DataSourceRequest]DataSourceRequest request,string filtreAnnee,bool filtreTous)
        //{
        //    string filtreMentor = "";
        //    int iAnnee = 0;
        //   string idMentor = "";
        //    bool boolDroitGerer = User.IsInRole("GererUtilisateur");

        //    int.TryParse(filtreAnnee,out iAnnee);

        //    if(User.IsInRole("Mentors"))
        //    {
        //        if(!filtreTous)
        //        {
        //            //on affiche seulement les mentorés du Mentor
        //            filtreMentor = User.Identity.GetUserId();
        //        }
        //        idMentor = User.Identity.GetUserId();
        //    }


        //    //if (request.Sorts.Count > 0 || request.Filters.Count>0 || request.Groups.Count>0) // par défaut le tri est par la date
        //    //{
        //    //    RemapperViewMemberSort(request);                
        //    //}

        //    List<Models.MentoratInscription> mentoresInsc;

        //    if (iAnnee>=2016)
        //    {
        //        if(filtreMentor!="")
        //        {
        //            //Afficher seulement les mentorés de l'utilisateur
        //            mentoresInsc = db.MentoratInscription.Where(w => w.Annee == iAnnee && w.Mentor.NoMentor == filtreMentor).ToList();
        //        }
        //        else
        //        {
        //            mentoresInsc = db.MentoratInscription.Where(w => w.Annee == iAnnee).ToList();
        //        }                 
        //    }
        //    else
        //    {
        //        if (filtreMentor != "")
        //        {
        //            //Afficher seulement les mentorés de l'utilisateur
        //            mentoresInsc = db.MentoratInscription.Where(w => w.Mentor.NoMentor == filtreMentor).ToList();
        //        }
        //        else
        //        {
        //            mentoresInsc = db.MentoratInscription.ToList();
        //        }

        //    }


        //    List<ApplicationUser> lesUsers = db.Users.ToList();

        //    if(!boolDroitGerer)
        //    {
        //        //on cache les mentorés qui sont non assignés pour ceux qui n'ont pas le droit de gérer. (ou celui administratif)
        //        mentoresInsc = mentoresInsc.Where(w => w.Mentor.NoMentor != "1").ToList();
        //    }

        //   //on retire le mentore administration
        //    DataSourceResult test = mentoresInsc.Where(w=> w.Mentore.No_Mentore!= "F1490F96-566E-4440-ABE1-11660546E914").ToDataSourceResult(request, m => new Mentorat.ViewModels.AssignationViewModel()
        //    {
        //        NoInscription = m.Id,

        //        NomUtilisateur = lesUsers.FirstOrDefault(u => u.Id == m.Mentore.No_Mentore).UserName.ToString(),
        //        Annee = m.Annee,
        //        //PrenomMentore = m.Mentore.Prenom_Mentore,
        //        //NomMentore = m.Mentore.Nom_Mentore,
        //        //Organisme_Mentore = m.Mentore.Organisme_Mentore,                
        //        Mentore = new Mentorat.ModelsViews.MentoreViewModel
        //        {
        //            NoMentore = m.Mentore.No_Mentore,
        //            PrenomMentore = m.Mentore.Prenom_Mentore,
        //            NomMentore = m.Mentore.Nom_Mentore,
        //            CellulaireMentore = m.Mentore.Cellulaire_Mentore,
        //            CourrielMentore = m.Mentore.Courriel_Mentore,
        //            Organisme_Mentore = m.Mentore.Organisme_Mentore,
        //            Expertises = m.Mentore.Expertises.Select(t => new Expertise
        //            {
        //                Nom_Expertise = t.Nom_Expertise,
        //                No_Expertise = t.No_Expertise
        //            }).ToList(),
        //           Objectifs_Mentore =  m.Mentore.Objectifs_Mentore

        //        },
        //        Mentor = new ViewModels.Assignation.AssignationMentorViewModel()
        //        {
        //            NoMentor=m.Mentor.NoMentor,
        //            PrenomMentor = m.Mentor.PrenomMentor,
        //            NomMentor = m.Mentor.NomMentor
        //        },
        //        //Objectifs_Mentore = m.Objectifs_Mentore,
        //        APaye = m.APaye,
        //        DateInscription = m.DateInscription,
        //         DateDebut = m.DateDebut,
        //         DateFin = m.DateFin,
        //         AfficherBoutonPlan = idMentor != ""? (m.Mentor.NoMentor == idMentor?true:false) :boolDroitGerer , 
        //          //Mentore = m.Mentore,
        //         // Mentor = m.Mentor,
        //        MentoratCategorie = new Mentorat.ModelsViews.MentoratCategorieViewModel() { Id = m.MentoratCategorie.Id, Nom = m.MentoratCategorie.Nom, Description = m.MentoratCategorie.Description }
        
        //    });
        //    return Json(test, JsonRequestBehavior.AllowGet);
        //}  

        [Authorize(Roles = "PageMentores")]
        public ActionResult Mentores_Read([DataSourceRequest]DataSourceRequest request, string filtreAnnee, bool filtreTous)
        {

            //filtreAnnee
            //1= dernière active pour tous
            //2= dernière inactive (si aucune inscription active)
            //3= toutes les inscriptions depuis le début

            string filtreMentor = "";
            int iAnnee = 0;
            string idMentor = "";
            bool boolDroitGerer = User.IsInRole("GererUtilisateur");

            int.TryParse(filtreAnnee, out iAnnee);

            if (User.IsInRole("Mentors"))
            {
                if (!filtreTous)
                {
                    //on affiche seulement les mentorés du Mentor
                    filtreMentor = _userManager.GetUserId(User);
                }
                idMentor = _userManager.GetUserId(User);
            }


            //if (request.Sorts.Count > 0 || request.Filters.Count>0 || request.Groups.Count>0) // par défaut le tri est par la date
            //{
            //    RemapperViewMemberSort(request);                
            //}

            List<Models.MentoratInscription> mentoresInsc;
            List<Models.MentoratInscription> mentoresEnCours;

            DateTime maintenant = DateTime.Now.Date;

            if (iAnnee == 1 || iAnnee == 2)
            {
                //On va chercher les inscriptions en cours
               // mentoresInsc = db.MentoratInscription.Where(w => w.DateDebut <= maintenant && w.DateFin >= maintenant).ToList();

                //je garde l'inscription qui se termine ne dernier pour les inscriptions valide du mentoré
                List<MentoratInscription> mentoresActifs = db.MentoratInscription.Where(w => w.DateDebut <= maintenant && w.DateFin >= maintenant).GroupBy(g => g.Mentore.No_Mentore ).SelectMany(s => s.OrderByDescending(d => d.DateFin).Take(1)).ToList();

                if(iAnnee == 1)
                {
                    mentoresInsc = mentoresActifs;
                }
                else
                {
                    //cas 2
                    if(mentoresActifs !=null && mentoresActifs.Count>0)
                    {
                        string[] idMentores = mentoresActifs.Select(s => s.Mentore.No_Mentore).ToArray();
                                               
                        mentoresInsc = db.MentoratInscription.Where(w=> !idMentores.Contains(w.Mentore.No_Mentore)).GroupBy(g=> g.Mentore.No_Mentore).SelectMany(s => s.OrderByDescending(d => d.DateFin).Take(1)).ToList(); //toutes les inscriptions sauf les actives
                    }
                    else
                    {
                        mentoresInsc = db.MentoratInscription.GroupBy(g => g.Mentore.No_Mentore).SelectMany(s => s.OrderByDescending(d => d.DateFin).Take(1)).ToList(); //toutes les inscriptions
                    }                  
                }
            }
            else
            {
                //toutes les inscriptions
                mentoresInsc = db.MentoratInscription.GroupBy(g => g.Mentore.No_Mentore).SelectMany(s => s.OrderByDescending(d => d.DateFin).Take(1)).ToList(); //toutes les inscriptions
            }

            if (filtreMentor != "")
            {
                if(mentoresInsc !=null && mentoresInsc.Count > 0)
                {
                    mentoresInsc = mentoresInsc.Where(w => w.Mentor.NoMentor == filtreMentor).ToList();
                }
            }            


            List<ApplicationUser> lesUsers = db.Users.ToList();

            if (!boolDroitGerer)
            {
                //on cache les mentorés qui sont non assignés pour ceux qui n'ont pas le droit de gérer. (ou celui administratif)

                if(mentoresInsc !=null && mentoresInsc.Count >0)
                {
                    mentoresInsc = mentoresInsc.Where(w => w.Mentor.NoMentor != "1").ToList();
                }                
            }

            //on retire le mentore administration
            DataSourceResult test1 = mentoresInsc.Where(w => w.Mentore.No_Mentore != "F1490F96-566E-4440-ABE1-11660546E914")
                        .ToDataSourceResult(request);

            DataSourceResult test = mentoresInsc.Where(w => w.Mentore.No_Mentore != "F1490F96-566E-4440-ABE1-11660546E914")
                                    .ToDataSourceResult(request, m => new MentoratNetCore.ViewModels.AssignationViewModel()
                                    {
                                        NoInscription = m.Id,

                                        NomUtilisateur = lesUsers.FirstOrDefault(u => u.Id == m.Mentore.No_Mentore).UserName.ToString(),
                                        Annee = m.Annee,
                                        //PrenomMentore = m.Mentore.Prenom_Mentore,
                                        //NomMentore = m.Mentore.Nom_Mentore,
                                        //Organisme_Mentore = m.Mentore.Organisme_Mentore,                
                                        Mentore = new MentoratNetCore.ModelsViews.MentoreViewModel
                                        {
                                            NoMentore = m.Mentore.No_Mentore,
                                            PrenomMentore = m.Mentore.Prenom_Mentore,
                                            NomMentore = m.Mentore.Nom_Mentore,
                                            CellulaireMentore = m.Mentore.Cellulaire_Mentore,
                                            CourrielMentore = m.Mentore.Courriel_Mentore,
                                            Organisme_Mentore = m.Mentore.Organisme_Mentore,
                                            //Expertises = m.Mentore.MentoresExpertises.Select(t => new Expertise // ********* TODO VÉRIFIER                                                                                                                 //Expertises = m.Mentore.MentoresExpertises.Select(e => e.Expertise).Select(t => new Expertise // ********* TODO VÉRIFIER
                                            //{
                                            //    Nom_Expertise = t.Expertise.Nom_Expertise,  //Nom_Expertise = t.Nom_Expertise, //SB: enlever pour EFCore2.0
                                            //    No_Expertise = t.No_Expertise
                                            //}).ToList(),

                                            Expertises = m.Mentore.MentoresExpertises.Select(e => e.Expertise).Select(t => new Expertise
                                            {
                                                Nom_Expertise = t.Nom_Expertise,
                                                No_Expertise = t.No_Expertise
                                            }).ToList(),

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
                                        AfficherBoutonPlan = idMentor != "" ? (m.Mentor.NoMentor == idMentor ? true : false) : boolDroitGerer,
                                        //Mentore = m.Mentore,
                                        // Mentor = m.Mentor,
                                        MentoratCategorie = new MentoratNetCore.ModelsViews.MentoratCategorieViewModel() { Id = m.MentoratCategorie.Id, Nom = m.MentoratCategorie.Nom, Description = m.MentoratCategorie.Description }


                                    });

            return Json(test);

        }


        [Authorize(Roles = "GererMentores")]
        [HttpPost]       
        //[ValidateAntiForgeryToken]
        public ActionResult Mentores_Update([DataSourceRequest]DataSourceRequest request, Mentore mentore)
        {
            
            if (ModelState.IsValid)
            {

                Mentore monMentore = db.Mentores.First(f => f.No_Mentore == mentore.No_Mentore);

                monMentore.Prenom_Mentore = mentore.Prenom_Mentore;
                monMentore.Nom_Mentore = mentore.Nom_Mentore;
                monMentore.Organisme_Mentore = mentore.Organisme_Mentore;
                monMentore.Courriel_Mentore = mentore.Courriel_Mentore;
                monMentore.Telephone_Mentore = mentore.Telephone_Mentore;
                monMentore.Cellulaire_Mentore = mentore.Cellulaire_Mentore;
                monMentore.No_Mentor_Mentore = mentore.No_Mentor_Mentore;
                // monMentore.MentorMentore = db.Mentors.First(f => f.NoMentor == mentore.MentorMentore.NoMentor);
                monMentore.Objectifs_Mentore = mentore.Objectifs_Mentore;
                monMentore.Paye_Mentore = mentore.Paye_Mentore;
                monMentore.DateInscription_Mentore = mentore.DateInscription_Mentore;
                monMentore.upsize_ts = mentore.upsize_ts;

                //// List<Expertise> listMentore = db.Expertises.ToList().Where(l => mentore.Expertises.Any(p => p.No_Expertise == l.No_Expertise)).ToList();
                //// List<Expertise> oldEx = monMentore.Expertises.ToList();
                     
                //// List<Expertise> deleteEx = oldEx.Except(listMentore).ToList();
                //// List<Expertise> addEx = listMentore.Except(oldEx).ToList();
                     
                //// deleteEx.ForEach(c => monMentore.Expertises.Remove(c));
                //// addEx.ForEach(c => monMentore.Expertises.Add(c));
                List<MentoresExpertises> listMentoreXp = db.MentoresExpertises.ToList().Where(l => mentore.MentoresExpertises.Any(p => p.No_Expertise == l.No_Expertise)).ToList();
                List<MentoresExpertises> oldExXp = monMentore.MentoresExpertises.ToList();

                List<MentoresExpertises> deleteExXp = oldExXp.Except(listMentoreXp).ToList();
                List<MentoresExpertises> addExXp = listMentoreXp.Except(oldExXp).ToList();

                deleteExXp.ForEach(c => monMentore.MentoresExpertises.Remove(c)); //deleteEx.ForEach(c => monMentore.Expertises.Remove(c));
                addExXp.ForEach(c => monMentore.MentoresExpertises.Add(c)); // addEx.ForEach(c => monMentore.Expertises.Add(c));


                db.SaveChanges();

            }


            return Json(new[] { mentore }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "AssignationSuppression")]
        [HttpPost]
        public ActionResult Mentores_Destroy([DataSourceRequest]DataSourceRequest request, Mentore mentore)
        {
            if (ModelState.IsValid)
            {
                var entity = new Mentore
                {
                    No_Mentore = mentore.No_Mentore,
                    Prenom_Mentore = mentore.Prenom_Mentore,
                    Nom_Mentore = mentore.Nom_Mentore,
                    Organisme_Mentore = mentore.Organisme_Mentore,
                    Courriel_Mentore = mentore.Courriel_Mentore,
                    Telephone_Mentore = mentore.Telephone_Mentore,
                    Cellulaire_Mentore = mentore.Cellulaire_Mentore,
                    No_Expert_Mentore = mentore.No_Expert_Mentore,
                    MentorMentore = mentore.MentorMentore,
                    Objectifs_Mentore = mentore.Objectifs_Mentore,
                    Paye_Mentore = mentore.Paye_Mentore,
                    DateInscription_Mentore = mentore.DateInscription_Mentore,
                    upsize_ts = mentore.upsize_ts
                };
                db.Mentores.Attach(entity);

                var objMentoreEBd = db.Mentores.Include("MentoresExpertises").First(e => e.No_Mentore == mentore.No_Mentore); //var objMentoreEBd = db.Mentores.Include("Expertises").First(e => e.No_Mentore == mentore.No_Mentore);
                var objMExpertisesBd = objMentoreEBd.MentoresExpertises.ToList(); //var objMExpertisesBd = objMentoreEBd.Expertises.ToList();

                foreach (MentoresExpertises objExpertise in objMExpertisesBd) // foreach (Expertise objExpertise in objMExpertisesBd)
                {
                    entity.MentoresExpertises.Remove(objExpertise); // entity.Expertises.Remove(objExpertise);

                }

                var objMentoreIBd = db.Mentores.Include("Interventions").First(e => e.No_Mentore == mentore.No_Mentore);
                var objMInterventionsBd = objMentoreIBd.Interventions.ToList();

                foreach (Intervention objIntervention in objMInterventionsBd)
                {
                    entity.Interventions.Remove(objIntervention);
                }

                db.Mentores.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { mentore }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "AssignationCourriel")]
        private static async Task AsyncEnvoyercourriel(ObjMail monMail,string apiKey)
        {
           
            dynamic sg = new SendGridAPIClient(apiKey);

            var lesCourriels = new List<string>();

            //Mon object de courriel
            var monCourriel = new Mail();
            var mesInformations = new Personalization();

            //De:
            var contactEmail = new Email();
            contactEmail.Address = monMail.De;
            contactEmail.Name = monMail.DeNom;        
            monCourriel.From = contactEmail;
            
            //À:
            for (int i = 0; i <= monMail.LesDestinataires.Count - 1; i++)
            {
                if(lesCourriels.IndexOf(monMail.LesDestinataires[i])==-1)
                {
                    contactEmail = new Email();
                    contactEmail.Address = monMail.LesDestinataires[i];
                    mesInformations.AddTo(contactEmail);
                    lesCourriels.Add(monMail.LesDestinataires[i]);
                }    
            }

            if(monMail.LesCC != null)
            {
                for (int i = 0; i <= monMail.LesCC.Count - 1; i++)
                {
                    if (lesCourriels.IndexOf(monMail.LesCC[i]) == -1)
                    {
                        contactEmail = new Email();
                        contactEmail.Address = monMail.LesCC[i];
                        mesInformations.AddCc(contactEmail);
                        lesCourriels.Add(monMail.LesCC[i]);
                    }
                }
            }

            //pour recevoir une copie du courriel envoyé du système.
            if(lesCourriels.IndexOf(monMail.De)==-1)
            {
                contactEmail = new Email();
                contactEmail.Address = monMail.De;
                mesInformations.AddCc(contactEmail);
            }

            //CC:


            //if (monMail.LesCCI != null)
            //{
            //    //CCI:
            //    for (int i = 0; i <= monMail.LesCCI.Count - 1; i++)
            //    {
            //        contactEmail = new Email();
            //        contactEmail.Address = monMail.LesCCI[i];
            //        mesInformations.AddBcc(contactEmail);
            //    }
            //}



            //Objet           
            mesInformations.Subject = monMail.Sujet;

            monCourriel.AddPersonalization(mesInformations);

            //Message
            string strMsg = HttpUtility.HtmlDecode(monMail.Message);
            var content = new Content();
            content.Type = "text/html";
            content.Value = strMsg;
            monCourriel.AddContent(content);

            for (int i = 0; i <= lstLiensImg.Count - 1; i++)
            {
                //je permet seulement les .png
                if (lstLiensImg[i].IndexOf(".png") > 0)
                {
                    string headerPath = Path.Combine(Startup.WebRootPath, "/Content/Signatures/" + lstLiensImg[i]); // string headerPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Signatures/" + lstLiensImg[i]);
                    var attch = new Attachment();
                    byte[] imageArray = System.IO.File.ReadAllBytes(headerPath);
                    string strAttachement = Convert.ToBase64String(imageArray);

                    attch.Content = strAttachement;
                    attch.Type = "image/png";
                    attch.ContentId = lstLiensImg[i].Replace(".png", "");
                    attch.Filename = lstLiensImg[i];
                    attch.Disposition = "inline";
                    monCourriel.AddAttachment(attch);
                }
            }

            //désactivé pour ne pas envoyer de courriel
            dynamic response = await sg.client.mail.send.post(requestBody: monCourriel.Get());
        }

        [Authorize(Roles = "AssignationCourriel")]
        [HttpPost]
        public ActionResult SendCourriel([DataSourceRequest]DataSourceRequest request, string idInscription)
        {
            var inscription = db.MentoratInscription.FirstOrDefault(f => f.Id == idInscription);
                     
            if (inscription!= null)
            {
                var lesDesti = new List<string>();
                string strNomMentor = "";
                string strNomMentorComplet = "";

                lesDesti.Add(inscription.Mentore.Courriel_Mentore);

                if (inscription.Mentore.No_Mentor_Mentore != "1")
                {
                    Mentor monMentor = db.Mentors.First(f => f.NoMentor == inscription.Mentor.NoMentor);

                    if (monMentor != null)
                    {
                        lesDesti.Add(monMentor.CourrielMentor);
                        strNomMentorComplet = monMentor.NomCompletMentor;
                        strNomMentor = monMentor.PrenomMentor;
                    }
                }

                var lesCC = new List<string>();
                //lesCC.Add("mbrisebois@coordination-sc.org");

                //var lesCCI = new List<string>();
                //lesCC.Add("sondagemartinqc101@hotmail.ca");

                string strMsg = "";

                strMsg += ConvertirHtmlToText();

                strMsg = ModifierModeleHtml(inscription.Mentore, strNomMentor, strNomMentorComplet, strMsg);


                var monCourriel = new ObjMail() { De = "mdupuis@coordination-sc.org", DeNom = "Michel Dupuis", Message = strMsg, LesDestinataires = lesDesti, LesCC = lesCC, Sujet = "Inscription au service de mentorat" };

                return PartialView("EnvoyerCourriel", monCourriel);
            }

            return Json(new { success = false, });


        }



        [Authorize(Roles = "AssignationCourriel")]
        private string ModifierModeleHtml(Mentore mentore,string nomMentor,string nomMentorComplet, string strHtml)
        {

            string monHtml = strHtml;
            string valeur = "";
                                 
            monHtml= monHtml.Replace(HttpUtility.HtmlEncode("<#:nomDestinataire>"), mentore.Prenom_Mentore);
            monHtml = monHtml.Replace(HttpUtility.HtmlEncode("<#:nomDestinataireComplet>"), mentore.NomComplet_Mentore);
            monHtml = monHtml.Replace(HttpUtility.HtmlEncode("<#:nomMentor>"), nomMentor);
            monHtml = monHtml.Replace(HttpUtility.HtmlEncode("<#:nomMentorComplet>"), nomMentorComplet);
            monHtml = monHtml.Replace(HttpUtility.HtmlEncode("<#:nomOrganismeDestinataire>"), mentore.Organisme_Mentore);
            monHtml = monHtml.Replace(HttpUtility.HtmlEncode("<#:adresseCourrielDestinataire>"), mentore.Courriel_Mentore);
            monHtml = monHtml.Replace(HttpUtility.HtmlEncode("<#:telephoneDestinataire>"), mentore.Telephone_Mentore);

            valeur = mentore.Cellulaire_Mentore;

            if (valeur == null || valeur.Trim() == "")
                valeur = "---";

            monHtml = monHtml.Replace(HttpUtility.HtmlEncode("<#:cellulaireDestinataire>"), valeur);

            valeur = ObtenirChampsExpertises(mentore.MentoresExpertises.Select(e => e.Expertise).ToList()); //valeur = ObtenirChampsExpertises(mentore.Expertises);

            if (valeur == null || valeur.Trim() == "")
                valeur = "---";

            monHtml = monHtml.Replace(HttpUtility.HtmlEncode("<#:champsExpertiseDestinataire>"), valeur);

            valeur = mentore.Objectifs_Mentore;

            if (valeur == null || valeur.Trim() == "")
                valeur = "---";

            monHtml = monHtml.Replace(HttpUtility.HtmlEncode("<#:objectifsDestinataire>"), valeur);

            return monHtml;
        }

        [Authorize]
        private string ObtenirChampsExpertises(ICollection<Expertise> lesExpertises)
        {

            string strExpertises = "";

            if (lesExpertises!=null)
            {
                List<Expertise> mesExpertises = db.Expertises.ToList().Where(w => lesExpertises.Any(a => a.No_Expertise == w.No_Expertise)).ToList();
                strExpertises = string.Join(",", mesExpertises);
            }

            return strExpertises;
        }

        [Authorize(Roles = "AssignationCourriel")]
        [HttpGet]
        public ActionResult EnvoyerCourriel( ObjMail monMail)
        {
            return PartialView(monMail);
        }

        [HttpPost] //[HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "AssignationSuppression")]
        [Authorize(Roles = "AssignationCourriel")]
        public ActionResult EnvoyerCourriel([DataSourceRequest]DataSourceRequest request, ObjMail monMail)
        {
            if (ModelState.IsValid)
            {
                string test = HttpUtility.HtmlDecode(monMail.Message);
                string apiKey = Environment.GetEnvironmentVariable("SENDGRID_KEY");
                ///AsyncEnvoyercourriel(monMail,apiKey).Wait();
                return Json(new { success = true, });
            }
            
            return PartialView(monMail);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public class ObjMail
        {
            public string De { get; set; }
            public string DeNom { get; set; }

            [Required(ErrorMessage = "Veuillez inscrire un destinataire.")]
            public List<string> LesDestinataires { get; set; }

            public List<string> LesCC { get; set; }

            public List<string> LesCCI { get; set; }

            [Required(ErrorMessage = "Veuillez inscrire un sujet.")]
            public string Sujet { get; set; }

            [Required(ErrorMessage = "Veuillez inscrire votre message.")]
            public string Message { get; set; }


        }

        [Authorize]
        public class ObjDestinataire:IComparable<ObjDestinataire>
        {
            public string NomComplet { get; set; }
            public string AdresseCourriel { get; set; }

            public int CompareTo(ObjDestinataire autre)
            {
                return NomComplet.CompareTo(autre.NomComplet);
            }
        }

        [Authorize]
        public JsonResult ObtenirCourriels()
        {
            List<Mentore> maSource = db.Mentores.ToList();
            List<Mentor> maSource2 = db.Mentors.ToList();

            List<ObjDestinataire> lstMentores = maSource.Where(w => w.Courriel_Mentore.IndexOf("@") > 0).Select(s => new ObjDestinataire { NomComplet = s.NomComplet_Mentore, AdresseCourriel = s.Courriel_Mentore }).ToList() ; //Take(1) to just fill with one item my multiselect
            List<ObjDestinataire> lstMentore2 = maSource2.Where(w=> w.CourrielMentor.IndexOf("@")>0).Select(s => new ObjDestinataire { NomComplet = s.NomCompletMentor, AdresseCourriel = s.CourrielMentor}).ToList();

            lstMentores.AddRange(lstMentore2);

            lstMentores.Sort();

            return Json(lstMentores);
        }

        [Authorize]
        private string ConvertirHtmlToText()
        {
            strPath = Path.Combine(Startup.WebRootPath, "/Content/Signatures/"); //strPath = HttpContext.Server.MapPath("~/Content/Signatures/");

           var myClient = new WebClient();
            //Il doit y avoir sur la première ligne des informations sur les images.
            string strHTML = myClient.DownloadString(Path.Combine(Startup.WebRootPath, "/Content/Signatures/BienvenueAuServiceDeMentorat.htm")); // string strHTML = myClient.DownloadString(HttpContext.Server.MapPath("~/Content/Signatures/BienvenueAuServiceDeMentorat.htm"));

            int iPosFin = strHTML.IndexOf("<html>");
            string strCodeInformations = strHTML.Substring(0, iPosFin);
            string[] tabImages = strCodeInformations.Split(':');

            for (int i = 1; i <= tabImages.Length-2; i++)
            {
                string strNomImage = tabImages[i];
                lstLiensImg.Add(strNomImage);
            }

            return strHTML;
        }

        [HttpGet]
        [AuthorizeCustom.Authorize.CustomAuthorizeMentoresDossierPhytoAttribute]
        public ActionResult Modification(string utilisateur,string type)
        {
            var db = new ApplicationDbContext();

            ApplicationUser monUser = db.Users.FirstOrDefault(f => f.UserName == utilisateur);
            
            if (monUser !=null)
            {



             Mentore monMentore =  db.Mentores.FirstOrDefault(f => f.No_Mentore == monUser.Id);

                if (monMentore != null)
                {
                    List<MentoratInscription> lesInscriptions = db.MentoratInscription.Where(w => w.Mentore.No_Mentore == monUser.Id && w.MentoratCategorie.Nom.ToLower() == type.ToLower()).OrderByDescending(o=> o.Annee).ToList();
                    int anneeDernier=0;
                    bool dernierEstPaye=false;
                    bool afficherRenouvellement = false;
                    double nbJours = 1000;

                    if (lesInscriptions != null && lesInscriptions.Count>0)
                    {
                        anneeDernier = lesInscriptions[0].Annee;
                        dernierEstPaye = lesInscriptions[0].APaye;

                        DateTime dateDernier = lesInscriptions[0].DateFin;
                        DateTime dateNow = DateTime.Now.Date;
                       

                        if(dernierEstPaye)
                        {
                            if (int.Parse(_configuration["AppSettings:AnneeMentorat"]) != anneeDernier) //if (int.Parse(System.Configuration.ConfigurationManager.AppSettings["AnneeMentorat"]) != anneeDernier)
                            {
                                if (dateDernier != null)
                                {
                                    nbJours = (dateDernier - dateNow).TotalDays;
                                }



                                if (double.Parse(_configuration["AppSettings:NbJrsAvantRenouvellement"]) >= nbJours)
                                {
                                    afficherRenouvellement = true;
                                }
                            }
                        }
                        else
                        {
                            afficherRenouvellement = true;
                        }
                        
                    }

                    string msg = "";
                    nbJours = Math.Floor(Math.Floor(nbJours));

                    string idUser = User.Id();
                    bool AfficherBoutonPaypal = false;

                    if (afficherRenouvellement)
                    {
                        if(dernierEstPaye)
                        {
                            //la dernière année est payé
                            if (idUser == monMentore.No_Mentore)
                            {
                                //est le Mentoré
                                if (nbJours > 0)
                                {
                                    //reste des jours
                                    msg = "Votre abonnnement se termine dans " + nbJours + " jours! Vous pouvez dès maintenant vous réinscrire pour la prochaine saison en cliquant sur le bouton Acheter!";
                                }
                                else
                                {
                                    //expiré
                                    msg = "Votre abonnnement est terminé! Vous pouvez vous réinscrire pour la prochaine saison en cliquant sur le bouton Acheter!";
                                }
                                AfficherBoutonPaypal = true;
                            }
                            else
                            {
                                //est un mentor ou admin
                                if (nbJours > 0)
                                {
                                    //reste des jours
                                    msg = "L'abonnnement du mentoré se termine dans " + nbJours + " jours! Le mentoré peut maintenant se réinscrire en se connectant à son espace client!";
                                }
                                else
                                {
                                    //expiré
                                    msg = "L'abonnnement du mentoré est terminé! Le mentoré peut se réinscrire en se connectant à son espace client!";
                                }
                            }
                        }
                        else
                        {
                            if (idUser == monMentore.No_Mentore)
                            {
                                msg = "Si ce n'est pas déjà fait, vous pouvez effectuer votre paiement pour la saison " + anneeDernier + " en cliquant sur le bouton Acheter!";
                                AfficherBoutonPaypal = true;
                            }
                            else
                            {
                                msg = "Le mentoré ne semble pas avoir effectué son paiement pour la saison " + anneeDernier + "! Le mentoré peut payer en se connectant à son espace client!";
                            }
                                
                        }
                       
                    }

                    if(idUser != monMentore.No_Mentore)
                    {
                        ViewData["ModificationPdf"] = true;
                    }
                    else
                    {
                        ViewData["ModificationPdf"] = false;
                    }

                    var monModel = new ViewModels.UtilisateurMentoratViewModel()
                    {
                        IdMentore = monMentore.No_Mentore,
                        Courriel_Mentore = monMentore.Courriel_Mentore,
                        Nom_Mentore = monMentore.Nom_Mentore,
                        Prenom_Mentore = monMentore.Prenom_Mentore,
                        // Expertises = monMentore.Expertises.Select(s=> new Mentorat.ModelsViews.ExpertiseViewModel() {No_Expertise = s.No_Expertise, Nom_Expertise = s.Nom_Expertise }).ToList(),
                        LesIdExpertises = monMentore.MentoresExpertises.Select(s => s.No_Expertise).ToArray(), // LesIdExpertises = monMentore.Expertises.Select(s => s.No_Expertise).ToArray(), 
                        Objectifs_Mentore = monMentore.Objectifs_Mentore,
                        Inscriptions = lesInscriptions,
                        TypeSectionMentorat = type,
                        NomUtilisateur = monUser.UserName,
                        DerniereAnnee = anneeDernier,
                        DerniereAnnePaye = dernierEstPaye,
                        AfficherRenouvellement = afficherRenouvellement,
                        NbJoursRenouvellement = int.Parse(Math.Floor(nbJours).ToString()),
                        MessageRenouvellement = msg,
                        AfficherBoutonPaypal= AfficherBoutonPaypal
                    };


                    //Remplir la liste des Mentors
                    IEnumerable<MentoratNetCore.ViewModels.ObjBaseVM> lstMentors = db.Mentors.OrderBy(o => o.PrenomMentor).OrderBy(o=>o.NomMentor).Select(s => new MentoratNetCore.ViewModels.ObjBaseVM{ Id = s.NoMentor, Nom =  s.PrenomMentor + " " + s.NomMentor  });
                    ViewData["lstMentors"]=lstMentors;


                    
                   


                    return PartialView(monModel);
                }

            }

            //Ne devrait pas arriver ici
            return RedirectToAction("Index", "Accueil"); 
        }

        [HttpPost]
        // [Authorize(Roles = "ParametresDroits")]
        [ValidateAntiForgeryToken]
        [AuthorizeCustom.Authorize.CustomAuthorizeMentoresDossierPhytoAttribute]
        public ActionResult Modification([DataSourceRequest]DataSourceRequest request, MentoratNetCore.ViewModels.UtilisateurMentoratViewModel model)
        {

          
            
            if (ModelState.IsValid)
            {

                var db = new ApplicationDbContext();

                Mentore mentore = db.Mentores.FirstOrDefault(f => f.No_Mentore == model.IdMentore);
                List<Expertise> expertises = db.Expertises.Where(w => model.LesIdExpertises.Contains(w.No_Expertise)).ToList();
                List<Expertise> expertisesOld = mentore.MentoresExpertises.Select(e => e.Expertise).ToList(); //List<Expertise> expertisesOld = mentore.Expertises.ToList();
                List<Expertise> expertisesToAdd = expertises.Except(expertisesOld).ToList();
                List<Expertise> expertisesToRemove = expertisesOld.Except(expertises).ToList();

                foreach (var expertise in expertisesToRemove) //  SB: TODO.. vérifier si le remove se fait dans les 2 tables (Mentore et Expertise)..
                {
                    mentore.MentoresExpertises.Remove(db.MentoresExpertises.Where(me => me.No_Mentore == mentore.No_Mentore && me.No_Expertise == expertise.No_Expertise).ToList().FirstOrDefault()); //mentore.Expertises.Remove(expertise);
                }

                // mentore.Expertises.AddRange(expertisesToAdd);
                foreach (var expertise in expertisesToAdd) // SB: TODO.. vérifier si l'ajout se fait dans les 2 tables (Mentore et Expertise)..
                {
                    var me = new MentoresExpertises() { Expertise = expertise, Mentore = mentore };
                    mentore.MentoresExpertises.Add(me);
                }

                //mentore.MentoresExpertises = model.LesIdExpertises;
                mentore.Objectifs_Mentore = model.Objectifs_Mentore;

                
                db.Entry(mentore).State = EntityState.Modified;

                db.SaveChanges();
                               

                return Json(new { success = true });
            }
            else
            {

                return Json(new { success = false, erreurs = CscExtensionsMethodes.ModelStateAllErrors(ModelState) });
            }

                      

        }
        [HttpPost]
        [AuthorizeCustom.Authorize.CustomAuthorizUserEstLeMentoreOnlyAttribute]
        public ActionResult RenouvellerMentorat(string utilisateur,string typeMentorat, string nomCompletMentore)
        {
            var db = new ApplicationDbContext();
            string invoice = "";

            ApplicationUser monUser = db.Users.FirstOrDefault(f => f.UserName == utilisateur);         



            if (monUser != null)
            {
                MentoratInscription inscription = db.MentoratInscription.OrderByDescending(o => o.Annee).FirstOrDefault(f => f.Mentore.No_Mentore == monUser.Id && f.MentoratCategorie.Nom.ToLower() == typeMentorat.ToLower());

                if ( inscription !=null && !inscription.APaye)
                {
                    invoice = inscription.Id;
                }
                else
                {                    
                    if (inscription != null && int.Parse(this._configuration["AppSettings:AnneeMentorat"]) > inscription.Annee)
                    {
                        Mentore mentore = db.Mentores.FirstOrDefault(f => f.No_Mentore == monUser.Id);

                        if(mentore!= null)
                        {
                            DateTime dateDebut;
                            
                            if(inscription.DateFin > DateTime.Now.Date )
                            {
                                dateDebut = inscription.DateFin.AddDays(1);
                            }                     
                            else
                            {
                                dateDebut = DateTime.Now.Date;
                            }
                            //il faut créer une inscription
                            inscription = new MentoratInscription()
                            {
                                Annee = int.Parse(this._configuration["AppSettings:AnneeMentorat"]),
                                DateInscription = DateTime.Now,
                                DateDebut = dateDebut.Date,
                                DateFin = dateDebut.Date.AddYears(1).AddDays(-1),
                                MentoratCategorie = db.MentoratCategorie.FirstOrDefault(w => w.Nom.ToLower() == typeMentorat.ToLower()),
                                Mentor = db.Mentors.FirstOrDefault(w => w.NoMentor == "1"),
                                Mentore = mentore,
                                APaye = false
                            };

                            db.MentoratInscription.Add(inscription);

                            db.SaveChanges();

                            invoice = inscription.Id;

                            string apiKey = Environment.GetEnvironmentVariable("SENDGRID_KEY");
                            Extensions.CscExtensionsMethodes.Envoyercourriel(mentore, Extensions.CscExtensionsMethodes.EcrireMessage(mentore), apiKey).Wait();
                        }

                    }
                }

            }


         return   PayerAvecPaypal(invoice,nomCompletMentore);
          
        }

        [Authorize]
        public  ActionResult PayerAvecPaypal(string invoice, string nomCompletMentore)
        {
            //le invoice doit être le id de l'inscription
            if (invoice != "")
            {
                var db = new ApplicationDbContext();
                //
                // SB... var urlRetour = HttpContext.Request.GetDisplayUrl();
                var urlRetour = new Uri(HttpContext.Request.GetDisplayUrl());  //var urlRetour = new Uri(Request.Url, Url.Content("~"));

                var paypal = new Paypal();
                paypal.cmd = "_s-xclick";

                System.Diagnostics.Trace.TraceError("bd name : " + db.Database.GetDbConnection().Database);

                if (db.Database.GetDbConnection().Database.ToString() != "BdMentorat")
                {
                    //sandbox
                    paypal.hosted_button_id = "BRY5UP2ZW8ETN";
                    paypal.actionURL = "https://www.sandbox.paypal.com/cgi-bin/webscr";
                }
                else
                {
                    paypal.hosted_button_id = "EX42EE9RSNYCE";
                    paypal.actionURL = "https://www.paypal.com/cgi-bin/webscr";
                }


                paypal.on0 = "Inscription au nom de ";
                paypal.os0 = nomCompletMentore;

                paypal.@return = urlRetour.AbsoluteUri;
                paypal.notify_url = urlRetour.AbsoluteUri + "Assignation/RetourPaiementPaypal/";
                paypal.invoice = invoice;

                return RedirectToAction("Paypal", paypal);
            }
            else
            {
                return null;
            }
        }

        [Authorize]
        public ActionResult RetourPaypalPhyto()
        {
            string user = _userManager.GetUserId(User); //string user = HttpContext.User.Identity.GetUserName();
            return RedirectToAction("Utilisateur", "Account", new { utilisateur = user, section = "phyto" });
        }

        [Authorize]
        public ActionResult Paypal(Paypal paypal)
        {          
            return View(paypal);
        }


        public async Task<EmptyResult> RetourPaiementPaypal(PayPalCheckoutInfo payPalCheckoutInfo)
      //  public EmptyResult RetourPaiementPaypal()
        {
            System.Diagnostics.Trace.TraceError("dans retourpaiementpaypal");

            var model = new PayPalListenerModel();
            model._PayPalCheckoutInfo = payPalCheckoutInfo;
            byte[] parameters;  // byte[] parameters = Request.BinaryRead(Request.ContentLength);
            using (var ms = new MemoryStream(2048))  
            {
                await Request.Body.CopyToAsync(ms);
                parameters = ms.ToArray();
            }


            System.Diagnostics.Trace.TraceError("Le tableau : "+ parameters.ToString());

            if (parameters != null)
            {
                model.GetStatus(parameters);
            }

            return new EmptyResult();


            //try
            //{
            //    var formVals = new Dictionary<string, string>();
            //    formVals.Add("cmd", "_notify-validate");

            //    string response = getpay
            //}catch(Exception ex)
            //{

            //}

            //if (payment_status != null && invoice != null)
            //{
            //    if(payment_status == "Completed" || payment_status=="Processed")
            //    {
            //        db = new ApplicationDbContext();
            //        MentoratInscription inscription = db.MentoratInscription.FirstOrDefault(f => f.Id == invoice);

            //        if(inscription!= null)
            //        {
            //            inscription.APaye = true;
            //            db.Entry(inscription).State = EntityState.Modified;
            //            db.SaveChanges();
            //        }

            //    }
            //}



        }

        [HttpGet]
        
        [AuthorizeCustom.Authorize.CustomAuthorizePageModifierPlanActionsAttribute]
        public ActionResult PlanAction(string noMentore, int noAnnee)
        {
            string mentore = "";
            string mentor = "";
            using (var db = new ApplicationDbContext())
            {
                MentoratInscription inscription = db.MentoratInscription.FirstOrDefault(f => f.Mentore.No_Mentore == noMentore && f.Annee == noAnnee);

                if(inscription!= null)
                {
                    mentore = inscription.Mentore.NomComplet_Mentore;
                    mentor = inscription.Mentor.NomCompletMentor;
                }

            }

            if(mentore !="" && mentor !="")
            {
                ViewBag.noMentorePlanAction = noMentore;
                ViewBag.noAnneePlanAction = noAnnee;
                ViewBag.NomMentorePlanAction = mentore;
                ViewBag.NomMentorPlanAction = mentor;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Accueil");
            }
           
        }

        
        [AuthorizeCustom.Authorize.CustomAuthorizePageModifierPlanActionsAttribute]
        public ActionResult PlanAction_Read([DataSourceRequest]DataSourceRequest request,string noMentore,int noAnnee)
        {
            //var lstActions = new List<ViewModels.PlanActionViewModel>();
            // lstActions.Add(new ViewModels.PlanActionViewModel() { Objectifs = "Plus <b>d'assurance</b> dans mes recommandations", Indicateurs = "Faire une première recommandation seul assuré à 8/10" });
            // lstActions.Add(new ViewModels.PlanActionViewModel() { Objectifs = "Être à l'aise à répondre aux questions des producteurs sur le désherbage des semi-directs", Indicateurs = "Je réponds au producteur seul. Je présente un premier diagnostic" ,Actions="Pratiquer les 4 grandes étapes de Luc intégré" });
            // lstActions.Add(new ViewModels.PlanActionViewModel() { Objectifs = "Introduire la phyto chez un client", Indicateurs = "Nombre de clients où j'ai présenté le service. Nombre de clients où j'ai introduit le service",Actions="Faire une cartographie. Faire un portrait de la clientèle." });

            //DataSourceResult maRequest = lstActions.ToDataSourceResult(request, m => new ViewModels.PlanActionViewModel() {Id=m.Id,Objectifs=m.Objectifs,Indicateurs=m.Indicateurs, Actions=m.Actions, Annee = m.Annee, Mentore=m.Mentore, Echeancier = m.Echeancier, Evaluation = m.Evaluation});
           // System.Diagnostics.Trace.TraceError("NoMentore: " + noMentore);

          //  System.Diagnostics.Trace.TraceError("noAnnee: " + noAnnee);
            var db = new ApplicationDbContext();
            List<MentoratInscription> lstInscriptions = db.MentoratInscription.ToList();
           // System.Diagnostics.Trace.TraceError("après to list");


            IQueryable<PlanAction> lstPlanActions = db.PlanAction.Where(w => w.Inscription.Mentore.No_Mentore == noMentore && w.Inscription.Annee == noAnnee).OrderBy(o=> o.Ordre);

           // System.Diagnostics.Trace.TraceError("après where");
            DataSourceResult donnes=  lstPlanActions.ToList().ToDataSourceResult(request, m => new ViewModels.PlanActionViewModel()
              {
                  Id= m.Id,
                  Actions=m.Actions,                 
                  Echeancier= m.Echeancier,
                  Evaluation = m.Evaluation,
                  Indicateurs = m.Indicateurs,
                  Inscription = lstInscriptions.Where(w=> w.Id == m.Inscription.Id).Select(s=> new MentoratNetCore.ModelsViews.MentoratInscriptionViewModel() { Id = s.Id, Annee = s.Annee, MentoratCategorie = new MentoratNetCore.ModelsViews.MentoratCategorieViewModel() { Id = s.MentoratCategorie.Id, Description = s.MentoratCategorie.Description,Nom = s.MentoratCategorie.Nom }, Mentore = new MentoratNetCore.ModelsViews.MentoreViewModel() { NoMentore = s.Mentore.No_Mentore, NomMentore = s.Mentore.NomComplet_Mentore, PrenomMentore = s.Mentore.Prenom_Mentore} }).FirstOrDefault(),
                  Objectifs = m.Objectifs,
                  Ordre = m.Ordre
              });

            //System.Diagnostics.Trace.TraceError("après sourceresult");

           // System.Diagnostics.Trace.TraceError("nb : " + donnes.Total);
            return Json(donnes);
        }

        [HttpPost]
        [AuthorizeCustom.Authorize.CustomAuthorizePageModifierPlanActions]
        public ActionResult PlanAction_Create([DataSourceRequest]DataSourceRequest request, ViewModels.PlanActionViewModel model, string noMentore, int noAnnee)
        {

            if(model!=null && ModelState.IsValid && noMentore !=null && noMentore !="" && noAnnee >2016)
            {
                var db = new ApplicationDbContext();

                Mentore mentore = db.Mentores.FirstOrDefault(f => f.No_Mentore == noMentore);
                int nb = db.PlanAction.Count(c => c.Inscription.Mentore.No_Mentore == noMentore && c.Inscription.Annee == noAnnee);

                if (mentore !=null)
                {
                    MentoratInscription inscription = db.MentoratInscription.Where(w => w.Mentore.No_Mentore == mentore.No_Mentore && w.Annee == noAnnee ).FirstOrDefault();

                    if(inscription !=null)
                    {
                        var ligne = new PlanAction()
                        {
                            Actions = model.Actions,
                            Echeancier = model.Echeancier,
                            Evaluation = model.Evaluation,
                            Indicateurs = model.Indicateurs,
                            // Mentore = model.Mentore,
                            Inscription = inscription,
                            Objectifs = model.Objectifs,
                            Ordre = nb + 1

                        };

                        db.PlanAction.Add(ligne);
                        db.SaveChanges();

                        model.Id = ligne.Id;
                    }                   
                }
            }            


            return Json(model);
        }

        [HttpPost]
        [AuthorizeCustom.Authorize.CustomAuthorizePageModifierPlanActions]
        public ActionResult PlanAction_Update([DataSourceRequest]DataSourceRequest request, ViewModels.PlanActionViewModel model, string noMentore, int noAnnee)
        {

            if (model != null && ModelState.IsValid)
            {
                var db = new ApplicationDbContext();

                PlanAction ligne = db.PlanAction.FirstOrDefault(f => f.Id == model.Id);

                if (ligne != null)
                {
                    ligne.Actions = model.Actions;
                    ligne.Echeancier = model.Echeancier;
                    ligne.Evaluation = model.Evaluation;
                    ligne.Indicateurs = model.Indicateurs;
                    ligne.Objectifs = model.Objectifs;
                    ligne.Ordre = model.Ordre;
                    db.PlanAction.Attach(ligne);
                    db.Entry(ligne).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(model);
        }

        [AuthorizeCustom.Authorize.CustomAuthorizePageModifierPlanActions]
        public ActionResult PlanAction_ModifierOrdre(ViewModels.PlanActionViewModel model, string noMentore, int noAnnee)
        {
            if(model.OrdreTmp > 0 && model.OrdreTmp != model.Ordre)
            {
                var db = new ApplicationDbContext();
                int min;
                int max;
                int iModification = 0;

                if (model.Ordre > model.OrdreTmp)
                {
                    min = model.OrdreTmp;
                    max = model.Ordre;
                    iModification = 1;
                }
                else
                {
                    min = model.Ordre;
                    max = model.OrdreTmp;
                    iModification = -1;
                }

                IOrderedQueryable<PlanAction> lstPlans = db.PlanAction.Where(w => w.Inscription.Mentore.No_Mentore == model.Inscription.Mentore.NoMentore && w.Inscription.Annee == model.Inscription.Annee && w.Ordre >= min && w.Ordre <= max).OrderBy(o=> o.Ordre);

                if (lstPlans !=null)
                {
                    foreach (var maLigne in lstPlans)
                    {
                        if(maLigne.Ordre != model.Ordre)
                        {
                            maLigne.Ordre += iModification;
                        }
                        else
                        {
                            maLigne.Ordre = model.OrdreTmp;
                        }
                    db.PlanAction.Attach(maLigne);                     
                      
                        db.Entry(maLigne).State = EntityState.Modified;
                    }                   

                    db.SaveChanges();

                    db.Dispose();
                    

                    return Json(new { success = true});

                }

            }

            return Json(new { success = false});
        }

        [HttpPost]
        [AuthorizeCustom.Authorize.CustomAuthorizePageModifierPlanActions]
        public ActionResult PlanAction_Destroy([DataSourceRequest]DataSourceRequest request, ViewModels.PlanActionViewModel model, string noMentore, int noAnnee)
        {

            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);

            if (model != null && ModelState.IsValid)
            {
                var db = new ApplicationDbContext();

                PlanAction ligne = db.PlanAction.FirstOrDefault(f => f.Id == model.Id);

                if (ligne != null)
                {
                    db.PlanAction.Remove(ligne);
                    db.Entry(ligne).State = EntityState.Deleted;
                    db.SaveChanges();

                    //reorganiser toutes les lignes.
                    IOrderedQueryable<PlanAction> lstPlans = db.PlanAction.Where(w => w.Inscription.Mentore.No_Mentore == model.Inscription.Mentore.NoMentore && w.Inscription.Annee == model.Inscription.Annee).OrderBy(o => o.Ordre);

                    if(lstPlans != null)
                    {
                        int iOrdre = 1;

                        foreach (var maLigne in lstPlans)
                        {
                            maLigne.Ordre = iOrdre;
                            db.PlanAction.Attach(maLigne);

                            db.Entry(maLigne).State = EntityState.Modified;
                            iOrdre++;
                        }

                        db.SaveChanges();

                        db.Dispose();
                    }


                   
                }
            }

                return Json(model);
        }

        [HttpGet]
        [AuthorizeCustom.Authorize.CustomAuthorizePageAfficherPlanActions]
        public ActionResult PlanActionAfficher(string noMentore, int noAnnee)
        {

            string mentore = "";
            string mentor = "";
            using (var db = new ApplicationDbContext())
            {
                MentoratInscription inscription = db.MentoratInscription.FirstOrDefault(f => f.Mentore.No_Mentore == noMentore && f.Annee == noAnnee);

                if (inscription != null)
                {
                    mentore = inscription.Mentore.NomComplet_Mentore;
                    mentor = inscription.Mentor.NomCompletMentor;
                }

            }

            if (mentore != "" && mentor != "")
            {
                ViewBag.noMentorePlanAction = noMentore;
                ViewBag.noAnneePlanAction = noAnnee;
                ViewBag.NomMentorePlanAction = mentore;
                ViewBag.NomMentorPlanAction = mentor;

                List<Mentore> lstMentores = db.Mentores.ToList();

                List<ViewModels.PlanActionViewModel> lstPlanActions = db.PlanAction.Where(w => w.Inscription.Mentore.No_Mentore == noMentore && w.Inscription.Annee == noAnnee).OrderBy(o => o.Ordre)
                                   .Select(s => new ViewModels.PlanActionViewModel()
                                   {
                                       Id = s.Id,
                                       Actions = s.Actions,
                                       Inscription = new MentoratNetCore.ModelsViews.MentoratInscriptionViewModel() { Id = s.Inscription.Id, Annee = s.Inscription.Annee, Mentore = new ModelsViews.MentoreViewModel() { NoMentore = s.Inscription.Mentore.No_Mentore, NomMentore = s.Inscription.Mentore.Nom_Mentore, PrenomMentore = s.Inscription.Mentore.Prenom_Mentore } },
                                       Echeancier = s.Echeancier,
                                       Evaluation = s.Evaluation,
                                       Indicateurs = s.Indicateurs,
                                       Objectifs = s.Objectifs,
                                       Ordre = s.Ordre
                                   }).ToList();



                ViewData["Actions"] = lstPlanActions;
                return View();
                               
            }
            else
            {
                return RedirectToAction("Index", "Accueil");
            }
               

         

        }

        [HttpPost]
        [Authorize]
        public ActionResult Pdf_Export_Enregistrer(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        [AuthorizeCustom.Authorize.CustomAuthorizePageModifierPlanActionsAttribute]
        public JsonResult PlanAction_Dupliquer(string noMentore, int noAnnee)
        {
            string msg = "Il n'y a pas d'action à dupliquer.";

            using (var db = new ApplicationDbContext())
            {
                MentoratInscription inscription = db.MentoratInscription.FirstOrDefault(f => f.Mentore.No_Mentore == noMentore && f.Annee == noAnnee);

                if (inscription !=null)
                {
                    PlanAction plan = db.PlanAction.Where(w => w.Inscription.Mentore.No_Mentore == noMentore && w.Inscription.Annee < noAnnee).OrderByDescending(o => o.Inscription.Annee).FirstOrDefault();
                    int nb = db.PlanAction.Count(c => c.InscriptionId == inscription.Id);

                    if (plan != null)
                    {
                        List<PlanAction> lstPlan = db.PlanAction.Where(w => w.Inscription.Mentore.No_Mentore == noMentore && w.Inscription.Annee == plan.Inscription.Annee).ToList().Select(s => new PlanAction() { Actions = s.Actions, Echeancier = s.Echeancier, Evaluation = s.Evaluation, Indicateurs = s.Indicateurs, InscriptionId = inscription.Id, Objectifs = s.Objectifs, Ordre = s.Ordre + nb}).ToList();

                        if(lstPlan.Count > 0)
                        {
                            db.PlanAction.AddRange(lstPlan);
                            db.SaveChanges();
                            msg = "";                                               
                        }
                       
                    }
                }
               
            }
                

            return Json(new { success = msg=="", message=msg });
        }

    }
}

//<form action = "https://www.sandbox.paypal.com/cgi-bin/webscr" method="post" target="_top">
//<input type = "hidden" name="cmd" value="_s-xclick">
//<input type = "hidden" name="hosted_button_id" value="BRY5UP2ZW8ETN">
//<input type = "image" src="https://www.sandbox.paypal.com/fr_CA/i/btn/btn_buynowCC_LG.gif" border="0" name="submit" alt="PayPal - la solution de paiement en ligne la plus simple et la plus sécurisée !">
//<img alt = "" border="0" src="https://www.sandbox.paypal.com/fr_CA/i/scr/pixel.gif" width="1" height="1">
//</form>
