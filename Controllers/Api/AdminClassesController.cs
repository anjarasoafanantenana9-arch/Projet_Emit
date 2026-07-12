using EMIT.Data;
using EMIT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projet_Emit.Models.DTO;


namespace EMIT.Controllers.Api
{
    [Route("api/admin/classes")]
    [ApiController]
    public class AdminClassesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;


        public AdminClassesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetClasses()
        {
            var classes = await _context.Classes
                .Include(c => c.Niveau)
                .Select(c => new ClasseListeDTO
                {
                    IdClasse = c.IdClasse,

                    NomClasse = c.NomClasse,

                    Niveau = c.Niveau != null
                        ? c.Niveau.NomNiveau
                        : "Aucun niveau",

                    // AJOUTÉ : nécessaire pour le filtrage par parcours et le regroupement par niveau
                    // dans la page Emploi du Temps
                    IdNiveau = c.IdNiveau,

                    NomNiveau = c.Niveau != null
                        ? c.Niveau.NomNiveau
                        : "",

                    Parcours = c.Parcours.ToString(),

                    Effectif = c.Effectif
                })
                .ToListAsync();


            return Ok(classes);
        }




        // GET : api/admin/classes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClasse(int id)
        {

            var classe = await _context.Classes
                .Where(c => c.IdClasse == id)
                .Select(c => new ClasseDetailDTO
                {
                    IdClasse = c.IdClasse,

                    NomClasse = c.NomClasse,

                    IdNiveau = c.IdNiveau,

                    Effectif = c.Effectif,

                    Parcours = c.Parcours
                })
                .FirstOrDefaultAsync();



            if(classe == null)
            {
                return NotFound(new
                {
                    message="Classe introuvable"
                });
            }



            return Ok(classe);
        }





        // POST : api/admin/classes
        [HttpPost]
        public async Task<IActionResult> AjouterClasse(ClasseDTO dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var niveauExiste = await _context.Niveaux
                .AnyAsync(n => n.IdNiveau == dto.IdNiveau);


            if(!niveauExiste)
            {
                return BadRequest(new
                {
                    message="Le niveau indiqué n'existe pas."
                });
            }



            var classe = new Classe
            {
                NomClasse = dto.NomClasse,

                IdNiveau = dto.IdNiveau,

                // AJOUTÉ : sans cette ligne, le parcours choisi dans le formulaire
                // n'était jamais sauvegardé (la classe restait sur la valeur par défaut AES)
                Parcours = dto.Parcours,

                Effectif = dto.Effectif
            };



            _context.Classes.Add(classe);


            await _context.SaveChangesAsync();



            return Ok(new
            {
                message="Classe ajoutée avec succès"
            });
        }





        // PUT : api/admin/classes/1
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifierClasse(
            int id,
            ClasseDTO dto)
        {

            var classeExistante = await _context.Classes
                .FindAsync(id);



            if(classeExistante == null)
            {
                return NotFound(new
                {
                    message="Classe introuvable"
                });
            }



            var niveauExiste = await _context.Niveaux
                .AnyAsync(n => n.IdNiveau == dto.IdNiveau);



            if(!niveauExiste)
            {
                return BadRequest(new
                {
                    message="Le niveau indiqué n'existe pas."
                });
            }




            classeExistante.NomClasse = dto.NomClasse;

            classeExistante.IdNiveau = dto.IdNiveau;

            // AJOUTÉ : même souci que pour AjouterClasse, la modification du parcours
            // n'était jamais persistée
            classeExistante.Parcours = dto.Parcours;

            classeExistante.Effectif = dto.Effectif;



            await _context.SaveChangesAsync();



            return Ok(new
            {
                message="Classe modifiée avec succès"
            });
        }







        // DELETE : api/admin/classes/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> SupprimerClasse(int id)
        {

            var classe = await _context.Classes
                .Include(c => c.Etudiants)
                .Include(c => c.Cours)
                .FirstOrDefaultAsync(c => c.IdClasse == id);



            if(classe == null)
            {
                return NotFound(new
                {
                    message="Classe introuvable"
                });
            }



            if(classe.Etudiants.Any())
            {
                return BadRequest(new
                {
                    message="Impossible de supprimer : cette classe possède des étudiants."
                });
            }



            if(classe.Cours.Any())
            {
                return BadRequest(new
                {
                    message="Impossible de supprimer : cette classe possède des cours."
                });
            }




            _context.Classes.Remove(classe);

            await _context.SaveChangesAsync();



            return Ok(new
            {
                message="Classe supprimée avec succès"
            });
        }
    }
}