using EMIT.Models;

namespace EMIT.ViewModels
{
    public class SalleDisponibiliteViewModel
    {
        public int IdSalle { get; set; }
        public string NomSalle { get; set; } = string.Empty;
        public int Capacite { get; set; }

        // Cours en train de s'y dérouler à l'instant (null si la salle est libre)
        public Cours? CoursEnCours { get; set; }

        public bool EstLibre => CoursEnCours == null;
    }
}
