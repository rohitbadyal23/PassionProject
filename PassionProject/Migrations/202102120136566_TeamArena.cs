namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TeamArena : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "TeamArena", c => c.String());
            DropColumn("dbo.Teams", "TeamBio");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Teams", "TeamBio", c => c.String());
            DropColumn("dbo.Teams", "TeamArena");
        }
    }
}
