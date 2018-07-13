using MentoratNetCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MentoratNetCore.ViewModels
{
    public class UtilisateurMentoratViewModel:IValidatableObject
    {
       
        [StringLength(128)]
        public string IdMentore { get; set; }

      public string NomUtilisateur { get; set; }

        [Display(Name = "prénom")]
        [StringLength(30)]
        public string Prenom_Mentore { get; set; }

        [Display(Name = "nom")]
        [StringLength(30)]
        public string Nom_Mentore { get; set; }

        public string NomComplet_Mentore
        {
            get { return Prenom_Mentore + " " + Nom_Mentore; }
        }

       
        [Display(Name = "organisme")]
        [StringLength(100)]
        public string Organisme_Mentore { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "courriel")]
        [StringLength(50)]
        public string Courriel_Mentore { get; set; }

        [StringLength(15)]
        [Display(Name = "numéro de téléphone")]       
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Le  numéro de téléphone n'est pas valide.")]
        public string Telephone_Mentore { get; set; }

        [StringLength(15)]
        [Display(Name = "numéro de cellulaire")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Le  numéro de cellulaire n'est pas valide.")]
        public string Cellulaire_Mentore { get; set; }

        [StringLength(1000)]
        [Column(TypeName = "ntext")]
        [Display(Name = "objectifs")]
        [Required(ErrorMessage ="Veuillez indiquer les objectifs.")]
        public string Objectifs_Mentore { get; set; }
      
        public virtual ICollection<MentoratNetCore.ModelsViews.ExpertiseViewModel> Expertises { get; set; }
        [Required(ErrorMessage = "Vous devez sélectionner des champs d'expertises.")]
        public int[] LesIdExpertises { get; set; }

        public string TypeSectionMentorat { get; set; }

        public bool DerniereAnnePaye { get; set; }

        public int DerniereAnnee { get; set; }

        public bool AfficherRenouvellement { get; set; }

        public int NbJoursRenouvellement { get; set; }

        public string MessageRenouvellement { get; set; }

        public bool AfficherBoutonPaypal { get; set; }

        //public string ListerExpertises
        //{
        //    get {
        //        if (Expertises != null)
        //        {
        //            return string.Join(",", Expertises);
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    // get { return "Martin";}           
        //}

        public virtual ICollection<MentoratInscription> Inscriptions { get; set; }


        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            var res = new List<ValidationResult>();

          
                if(LesIdExpertises == null || LesIdExpertises.Count() == 0)
                {
                    var mss = new ValidationResult("Vous devez sélectionner des expertises.", new[] { "LesIdExpertises" });
                    res.Add(mss);
                }
           
            


            return res;
        }

    }
}