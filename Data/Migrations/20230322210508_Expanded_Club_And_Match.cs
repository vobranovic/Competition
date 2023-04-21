using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Competition.Data.Migrations
{
    public partial class Expanded_Club_And_Match : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MatchTime",
                table: "Matches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Draw",
                table: "Clubs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoalsAgainst",
                table: "Clubs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoalsFor",
                table: "Clubs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Loss",
                table: "Clubs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MatchesPlayed",
                table: "Clubs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "Clubs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Win",
                table: "Clubs",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchTime",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Draw",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "GoalsAgainst",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "GoalsFor",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "Loss",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "MatchesPlayed",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "Clubs");

            migrationBuilder.DropColumn(
                name: "Win",
                table: "Clubs");
        }
    }
}
