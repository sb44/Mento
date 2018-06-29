using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MentoratNetCore.Models
{
    public class PlanAction
    {
        public PlanAction()
        {
            Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string Id { get; set; }

        //[Required]
        //[ForeignKey("Mentore")]
        //public string Mentore_No_Mentore { get; set; }


        //public virtual Mentore Mentore { get; set; }

        [Required]
        [ForeignKey("Inscription")]
        public string InscriptionId { get; set; }

        public virtual MentoratInscription Inscription { get; set; }

        //[Required]
        //public int Annee { get; set; }

        public string Objectifs { get; set; }

        public string Indicateurs { get; set; }

        public string Actions { get; set; }

        public string Echeancier { get; set; }

        public string Evaluation { get; set; }

        public int Ordre { get; set; }
    }
}
