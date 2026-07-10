using System.ComponentModel.DataAnnotations;

namespace EMIT.Models
{
    // RG05 : une classe appartient obligatoirement à un niveau d'études précis (L1, L2, M1...)
    public class Niveau
    {
        [Key]
        public int IdNiveau { get; set; }

        [Required(ErrorMessage = "Le nom du niveau est obligatoire.")]
        [MaxLength(20)]
        public string NomNiveau { get; set; } = string.Empty;

        public ICollection<Classe> Classes { get; set; } = new List<Classe>();
    }
}
