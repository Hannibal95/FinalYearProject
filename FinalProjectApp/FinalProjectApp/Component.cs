//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FinalProjectApp
{
    using System;
    using System.Collections.Generic;
    
    public partial class Component
    {
        public long Id { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double ValueToSystem { get; set; }
        public double CostToBuild { get; set; }
        public double RFInterestRate { get; set; }
        public int DurationDays { get; set; }
        public double Volatility { get; set; }
        public bool Selected { get; set; }
    }
}
