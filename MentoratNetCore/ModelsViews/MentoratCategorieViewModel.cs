using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MentoratNetCore.ModelsViews
{

    public class MentoratCategorieViewModel
    {
              
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        public string Description { get; set; }


        public virtual ICollection<MentoreViewModel> Mentores { get; set; }

        public virtual ICollection<MentorViewModel> Mentors { get; set; }

    }


}