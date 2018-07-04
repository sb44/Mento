using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MentoratNetCore.Models
{
    public class MentoresExpertises
    {
        public int No_Expertise { get; set; }
        public virtual Expertise Expertise { get; set; }
        public string No_Mentore { get; set; }
        public virtual Mentore Mentore { get; set; }
    }
}
