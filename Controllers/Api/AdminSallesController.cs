using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMIT.Models;
using EMIT.Data;
using Projet_Emit.Models.DTO;

namespace EMIT.Controllers.Api
{
    [Route("api/admin/salles")]
    [ApiController]
    public class AdminSallesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;


        public AdminSallesController(ApplicationDbContext context)
        {
            _context = context;
        }



        // GET : api/admin/salles
        [HttpGet]
public async Task<IActionResult> GetSalles()
{
    var salles = await _context.Salles
        .Select(s => new SalleDTO
        {
            IdSalle = s.IdSalle,
            NomSalle = s.NomSalle,
            Capacite = s.Capacite
        })
        .ToListAsync();

    return Ok(salles);
}



        // GET : api/admin/salles/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Salle>> GetSalle(int id)
        {
            var salle = await _context.Salles
                .Include(s => s.Cours)
                .FirstOrDefaultAsync(s => s.IdSalle == id);


            if (salle == null)
            {
                return NotFound(new
                {
                    message = "Salle introuvable"
                });
            }


            return Ok(salle);
        }




        // POST : api/admin/salles
        [HttpPost]
        public async Task<ActionResult<Salle>> AjouterSalle(Salle salle)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Salles.Add(salle);

            await _context.SaveChangesAsync();



            return Ok(new
            {
                message = "Salle ajoutée avec succès",
                salle
            });
        }





        // PUT : api/admin/salles/1
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifierSalle(
            int id,
            Salle salle)
        {

            if(id != salle.IdSalle)
            {
                return BadRequest(new
                {
                    message = "L'identifiant ne correspond pas"
                });
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var salleExistante = await _context.Salles
                .FindAsync(id);


            if(salleExistante == null)
            {
                return NotFound(new
                {
                    message="Salle introuvable"
                });
            }



            salleExistante.NomSalle = salle.NomSalle;
            salleExistante.Capacite = salle.Capacite;



            await _context.SaveChangesAsync();



            return Ok(new
            {
                message="Salle modifiée avec succès"
            });
        }







        // DELETE : api/admin/salles/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> SupprimerSalle(int id)
        {

            var salle = await _context.Salles
                .Include(s => s.Cours)
                .FirstOrDefaultAsync(s => s.IdSalle == id);



            if(salle == null)
            {
                return NotFound(new
                {
                    message="Salle introuvable"
                });
            }



            // RG : empêcher suppression si salle utilisée
            if(salle.Cours.Any())
            {
                return BadRequest(new
                {
                    message="Impossible de supprimer cette salle car elle est utilisée dans un cours."
                });
            }



            _context.Salles.Remove(salle);


            await _context.SaveChangesAsync();



            return Ok(new
            {
                message="Salle supprimée avec succès"
            });
        }
    }
}