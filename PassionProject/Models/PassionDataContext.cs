using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace PassionProject.Models
{
    public class PassionDataContext : DbContext
    {
        //Connection to Database
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

    }
}