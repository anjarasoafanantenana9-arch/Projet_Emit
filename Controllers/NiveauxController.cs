using EMIT.Data;
using EMIT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Controllers
{
    [Authorize(Roles = "Administrateur")] // RG02 : seul l'Administrateur gère les niveaux
    public class NiveauxController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NiveauxController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Niveaux.OrderBy(n => n.NomNiveau).ToListAsync());
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomNiveau")] Niveau niveau)
        {
            if (ModelState.IsValid)
            {
                _context.Add(niveau);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(niveau);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var niveau = await _context.Niveaux.FindAsync(id);
            if (niveau == null) return NotFound();
            return View(niveau);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdNiveau,NomNiveau")] Niveau niveau)
        {
            if (id != niveau.IdNiveau) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(niveau);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Niveaux.Any(e => e.IdNiveau == niveau.IdNiveau)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(niveau);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var niveau = await _context.Niveaux.FirstOrDefaultAsync(m => m.IdNiveau == id);
            if (niveau == null) return NotFound();
            return View(niveau);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var niveau = await _context.Niveaux.FindAsync(id);
            if (niveau != null)
            {
                _context.Niveaux.Remove(niveau);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
