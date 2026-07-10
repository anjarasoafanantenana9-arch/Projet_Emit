using System.ComponentModel.DataAnnotations;

namespace EMIT.Models
{
    public class Matiere
    {
        [Key]
        public int IdMatiere { get; set; }

        [Required(ErrorMessage = "Le nom de la matière est obligatoire.")]
        [MaxLength(100)]
        public string NomMatiere { get; set; } = string.Empty;

        public ICollection<Cours> Cours { get; set; } = new List<Cours>();
    }
}
