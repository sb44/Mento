using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MentoratNetCore.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public class MentoratInscription
    {
        public MentoratInscription()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        //[Key, Column("No_Mentore",Order =0)]
        //[ForeignKey("Mentore")]
        //[StringLength(128)]        
        //public string NoMentore { get; set; }

        [StringLength(128)]
        public string Id { get; set; }

        public virtual Mentore Mentore { get; set; }

        //   [Key, Column(Order = 1)]
        public int Annee { get; set; }

        //[Column("No_Mentor")]
        //[ForeignKey("Mentor")]
        //[StringLength(128)]
        //public string NoMentor { get; set; }

        public virtual Mentor Mentor { get; set; }


        //[ForeignKey("IdMentoratCategorie")]
        public virtual MentoratCategorie MentoratCategorie { get; set; }
        //[Column("Id_MentoratCategorie")]
        //public int IdMentoratCategorie { get; set; }


        public bool APaye { get; set; }

        public DateTime DateInscription { get; set; }
        public DateTime DateDebut { get; set; }

        public DateTime DateFin { get; set; }
    }
}
