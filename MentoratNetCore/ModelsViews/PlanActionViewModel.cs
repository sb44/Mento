using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MentoratNetCore.ModelsViews
{
    public class PlanActionViewModel
    {
        public PlanActionViewModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        
        public string Id { get; set; }

      
        public ModelsViews.MentoreViewModel Mentore { get; set; }

       
        public int Annee { get; set; }

        public string Objectifs { get; set; }

        public string Indicateurs { get; set; }

        public string Actions { get; set; }

        public string Echeancier { get; set; }

        public string Evaluation { get; set; }
    }
}