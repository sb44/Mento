using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MentoratNetCore.ViewModels.Assignation
{
    public class AssignationMentoreViewModel : ModelsViews.MentoreViewModel
    {
        public AssignationMentoreViewModel()
        {
            No_Mentore = Guid.NewGuid().ToString();
        }
        public int[] MentoresExpertises { get; set; }


        [Key]
        [StringLength(128)]
        public string No_Mentore { get; set; }

        [Required(ErrorMessage = "Le prénom est requis.")]
        [Display(Name = "prénom")]
        [StringLength(30)]
        public string Prenom_Mentore { get; set; }

        [Required(ErrorMessage = "Le nom est requis.")]
        [Display(Name = "nom")]
        [StringLength(30)]
        public string Nom_Mentore { get; set; }

        public string NomComplet_Mentore
        {
            get { return Prenom_Mentore + " " + Nom_Mentore; }
        }

        [Required(ErrorMessage = "L'organisme est requis.")]
        [Display(Name = "organisme")]
        [StringLength(100)]
        public string Organisme_Mentore { get; set; }

        [Required(ErrorMessage = "Le courriel est requis.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "courriel")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Le format de l'adresse courriel n'est pas valide.")]
        [StringLength(50)]
        public string Courriel_Mentore { get; set; }

        [StringLength(15)]
        [Display(Name = "numéro de téléphone")]
        [Required(ErrorMessage = "Le numéro de téléphone est requis.")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Le  numéro de téléphone n'est pas valide.")]
        public string Telephone_Mentore { get; set; }

        [StringLength(15)]
        [Display(Name = "numéro de cellulaire")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Le  numéro de cellulaire n'est pas valide.")]
        public string Cellulaire_Mentore { get; set; }
    }
       
}