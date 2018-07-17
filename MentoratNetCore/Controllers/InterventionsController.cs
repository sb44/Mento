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
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MentoratNetCore.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Mentorat.Views
{
    public class InterventionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static DateTime dateMin ;

        [Authorize(Roles = "Interventions")]
        public ActionResult Index()
        {
            ViewBag.Message = "Veuillez inscrire vos interventions et n'oubliez pas d'enregistrer les modifications.";

            var values = new ForeignKeyValues
            {
                Mentores = db.Mentores
            };

            //if (User.Identity.GetUserName() == "luc")
            //{
            //    Session["intNoMentor"] = 2;
            //}
            //else if (User.Identity.GetUserName() == "david")
            //{
            //    Session["intNoMentor"] = 3;
            //}
            //else if (User.Identity.GetUserName() == "isabelle")
            //{
            //    Session["intNoMentor"] = 4;
            //}
            //else if (User.Identity.GetUserName() == "veronique")
            //{
            //    Session["intNoMentor"] = 5;
            //}
            //else if (User.Identity.GetUserName() == "patrice")
            //{
            //    Session["intNoMentor"] = 6;
            //}
            //else if (User.Identity.GetUserName() == "vicky")
            //{
            //    Session["intNoMentor"] = 7;
            //}
            //else if (User.Identity.GetUserName() == "nadia")
            //{
            //    Session["intNoMentor"] = 8;
            //}
            //else if (User.Identity.GetUserName() == "dominique")
            //{
            //    Session["intNoMentor"] = 9;
            //}

            var monContext = new ApplicationDbContext();

            List<Mentor> mesMentors = monContext.Mentors.ToList();
            
            if (mesMentors.Any(a => a.NoMentor == HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier))) // if (mesMentors.Any(a=>a.NoMentor==User.Identity.GetUserId()))
            {
                HttpContext.Session.SetString("intNoMentor", HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)); //Session["intNoMentor"] = User.Identity.GetUserId();
            }
            else
            {
                HttpContext.Session.SetString("intNoMentor", ""); // Session["intNoMentor"] = "";
            }

           ViewData["AnneeEnCours"]= Environment.GetEnvironmentVariable("AnneePhytoEnCours");

            // string strSQL as string = "";

            string strNoMentor = (HttpContext.Session.GetString("intNoMentor")).ToString(); //string strNoMentor = (Session["intNoMentor"].ToString());

            ViewData["ListeMentor"] = ObtenirListeMentore(strNoMentor);

           // IQueryable<Mentor> monMentor = db.Mentors.Where(b => b.NoMentor == iNoMentor);

            Mentor monMentor = db.Mentors.FirstOrDefault(m => m.NoMentor == strNoMentor);

            if (monMentor != null && monMentor.DateConnexionMentor != null)
            {
                dateMin = (DateTime)monMentor.DateConnexionMentor;
                dateMin = dateMin.AddDays(1);
            }
            else
            {
                dateMin = DateTime.Now.Date.AddDays(-1);
            }

            ViewData["DateFermeture"] = dateMin;

            return View(values);
        }


        private List<Mentore> ObtenirListeMentore(string iNoMentor)
        {
            var lstDuMentor = new List<Mentore>();
            var lstAutreMentor = new List<Mentore>();

            foreach (Mentore monMentore in db.Mentores.ToList().OrderBy(c => c.Prenom_Mentore).ThenBy(c => c.Nom_Mentore))
            {
                MentoratInscription inscription = monMentore.Inscriptions.Where(w=> w.Mentor.NoMentor !="1" || w.Mentore.No_Mentore == "F1490F96-566E-4440-ABE1-11660546E914" || w.Mentore.No_Mentore == "FB0660F1-0EE6-4CA6-9D31-BA74B02CE204").OrderByDescending(o => o.Annee).FirstOrDefault(); //la dernière inscription
                Mentor mentorTmp ;

                if(inscription!=null)
                {                    
                    mentorTmp = inscription.Mentor;

                    if(mentorTmp!=null && mentorTmp.NoMentor=="1" &&  monMentore.No_Mentore != "F1490F96-566E-4440-ABE1-11660546E914")
                    {
                        //pour filtrer les mentorés qui n'ont pas de mentor
                        mentorTmp = null;
                    }
                }
                else
                {
                    //mentorTmp = db.Mentors.OrderBy(o => o.NoMentor).First();
                    if(monMentore.No_Mentore=="1" || monMentore.No_Mentore == "FB0660F1-0EE6-4CA6-9D31-BA74B02CE204")
                    {
                        mentorTmp = db.Mentors.OrderBy(o => o.NoMentor).First();
                    }
                    else
                    {
                        mentorTmp = null;
                    }                   
                }

                if(mentorTmp!=null)
                {
                    //on ne veut pas des mentorés pas encore assignés
                    var mentoreTmp = new Mentore
                    {
                        No_Mentore = monMentore.No_Mentore,
                        Prenom_Mentore = monMentore.Prenom_Mentore,
                        Nom_Mentore = monMentore.Nom_Mentore,
                        MentorMentore = mentorTmp
                    };
                    if (mentoreTmp.MentorMentore.NoMentor == iNoMentor || mentoreTmp.MentorMentore.NoMentor == "1") //est le mentor ou est le mentore "sélectionner un mentoré.."
                        lstDuMentor.Add(mentoreTmp);
                    else
                    {
                        mentoreTmp.Nom_Mentore += " (" + mentoreTmp.MentorMentore.NomCompletMentor + ")";
                        lstAutreMentor.Add(mentoreTmp);
                    }
                }
                
            }

           



            if (lstAutreMentor != null)
                lstDuMentor.AddRange(lstAutreMentor);

            return lstDuMentor;
        }

        [Authorize(Roles = "Interventions")]
        [HttpPost]
        public ActionResult Interventions_Read([DataSourceRequest]DataSourceRequest request,string filtreAnnee)
        {
            string intNoMentor = HttpContext.Session.GetString("intNoMentor"); //string intNoMentor = Session["intNoMentor"].ToString();

            Nullable<DateTime> dateDebut;
            Nullable<DateTime> dateFin;

           
            var strMessageDatasource = "";

            MentoratNetCore.Extensions.CscExtensionsMethodes.AttribuerDateDebutDateFinAnneeFinanciereProjet(filtreAnnee,out dateDebut, out dateFin);

            List<Intervention> interventions;

            //on filtre selon l'année financière sélectionner par le combobox...par défaut on affiche tout.
            if (dateDebut==null || dateFin ==null)
            {
                 interventions = db.Interventions.Where(b => b.No_Mentor_Intervention == intNoMentor).ToList();
                strMessageDatasource = "";
            }
            else
            {
                interventions = db.Interventions.Where(b => b.No_Mentor_Intervention == intNoMentor && b.Date_Intervention >= dateDebut && b.Date_Intervention <= dateFin).ToList();
                strMessageDatasource = "(" + DateTime.Parse(dateDebut.ToString()).ToShortDateString() + " au " + DateTime.Parse(dateFin.ToString()).ToShortDateString() + ")";
            }
            
            //interventions = interventions.OrderByDescending(c => c.Date_Intervention); Ne fonctionne pas
            GroupDescriptor monGroup = null;

            if (request.Groups.Count > 0)
            {
              monGroup = request.Groups.Where(g => g.Member == "No_Mentore_Intervention").FirstOrDefault();

                if (monGroup != null)
                {
                    monGroup.Member = "Mentore.NomComplet_Mentore";                  
                    monGroup.DisplayContent = "No_Mentore_Intervention";                    
                }               
            }

            if (request.Sorts.Count == 0) // par défaut le tri est par la date
            {
                request.Sorts.Add(new SortDescriptor("Date_Intervention", ListSortDirection.Descending));
                request.Sorts.Add(new SortDescriptor("No_Intervention", ListSortDirection.Descending));
            }
            else
            {
                SortDescriptor monsort =  request.Sorts.Where(w => w.Member == "No_Mentore_Intervention").FirstOrDefault();

                if(monsort !=null)
                {//Le tri est la le combobox alors on va trier sur le nom du Mentore
                    var sort1 = new SortDescriptor("Mentore.NomComplet_Mentore", monsort.SortDirection);                   
                    request.Sorts.Insert(request.Sorts.IndexOf(monsort), sort1);                  
                }

            }

            DataSourceResult result = interventions.ToDataSourceResult(request, i => new MentoratNetCore.ViewModels.InterventionsViewModel
            {
                No_Intervention = i.No_Intervention,
                Date_Intervention = i.Date_Intervention,
                No_Mentor_Intervention = i.No_Mentor_Intervention,
                No_Mentore_Intervention = i.No_Mentore_Intervention,
               NomComplet_Mentore_Intervention = i.Mentore.NomComplet_Mentore,
                Duree_Intervention = i.Duree_Intervention,
                Description_Intervention = i.Description_Intervention
                //Mentore = new Mentore
                //{
                //    No_Mentore = intervention.Mentore.No_Mentore,
                //    Nom_Mentore = intervention.Mentore.Nom_Mentore,
                //    Prenom_Mentore = intervention.Mentore.Prenom_Mentore

                //},                                                              
            });

            if (monGroup != null && result.Data != null)
            {
                ModifierNomColonneGroup(result.Data); //On s'assure de renommer la colonne si le group est sur une colonne de cbo.

            }

            var resultModif = new MentoratNetCore.Extensions.KendoDataSourceResult(result);

            resultModif.MessageDataSource = strMessageDatasource;

            return Json(resultModif);
                       
        }

        private static void ModifierNomColonneGroup(System.Collections.IEnumerable pItem)
        {
    
            foreach (Kendo.Mvc.Infrastructure.AggregateFunctionsGroup item in pItem)
            {
                //on s'assure d'avoir le nom de la bonne colonne.
                if (item.Member == "Mentore.NomComplet_Mentore")
                    item.Member = "NomComplet_Mentore_Intervention";
                
                if (item.Items != null && item.Items.Cast<object>().First().GetType().ToString() == "Kendo.Mvc.Infrastructure.AggregateFunctionsGroup")
                    ModifierNomColonneGroup(item.Items);

            }
            
        }

        [Authorize(Roles = "Interventions")]
        [HttpPost]
        public ActionResult Interventions_Create([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<MentoratNetCore.ViewModels.InterventionsViewModel> interventions)
        {
            var entities = new List<Intervention>();
            List<Error> currentErrors = new List<Error>();

            if (interventions != null && ModelState.IsValid)
            {
                ValiderGrid(interventions, currentErrors);
                if (currentErrors.Count > 0)
                {
                    return Json(new { Errors = currentErrors });
                }
                else
                {
                    foreach (var intervention in interventions)
                    {
                        var entity = new Intervention
                        {
                            Date_Intervention = intervention.Date_Intervention,
                            No_Mentor_Intervention = HttpContext.Session.GetString("intNoMentor").ToString(), //No_Mentor_Intervention = Session["intNoMentor"].ToString(),
                            No_Mentore_Intervention = intervention.No_Mentore_Intervention,
                            Duree_Intervention = intervention.Duree_Intervention,
                            Description_Intervention = intervention.Description_Intervention
                        };

                        db.Interventions.Add(entity);
                        entities.Add(entity);
                    }
                    db.SaveChanges();
                }
            }
            return Json(interventions);
            //return Json(interventions.ToDataSourceResult(request, ModelState));
            //return Json(entities.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Interventions")]
        [HttpPost]
        public ActionResult Interventions_Update([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<MentoratNetCore.ViewModels.InterventionsViewModel> interventions)
        {
            var entities = new List<Intervention>();
            List<Error> currentErrors = new List<Error>();

            //foreach (var key in ModelState.Keys.ToList().Where(key => ModelState.ContainsKey(key)))
            //{
            //    if(key.Split('.').Length >2)
            //    {
            //        ModelState[key].Errors.Clear(); //This is my new solution. Thanks bbak
            //    }                               
            //}

            if (interventions != null && ModelState.IsValid)
            {
                ValiderGrid(interventions, currentErrors);
                
                if(currentErrors.Count > 0)
                {
                    return Json(new { Errors = currentErrors });
                }
                else
                {
                    foreach (var intervention in interventions)
                    {
                        var entity = new Intervention
                        {
                            No_Intervention = intervention.No_Intervention,
                            Date_Intervention = intervention.Date_Intervention,
                            No_Mentor_Intervention = HttpContext.Session.GetString("intNoMentor").ToString(), //No_Mentor_Intervention =Session["intNoMentor"].ToString(),
                            No_Mentore_Intervention = intervention.No_Mentore_Intervention,
                            Duree_Intervention = intervention.Duree_Intervention,
                            Description_Intervention = intervention.Description_Intervention
                        };

                        entities.Add(entity);
                        db.Interventions.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                    }
                    db.SaveChanges();

                }
                
            }
            //Je dois retourner le même object (interventions) pour qu'il garde la colonne cachée (Nomcomplet_mentore).
            //return Json(interventions.ToDataSourceResult(request,ModelState));
            return Json(interventions);
             //return Json(entities.ToDataSourceResult(request, ModelState));
          //  return Interventions_Read(request);
            //var resultData = new[] { interventions };
            //return Json(resultData.AsQueryable().ToDataSourceResult(request, ModelState));           
        }

        [Authorize(Roles = "Interventions")]
        [HttpPost]
        public ActionResult Interventions_Destroy([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<MentoratNetCore.ViewModels.InterventionsViewModel> interventions)
        {
            var entities = new List<Intervention>();
            if (ModelState.IsValid)
            {
                    foreach (var intervention in interventions)
                    {
                        var entity = new Intervention
                        {
                            No_Intervention = intervention.No_Intervention,
                            Date_Intervention = intervention.Date_Intervention,
                            No_Mentor_Intervention = HttpContext.Session.GetString("intNoMentor").ToString(), //No_Mentor_Intervention = Session["intNoMentor"].ToString(),
                            No_Mentore_Intervention = intervention.No_Mentore_Intervention,
                            Duree_Intervention = intervention.Duree_Intervention,
                            Description_Intervention = intervention.Description_Intervention
                        };

                        entities.Add(entity);
                        db.Interventions.Attach(entity);
                        db.Interventions.Remove(entity);
                    }
                    db.SaveChanges();
                
               
            }
            
            return Json(entities.ToDataSourceResult(request, ModelState));
        }
               

        private static void ValiderGrid(IEnumerable<MentoratNetCore.ViewModels.InterventionsViewModel> interventions, List<Error> currentErrors)
        {          
            
            foreach (var intervention in interventions)
            {
                List<ErrorMessage> errorMessages = new List<ErrorMessage>();

                bool dateValide = true;
                bool MentoreValide = true;              

                if (intervention.Date_Intervention == null)
                {
                    string fieldName = "Date_Intervention";
                    errorMessages.Add(new ErrorMessage() { field = fieldName, message = "Vous devez inscrire une date." });
                    dateValide = false;
                }

                if( !(intervention.Duree_Intervention > 0))
                {
                    string fieldName = "Duree_Intervention";
                    errorMessages.Add(new ErrorMessage() { field = fieldName, message = "Vous devez inscrire une durée." });
                }

                if ((intervention.No_Mentore_Intervention == "1"))
                {
                    string fieldName = "No_Mentore_Intervention";
                    errorMessages.Add(new ErrorMessage() { field = fieldName, message = "Vous devez sélectionner un mentoré." });
                    MentoreValide = false;
                }

                if (intervention.Description_Intervention == null || !(intervention.Description_Intervention.ToString().Length > 0))
                {
                    string fieldName = "Description_Intervention";
                    errorMessages.Add(new ErrorMessage() { field = fieldName, message = "Vous devez inscrire une description." });
                }


                int result = 1;
                if(intervention.Date_Intervention != null)
                {
                    result = DateTime.Compare((DateTime) dateMin, (DateTime) intervention.Date_Intervention);
                }
                              

                if (result>0)
                {
                    string fieldName = "Date_Intervention";
                    errorMessages.Add(new ErrorMessage() { field = fieldName, message = "Vous devez saisir une date à partir du " + dateMin + "." });
                    dateValide = false;
                }

                bool valide = ValiderInscriptionMentore(intervention.Date_Intervention, intervention.No_Mentore_Intervention);
                    if (!valide)
                    {
                        string fieldName = "No_Mentore_Intervention";
                        errorMessages.Add(new ErrorMessage() { field = fieldName, message = "Le mentoré n'est pas inscrit au service de mentorat!" });
                    }                  


                if (errorMessages.Count > 0)
                {
                    currentErrors.Add(new Error() { id = intervention.No_Intervention, errors = errorMessages });
                }


                
            }            
        }

        private static bool ValiderInscriptionMentore(DateTime? date, string noMentore)
        {
            //il doit y avoir une date valide et un mentoreValide pour valider l'inscription le id vérifié est l'administration du mentorat
            using (var db = new ApplicationDbContext())
            {
                bool valide = db.MentoratInscription.Any(a => a.Mentore.No_Mentore == noMentore && a.DateDebut <= date && a.DateFin >= date && a.APaye == true);
                if (!valide)
                {
                    return false;
                }
            }
            return true;
        }
        //public JsonResult GetMentores()
        //{
        //    int intNoMentor = (int)Session["intNoMentor"];
        //    IQueryable<Mentore> mentoresCbo = db.Mentores.Where(b => b.No_Mentor_Mentore == intNoMentor).OrderBy(c => c.Nom_Mentore).ThenBy(c => c.Prenom_Mentore).Concat(db.Mentores.Where(b => b.No_Mentor_Mentore != intNoMentor).OrderBy(c => c.Nom_Mentore).ThenBy(c => c.Prenom_Mentore));

        //    return Json(mentoresCbo.Select(g => new
        //    {
        //        Nom_Mentore = g.Prenom_Mentore + " " + g.Nom_Mentore ,
        //        No_Mentore = g.No_Mentore
        //    }), JsonRequestBehavior.AllowGet);
        //}

        //public string GetMentor(int? intNoMentor)
        //{
        //    return (from m in db.Mentors
        //            where m.No_Mentor.Equals(intNoMentor)
        //            select m.Prenom_Mentor).FirstOrDefault();
        //}

        //public ActionResult RepartirContent()
        //{
        //    if (Request.HttpMethod == "POST")
        //    {
        //        string dateInterventionint = Request.Form["dateIntervention"];
        //        //int duree = 
        //        //string description = Request.Form[""];

        //        Console.WriteLine(dateInterventionint);
        //        return PartialView();
        //    }
        //    else
        //        return PartialView();
        //}

        [Authorize(Roles = "Interventions")]
        public ActionResult RepartirContent()
        {
            ViewData["DateFermeture"] = dateMin;
            return PartialView();
        }

        [Authorize(Roles = "Interventions")]
        [HttpPost]   
        [ValidateAntiForgeryToken]
        public ActionResult RepartirContent(ObjRepartir resultat)
        {

                      
           
            if (ModelState.IsValid)
            {
                var erreursMentore = new List<string>();
                foreach (string noMentore in resultat.ChoixMentores)
                {                  


                    if (!ValiderInscriptionMentore(resultat.Date,noMentore))
                    {
                        string nomMentore = "";
                        using (var db = new ApplicationDbContext())
                        {
                            Mentore mentore = db.Mentores.FirstOrDefault(f => f.No_Mentore == noMentore);

                            if (mentore != null)
                            {
                                nomMentore = mentore.NomComplet_Mentore;
                            }
                        }

                        if (nomMentore == "")
                        {
                            nomMentore = "inconnu";
                        }
                        erreursMentore.Add(nomMentore);
                    }
                }

                if(erreursMentore.Count == 0 )
                {
                    EnregistrerTempsRepartir(resultat);
                     return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, msg="Ces mentorés ne sont pas inscrits au service de mentorat : \n\n" + string.Join(", " , erreursMentore.ToArray())});
                }
               
               
            }
           



            return Json(new { success = false });
        }
               
        

        private void EnregistrerTempsRepartir(ObjRepartir resultat)
        {

           int tempsParPersonne = (int)( Math.Round((double)resultat.Duree / resultat.ChoixMentores.Length,MidpointRounding.AwayFromZero));
            //int tempsParPersonne = Convert.ToInt32((double) (resultat.Duree / resultat.ChoixMentores.Length));

             var entities = new List<Intervention>();
             if (tempsParPersonne > 0)
             {
                 foreach (var iNoMentore in resultat.ChoixMentores)
                 {
                    var entity = new Intervention
                    {
                        Date_Intervention = resultat.Date,
                        No_Mentor_Intervention = HttpContext.Session.GetString("intNoMentor").ToString(), //No_Mentor_Intervention =Session["intNoMentor"].ToString(),
                        No_Mentore_Intervention = iNoMentore,
                        Duree_Intervention = tempsParPersonne,
                        Description_Intervention = resultat.Commentaire,
                    };

                    db.Interventions.Add(entity);
                    entities.Add(entity);
            }
                    db.SaveChanges();
        }
    }

        public class ObjRepartir
        {
            [Required(ErrorMessage = "Veuillez inscrire la date de l'activité.")]
            public DateTime Date { get; set; }
            [Required(ErrorMessage = "Veuillez inscrire la durée de l'activité.")]
            public short Duree { get; set; }
            [StringLength(500, ErrorMessage = "La description ne doit pas dépasser 500 caractères.")]
            [Required(ErrorMessage ="Veuillez inscrire la description de l'activité.")]
            public string Commentaire { get; set; }

            [Required(ErrorMessage = "Veuillez sélectionner les mentorés.")]
           
            public string[] ChoixMentores { get; set; }
            public List<ObjTest> LesMentores { get; set; }
        }

        public class ObjTest
        {
            public int No_Mentore { get; set; }
            public string Nom_Mentore { get; set; }
            public string Prenom_Mentore { get; set; }
        }


        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult RepartirContent(string strResult)
        //{
        //    string dateInterventionint = Request.Form["dateIntervention"];
        //    //int duree = 
        //    //string description = Request.Form[""];

        //    Console.WriteLine(dateInterventionint);
        //    return PartialView(strResult);
        //}

        [Authorize(Roles = "Interventions")]
        public JsonResult ObtenirMentores()
        {
            string iNoMentor = HttpContext.Session.GetString("intNoMentor").ToString(); // string iNoMentor =Session["intNoMentor"].ToString();
            List<Mentore> mentores = ObtenirListeMentore(iNoMentor);
            var pourJson = new List<Mentore>();
                     

            foreach(var monMentore in mentores)
            {
                if(monMentore.No_Mentore!="1") //n'est pas le "Sélectionner un mentoré.."
                {
                    var entity = new Mentore
                    {
                        No_Mentore = monMentore.No_Mentore,
                        Nom_Mentore = monMentore.Nom_Mentore,
                        Prenom_Mentore = monMentore.Prenom_Mentore
                    };
                    pourJson.Add(entity);
                }                
            }

            return Json(pourJson);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }       

    }
}
