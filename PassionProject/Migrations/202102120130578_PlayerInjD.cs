namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlayerInjD : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "PlayerInjuryDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "PlayerInjuryDescription");
        }
    }
}
