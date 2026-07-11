using System.Security.Claims;
using EMIT.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Controllers
{
    [Authorize(Roles = "Administrateur")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            int idUtilisateur = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var admin = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.IdUtilisateur == idUtilisateur);

            if (admin == null) return NotFound();

            return View(admin);
        }
    }
}
