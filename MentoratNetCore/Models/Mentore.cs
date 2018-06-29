using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MentoratNetCore.Models
{
    public class Mentore
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Mentore()
        {
            No_Mentore = Guid.NewGuid().ToString();
            Interventions = new HashSet<Intervention>();
            Expertises = new HashSet<Expertise>();
            Inscriptions = new HashSet<MentoratInscription>();
        }
        public int[] MentoresExpertises { get; set; }


        [Key]
        [StringLength(128)]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public string No_Mentore { get; set; }

        [Required(ErrorMessage = "Le prénom est requis.")]
        [Display(Name = "prénom")]
        [StringLength(30)]
        public string Prenom_Mentore { get; set; }

        [Required(ErrorMessage = "Le nom est requis.")]
        [Display(Name = "nom")]
        [StringLength(30)]
        public string Nom_Mentore { get; set; }

        public string NomComplet_Mentore {
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

        public int? No_Expert_Mentore { get; set; }

        //public Expert Expert { get; set; }

        [ForeignKey("MentorMentore")]
        [StringLength(128)]
        public string No_Mentor_Mentore { get; set; }


        public virtual Mentor MentorMentore { get; set; }

        [StringLength(1000)]
        [Column(TypeName = "ntext")]
        [Display(Name = "objectifs")]
        public string Objectifs_Mentore { get; set; }

        public bool Paye_Mentore { get; set; }

        public DateTime? DateInscription_Mentore { get; set; }

        [StringLength(255)]
        public string MotPasse_Mentore { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] upsize_ts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Intervention> Interventions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Expertise> Expertises { get; set; }

        [System.ComponentModel.ReadOnly(true)]
        public string ListerExpertises {
            get { return string.Join(",", Expertises); }
            // get { return "Martin";}           
        }

        public virtual ICollection<MentoratInscription> Inscriptions { get; set; }

    }
}
