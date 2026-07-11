using Microsoft.AspNetCore.Mvc;
using EMIT.Data;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Controllers
{
    public class MatieresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MatieresController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var matieres = await _context.Matieres.ToListAsync();
            return View(matieres);
        }
    }
}