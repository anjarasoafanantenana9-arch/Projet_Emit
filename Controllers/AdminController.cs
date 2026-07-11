
using Microsoft.AspNetCore.Mvc;

namespace EMIT.Controllers
{
    public class AdminController : Controller
    {
        // Dashboard
        public IActionResult Index()
        {
            return View();
        }

        // Emploi du temps
        public IActionResult EmploiTemps()
        {
            return View();
        }

        // Gestion des salles
        public IActionResult Salles()
        {
            return View();
        }

        // Gestion des enseignants
        public IActionResult Enseignants()
        {
            return View();
        }

        // Gestion des classes
        public IActionResult Classes()
        {
            return View();
        }

        // Gestion des matières
        public IActionResult Matieres()
        {
            return View();
        }
    }
}
