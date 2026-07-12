using System.Security.Claims;
using EMIT.Data;
using EMIT.Models;
using EMIT.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Controllers
{
    // Espace Enseignant : toutes les pages ici ne montrent que les données
    // qui appartiennent à l'enseignant actuellement connecté (RG01/RG03).
    [Authorize(Roles = "Enseignant")]
    public class ProfController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Utilisateur> _passwordHasher = new();

        public ProfController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Rend le nom de l'enseignant connecté disponible dans toutes les vues
        // (sidebar commune), sans avoir à le répéter dans chaque action.
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.NomEnseignant = User.FindFirstValue(ClaimTypes.Name) ?? "Enseignant";
            base.OnActionExecuting(context);
        }

        // GET: Prof  -> Tableau de bord / emploi du temps de la semaine
        public async Task<IActionResult> Index()
        {
            int idUtilisateur = IdEnseignantConnecte();

            var enseignant = await _context.Enseignants
                .FirstOrDefaultAsync(e => e.IdUtilisateur == idUtilisateur);

            if (enseignant == null) return NotFound();

            enseignant.Cours = await _context.Cours
                .Where(c => c.IdEnseignant == idUtilisateur)
                .Include(c => c.Classe)
                    .ThenInclude(cl => cl!.Niveau)
                .Include(c => c.Salle)
                .Include(c => c.Matiere)
                .OrderBy(c => c.Jour)
                .ThenBy(c => c.HeureDebut)
                .ToListAsync();

            // Une grille sera construite pour chaque niveau (L1, L2, L3...),
            // remplie uniquement avec les cours de CET enseignant (déjà filtré ci-dessus)
            ViewData["Niveaux"] = await _context.Niveaux
                .OrderBy(n => n.NomNiveau)
                .ToListAsync();

            return View(enseignant);
        }

        // GET: Prof/Cours -> "Mes Cours" : liste des matières/créneaux assignés
        public async Task<IActionResult> Cours()
        {
            int idUtilisateur = IdEnseignantConnecte();

            var mesCours = await _context.Cours
                .Where(c => c.IdEnseignant == idUtilisateur)
                .Include(c => c.Classe)
                .Include(c => c.Matiere)
                .OrderBy(c => c.Matiere!.NomMatiere)
                .ToListAsync();

            return View(mesCours);
        }

        // GET: Prof/Salles -> disponibilité des salles en temps réel
        public async Task<IActionResult> Salles()
        {
            var maintenant = DateTime.Now;
            var jourActuel = ConvertirJourSemaine(maintenant.DayOfWeek);
            var heureActuelle = maintenant.TimeOfDay;

            var coursEnCoursParSalle = await _context.Cours
                .Include(c => c.Classe)
                .Include(c => c.Matiere)
                .Where(c => jourActuel != null
                            && c.Jour == jourActuel
                            && c.HeureDebut <= heureActuelle
                            && heureActuelle < c.HeureFin)
                .ToListAsync();

            var salles = await _context.Salles
                .OrderBy(s => s.NomSalle)
                .Select(s => new SalleDisponibiliteViewModel
                {
                    IdSalle = s.IdSalle,
                    NomSalle = s.NomSalle,
                    Capacite = s.Capacite
                })
                .ToListAsync();

            foreach (var salle in salles)
            {
                salle.CoursEnCours = coursEnCoursParSalle.FirstOrDefault(c => c.IdSalle == salle.IdSalle);
            }

            return View(salles);
        }

        // GET: Prof/Profil
        public async Task<IActionResult> Profil()
        {
            var enseignant = await _context.Enseignants
                .FirstOrDefaultAsync(e => e.IdUtilisateur == IdEnseignantConnecte());

            if (enseignant == null) return NotFound();

            return View(enseignant);
        }

        // POST: Prof/ModifierMotDePasse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifierMotDePasse(ChangerMotDePasseViewModel model)
        {
            var enseignant = await _context.Enseignants
                .FirstOrDefaultAsync(e => e.IdUtilisateur == IdEnseignantConnecte());

            if (enseignant == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View("Profil", enseignant);
            }

            var verification = _passwordHasher.VerifyHashedPassword(
                enseignant, enseignant.MotDePasseHash, model.MotDePasseActuel);

            if (verification == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(nameof(model.MotDePasseActuel), "Le mot de passe actuel est incorrect.");
                return View("Profil", enseignant);
            }

            enseignant.MotDePasseHash = _passwordHasher.HashPassword(enseignant, model.NouveauMotDePasse);
            await _context.SaveChangesAsync();

            TempData["Succes"] = "Mot de passe mis à jour avec succès.";
            return RedirectToAction(nameof(Profil));
        }

        private int IdEnseignantConnecte()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private static JourSemaine? ConvertirJourSemaine(DayOfWeek jour) => jour switch
        {
            DayOfWeek.Monday => JourSemaine.Lundi,
            DayOfWeek.Tuesday => JourSemaine.Mardi,
            DayOfWeek.Wednesday => JourSemaine.Mercredi,
            DayOfWeek.Thursday => JourSemaine.Jeudi,
            DayOfWeek.Friday => JourSemaine.Vendredi,
            _ => null // week-end : aucun cours possible (RG12)
        };
    }
}