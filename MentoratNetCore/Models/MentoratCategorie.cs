using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MentoratNetCore.Models
{
    public class MentoratCategorie
    {
        public MentoratCategorie()
        {
            ///this.MentoratCategorieMentors = new HashSet<MentoratCategorieMentors>();
            this.Mentores = new HashSet<Mentore>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Id { get; set; }

        [StringLength(8)]
        [Required]
        public string Nom { get; set; }

        public string Description { get; set; }


        public virtual ICollection<Mentore> Mentores { get; set; }

        ///public virtual ICollection<MentoratCategorieMentors> MentoratCategorieMentors { get; set; }
    }
}
