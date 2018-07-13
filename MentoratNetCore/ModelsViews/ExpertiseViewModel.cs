using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MentoratNetCore.ModelsViews
{

    public class ExpertiseViewModel
    {
        public ExpertiseViewModel()
        {
        }
               
        
        public int No_Expertise { get; set; }

        [StringLength(255)]
        public string Nom_Expertise { get; set; }

        public override string ToString()
        {
            return Nom_Expertise;
        }

        public int? No_Regroupement_Expertise { get; set; }

    
      public virtual ICollection<MentoreViewModel> Mentores { get; set; }

    }


}