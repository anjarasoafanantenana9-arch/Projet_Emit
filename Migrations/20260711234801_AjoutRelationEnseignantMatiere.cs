using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMIT.Migrations
{
    /// <inheritdoc />
    public partial class AjoutRelationEnseignantMatiere : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.CreateTable(
                name: "EnseignantMatiere",
                columns: table => new
                {
                    EnseignantsIdUtilisateur = table.Column<int>(type: "integer", nullable: false),
                    MatieresIdMatiere = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnseignantMatiere", x => new { x.EnseignantsIdUtilisateur, x.MatieresIdMatiere });
                    table.ForeignKey(
                        name: "FK_EnseignantMatiere_Enseignants_EnseignantsIdUtilisateur",
                        column: x => x.EnseignantsIdUtilisateur,
                        principalTable: "Enseignants",
                        principalColumn: "IdUtilisateur",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnseignantMatiere_Matieres_MatieresIdMatiere",
                        column: x => x.MatieresIdMatiere,
                        principalTable: "Matieres",
                        principalColumn: "IdMatiere",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnseignantMatiere_MatieresIdMatiere",
                table: "EnseignantMatiere",
                column: "MatieresIdMatiere");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnseignantMatiere");

            
        }
    }
}
