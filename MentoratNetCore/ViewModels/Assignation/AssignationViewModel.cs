using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MentoratNetCore.Models;
using MentoratNetCore.ViewModels;
using MentoratNetCore.ModelsViews;

namespace MentoratNetCore.ViewModels
{
    public class AssignationViewModel : IValidatableObject
    {
        public string NoInscription { get; set; }

        public string NomUtilisateur { get; set; }

        public int Annee { get; set; }

        //[Required(ErrorMessage = "Le prénom est requis.")]
        //[Display(Name = "prénom")]
        //[StringLength(30)]
        //public string PrenomMentore { get; set; }

        //[Required(ErrorMessage = "Le nom est requis.")]
        //[Display(Name = "nom")]
        //[StringLength(30)]
        //public string NomMentore { get; set; }

        //public string NomComplet_Mentore
        //{
        //    get { return PrenomMentore + " " + NomMentore; }
        //}

        //[Required(ErrorMessage = "L'organisme est requis.")]
        //[Display(Name = "organisme")]
        //[StringLength(100)]
        //public string Organisme_Mentore { get; set; }

        public bool APaye { get; set; }

        public DateTime DateInscription { get; set; }

        [Required(ErrorMessage="La date est requise.")]
        [Display(Name = "Début")]
        public DateTime DateDebut { get; set; }
        [Required(ErrorMessage = "La date est requise.")]
        [Display(Name ="Fin")]
        public DateTime DateFin { get; set; }

        [Required(ErrorMessage ="Vous devez sélectionner un mentor.")]
        public Assignation.AssignationMentorViewModel Mentor { get; set; }

        [Required]
        public MentoratCategorieViewModel MentoratCategorie {get;set;}

        [Required]
        public MentoratNetCore.ModelsViews.MentoreViewModel Mentore { get; set; }

        public bool PlanActionEdit { get; set; }

        public bool AfficherBoutonPlan { get; set; } 

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            var res = new List<ValidationResult>();
            if (DateFin < DateDebut)
            {
                var mss = new ValidationResult("La date de fin doit être après la date de début.",new[] { "DateFin"});
                res.Add(mss);
            }

            //Vérifier si le Mentor existe
            var db = new ApplicationDbContext();

            if(!(Mentor!=null && db.Mentors.Any(a => a.NoMentor == Mentor.NoMentor)))
            {
                var mss = new ValidationResult("Vous devez sélectionner un mentor.", new[] { "Mentor.NoMentor" });
                res.Add(mss);
            }

            //vérifier si l'année n'existe pas déjà

            bool boolAnnee =  db.MentoratInscription.Any(a => a.Mentore.No_Mentore == Mentore.NoMentore && a.MentoratCategorie.Id == MentoratCategorie.Id && (a.Annee == Annee && a.Id != this.NoInscription));
           
            if(boolAnnee)
            {
                var mss = new ValidationResult("Il y a déjà un enregistrement pour l'année " + Annee + ".", new[] { "Annee" });
                res.Add(mss);
            }

            return res;
        }

    }
}