namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PassionDataContext : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Injuries",
                c => new
                    {
                        InjuryID = c.Int(nullable: false, identity: true),
                        InjuryName = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InjuryID);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        PlayerID = c.Int(nullable: false, identity: true),
                        PlayerName = c.String(),
                        PlayerNumber = c.Int(nullable: false),
                        PlayerBio = c.String(),
                        TeamID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PlayerID)
                .ForeignKey("dbo.Teams", t => t.TeamID, cascadeDelete: true)
                .Index(t => t.TeamID);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        TeamID = c.Int(nullable: false, identity: true),
                        TeamName = c.String(),
                        TeamLocation = c.String(),
                        TeamBio = c.String(),
                    })
                .PrimaryKey(t => t.TeamID);
            
            CreateTable(
                "dbo.PlayerInjuries",
                c => new
                    {
                        Player_PlayerID = c.Int(nullable: false),
                        Injury_InjuryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Player_PlayerID, t.Injury_InjuryID })
                .ForeignKey("dbo.Players", t => t.Player_PlayerID, cascadeDelete: true)
                .ForeignKey("dbo.Injuries", t => t.Injury_InjuryID, cascadeDelete: true)
                .Index(t => t.Player_PlayerID)
                .Index(t => t.Injury_InjuryID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Players", "TeamID", "dbo.Teams");
            DropForeignKey("dbo.PlayerInjuries", "Injury_InjuryID", "dbo.Injuries");
            DropForeignKey("dbo.PlayerInjuries", "Player_PlayerID", "dbo.Players");
            DropIndex("dbo.PlayerInjuries", new[] { "Injury_InjuryID" });
            DropIndex("dbo.PlayerInjuries", new[] { "Player_PlayerID" });
            DropIndex("dbo.Players", new[] { "TeamID" });
            DropTable("dbo.PlayerInjuries");
            DropTable("dbo.Teams");
            DropTable("dbo.Players");
            DropTable("dbo.Injuries");
        }
    }
}
