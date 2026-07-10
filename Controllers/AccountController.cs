using System.Security.Claims;
using EMIT.Data;
using EMIT.Models;
using EMIT.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        // PasswordHasher fait partie du framework partagé ASP.NET Core (aucun package NuGet requis)
        private readonly PasswordHasher<Utilisateur> _passwordHasher = new();

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            RemplirListeClasses();
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // RG02 : l'auto-inscription en Administrateur est interdite (défense en profondeur,
            // même si le formulaire ne propose plus ce choix)
            if (model.Role == RoleUtilisateur.Administrateur)
            {
                ModelState.AddModelError(string.Empty, "L'inscription en tant qu'Administrateur n'est pas autorisée.");
            }

            // RG04 : un étudiant doit obligatoirement appartenir à une classe
            if (model.Role == RoleUtilisateur.Etudiant && model.IdClasse == null)
            {
                ModelState.AddModelError(nameof(model.IdClasse), "Veuillez sélectionner une classe pour un étudiant.");
            }

            // Email unique
            if (await _context.Utilisateurs.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Cet email est déjà utilisé.");
            }

            if (!ModelState.IsValid)
            {
                RemplirListeClasses();
                return View(model);
            }

            // RG01 : héritage exclusif -> instancier le bon type selon le rôle
            Utilisateur utilisateur = model.Role == RoleUtilisateur.Enseignant
                ? new Enseignant { PlafondHeuresJournalieres = model.PlafondHeuresJournalieres ?? 6 }
                : new Utilisateur();

            utilisateur.Nom = model.Nom;
            utilisateur.Prenom = model.Prenom;
            utilisateur.Email = model.Email;
            utilisateur.Role = model.Role;
            utilisateur.IdClasse = model.Role == RoleUtilisateur.Etudiant ? model.IdClasse : null;
            utilisateur.MotDePasseHash = _passwordHasher.HashPassword(utilisateur, model.MotDePasse);

            _context.Add(utilisateur);
            await _context.SaveChangesAsync();

            TempData["Succes"] = "Compte créé avec succès. Vous pouvez maintenant vous connecter.";
            return RedirectToAction(nameof(Login));
        }

        // GET: Account/Login
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            // Message volontairement générique : ne pas révéler si l'email existe
            if (utilisateur == null)
            {
                ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
                return View(model);
            }

            var resultat = _passwordHasher.VerifyHashedPassword(utilisateur, utilisateur.MotDePasseHash, model.MotDePasse);
            if (resultat == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, utilisateur.IdUtilisateur.ToString()),
                new(ClaimTypes.Name, $"{utilisateur.Prenom} {utilisateur.Nom}"),
                new(ClaimTypes.Email, utilisateur.Email),
                new(ClaimTypes.Role, utilisateur.Role.ToString()) // RG02/RG03 : base des futures restrictions par rôle
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = model.SeSouvenirDeMoi });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/AccesRefuse
        public IActionResult AccesRefuse() => View();

        private void RemplirListeClasses()
        {
            ViewData["IdClasse"] = new SelectList(
                _context.Classes.OrderBy(c => c.NomClasse), "IdClasse", "NomClasse");
        }
    }
}
