using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace The_Bank.Migrations
{
    public partial class AddCurrencyAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFreezed",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnfreezeTime",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFreezed",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UnfreezeTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Accounts");
        }
    }
}
