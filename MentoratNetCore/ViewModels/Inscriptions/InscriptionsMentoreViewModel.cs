using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MentoratNetCore.ViewModels.Inscriptions
{
    public class InscriptionsMentoreViewModel : ModelsViews.MentoreViewModel
    {

        public InscriptionsMentoreViewModel()
        {
            NoMentore = Guid.NewGuid().ToString();
        }

        [Required]
        public new string NoMentore { get; set; }

        [Display(Name = "Prénom")]
        [Required(ErrorMessage = "Le prénom est requis")]
        public new string PrenomMentore { get; set; }

        [Display(Name = "Nom")]
        [Required(ErrorMessage = "Le nom est requis")]
        public new string NomMentore { get; set; }

        
        [Display(Name = "Courriel")]
          [Required(ErrorMessage = "Le courriel est requis")]
        
        
        [Extensions.Classes.RemoteCSC("VerifierCourrielExiste", "Inscriptions", HttpMethod = "POST", ErrorMessage = "Nous avons déjà un compte associé à cette adresse courriel. Veuillez vous connecter à l’espace client pour renouveler votre inscription.")]
        public new string CourrielMentore { get; set; }

        [Display(Name ="Organisme")]
        [Required(ErrorMessage = "L'organisme est requis")]
        public new string Organisme_Mentore { get; set; }

        [Display(Name = "Téléphone")]
        [Required(ErrorMessage = "Le téléphone est requis")]
        public new string TelephoneMentore { get; set; }

        [Display(Name = "Cellulaire")]
        public new string CellulaireMentore { get; set; }

        public int No_Expert_Mentore { get; set; }

        
       
        [DataType(DataType.Password)]      
        [RegularExpression("^((?=.*\\d)(?=.*[a-z]).{6,20})$", ErrorMessage = "Le mot de passe doit contenir de 6 à 20 caractères incluant un minimum de 1 chiffre.")]
        [Display(Name = "Mot de passe :")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe :")]
        [System.ComponentModel.DataAnnotations.CompareAttribute("Password", ErrorMessage = "Le mot de passe et la confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }


        [StringLength(128)]
        public string NoMentorMentore { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Vous devez accepter le fonctionnement et le coût d'inscription de 150 $.")]
        //[BooleanRequired(ErrorMessage = "Vous devez accepter le fonctionnement et le coût d'inscription de 150 $.")]
          [Display(Name = "J'accepte le fonctionnement¹ et le coût d'inscription de 150 $ (plus taxes)")]
       // [Required(ErrorMessage ="Yoo man")]
        public bool Paye_Mentore { get; set; }

        public DateTime DateInscription_Mentor { get; set; }

       
        

    }

    public class BooleanRequiredAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            if (value is bool)
                return (bool)value;
            else
                return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(
            ModelMetadata metadata,
            ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "booleanrequired"
            };
        }
    }




}