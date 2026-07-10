using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EMIT.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matieres",
                columns: table => new
                {
                    IdMatiere = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomMatiere = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matieres", x => x.IdMatiere);
                });

            migrationBuilder.CreateTable(
                name: "Niveaux",
                columns: table => new
                {
                    IdNiveau = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomNiveau = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Niveaux", x => x.IdNiveau);
                });

            migrationBuilder.CreateTable(
                name: "Salles",
                columns: table => new
                {
                    IdSalle = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomSalle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Capacite = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salles", x => x.IdSalle);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    IdClasse = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomClasse = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IdNiveau = table.Column<int>(type: "integer", nullable: false),
                    Effectif = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.IdClasse);
                    table.ForeignKey(
                        name: "FK_Classes_Niveaux_IdNiveau",
                        column: x => x.IdNiveau,
                        principalTable: "Niveaux",
                        principalColumn: "IdNiveau",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    IdUtilisateur = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Prenom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    MotDePasseHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    IdClasse = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.IdUtilisateur);
                    table.ForeignKey(
                        name: "FK_Utilisateurs_Classes_IdClasse",
                        column: x => x.IdClasse,
                        principalTable: "Classes",
                        principalColumn: "IdClasse",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enseignants",
                columns: table => new
                {
                    IdUtilisateur = table.Column<int>(type: "integer", nullable: false),
                    PlafondHeuresJournalieres = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enseignants", x => x.IdUtilisateur);
                    table.ForeignKey(
                        name: "FK_Enseignants_Utilisateurs_IdUtilisateur",
                        column: x => x.IdUtilisateur,
                        principalTable: "Utilisateurs",
                        principalColumn: "IdUtilisateur",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cours",
                columns: table => new
                {
                    IdCours = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdClasse = table.Column<int>(type: "integer", nullable: false),
                    IdSalle = table.Column<int>(type: "integer", nullable: false),
                    IdEnseignant = table.Column<int>(type: "integer", nullable: false),
                    IdMatiere = table.Column<int>(type: "integer", nullable: false),
                    Jour = table.Column<int>(type: "integer", nullable: false),
                    HeureDebut = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HeureFin = table.Column<TimeSpan>(type: "interval", nullable: false),
                    DerogationExceptionnelle = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cours", x => x.IdCours);
                    table.ForeignKey(
                        name: "FK_Cours_Classes_IdClasse",
                        column: x => x.IdClasse,
                        principalTable: "Classes",
                        principalColumn: "IdClasse",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cours_Enseignants_IdEnseignant",
                        column: x => x.IdEnseignant,
                        principalTable: "Enseignants",
                        principalColumn: "IdUtilisateur",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cours_Matieres_IdMatiere",
                        column: x => x.IdMatiere,
                        principalTable: "Matieres",
                        principalColumn: "IdMatiere",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cours_Salles_IdSalle",
                        column: x => x.IdSalle,
                        principalTable: "Salles",
                        principalColumn: "IdSalle",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_IdNiveau",
                table: "Classes",
                column: "IdNiveau");

            migrationBuilder.CreateIndex(
                name: "IX_Cours_IdClasse",
                table: "Cours",
                column: "IdClasse");

            migrationBuilder.CreateIndex(
                name: "IX_Cours_IdEnseignant",
                table: "Cours",
                column: "IdEnseignant");

            migrationBuilder.CreateIndex(
                name: "IX_Cours_IdMatiere",
                table: "Cours",
                column: "IdMatiere");

            migrationBuilder.CreateIndex(
                name: "IX_Cours_IdSalle",
                table: "Cours",
                column: "IdSalle");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Email",
                table: "Utilisateurs",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_IdClasse",
                table: "Utilisateurs",
                column: "IdClasse");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cours");

            migrationBuilder.DropTable(
                name: "Enseignants");

            migrationBuilder.DropTable(
                name: "Matieres");

            migrationBuilder.DropTable(
                name: "Salles");

            migrationBuilder.DropTable(
                name: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Niveaux");
        }
    }
}
