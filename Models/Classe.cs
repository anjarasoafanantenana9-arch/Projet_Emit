using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMIT.Models
{
    // RG05 : une classe appartient obligatoirement à un niveau d'études précis
    public class Classe
    {
        [Key]
        public int IdClasse { get; set; }

        [Required(ErrorMessage = "Le nom de la classe est obligatoire.")]
        [MaxLength(50)]
        public string NomClasse { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le niveau est obligatoire.")]
        public int IdNiveau { get; set; }

        [ForeignKey(nameof(IdNiveau))]
        public Niveau? Niveau { get; set; }

        [Range(1, 500, ErrorMessage = "L'effectif doit être supérieur à 0.")]
        public int Effectif { get; set; }

        // Utilisateurs ayant Role = Etudiant et IdClasse = cette classe
        public ICollection<Utilisateur> Etudiants { get; set; } = new List<Utilisateur>();
        public ICollection<Cours> Cours { get; set; } = new List<Cours>();
    }
}
