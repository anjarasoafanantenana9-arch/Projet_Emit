using EMIT.Data;
using EMIT.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projet_Emit.Models.DTO;

namespace EMIT.Controllers.Api
{
    [Route("api/admin/enseignants")]
    [ApiController]
    public class AdminEnseignantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Utilisateur> _passwordHasher = new();


        public AdminEnseignantsController(ApplicationDbContext context)
        {
            _context = context;
        }



        // GET : api/admin/enseignants
        // GET : api/admin/enseignants?idMatiere=3  (filtre les enseignants habilités pour cette matière)
        [HttpGet]
        public async Task<IActionResult> GetEnseignants([FromQuery] int? idMatiere = null)
        {
            var query = _context.Enseignants
                .Include(e => e.Matieres)
                .AsQueryable();

            // Filtre utilisé par le formulaire de planification : quand on choisit une matière,
            // on ne propose que les enseignants habilités à la donner
            if (idMatiere.HasValue)
            {
                query = query.Where(e => e.Matieres.Any(m => m.IdMatiere == idMatiere.Value));
            }

            var enseignants = await query
                .Select(e => new EnseignantDTO
                {
                    IdUtilisateur = e.IdUtilisateur,

                    Nom = e.Nom,

                    Prenom = e.Prenom,

                    Email = e.Email,

                    PlafondHeuresJournalieres =
                        e.PlafondHeuresJournalieres,

                    IdMatieres = e.Matieres.Select(m => m.IdMatiere).ToList(),

                    NomsMatieres = e.Matieres.Select(m => m.NomMatiere).ToList()
                })
                .ToListAsync();


            return Ok(enseignants);
        }




        // GET : api/admin/enseignants/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Enseignant>> GetEnseignant(int id)
        {

            var enseignant = await _context.Enseignants
                .Include(e => e.Cours)
                .Include(e => e.Matieres)
                .FirstOrDefaultAsync(e => e.IdUtilisateur == id);



            if(enseignant == null)
            {
                return NotFound(new
                {
                    message="Enseignant introuvable"
                });
            }


            return Ok(enseignant);
        }







        // POST : api/admin/enseignants
        [HttpPost]
        public async Task<IActionResult> AjouterEnseignant(
            EnseignantDTO dto)
        {

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(new { message = "L'email est obligatoire." });
            }

            if (string.IsNullOrWhiteSpace(dto.MotDePasse))
            {
                return BadRequest(new { message = "Le mot de passe est obligatoire à la création." });
            }

            bool emailExiste = await _context.Utilisateurs
                .AnyAsync(u => u.Email == dto.Email);

            if (emailExiste)
            {
                return BadRequest(new { message = "Cet email est déjà utilisé." });
            }


            var enseignant = new Enseignant
            {
                Nom = dto.Nom,

                Prenom = dto.Prenom,

                Email = dto.Email,

                PlafondHeuresJournalieres =
                    dto.PlafondHeuresJournalieres,

                Role = RoleUtilisateur.Enseignant
            };

            enseignant.MotDePasseHash = _passwordHasher.HashPassword(enseignant, dto.MotDePasse);


            // Assignation des matières choisies
            if (dto.IdMatieres != null && dto.IdMatieres.Any())
            {
                var matieres = await _context.Matieres
                    .Where(m => dto.IdMatieres.Contains(m.IdMatiere))
                    .ToListAsync();

                foreach (var matiere in matieres)
                {
                    enseignant.Matieres.Add(matiere);
                }
            }


            _context.Enseignants.Add(enseignant);


            await _context.SaveChangesAsync();



            return Ok(new
            {
                message = "Enseignant ajouté avec succès"
            });

        }








        // PUT : api/admin/enseignants/1
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifierEnseignant(
            int id,
            EnseignantDTO dto)
        {

            var enseignant = await _context.Enseignants
                .Include(e => e.Matieres)
                .FirstOrDefaultAsync(e => e.IdUtilisateur == id);



            if (enseignant == null)
            {
                return NotFound(new
                {
                    message = "Enseignant introuvable"
                });
            }



            enseignant.Nom = dto.Nom;

            enseignant.Prenom = dto.Prenom;

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != enseignant.Email)
            {
                bool emailExiste = await _context.Utilisateurs
                    .AnyAsync(u => u.Email == dto.Email && u.IdUtilisateur != id);

                if (emailExiste)
                {
                    return BadRequest(new { message = "Cet email est déjà utilisé." });
                }

                enseignant.Email = dto.Email;
            }

            // Mot de passe changé uniquement si un nouveau a été saisi
            if (!string.IsNullOrWhiteSpace(dto.MotDePasse))
            {
                enseignant.MotDePasseHash = _passwordHasher.HashPassword(enseignant, dto.MotDePasse);
            }

            enseignant.PlafondHeuresJournalieres =
                dto.PlafondHeuresJournalieres;


            // Remplace la liste des matières assignées par la nouvelle sélection
            enseignant.Matieres.Clear();

            if (dto.IdMatieres != null && dto.IdMatieres.Any())
            {
                var matieres = await _context.Matieres
                    .Where(m => dto.IdMatieres.Contains(m.IdMatiere))
                    .ToListAsync();

                foreach (var matiere in matieres)
                {
                    enseignant.Matieres.Add(matiere);
                }
            }


            await _context.SaveChangesAsync();



            return Ok(new
            {
                message = "Enseignant modifié avec succès"
            });
        }








        // DELETE : api/admin/enseignants/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> SupprimerEnseignant(int id)
        {

            var enseignant = await _context.Enseignants
                .Include(e => e.Cours)
                .FirstOrDefaultAsync(e => e.IdUtilisateur == id);



            if(enseignant == null)
            {
                return NotFound(new
                {
                    message="Enseignant introuvable"
                });
            }




            if(enseignant.Cours.Any())
            {
                return BadRequest(new
                {
                    message="Impossible de supprimer : cet enseignant possède des cours."
                });
            }



            _context.Enseignants.Remove(enseignant);


            await _context.SaveChangesAsync();



            return Ok(new
            {
                message="Enseignant supprimé avec succès"
            });
        }
    }
}