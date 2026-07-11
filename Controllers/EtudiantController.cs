using System.Security.Claims;
using EMIT.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Controllers
{
    [Authorize(Roles = "Etudiant")]
    public class EtudiantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EtudiantController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Etudiant
        public async Task<IActionResult> Index()
        {
            int idUtilisateur = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var etudiant = await _context.Utilisateurs
                .Include(u => u.Classe)
                    .ThenInclude(c => c!.Niveau)
                .FirstOrDefaultAsync(u => u.IdUtilisateur == idUtilisateur);

            if (etudiant == null) return NotFound();

            return View(etudiant);
        }
    }
}
