namespace CorridorSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.eventModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DTEnd = c.DateTime(nullable: false),
                        DTStart = c.DateTime(nullable: false),
                        Duration = c.Time(nullable: false, precision: 7),
                        DTStamp = c.DateTime(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                        Summary = c.String(),
                        Location = c.String(),
                        externalId = c.String(),
                        scheduleModel_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.scheduleModel", t => t.scheduleModel_Id)
                .Index(t => t.scheduleModel_Id);
            
            CreateTable(
                "dbo.CorrUser",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        UserType = c.Int(nullable: false),
                        UserName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        signature = c.String(nullable: false),
                        schedule_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.scheduleModel", t => t.schedule_Id)
                .Index(t => t.schedule_Id);
            
            CreateTable(
                "dbo.scheduleModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        signature = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RemovedUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        UserType = c.Int(nullable: false),
                        UserName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        CorrUser_Id = c.Int(),
                        RemovedUsers_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CorrUser", t => t.CorrUser_Id)
                .ForeignKey("dbo.RemovedUsers", t => t.RemovedUsers_Id)
                .Index(t => t.CorrUser_Id)
                .Index(t => t.RemovedUsers_Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "RemovedUsers_Id", "dbo.RemovedUsers");
            DropForeignKey("dbo.AspNetUsers", "CorrUser_Id", "dbo.CorrUser");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CorrUser", "schedule_Id", "dbo.scheduleModel");
            DropForeignKey("dbo.eventModel", "scheduleModel_Id", "dbo.scheduleModel");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "RemovedUsers_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "CorrUser_Id" });
            DropIndex("dbo.CorrUser", new[] { "schedule_Id" });
            DropIndex("dbo.eventModel", new[] { "scheduleModel_Id" });
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.RemovedUsers");
            DropTable("dbo.scheduleModel");
            DropTable("dbo.CorrUser");
            DropTable("dbo.eventModel");
        }
    }
}
