using EMIT.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Data
{
    public static class DbInitializer
    {
        // RG02 : crée un compte Administrateur initial si aucun n'existe encore.
        // Évite de dépendre du formulaire d'inscription public (désormais interdit pour ce rôle).
        public static async Task SeedAdminAsync(ApplicationDbContext context, IConfiguration configuration)
        {
            await context.Database.MigrateAsync();

            bool adminExiste = await context.Utilisateurs.AnyAsync(u => u.Role == RoleUtilisateur.Administrateur);
            if (adminExiste) return;

            string email = configuration["AdminSeed:Email"] ?? "admin@emit.local";
            string motDePasse = configuration["AdminSeed:MotDePasse"] ?? "ChangeMoi123!";

            var admin = new Utilisateur
            {
                Nom = "Administrateur",
                Prenom = "Principal",
                Email = email,
                Role = RoleUtilisateur.Administrateur
            };

            var hasher = new PasswordHasher<Utilisateur>();
            admin.MotDePasseHash = hasher.HashPassword(admin, motDePasse);

            context.Utilisateurs.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
