namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class playerposition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "PlayerPosition", c => c.String());
            DropColumn("dbo.Players", "PlayerBio");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Players", "PlayerBio", c => c.String());
            DropColumn("dbo.Players", "PlayerPosition");
        }
    }
}
