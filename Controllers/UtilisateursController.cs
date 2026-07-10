using EMIT.Data;
using EMIT.Models;
using EMIT.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Controllers
{
    [Authorize(Roles = "Administrateur")] // RG02 : ajout/modification/suppression des comptes
    public class UtilisateursController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Utilisateur> _passwordHasher = new();

        public UtilisateursController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Utilisateurs
        public async Task<IActionResult> Index()
        {
            var utilisateurs = await _context.Utilisateurs
                .Include(u => u.Classe)
                .OrderBy(u => u.Role)
                .ThenBy(u => u.Nom)
                .ToListAsync();

            return View(utilisateurs);
        }

        // GET: Utilisateurs/CreerAdministrateur
        // Seule façon de créer un compte Administrateur (l'inscription publique le refuse)
        public IActionResult CreerAdministrateur() => View();

        // POST: Utilisateurs/CreerAdministrateur
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreerAdministrateur(RegisterViewModel model)
        {
            if (await _context.Utilisateurs.AnyAsync(u => u.Email == model.Email))
                ModelState.AddModelError(nameof(model.Email), "Cet email est déjà utilisé.");

            if (!ModelState.IsValid) return View(model);

            var admin = new Utilisateur
            {
                Nom = model.Nom,
                Prenom = model.Prenom,
                Email = model.Email,
                Role = RoleUtilisateur.Administrateur
            };
            admin.MotDePasseHash = _passwordHasher.HashPassword(admin, model.MotDePasse);

            _context.Add(admin);
            await _context.SaveChangesAsync();

            TempData["Succes"] = "Compte Administrateur créé avec succès.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Utilisateurs/Edit/5
        // Le rôle n'est volontairement pas modifiable ici (changer de rôle = supprimer et recréer le compte,
        // à cause de l'héritage TPT sur Enseignant).
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null) return NotFound();

            RemplirListeClasses(utilisateur);
            return View(utilisateur);
        }

        // POST: Utilisateurs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUtilisateur,Nom,Prenom,Email,IdClasse")] Utilisateur formulaire)
        {
            if (id != formulaire.IdUtilisateur) return NotFound();

            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null) return NotFound();

            if (await _context.Utilisateurs.AnyAsync(u => u.Email == formulaire.Email && u.IdUtilisateur != id))
                ModelState.AddModelError(nameof(formulaire.Email), "Cet email est déjà utilisé.");

            // RG04 : un étudiant doit toujours avoir une classe
            if (utilisateur.Role == RoleUtilisateur.Etudiant && formulaire.IdClasse == null)
                ModelState.AddModelError(nameof(formulaire.IdClasse), "Un étudiant doit être rattaché à une classe.");

            if (!ModelState.IsValid)
            {
                RemplirListeClasses(utilisateur);
                return View(formulaire);
            }

            utilisateur.Nom = formulaire.Nom;
            utilisateur.Prenom = formulaire.Prenom;
            utilisateur.Email = formulaire.Email;
            if (utilisateur.Role == RoleUtilisateur.Etudiant)
                utilisateur.IdClasse = formulaire.IdClasse;

            await _context.SaveChangesAsync();
            TempData["Succes"] = "Compte modifié avec succès.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Utilisateurs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var utilisateur = await _context.Utilisateurs
                .Include(u => u.Classe)
                .FirstOrDefaultAsync(u => u.IdUtilisateur == id);

            if (utilisateur == null) return NotFound();
            return View(utilisateur);
        }

        // POST: Utilisateurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur != null)
            {
                _context.Utilisateurs.Remove(utilisateur);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private void RemplirListeClasses(Utilisateur? utilisateur = null)
        {
            ViewData["IdClasse"] = new SelectList(
                _context.Classes.OrderBy(c => c.NomClasse), "IdClasse", "NomClasse", utilisateur?.IdClasse);
        }
    }
}
