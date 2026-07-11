using System.Security.Claims;
using EMIT.Data;
using EMIT.Models;
using EMIT.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Controllers
{
    // Espace étudiant : tableau de bord, emploi du temps, salles, profil.
    // Seuls les utilisateurs connectés avec le rôle Etudiant peuvent y accéder.
    [Authorize(Roles = "Etudiant")]
    public class EtudiantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EtudiantController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int IdUtilisateurConnecte => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: /Etudiant  -> Tableau de bord (résumé)
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Dashboard";

            var utilisateur = await _context.Utilisateurs
                .Include(u => u.Classe)
                .FirstOrDefaultAsync(u => u.IdUtilisateur == IdUtilisateurConnecte);

            if (utilisateur == null) return NotFound();

            var nombreCoursSemaine = utilisateur.IdClasse == null
                ? 0
                : await _context.Cours.CountAsync(c => c.IdClasse == utilisateur.IdClasse);

            var prochainCours = utilisateur.IdClasse == null
                ? null
                : await _context.Cours
                    .Include(c => c.Matiere)
                    .Include(c => c.Salle)
                    .Where(c => c.IdClasse == utilisateur.IdClasse)
                    .OrderBy(c => c.Jour).ThenBy(c => c.HeureDebut)
                    .FirstOrDefaultAsync();

            ViewData["NombreCoursSemaine"] = nombreCoursSemaine;
            ViewData["ProchainCours"] = prochainCours;
            ViewData["ProfilComplet"] = !string.IsNullOrEmpty(utilisateur.NumeroInscription)
                && !string.IsNullOrEmpty(utilisateur.Mention)
                && utilisateur.DateNaissance != null;

            return View(utilisateur);
        }

        // GET: /Etudiant/EmploiDuTemps
        public IActionResult EmploiDuTemps()
        {
            ViewData["ActivePage"] = "EmploiDuTemps";
            return View();
        }

        // GET: /Etudiant/Salles
        public async Task<IActionResult> Salles()
        {
            ViewData["ActivePage"] = "Salles";

            var utilisateur = await _context.Utilisateurs.FindAsync(IdUtilisateurConnecte);
            if (utilisateur?.IdClasse == null)
            {
                return View(new List<Cours>());
            }

            var cours = await _context.Cours
                .Include(c => c.Salle)
                .Include(c => c.Matiere)
                .Where(c => c.IdClasse == utilisateur.IdClasse)
                .OrderBy(c => c.Salle!.NomSalle)
                .ToListAsync();

            return View(cours);
        }

        // GET: /Etudiant/Profil
        public async Task<IActionResult> Profil()
        {
            ViewData["ActivePage"] = "Profil";

            var utilisateur = await _context.Utilisateurs.FindAsync(IdUtilisateurConnecte);
            if (utilisateur == null) return NotFound();

            var model = new ProfilEtudiantViewModel
            {
                Nom = utilisateur.Nom,
                Prenom = utilisateur.Prenom,
                Email = utilisateur.Email,
                DateNaissance = utilisateur.DateNaissance,
                NumeroInscription = utilisateur.NumeroInscription ?? string.Empty,
                Mention = utilisateur.Mention ?? string.Empty,
                IdClasse = utilisateur.IdClasse
            };

            RemplirListeClasses(model.IdClasse);
            RemplirListeMentions(model.Mention);
            return View(model);
        }

        // POST: /Etudiant/Profil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profil(ProfilEtudiantViewModel model)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(IdUtilisateurConnecte);
            if (utilisateur == null) return NotFound();

            // Le numéro d'inscription doit être unique
            if (await _context.Utilisateurs.AnyAsync(u =>
                    u.NumeroInscription == model.NumeroInscription && u.IdUtilisateur != utilisateur.IdUtilisateur))
            {
                ModelState.AddModelError(nameof(model.NumeroInscription), "Ce numéro d'inscription est déjà utilisé.");
            }

            if (!ModelState.IsValid)
            {
                model.Email = utilisateur.Email;
                RemplirListeClasses(model.IdClasse);
                RemplirListeMentions(model.Mention);
                return View(model);
            }

            utilisateur.Nom = model.Nom;
            utilisateur.Prenom = model.Prenom;
            utilisateur.DateNaissance = model.DateNaissance.HasValue
                ? DateTime.SpecifyKind(model.DateNaissance.Value, DateTimeKind.Utc)
                : null;
            utilisateur.NumeroInscription = model.NumeroInscription;
            utilisateur.Mention = model.Mention;
            utilisateur.IdClasse = model.IdClasse;

            await _context.SaveChangesAsync();

            TempData["Succes"] = "Votre profil a été enregistré avec succès.";
            return RedirectToAction(nameof(Profil));
        }

        private void RemplirListeClasses(int? idClasseSelectionnee = null)
        {
            ViewData["IdClasse"] = new SelectList(
                _context.Classes.OrderBy(c => c.NomClasse), "IdClasse", "NomClasse", idClasseSelectionnee);
        }

        // Les mentions correspondent aux parcours existants (AES, Informatique, RPM)
        private void RemplirListeMentions(string? mentionSelectionnee = null)
        {
            var mentions = new[]
            {
                new { Valeur = "AES", Libelle = "AES" },
                new { Valeur = "INFORMATIQUE", Libelle = "Informatique" },
                new { Valeur = "RPM", Libelle = "RPM" },
            };

            ViewData["Mention"] = new SelectList(mentions, "Valeur", "Libelle", mentionSelectionnee);
        }
    }
}