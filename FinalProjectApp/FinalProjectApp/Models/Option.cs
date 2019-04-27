using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectApp.Models
{
    public class Option
    {
        public int Id { get; set; }
        public int? BuildID { get; set; }
        public int? BuildOrder { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double CallOption { get; set; }
        public double Value { get; set; }
        public double Volatility { get; set; }
        public double TimeToComplete { get; set; }
        public bool Completed { get; set; }
    }
}
