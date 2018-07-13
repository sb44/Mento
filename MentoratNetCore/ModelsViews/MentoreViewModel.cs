using MentoratNetCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MentoratNetCore.ModelsViews
{
    public class MentoreViewModel
    {
        public MentoreViewModel() { }

        public int[] MentoresExpertises { get; set; }


        
        [StringLength(128)]       
        public string NoMentore { get; set; }       
       
        [StringLength(30)]
        public string PrenomMentore { get; set; }

       
        
        [StringLength(30)]
        public string NomMentore { get; set; }

        public string NomComplet_Mentore
        {
            get { return PrenomMentore + " " + NomMentore; }
        }

       
        [StringLength(100)]
        public string Organisme_Mentore { get; set; }

        public virtual ICollection<Expertise> Expertises { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "courriel")]
        [StringLength(50)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Adresse courriel invalide.")]
        public virtual string CourrielMentore { get; set; }

        [StringLength(15)]        
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Le  numéro de téléphone n'est pas valide.")]
        public string TelephoneMentore { get; set; }

        [StringLength(15)]      
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Le  numéro de cellulaire n'est pas valide.")]
        public string CellulaireMentore { get; set; }
                   

   
        public virtual ModelsViews.MentorViewModel Mentor { get; set; }

        [StringLength(1000)]      
        
        public string Objectifs_Mentore { get; set; }

        public bool PayeMentore { get; set; }

        public DateTime? DateInscriptionMentore { get; set; }

        [StringLength(255)]
        public string MotPasseMentore { get; set; }


        //public virtual ICollection<Intervention> Interventions { get; set; }


        //public virtual ICollection<Expertise> Expertises { get; set; }

        [System.ComponentModel.ReadOnly(true)]
        public string ListerExpertises
        {
            get { return Expertises!=null? string.Join(",", Expertises):""; }
            // get { return "Martin";}           
        }

        //public virtual ICollection<MentoratInscription> Inscriptions { get; set; }
    }
}