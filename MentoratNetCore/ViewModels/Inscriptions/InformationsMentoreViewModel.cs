using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MentoratNetCore.ViewModels.Inscriptions
{
    public class InformationsMentoreViewModel : ModelsViews.MentoreViewModel
    {
        public MentoratNetCore.ModelsViews.MentoreViewModel Mentore { get; set; }


        //public InformationsMentoreViewModel()
        //{
        //    this.Mentore = new ModelsViews.MentoreViewModel();
        //}

        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        [Display(Name="Nom d'utilisateur")]
        public string UserName { get; set;}
        public string UserNameHidden { get; set; }

        [Display(Name = "Prénom")]
        [Required(ErrorMessage = "Le prénom est requis")]
        public new string PrenomMentore { get; set; }

        [Display(Name = "Nom")]
        [Required(ErrorMessage = "Le nom est requis")]
        public new string NomMentore { get; set; }

        [Display(Name = "Courriel")]
        [Required(ErrorMessage = "Le courriel est requis")]
        public new string CourrielMentore { get; set; }

        [Display(Name ="Organisme")]
        [Required(ErrorMessage = "L'organisme est requis")]
        public new string Organisme_Mentore { get; set; }

        [Display(Name = "Téléphone")]
        [Required(ErrorMessage = "Le téléphone est requis")]
        public new string TelephoneMentore { get; set; }

        [Display(Name = "Cellulaire")]
        public new string CellulaireMentore { get; set; }




        public string CourrielHidden { get; set; }
    }
}