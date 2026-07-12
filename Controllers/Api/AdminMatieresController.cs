using EMIT.Data;
using EMIT.Models;
using Microsoft.AspNetCore.Mvc;
using Projet_Emit.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Controllers.Api
{
    [Route("api/admin/matieres")]
    [ApiController]
    public class AdminMatieresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;


        public AdminMatieresController(ApplicationDbContext context)
        {
            _context = context;
        }



        // GET : api/admin/matieres
        [HttpGet]
public async Task<IActionResult> GetMatieres()
{
    var matieres = await _context.Matieres
        .Select(m => new MatiereDTO
        {
            IdMatiere = m.IdMatiere,
            NomMatiere = m.NomMatiere
        })
        .ToListAsync();

    return Ok(matieres);
}





        // GET : api/admin/matieres/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Matiere>> GetMatiere(int id)
        {

            var matiere = await _context.Matieres
    .Where(m => m.IdMatiere == id)
    .Select(m => new MatiereDTO
    {
        IdMatiere = m.IdMatiere,
        NomMatiere = m.NomMatiere
    })
    .FirstOrDefaultAsync();

if (matiere == null)
{
    return NotFound(new
    {
        message = "Matière introuvable"
    });
}

return Ok(matiere);



        }







        // POST : api/admin/matieres
       [HttpPost]
public async Task<IActionResult> AjouterMatiere(CreateMatiereDTO dto)
{

    if(!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }



    var matiere = new Matiere
    {
        NomMatiere = dto.NomMatiere
    };


    _context.Matieres.Add(matiere);


    await _context.SaveChangesAsync();



    return Ok(new
    {
        message="Matière ajoutée avec succès"
    });

}








        // PUT : api/admin/matieres/1
        [HttpPut("{id}")]
public async Task<IActionResult> ModifierMatiere(
    int id,
    CreateMatiereDTO dto)
{

    var matiereExistante =
        await _context.Matieres.FindAsync(id);



    if(matiereExistante == null)
    {
        return NotFound(new
        {
            message="Matière introuvable"
        });
    }



    matiereExistante.NomMatiere =
        dto.NomMatiere;



    await _context.SaveChangesAsync();



    return Ok(new
    {
        message="Matière modifiée avec succès"
    });

}








        // DELETE : api/admin/matieres/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> SupprimerMatiere(int id)
        {

            var matiere = await _context.Matieres
                .Include(m => m.Cours)
                .FirstOrDefaultAsync(m => m.IdMatiere == id);



            if(matiere == null)
            {
                return NotFound(new
                {
                    message="Matière introuvable"
                });
            }





            if(matiere.Cours.Any())
            {
                return BadRequest(new
                {
                    message="Impossible de supprimer : cette matière possède des cours."
                });
            }




            _context.Matieres.Remove(matiere);


            await _context.SaveChangesAsync();



            return Ok(new
            {
                message="Matière supprimée avec succès"
            });
        }
    }
}