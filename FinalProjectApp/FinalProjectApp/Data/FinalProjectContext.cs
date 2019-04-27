using System.Data.Entity;
using FinalProjectApp.Models;

namespace FinalProjectApp.Data
{
    public class FinalProjectContext : DbContext
    {
        public FinalProjectContext() : base("name=DefaultConnection")
        {

        }

        DbSet<Project> Projects { get; set; }
        DbSet<Build> Builds { get; set; }
        DbSet<Option> Options { get; set; }
        DbSet<Component> Components { get; set; }
    }
}
