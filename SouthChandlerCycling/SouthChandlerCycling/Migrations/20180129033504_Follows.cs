using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SouthChandlerCycling.Migrations
{
    public partial class Follows : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Follows",
                columns: table => new
                {
                    FollowID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FollowState = table.Column<int>(nullable: false),
                    FollowerID = table.Column<int>(nullable: false),
                    FollowerRiderID = table.Column<int>(nullable: true),
                    FollowingID = table.Column<int>(nullable: false),
                    FollowingRiderID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Follows", x => x.FollowID);
                    table.ForeignKey(
                        name: "FK_Follows_Riders_FollowerRiderID",
                        column: x => x.FollowerRiderID,
                        principalTable: "Riders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Follows_Riders_FollowingRiderID",
                        column: x => x.FollowingRiderID,
                        principalTable: "Riders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowerRiderID",
                table: "Follows",
                column: "FollowerRiderID");

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowingRiderID",
                table: "Follows",
                column: "FollowingRiderID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Follows");
        }
    }
}
