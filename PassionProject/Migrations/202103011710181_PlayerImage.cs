namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlayerImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "PlayerHasPic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Players", "PlayerPicExtension", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "PlayerPicExtension");
            DropColumn("dbo.Players", "PlayerHasPic");
        }
    }
}
