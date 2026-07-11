using EMIT.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMIT.Models;

namespace EMIT.Controllers.Api
{
    [Route("api/admin/dashboard")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public AdminDashboardController(
            ApplicationDbContext context)
        {
            _context = context;
        }




        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {

            var data = new
            {

                enseignants =
                    await _context.Enseignants.CountAsync(),


                classes =
                    await _context.Classes.CountAsync(),


                salles =
                    await _context.Salles.CountAsync(),


                matieres =
                    await _context.Matieres.CountAsync(),


                cours =
                    await _context.Cours.CountAsync(),


                etudiants =
                    await _context.Utilisateurs
                    .CountAsync(u => u.Role == RoleUtilisateur.Etudiant)

            };


            return Ok(data);

        }

    }
}