namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class injuryname : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Injuries", "InjuryName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Injuries", "InjuryName", c => c.Int(nullable: false));
        }
    }
}
