using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MentoratNetCore.Models
{
    public class MentoratCategorieMentors
    {
        //[Column( "No_Mentor")]
        //[Column("No_Mentor")]
        //[StringLength(128)]
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("No_Mentor")]
        public string NoMentor { get; set; }

        public virtual Mentor Mentor { get; set; }

        public int MentoratCategorieId { get; set; }
        public virtual MentoratCategorie MentoratCategorie { get; set; }
    }
}
