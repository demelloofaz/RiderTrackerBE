using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SouthChandlerCycling.Migrations
{
    public partial class Update122017b : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveRide",
                table: "Riders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "LastRide",
                table: "Riders",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveRide",
                table: "Riders");

            migrationBuilder.DropColumn(
                name: "LastRide",
                table: "Riders");
        }
    }
}
