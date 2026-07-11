using System.Security.Claims;
using EMIT.Data;
using EMIT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Controllers
{
    // API JSON consommée par le fetch() de Views/Etudiant/Index.cshtml
    // Route : GET /api/EmploiDuTemps
    [ApiController]
    [Route("api/EmploiDuTemps")]
    [Authorize] // il faut être connecté
    public class EmploiDuTempsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmploiDuTempsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class CoursDto
        {
            public int IdCours { get; set; }
            public int Jour { get; set; } // 1 = Lundi ... 5 = Vendredi
            public string HeureD { get; set; } = string.Empty;
            public string HeureFin { get; set; } = string.Empty;
            public int IdMat { get; set; }
            public string NomMatiere { get; set; } = string.Empty;
            public int IdProf { get; set; }
            public string NomProf { get; set; } = string.Empty;
            public int IdSalle { get; set; }
            public string NomSalle { get; set; } = string.Empty;
        }

        // GET: api/EmploiDuTemps
        // RG03 : un étudiant ne voit que le planning de SA classe (le paramètre idClasse
        // envoyé par le front est ignoré pour un étudiant, on se base sur son propre compte).
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoursDto>>> Get()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var idUtilisateur = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var utilisateur = await _context.Utilisateurs.FindAsync(idUtilisateur);
            if (utilisateur == null) return NotFound();

            if (role == RoleUtilisateur.Etudiant.ToString() && utilisateur.IdClasse == null)
            {
                // Un étudiant sans classe (cas anormal) : aucun cours à afficher
                return Ok(Array.Empty<CoursDto>());
            }

            var requete = _context.Cours
                .Include(c => c.Matiere)
                .Include(c => c.Enseignant)
                .Include(c => c.Salle)
                .AsQueryable();

            if (role == RoleUtilisateur.Etudiant.ToString())
            {
                requete = requete.Where(c => c.IdClasse == utilisateur.IdClasse);
            }
            else if (role == RoleUtilisateur.Enseignant.ToString())
            {
                requete = requete.Where(c => c.IdEnseignant == idUtilisateur);
            }
            // Administrateur : pas de filtre

            var cours = await requete
                .OrderBy(c => c.Jour)
                .ThenBy(c => c.HeureDebut)
                .Select(c => new CoursDto
                {
                    IdCours = c.IdCours,
                    Jour = (int)c.Jour + 1, // enum Lundi=0 -> 1, ... Vendredi=4 -> 5
                    HeureD = c.HeureDebut.ToString(@"hh\:mm"),
                    HeureFin = c.HeureFin.ToString(@"hh\:mm"),
                    IdMat = c.IdMatiere,
                    NomMatiere = c.Matiere!.NomMatiere,
                    IdProf = c.IdEnseignant,
                    NomProf = c.Enseignant!.Prenom + " " + c.Enseignant!.Nom,
                    IdSalle = c.IdSalle,
                    NomSalle = c.Salle!.NomSalle
                })
                .ToListAsync();

            return Ok(cours);
        }
    }
}
