using EMIT.Data;
using EMIT.Models;
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


        public AdminEnseignantsController(ApplicationDbContext context)
        {
            _context = context;
        }



        // GET : api/admin/enseignants
[HttpGet]
public async Task<IActionResult> GetEnseignants()
{
    var enseignants = await _context.Enseignants
        .Select(e => new EnseignantDTO
        {
            IdUtilisateur = e.IdUtilisateur,

            Nom = e.Nom,

            Prenom = e.Prenom,

            PlafondHeuresJournalieres = 
                e.PlafondHeuresJournalieres
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



    var enseignant = new Enseignant
    {
        Nom = dto.Nom,

        Prenom = dto.Prenom,

        PlafondHeuresJournalieres =
            dto.PlafondHeuresJournalieres,

        Role = RoleUtilisateur.Enseignant
    };



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

    enseignant.PlafondHeuresJournalieres =
        dto.PlafondHeuresJournalieres;



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