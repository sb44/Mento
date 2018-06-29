using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MentoratNetCore.Models
{
    public class MentoresExpertises
    {
        public int ExpertiseId { get; set; }
        public virtual Expertise Expertise { get; set; }

        public int MentoreId { get; set; }
        public virtual Mentore Mentore { get; set; }
    }
}
