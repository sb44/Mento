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
using MentoratNetCore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Mentorat.Views
{
    public class InterventionsAdmController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        [Authorize(Roles ="InterventionsAdm")]
        public ActionResult Index()
        {
            ViewBag.Message = "Interventions inscrites par les mentors.";

            var values = new ForeignKeyValues
            {
                Mentores = db.Mentores,
                Mentors = db.Mentors
            };

            ViewData["AnneeEnCours"] = Environment.GetEnvironmentVariable("AnneePhytoEnCours");

            return View(values);
        }

        [Authorize(Roles = "InterventionsAdm")]
        public ActionResult Interventions_Read([DataSourceRequest]DataSourceRequest request, string filtreAnnee)
        {

            Nullable<DateTime> dateDebut;
            Nullable<DateTime> dateFin;

            MentoratNetCore.Extensions.CscExtensionsMethodes.AttribuerDateDebutDateFinAnneeFinanciereProjet(filtreAnnee, out dateDebut, out dateFin);

            List<Intervention> interventions ;

            var strMessageDatasource = "";

            //on filtre selon l'année financière sélectionner par le combobox...par défaut on affiche tout.
            if (dateDebut == null || dateFin == null)
            {
                interventions = db.Interventions.ToList();
                strMessageDatasource = "";
            }
            else
            {
                interventions = db.Interventions.Where(b=> b.Date_Intervention >= dateDebut && b.Date_Intervention <= dateFin).ToList();
                strMessageDatasource = "("+ DateTime.Parse(dateDebut.ToString()).ToShortDateString() +" au " + DateTime.Parse(dateFin.ToString()).ToShortDateString() + ")";
            }

            

            if (request.Groups.Count > 0)
            {
                GroupDescriptor monGroup = request.Groups.Where(g => g.Member == "No_Mentore_Intervention").FirstOrDefault();

                if (monGroup != null)
                {
                    monGroup.Member = "Mentore.NomComplet_Mentore";
                    // monGroup.SortDirection = System.ComponentModel.ListSortDirection.Ascending;
                    monGroup.DisplayContent = "No_Mentore_Intervention";
                }

                monGroup = request.Groups.Where(g => g.Member == "No_Mentor_Intervention").FirstOrDefault();

                if (monGroup != null)
                {
                    monGroup.Member = "Mentor.NomCompletMentor";
                    monGroup.DisplayContent = "No_Mentor_Intervention";
                    //   monGroup.SortDirection = System.ComponentModel.ListSortDirection.Ascending;
                }
            }

            if (request.Sorts.Count == 0) // par défaut le tri est par la date
            {
                request.Sorts.Add(new SortDescriptor("Date_Intervention", ListSortDirection.Descending));
            }
            else
            {
                SortDescriptor monsort = request.Sorts.Where(w => w.Member == "No_Mentore_Intervention").FirstOrDefault();

                if (monsort != null)
                {//Le tri est la le combobox alors on va trier sur le nom du Mentore
                    var sort1 = new SortDescriptor("Mentore.NomComplet_Mentore", monsort.SortDirection);
                    request.Sorts.Insert(request.Sorts.IndexOf(monsort), sort1);
                }
                monsort = request.Sorts.Where(w => w.Member == "No_Mentor_Intervention").FirstOrDefault();

                if (monsort != null)
                {//Le tri est la le combobox alors on va trier sur le nom du Mentor
                    var sort1 = new SortDescriptor("Mentor.NomCompletMentor", monsort.SortDirection);
                    request.Sorts.Insert(request.Sorts.IndexOf(monsort), sort1);
                }
            }

             

            DataSourceResult result =   interventions.ToDataSourceResult(request, intervention => new Intervention
            {
                No_Intervention = intervention.No_Intervention,
                Date_Intervention = intervention.Date_Intervention,
                No_Mentor_Intervention = intervention.No_Mentor_Intervention,
                Mentor = new Mentor
                {
                    NoMentor = intervention.Mentor.NoMentor,
                    PrenomMentor = intervention.Mentor.PrenomMentor,
                    NomMentor = intervention.Mentor.NomMentor
                },
                No_Mentore_Intervention = intervention.No_Mentore_Intervention,
                Mentore = new Mentore
                {
                    No_Mentore = intervention.Mentore.No_Mentore,
                    Nom_Mentore = intervention.Mentore.Nom_Mentore,
                    Prenom_Mentore = intervention.Mentore.Prenom_Mentore
                },
                Duree_Intervention = intervention.Duree_Intervention,
                Description_Intervention = intervention.Description_Intervention
            }) ;


            var resultModif = new MentoratNetCore.Extensions.KendoDataSourceResult(result);

            resultModif.MessageDataSource = strMessageDatasource;
            
            return Json(resultModif);
        }

        [Authorize(Roles = "InterventionsAdm")]
        public JsonResult GetMentors()
        {
            IQueryable<Mentor> mentorsCbo = db.Mentors;//.Where(b => b.No_Mentor_Mentore == intNoMentor).OrderBy(c => c.Nom_Mentore).ThenBy(c => c.Prenom_Mentore).Concat(db.Mentores.Where(b => b.No_Mentor_Mentore != intNoMentor).OrderBy(c => c.Nom_Mentore).ThenBy(c => c.Prenom_Mentore));

            return Json(mentorsCbo.Select(g => new
            {
                Nom_Mentor = g.PrenomMentor + " " + g.NomMentor,
                No_Mentor = g.NoMentor
            }));
        }

        //public string GetMentor(int? intNoMentor)
        //{
        //    return (from m in db.Mentors
        //            where m.No_Mentor.Equals(intNoMentor)
        //            select m.Prenom_Mentor).FirstOrDefault();
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
