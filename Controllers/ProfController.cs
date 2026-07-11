using System.Security.Claims;
using EMIT.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Controllers
{
    [Authorize(Roles = "Enseignant")]
    public class ProfController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Prof
        public async Task<IActionResult> Index()
        {
            int idUtilisateur = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // On charge l'Enseignant (et pas Utilisateur) pour avoir PlafondHeuresJournalieres
            var enseignant = await _context.Enseignants
                .FirstOrDefaultAsync(e => e.IdUtilisateur == idUtilisateur);

            if (enseignant == null) return NotFound();

            return View(enseignant);
        }
    }
}
