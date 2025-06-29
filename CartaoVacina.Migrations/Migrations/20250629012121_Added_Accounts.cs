using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CartaoVacina.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Added_Accounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vaccinations_Users_UserId1",
                table: "Vaccinations");

            migrationBuilder.DropForeignKey(
                name: "FK_Vaccinations_Vaccines_VaccineId1",
                table: "Vaccinations");

            migrationBuilder.DropIndex(
                name: "IX_Vaccinations_UserId1",
                table: "Vaccinations");

            migrationBuilder.DropIndex(
                name: "IX_Vaccinations_VaccineId1",
                table: "Vaccinations");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Vaccinations");

            migrationBuilder.DropColumn(
                name: "VaccineId1",
                table: "Vaccinations");

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Salt = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_Email",
                table: "Account",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Vaccinations",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VaccineId1",
                table: "Vaccinations",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vaccinations_UserId1",
                table: "Vaccinations",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Vaccinations_VaccineId1",
                table: "Vaccinations",
                column: "VaccineId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Vaccinations_Users_UserId1",
                table: "Vaccinations",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vaccinations_Vaccines_VaccineId1",
                table: "Vaccinations",
                column: "VaccineId1",
                principalTable: "Vaccines",
                principalColumn: "Id");
        }
    }
}
