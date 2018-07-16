namespace MentoratNetCore.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Models;


    public partial class InterventionsViewModel
    {
        [Key]
        [StringLength(128)]
        public string No_Intervention { get; set; }

        //[Required(ErrorMessage = "La date est requise.")]
        [Display(Name = "date")]
        public DateTime? Date_Intervention { get; set; }

        [StringLength(128)]
        public string No_Mentor_Intervention { get; set; }

        [StringLength(128)]
        public string No_Mentore_Intervention { get; set; }

        [Column("Mentore.NomComplet_Mentor")]
        public string NomComplet_Mentore_Intervention { get; set; }

       
        //[Required(ErrorMessage = "La durée est requise.")]
        public int? Duree_Intervention { get; set; }

        [StringLength(500, ErrorMessage = "La description ne doit pas dépasser 500 caractères.")]
        public string Description_Intervention { get; set; }

        //   [Column("No_Mentore_Intervention")]
        public virtual Mentore Mentore { get; set; }
     //   [Column("No_Mentor_Intervention")]
        public virtual Mentor Mentor { get; set; }

       
    }
}
