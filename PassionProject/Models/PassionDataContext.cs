using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace PassionProject.Models
{
    public class PassionDataContext : DbContext
    {
        //Connection string properties can be defined within base()
        //If left empty, a database will be automatically created
        public PassionDataContext() : base("name=PassionDataContext")
        {

        }

        public static PassionDataContext Create()
        {
            return new PassionDataContext();
        }

        //Instruction to set the models as tables in our database.
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Injury> Injuries { get; set; }

        //Tools > NuGet Package Manager > Package Manager Console
        //enable-migrations (only once)
        //add-migration {migration name}
        //update-database

        //To View the Database Changes sequentially, go to Project/Migrations folder

        //To View Database itself, go to View > SQL Server Object Explorer
        // (localdb)\MSSQLLocalDB
        // Right Click {ProjectName}.Models.DataContext > Tables
        // Can Right Click Tables to View Data
        // Can Right Click Database to Query
    }
}