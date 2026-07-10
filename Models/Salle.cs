using System.ComponentModel.DataAnnotations;

namespace EMIT.Models
{
    public class Salle
    {
        [Key]
        public int IdSalle { get; set; }

        [Required(ErrorMessage = "Le nom de la salle est obligatoire.")]
        [MaxLength(50)]
        public string NomSalle { get; set; } = string.Empty;

        // RG09 : la capacité conditionne l'affectation d'une classe à la salle
        [Range(1, 1000, ErrorMessage = "La capacité doit être supérieure à 0.")]
        public int Capacite { get; set; }

        public ICollection<Cours> Cours { get; set; } = new List<Cours>();
    }
}
