using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MentoratNetCore.Models;
using MentoratNetCore.ViewModels;
using MentoratNetCore.ModelsViews;
using System.Web.Mvc;

namespace MentoratNetCore.ViewModels
{
    public class PlanActionViewModel 
    {
        public PlanActionViewModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        
        public string Id { get; set; }

        public MentoratNetCore.ModelsViews.MentoratInscriptionViewModel Inscription { get; set; }

        
        //public MentoratNetCore.ModelsViews.MentoreViewModel Mentore { get; set; }

       
        //public int Annee { get; set; }

        [AllowHtml]
        public string Objectifs { get; set; }

        [AllowHtml]
        public string Indicateurs { get; set; }

        [AllowHtml]
        public string Actions { get; set; }

        [AllowHtml]
        public string Echeancier { get; set; }

        [AllowHtml]
        public string Evaluation { get; set; }

        public int Ordre { get; set; }

        public int OrdreTmp { get; set; }
    }

}