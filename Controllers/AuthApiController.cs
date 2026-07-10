using System.Security.Claims;
using EMIT.Data;
using EMIT.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Controllers
{
    // Contrôleur API JSON consommé par le fetch() de Views/home/Index.cshtml
    // (routes /api/auth/login et /api/auth/register)
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Utilisateur> _passwordHasher = new();

        public AuthApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class RegisterApiModel
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty; // "Admin" | "Prof" | "Etudiant"
        }

        public class LoginApiModel
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        // POST /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterApiModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest(new { message = "Email et mot de passe sont obligatoires." });

            if (!TryMapRole(model.Role, out var role))
                return BadRequest(new { message = "Rôle invalide." });

            // RG02 : l'auto-inscription en Administrateur est interdite (même règle que AccountController)
            if (role == RoleUtilisateur.Administrateur)
                return BadRequest(new { message = "L'inscription en tant qu'Administrateur n'est pas autorisée." });

            if (await _context.Utilisateurs.AnyAsync(u => u.Email == model.Email))
                return BadRequest(new { message = "Cet email est déjà utilisé." });

            Utilisateur utilisateur = role == RoleUtilisateur.Enseignant
                ? new Enseignant()
                : new Utilisateur();

            // Le front n'envoie pas de Nom/Prénom : on utilise la partie locale de l'email
            // en attendant que l'utilisateur complète son profil (modifiable via Utilisateurs/Edit).
            var pseudo = model.Email.Split('@')[0];
            utilisateur.Nom = pseudo;
            utilisateur.Prenom = pseudo;
            utilisateur.Email = model.Email;
            utilisateur.Role = role;
            utilisateur.MotDePasseHash = _passwordHasher.HashPassword(utilisateur, model.Password);

            _context.Add(utilisateur);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Compte créé avec succès." });
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginApiModel model)
        {
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (utilisateur == null)
                return Unauthorized(new { message = "Email ou mot de passe incorrect." });

            var resultat = _passwordHasher.VerifyHashedPassword(utilisateur, utilisateur.MotDePasseHash, model.Password);
            if (resultat == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Email ou mot de passe incorrect." });

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, utilisateur.IdUtilisateur.ToString()),
                new(ClaimTypes.Name, $"{utilisateur.Prenom} {utilisateur.Nom}"),
                new(ClaimTypes.Email, utilisateur.Email),
                new(ClaimTypes.Role, utilisateur.Role.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return Ok(new { email = utilisateur.Email, role = MapRoleToFront(utilisateur.Role) });
        }

        private static bool TryMapRole(string role, out RoleUtilisateur mapped)
        {
            switch (role)
            {
                case "Admin": mapped = RoleUtilisateur.Administrateur; return true;
                case "Prof": mapped = RoleUtilisateur.Enseignant; return true;
                case "Etudiant": mapped = RoleUtilisateur.Etudiant; return true;
                default: mapped = default; return false;
            }
        }

        private static string MapRoleToFront(RoleUtilisateur role) => role switch
        {
            RoleUtilisateur.Administrateur => "Admin",
            RoleUtilisateur.Enseignant => "Prof",
            _ => "Etudiant"
        };
    }
}
