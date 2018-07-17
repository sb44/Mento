﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MentoratNetCore.Data;
using MentoratNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mentorat.Views
{
    public class MentorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "GestionMentors")]
        public ActionResult Index()
        {
            ViewBag.Message = "Liste des mentors (accessible seulement par l'administrateur).";

            return View();
        }

        [Authorize(Roles = "GestionMentors")]
        public ActionResult Mentors_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Mentor> mentors = db.Mentors;
            DataSourceResult result = mentors.ToDataSourceResult(request, mentor => new Mentor(){
                NoMentor = mentor.NoMentor,
               PrenomMentor = mentor.PrenomMentor,                
                NomMentor = mentor.NomMentor,
                TaxeMentor = mentor.TaxeMentor,
                NoTpsMentor = mentor.NoTpsMentor,
                NoTvqMentor = mentor.NoTvqMentor,
                DateConnexionMentor = mentor.DateConnexionMentor, 
                CourrielMentor = mentor.CourrielMentor              
            });

            

            return Json(result);
        }

        //[Authorize(Roles = "GestionMentors")]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Mentors_Create([DataSourceRequest]DataSourceRequest request, Mentor mentor)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new Mentor
        //        {
        //            PrenomMentor = mentor.PrenomMentor,
        //            NomMentor = mentor.NomMentor,
        //            TaxeMentor = mentor.TaxeMentor,
        //            NoTpsMentor = mentor.NoTpsMentor,
        //            NoTvqMentor = mentor.NoTvqMentor,
        //            DateConnexionMentor = mentor.DateConnexionMentor,
        //            CourrielMentor = mentor.CourrielMentor
        //        };

        //        db.Mentors.Add(entity);
        //        db.SaveChanges();
        //        mentor.NoMentor = entity.NoMentor;
        //    }

        //    return Json(new[] { mentor }.ToDataSourceResult(request, ModelState));
        //}

        [Authorize(Roles = "GestionMentors")]
        [HttpPost]
        public ActionResult Mentors_Update([DataSourceRequest]DataSourceRequest request, Mentor mentor)
        {
            if (ModelState.IsValid)
            {
                var entity = new Mentor
                {
                    NoMentor = mentor.NoMentor,
                    //PrenomMentor = mentor.PrenomMentor,
                    //NomMentor = mentor.NomMentor,
                    TaxeMentor = mentor.TaxeMentor,
                    NoTpsMentor = mentor.NoTpsMentor,
                    NoTvqMentor = mentor.NoTvqMentor,
                    DateConnexionMentor = mentor.DateConnexionMentor,
                    //CourrielMentor = mentor.CourrielMentor
                };

                db.Mentors.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
               
            }

            return Json(new[] { mentor }.ToDataSourceResult(request, ModelState));


        }

        [Authorize(Roles = "GestionMentorsSuppression")]
        [HttpPost]
        public ActionResult Mentors_Destroy([DataSourceRequest]DataSourceRequest request, Mentor mentor)
        {
            if (ModelState.IsValid)
            {
                var entity = new Mentor
                {
                    NoMentor = mentor.NoMentor,
                    PrenomMentor = mentor.PrenomMentor,
                    NomMentor = mentor.NomMentor,
                    TaxeMentor = mentor.TaxeMentor,
                    NoTpsMentor = mentor.NoTpsMentor,
                    NoTvqMentor = mentor.NoTvqMentor,
                    DateConnexionMentor = mentor.DateConnexionMentor,
                    CourrielMentor = mentor.CourrielMentor
                };

                db.Mentors.Attach(entity);
                db.Mentors.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { mentor }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
