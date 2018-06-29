using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MentoratNetCore.Models
{
    public class Error
    {
        [StringLength(128)]
        public string id { get; set; }
        public List<ErrorMessage> errors { get; set; }
    }
}
