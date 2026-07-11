namespace Projet_Emit.Models.DTO
{
    public class CoursDTO
    {
        public int IdCours { get; set; }

        public int Jour { get; set; }

        public TimeSpan HeureDebut { get; set; }

        public TimeSpan HeureFin { get; set; }

        public string Classe { get; set; } = string.Empty;

        public string Salle { get; set; } = string.Empty;

        public string Matiere { get; set; } = string.Empty;

        public string Enseignant { get; set; } = string.Empty;
    }
}