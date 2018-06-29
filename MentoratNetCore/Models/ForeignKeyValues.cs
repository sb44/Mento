using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MentoratNetCore.Models
{
    public class ForeignKeyValues
    {
        public IEnumerable<Expert> Experts { get; set; }
        public IEnumerable<Mentor> Mentors { get; set; }
        public IEnumerable<Mentore> Mentores { get; set; }
    }
}
