using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MentoratNetCore.ModelsViews
{

    public class MentorViewModel
    {
        public MentorViewModel()
        {
        }
        
        public string NoMentor { get; set; }

        [StringLength(255)]
        public string PrenomMentor { get; set; }

        [StringLength(255)]

        public string NomMentor { get; set; }

        public string NomCompletMentor
        {
            get { return PrenomMentor + " " + NomMentor; }
        }

        public bool TaxeMentor { get; set; }

        [StringLength(255)]      
        public string NoTpsMentor { get; set; }

        [StringLength(255)]      
        public string NoTvqMentor { get; set; }

        
        [DataType(DataType.EmailAddress)]
        [EmailAddress]       
        [StringLength(50)]       
        public virtual string CourrielMentor { get; set; }

        public DateTime? DateConnexionMentor { get; set; }

        //public virtual ICollection<MentoratCategorie> MentoratCategories { get; set; }
               
        //public virtual ICollection<Intervention> Interventions { get; set; }

        //public virtual ICollection<MentoratInscription> MentoratInscriptions { get; set; }

    }


}