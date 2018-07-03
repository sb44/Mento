using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MentoratNetCore.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public class Mentor
    {
        public Mentor()
        {
            //Interventions = new HashSet<Intervention>();
            NoMentor = Guid.NewGuid().ToString();
            this.MentoratCategorieMentors = new HashSet<MentoratCategorieMentors>();
            this.MentoratInscriptions = new HashSet<MentoratInscription>();
        }

        [Key]
        //[Column( "No_Mentor")]
        [Column("No_Mentor")]
        [StringLength(128)]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public string NoMentor { get; set; }

        [StringLength(255)]
        [Column("Prenom_Mentor")]
        public string PrenomMentor { get; set; }

        [StringLength(255)]
        [Column("Nom_Mentor")]
        public string NomMentor { get; set; }

        [Column("NomComplet_Mentor")]
        public string NomCompletMentor {
            get { return PrenomMentor + " " + NomMentor; }
        }
        [Column("Taxe_Mentor")]
        public bool TaxeMentor { get; set; }

        [StringLength(255)]
        [Column("NoTPS_Mentor")]
        public string NoTpsMentor { get; set; }

        [StringLength(255)]
        [Column("NoTVQ_Mentor")]
        public string NoTvqMentor { get; set; }

        [Required(ErrorMessage = "Le courriel est requis.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "courriel")]
        [StringLength(50)]
        [Column("Courriel_Mentor")]
        public string CourrielMentor { get; set; }

        public virtual ICollection<MentoratCategorieMentors> MentoratCategorieMentors { get; set; }


        [Column("DateConnexion_Mentor")]
        public DateTime? DateConnexionMentor { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Intervention> Interventions { get; set; }

        public virtual ICollection<MentoratInscription> MentoratInscriptions { get; set; }

    }
}
