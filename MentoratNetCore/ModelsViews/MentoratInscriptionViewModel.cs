using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MentoratNetCore.ModelsViews
{

    public class MentoratInscriptionViewModel
    {       
        public string Id { get; set; }

        public virtual MentoreViewModel Mentore { get; set; }

        public int Annee { get; set; }
             
        public virtual MentorViewModel Mentor { get; set; }
                      
        public virtual  MentoratCategorieViewModel MentoratCategorie  { get; set; }
      
        public bool APaye { get; set; }

        public DateTime? DateInscription { get; set; }
       
        
        public DateTime? DateDebut { get; set; }
        
        public DateTime? DateFin { get; set; }

    }
}

