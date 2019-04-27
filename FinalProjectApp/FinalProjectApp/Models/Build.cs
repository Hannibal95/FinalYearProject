using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectApp.Models
{
    public class Build
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Version { get; set; }
        public double Budget { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public bool Completed { get; set; }

        public Project Project { get; set; }
        public ICollection<Option> Options { get; set; }
    }
}
