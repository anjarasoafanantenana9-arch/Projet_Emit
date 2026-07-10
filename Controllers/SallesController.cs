using EMIT.Data;
using EMIT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Controllers
{
    [Authorize(Roles = "Administrateur")] // RG02 : seul l'Administrateur gère les salles
    public class SallesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SallesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Salles.OrderBy(s => s.NomSalle).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var salle = await _context.Salles.FirstOrDefaultAsync(m => m.IdSalle == id);
            if (salle == null) return NotFound();
            return View(salle);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomSalle,Capacite")] Salle salle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salle);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var salle = await _context.Salles.FindAsync(id);
            if (salle == null) return NotFound();
            return View(salle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSalle,NomSalle,Capacite")] Salle salle)
        {
            if (id != salle.IdSalle) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Salles.Any(e => e.IdSalle == salle.IdSalle)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(salle);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var salle = await _context.Salles.FirstOrDefaultAsync(m => m.IdSalle == id);
            if (salle == null) return NotFound();
            return View(salle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salle = await _context.Salles.FindAsync(id);
            if (salle != null)
            {
                _context.Salles.Remove(salle);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
