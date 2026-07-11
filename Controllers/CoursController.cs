using System.Security.Claims;
using EMIT.Data;
using EMIT.Models;
using EMIT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Controllers
{
    [Authorize]
    public class CoursController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICoursValidationService _validationService;

        public CoursController(ApplicationDbContext context, ICoursValidationService validationService)
        {
            _context = context;
            _validationService = validationService;
        }

        // GET: Cours (Ity ilay "Mes Cours" ho an'ny cours.cshtml)
        public async Task<IActionResult> Index()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var idUtilisateur = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Alaina ny anaran'ny mpampianatra tafiditra (ho an'ny Layout/Sidebar)
            var nmp = User.FindFirstValue(ClaimTypes.Name);
            ViewBag.NomEnseignant = nmp ?? "Nom Enseignant";

            IQueryable<Cours> requete = _context.Cours
                .Include(c => c.Classe)
                .Include(c => c.Salle)
                .Include(c => c.Enseignant)
                .Include(c => c.Matiere);

            // Sivana: ny azy ihany no hita raha mpampianatra
            if (role == RoleUtilisateur.Enseignant.ToString())
            {
                requete = requete.Where(c => c.IdEnseignant == idUtilisateur);
            }
            else if (role == RoleUtilisateur.Etudiant.ToString())
            {
                var utilisateur = await _context.Utilisateurs.FindAsync(idUtilisateur);
                requete = requete.Where(c => c.IdClasse == utilisateur!.IdClasse);
            }

            var cours = await requete
                .OrderBy(c => c.Jour)
                .ThenBy(c => c.HeureDebut)
                .ToListAsync();

            // Mamerina ny pejy Index.cshtml (izay ho lasa cours.cshtml-nao)
            return View(cours);
        }

        // GET: Cours/Salles (Ity ny ho an'ny salles.cshtml)
        public async Task<IActionResult> Salles()
        {
            var nmp = User.FindFirstValue(ClaimTypes.Name);
            ViewBag.NomEnseignant = nmp ?? "Nom Enseignant";

            var salles = await _context.Salles.OrderBy(s => s.NomSalle).ToListAsync();
            ViewBag.CoursAnkehitriny = await _context.Cours.ToListAsync();

            return View(salles);
        }

        // GET: Cours/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cours = await _context.Cours
                .Include(c => c.Classe)
                .Include(c => c.Salle)
                .Include(c => c.Enseignant)
                .Include(c => c.Matiere)
                .FirstOrDefaultAsync(m => m.IdCours == id);

            if (cours == null) return NotFound();
            if (!PeutConsulter(cours)) return Forbid();

            return View(cours);
        }

        // GET: Cours/Create
        [Authorize(Roles = "Administrateur")]
        public IActionResult Create()
        {
            RemplirListesDeroulantes();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrateur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdClasse,IdSalle,IdEnseignant,IdMatiere,Jour,HeureDebut,HeureFin,DerogationExceptionnelle")] Cours cours)
        {
            if (ModelState.IsValid)
            {
                var validation = await _validationService.ValiderAsync(cours);
                if (!validation.EstValide)
                {
                    foreach (var erreur in validation.Erreurs)
                        ModelState.AddModelError(string.Empty, erreur);
                }
                else
                {
                    _context.Add(cours);
                    await _context.SaveChangesAsync();
                    TempData["Succes"] = "Le cours a été planifié avec succès.";
                    return RedirectToAction(nameof(Index));
                }
            }
            RemplirListesDeroulantes(cours);
            return View(cours);
        }

        // GET: Cours/Edit/5
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var cours = await _context.Cours.FindAsync(id);
            if (cours == null) return NotFound();
            RemplirListesDeroulantes(cours);
            return View(cours);
        }

        [HttpPost]
        [Authorize(Roles = "Administrateur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCours,IdClasse,IdSalle,IdEnseignant,IdMatiere,Jour,HeureDebut,HeureFin,DerogationExceptionnelle")] Cours cours)
        {
            if (id != cours.IdCours) return NotFound();

            if (ModelState.IsValid)
            {
                var validation = await _validationService.ValiderAsync(cours);
                if (!validation.EstValide)
                {
                    foreach (var erreur in validation.Erreurs)
                        ModelState.AddModelError(string.Empty, erreur);
                }
                else
                {
                    try
                    {
                        _context.Update(cours);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!_context.Cours.Any(e => e.IdCours == cours.IdCours)) return NotFound();
                        throw;
                    }
                    TempData["Succes"] = "Le cours a été modifié avec succès.";
                    return RedirectToAction(nameof(Index));
                }
            }
            RemplirListesDeroulantes(cours);
            return View(cours);
        }

        // GET: Cours/Delete/5
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var cours = await _context.Cours
                .Include(c => c.Classe)
                .Include(c => c.Salle)
                .Include(c => c.Enseignant)
                .Include(c => c.Matiere)
                .FirstOrDefaultAsync(m => m.IdCours == id);
            if (cours == null) return NotFound();
            return View(cours);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrateur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cours = await _context.Cours.FindAsync(id);
            if (cours != null)
            {
                _context.Cours.Remove(cours);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PeutConsulter(Cours cours)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role == RoleUtilisateur.Administrateur.ToString()) return true;

            var idUtilisateur = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (role == RoleUtilisateur.Enseignant.ToString()) return cours.IdEnseignant == idUtilisateur;

            if (role == RoleUtilisateur.Etudiant.ToString())
            {
                var idClasse = _context.Utilisateurs.Find(idUtilisateur)?.IdClasse;
                return cours.IdClasse == idClasse;
            }
            return false;
        }

        private void RemplirListesDeroulantes(Cours? cours = null)
        {
            ViewData["IdClasse"] = new SelectList(_context.Classes.OrderBy(c => c.NomClasse), "IdClasse", "NomClasse", cours?.IdClasse);
            ViewData["IdSalle"] = new SelectList(_context.Salles.OrderBy(s => s.NomSalle), "IdSalle", "NomSalle", cours?.IdSalle);
            ViewData["IdEnseignant"] = new SelectList(_context.Enseignants.OrderBy(e => e.Nom), "IdEnseignant", "Nom", cours?.IdEnseignant);
            ViewData["IdMatiere"] = new SelectList(_context.Matieres.OrderBy(m => m.NomMatiere), "IdMatiere", "NomMatiere", cours?.IdMatiere);
        }
    }
}