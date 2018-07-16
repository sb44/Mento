namespace MentoratNetCore.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Models;


    public class ObjBaseVM
    {        
        public string Id { get; set; }
        public string Nom { get; set; }
    }
   
    public class _RechercherUtilisateursViewModels
    {
        public string TitreFenetre { get; set; }
        public string TexteBoutonEnregistrer { get; set; }
       public string RoleExclus { get; set;}
    }

    public class RechercherUtilisateursViewModels
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

        public string CategorieUser { get; set; }
    }


}