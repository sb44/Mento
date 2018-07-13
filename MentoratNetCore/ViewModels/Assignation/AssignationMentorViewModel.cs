using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MentoratNetCore.ViewModels.Assignation
{
    public class AssignationMentorViewModel : MentoratNetCore.ModelsViews.MentorViewModel
    {

        public AssignationMentorViewModel()
        {

        }

        [Required]
        public new string NoMentor { get; set; }

    }
}