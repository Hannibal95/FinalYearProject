using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectApp.Models
{
    public class Component
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double ValueToSystem { get; set; }
        public double CostToBuild { get; set; }
        public double RFInterestRate { get; set; }
        public double DividendYield { get; set; }
        public int DaysToComplete { get; set; }
        public double Volatility { get; set; }
        public bool Selected { get; set; }

        public Project Project { get; set; }
    }
}
