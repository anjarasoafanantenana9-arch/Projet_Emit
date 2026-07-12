using System.ComponentModel.DataAnnotations;

namespace EMIT.Models
{
    // RG13 : plafond d'heures journalières
    public class Enseignant : Utilisateur
    {
        [Range(1, 12, ErrorMessage = "Le plafond doit être compris entre 1 et 12 heures.")]
        public int PlafondHeuresJournalieres { get; set; } = 6;

        public ICollection<Cours> Cours { get; set; } = new List<Cours>();

        // Un enseignant peut enseigner plusieurs matières (et une matière peut être enseignée
        // par plusieurs enseignants) -> relation plusieurs-à-plusieurs
        public ICollection<Matiere> Matieres { get; set; } = new List<Matiere>();
    }
}