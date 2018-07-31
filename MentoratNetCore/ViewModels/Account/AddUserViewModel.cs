using MentoratNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MentoratNetCore.ViewModels.Account
{
    public class AddUserViewModel
    {
        public string nomRole { get; set; }

        public List<ApplicationUser> userToAdd { get; set; }
    }
}
