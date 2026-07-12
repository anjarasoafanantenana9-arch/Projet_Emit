using EMIT.Models;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();
        public DbSet<Enseignant> Enseignants => Set<Enseignant>();
        public DbSet<Niveau> Niveaux => Set<Niveau>();
        public DbSet<Classe> Classes => Set<Classe>();
        public DbSet<Matiere> Matieres => Set<Matiere>();
        public DbSet<Salle> Salles => Set<Salle>();
        public DbSet<Cours> Cours => Set<Cours>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Héritage exclusif (TPT) : Utilisateurs / Enseignants
            // Etudiant n'a pas de table propre : c'est un Utilisateur avec Role = Etudiant + IdClasse renseigné.
            modelBuilder.Entity<Utilisateur>().ToTable("Utilisateurs");
            modelBuilder.Entity<Enseignant>().ToTable("Enseignants");

            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // RG04 : Utilisateur (étudiant) -> Classe (optionnel au niveau BDD, imposé par le code pour Role=Etudiant)
            modelBuilder.Entity<Utilisateur>()
                .HasOne(u => u.Classe)
                .WithMany(c => c.Etudiants)
                .HasForeignKey(u => u.IdClasse)
                .OnDelete(DeleteBehavior.Restrict);

            // RG05 : Classe -> Niveau (obligatoire, 1,1)
            modelBuilder.Entity<Classe>()
                .HasOne(c => c.Niveau)
                .WithMany(n => n.Classes)
                .HasForeignKey(c => c.IdNiveau)
                .OnDelete(DeleteBehavior.Restrict);

            // Relations de Cours (table pivot)
            modelBuilder.Entity<Cours>()
                .HasOne(c => c.Classe)
                .WithMany(cl => cl.Cours)
                .HasForeignKey(c => c.IdClasse)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cours>()
                .HasOne(c => c.Salle)
                .WithMany(s => s.Cours)
                .HasForeignKey(c => c.IdSalle)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cours>()
                .HasOne(c => c.Enseignant)
                .WithMany(e => e.Cours)
                .HasForeignKey(c => c.IdEnseignant)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cours>()
                .HasOne(c => c.Matiere)
                .WithMany(m => m.Cours)
                .HasForeignKey(c => c.IdMatiere)
                .OnDelete(DeleteBehavior.Restrict);

            // AJOUTÉ : relation plusieurs-à-plusieurs entre Enseignant et Matiere.
            // Un enseignant peut enseigner plusieurs matières, une matière peut être
            // enseignée par plusieurs enseignants. Table pivot "EnseignantMatiere".
            modelBuilder.Entity<Enseignant>()
                .HasMany(e => e.Matieres)
                .WithMany(m => m.Enseignants)
                .UsingEntity(j => j.ToTable("EnseignantMatiere"));
        }
    }
}