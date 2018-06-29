using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MentoratNetCore.Models
{
    public class Expertise
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Expertise()
        {
            Mentores = new HashSet<Mentore>();
        }

        [Key]
        public int No_Expertise { get; set; }

        [StringLength(255)]
        public string Nom_Expertise { get; set; }

        public override string ToString()
        {
            return Nom_Expertise;
        }

        public int? No_Regroupement_Expertise { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mentore> Mentores { get; set; }
    }
}
