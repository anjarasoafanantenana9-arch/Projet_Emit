using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMIT.Models
{
    // RG01 : chaque personne enregistrée est un Utilisateur avec un rôle unique.
    // Table mère unique : centralise la connexion (Admin/Enseignant/Étudiant).
    public class Utilisateur
    {
        [Key]
        public int IdUtilisateur { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        [MaxLength(100)]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Format d'email invalide.")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string MotDePasseHash { get; set; } = string.Empty;

        [Required]
        public RoleUtilisateur Role { get; set; }

        // RG04 : un étudiant appartient obligatoirement à une et une seule classe.
        // Champ utilisé (et obligatoire) uniquement lorsque Role == Etudiant.
        public int? IdClasse { get; set; }

        [ForeignKey(nameof(IdClasse))]
        public Classe? Classe { get; set; }
    }
}
