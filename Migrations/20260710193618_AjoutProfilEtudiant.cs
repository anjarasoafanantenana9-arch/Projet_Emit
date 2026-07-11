using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMIT.Migrations
{
    /// <inheritdoc />
    public partial class AjoutProfilEtudiant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateNaissance",
                table: "Utilisateurs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mention",
                table: "Utilisateurs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroInscription",
                table: "Utilisateurs",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateNaissance",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "Mention",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "NumeroInscription",
                table: "Utilisateurs");
        }
    }
}
