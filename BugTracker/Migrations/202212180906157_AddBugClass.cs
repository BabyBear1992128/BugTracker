namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBugClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bugs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        CreatorId = c.Int(nullable: false),
                        PriorityId = c.Int(nullable: false),
                        SeverityId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        Solved = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BugId = c.Int(nullable: false),
                        Text = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BugId = c.Int(nullable: false),
                        Title = c.String(),
                        Text = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        JobTitle = c.String(),
                        Salary = c.String(),
                        YearsOfExperience = c.String(),
                        HiredDate = c.String(),
                        Address = c.String(),
                        Email = c.String(),
                        DateOfBirth = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Priorities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Desc = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Severities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Desc = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Severities");
            DropTable("dbo.Priorities");
            DropTable("dbo.People");
            DropTable("dbo.Messages");
            DropTable("dbo.Logs");
            DropTable("dbo.Bugs");
        }
    }
}
