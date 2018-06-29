using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MentoratNetCore.Models
{
    public class Expert
    {
        [Key]
        public int No_Expert { get; set; }

        [StringLength(255)]
        public string Nom_Expert { get; set; }
    }
}
