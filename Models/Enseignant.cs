using System.ComponentModel.DataAnnotations;

namespace EMIT.Models
{
    // RG13 : plafond d'heures journalières
    public class Enseignant : Utilisateur
    {
        [Range(1, 12, ErrorMessage = "Le plafond doit être compris entre 1 et 12 heures.")]
        public int PlafondHeuresJournalieres { get; set; } = 6;

        public ICollection<Cours> Cours { get; set; } = new List<Cours>();
    }
}
