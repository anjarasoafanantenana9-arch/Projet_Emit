using EMIT.Data;
using EMIT.Models;
using EMIT.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projet_Emit.Models.DTO;

namespace EMIT.Controllers
{
    [Route("api/admin/cours")]
    [ApiController]
    public class AdminCoursController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICoursValidationService _validationService;


        public AdminCoursController(
            ApplicationDbContext context,
            ICoursValidationService validationService)
        {
            _context = context;
            _validationService = validationService;
        }


        // POST : api/admin/cours
        [HttpPost]
        public async Task<IActionResult> AjouterCours([FromBody] Cours cours)
        {

            var validation = await _validationService.ValiderAsync(cours);


            if(!validation.EstValide)
            {
                return BadRequest(new
                {
                    message = validation.Erreurs
                });
            }


            _context.Cours.Add(cours);

            await _context.SaveChangesAsync();


            return Ok(new
            {
                message="Cours ajouté avec succès",
                idCours=cours.IdCours
            });

        }



        // GET : api/admin/cours
        // GET : api/admin/cours?parcours=AES
        [HttpGet]
        public async Task<IActionResult> GetCours([FromQuery] ParcoursType? parcours = null)
        {
            var query = _context.Cours
                .Include(c => c.Classe)
                    .ThenInclude(cl => cl!.Niveau)
                .Include(c => c.Salle)
                .Include(c => c.Matiere)
                .Include(c => c.Enseignant)
                .AsQueryable();

            if (parcours.HasValue)
            {
                query = query.Where(c => c.Classe!.Parcours == parcours.Value);
            }

            var cours = await query
                .Select(c => new
                {
                    idCours = c.IdCours,
                    jour = c.Jour,
                    heureDebut = c.HeureDebut,
                    heureFin = c.HeureFin,
                    idClasse = c.IdClasse,
                    idSalle = c.IdSalle,
                    idEnseignant = c.IdEnseignant,
                    idMatiere = c.IdMatiere,
                    derogationExceptionnelle = c.DerogationExceptionnelle,

                    classe = new
                    {
                        nomClasse = c.Classe!.NomClasse,
                        parcours = c.Classe.Parcours.ToString(),
                        idNiveau = c.Classe.IdNiveau,
                        nomNiveau = c.Classe.Niveau!.NomNiveau
                    },

                    salle = new
                    {
                        nomSalle = c.Salle!.NomSalle
                    },

                    matiere = new
                    {
                        nomMatiere = c.Matiere!.NomMatiere
                    },

                    enseignant = new
                    {
                        nom = c.Enseignant!.Nom
                    }
                })
                .ToListAsync();

            return Ok(cours);
        }


        // GET : api/admin/cours/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCoursParId(int id)
        {
            var cours = await _context.Cours.FindAsync(id);

            if (cours == null)
            {
                return NotFound(new
                {
                    message = "Cours introuvable"
                });
            }

            return Ok(cours);
        }


        // PUT : api/admin/cours/5
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifierCours(int id, [FromBody] Cours coursModifie)
        {

            var coursExistant = await _context.Cours.FindAsync(id);

            if (coursExistant == null)
            {
                return NotFound(new
                {
                    message = "Cours introuvable"
                });
            }

            // On force l'id pour que la vérification de doublon s'exclue elle-même
            coursModifie.IdCours = id;

            var validation = await _validationService.ValiderAsync(coursModifie);

            if (!validation.EstValide)
            {
                return BadRequest(new
                {
                    message = validation.Erreurs
                });
            }

            coursExistant.IdClasse = coursModifie.IdClasse;
            coursExistant.IdSalle = coursModifie.IdSalle;
            coursExistant.IdEnseignant = coursModifie.IdEnseignant;
            coursExistant.IdMatiere = coursModifie.IdMatiere;
            coursExistant.Jour = coursModifie.Jour;
            coursExistant.HeureDebut = coursModifie.HeureDebut;
            coursExistant.HeureFin = coursModifie.HeureFin;
            coursExistant.DerogationExceptionnelle = coursModifie.DerogationExceptionnelle;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cours modifié avec succès"
            });
        }


        // DELETE : api/admin/cours/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> SupprimerCours(int id)
        {

            var cours = await _context.Cours.FindAsync(id);

            if (cours == null)
            {
                return NotFound(new
                {
                    message = "Cours introuvable"
                });
            }

            _context.Cours.Remove(cours);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cours supprimé avec succès"
            });
        }

    }
}