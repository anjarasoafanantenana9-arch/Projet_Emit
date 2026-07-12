using EMIT.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Controllers.Api
{
    [Route("api/admin/niveaux")]
    [ApiController]
    public class AdminNiveauController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminNiveauController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET : api/admin/niveaux
        [HttpGet]
        public async Task<IActionResult> GetNiveaux()
        {
            var niveaux = await _context.Niveaux
                .OrderBy(n => n.NomNiveau)
                .Select(n => new
                {
                    idNiveau = n.IdNiveau,
                    nomNiveau = n.NomNiveau
                })
                .ToListAsync();

            return Ok(niveaux);
        }
    }
}