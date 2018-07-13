using System.ComponentModel.DataAnnotations;

namespace MentoratNetCore.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Code d'utilisateur")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Ancien mot de passe")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Le {0} doit être d'au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le nouveau mot de passe")]
        [Compare("NewPassword", ErrorMessage = "Le mot de passe et la confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "code d'utilisateur")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "mot de passe")]
        public string Password { get; set; }

        [Display(Name = "Garder ma session active?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Code d'utilisateur :")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Prénom :")]
        public string Prenom { get; set; }

        [Required]
        [Display(Name = "Nom :")]
        public string Nom { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Adresse courriel invalide.")]
        [Display(Name = "Veuillez inscrire votre adresse courriel :")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Le {0} doit être d'au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe :")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe :")]
        [Compare("Password", ErrorMessage = "Le mot de passe et la confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Copier les droits de :")]
        public string CopierDroitsDe { get; set; }

        [Required]
        [Display(Name = "Catégorie de l'utilisateur :")]
        public int idCategorieUtilisateur { get; set; }


    }

    public class ReinitialiserMotDePasseViewModel
    {      
        public string UserId { get; set; } 

        [Required(ErrorMessage ="Veuillez inscrire l'adresse courriel inscrite à votre dossier.")]        
        [Display(Name = "Adresse inscrite à votre dossier :")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Adresse courriel invalide.")]
        public string Email { get; set; }



        [Display(Name = "Code d'utilisateur :")]
        public string Utilisateur { get; set; }

        public string Code { get; set; }     

        public string Message { get; set; }

        [DataType(DataType.Password)]
        [RegularExpression("^((?=.*\\d)(?=.*[a-z]).{6,20})$", ErrorMessage = "Le mot de passe doit contenir de 6 à 20 caractères incluant un minimum de 1 chiffre.")]
        [Display(Name = "Nouveau mot de passe :")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le nouveau mot de passe :")]
        [System.ComponentModel.DataAnnotations.CompareAttribute("Password", ErrorMessage = "Le mot de passe et la confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }


    }

    public class UtilisateurViewModel
    {
        [Required]
        public string userName { get; set; }
        //public string Id { get; set; }

        public string section { get; set; }

        public string Id { get; set; }

        public string Nom { get; set; }
    }

    public class UtilisateurInfoViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Code d'utilisateur :")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Prénom :")]
        public string Prenom { get; set; }

        [Required]
        [Display(Name = "Nom :")]
        public string Nom { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Adresse courriel invalide.")]
        [Display(Name = "Courriel :")]
        public string Email { get; set; }


        [StringLength(100, ErrorMessage = "Le {0} doit être d'au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe :")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe :")]
        [System.ComponentModel.DataAnnotations.CompareAttribute("Password", ErrorMessage = "Le mot de passe et la confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        public string NomComplet
        {
            get { return Prenom + " " + Nom; }
        }
    }


    public class UtilisateurDroitsViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Prénom :")]
        public string Prenom { get; set; }

        [Required]
        [Display(Name = "Nom :")]
        public string Nom { get; set; }

        public string NomComplet
        {
            get { return Prenom + " " + Nom; }
        }

        public string CopierDroitsDe { get; set; }

        public string LesDroits { get; set; }
    }


    public class UtilisateursViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }

        [Required]
        [Display(Name = "Nom")]
        public string Nom { get; set; }

        public string NomComplet
        {
            get { return Prenom + " " + Nom; }
        }

        [Display(Name = "Courriel")]
        public string Email { get; set; }

        public bool ActifUser { get; set; }

        public string StatutUser
        {
            get
            {
                if (ActifUser) return "Actif";
                return "Inactif";
            }
        }

    }

    public class ParametresDroitsViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Le champ Nom court est requis.")]
        [StringLength(30, ErrorMessage = "Le champ doit contenir un maxime de 100 caractères.")]
        [Display(Name = "Nom court")]
        public string Nom { get; set; } 
        public string NomHidden { get; set; }

        [StringLength(100, ErrorMessage = "Le champ doit contenir un maxime de 100 caractères.")]
        [Required(ErrorMessage = "Le champ Nom est requis.")]
        [Display(Name = "Nom")]
        public string NomLong { get; set; }
        [Display(Name = "Droit parent")]
        public string IdParent { get; set; }
        [Required(ErrorMessage = "Le champ Catégorie est requis.")]
        [Display(Name = "Catégorie")]
        public int? IdCategorie { get; set; }

        

    }

    public class UtilisateursParametresDrtoisViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }

        [Required]
        [Display(Name = "Nom")]
        public string Nom { get; set; }

        public string NomComplet
        {
            get { return Prenom + " " + Nom; }
        }

        [Display(Name = "Courriel")]
        public string Email { get; set; }

        public bool ActifUser { get; set; }

        public bool IsChk { get; set; }

        public string StatutUser
        {
            get
            {
                if (ActifUser) return "Actif";
                return "Inactif";
            }
        }

    }
}
