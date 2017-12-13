using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SouthChandlerCycling.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Riders",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmailAddress = table.Column<string>(maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastLatitude = table.Column<string>(nullable: true),
                    LastLongitude = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    Password = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 14, nullable: false),
                    Role = table.Column<string>(maxLength: 50, nullable: true),
                    Salt = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Riders", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Rides",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 120, nullable: false),
                    Distance = table.Column<double>(nullable: false),
                    RideName = table.Column<string>(maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rides", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SignUps",
                columns: table => new
                {
                    SignupID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RideID = table.Column<int>(nullable: false),
                    RiderID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignUps", x => x.SignupID);
                    table.ForeignKey(
                        name: "FK_SignUps_Rides_RideID",
                        column: x => x.RideID,
                        principalTable: "Rides",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignUps_Riders_RiderID",
                        column: x => x.RiderID,
                        principalTable: "Riders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SignUps_RideID",
                table: "SignUps",
                column: "RideID");

            migrationBuilder.CreateIndex(
                name: "IX_SignUps_RiderID",
                table: "SignUps",
                column: "RiderID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SignUps");

            migrationBuilder.DropTable(
                name: "Rides");

            migrationBuilder.DropTable(
                name: "Riders");
        }
    }
}
