using FinalProjectApp.Classes;
using FinalProjectApp.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectApp
{
    public class SysVars
    {
        //Project System Variables
        public static int CurrentProjectId { get; set; }
        public static string CurrentProjectName { get; set; }
        public static string CurrentProjectDescription { get; set; }
        //Build System Variables
        public static int CurrentBuildId { get; set; }
        public static string CurrentBuildName { get; set; }
        public static Nullable<System.DateTime> CurrentBuildDeadline { get; set; }
        //Component System Variable
        public static long CurrentComponentId { get; set; }
        //Option System Variable
        public static long CurrentOptionId { get; set; }
        //Graph Type System Variable
        public static GraphTypes CurrentGraphType { get; set; }
        public static double MaxRisk = 90; //Default
        public static double MinRisk = 5; //Default
        
    }
}
