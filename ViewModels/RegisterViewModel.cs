using System.ComponentModel.DataAnnotations;
using EMIT.Models;

namespace EMIT.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [MaxLength(100)]
        [Display(Name = "Nom")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        [MaxLength(100)]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string MotDePasse { get; set; } = string.Empty;

        [Required(ErrorMessage = "Veuillez confirmer le mot de passe.")]
        [DataType(DataType.Password)]
        [Compare(nameof(MotDePasse), ErrorMessage = "Les mots de passe ne correspondent pas.")]
        [Display(Name = "Confirmer le mot de passe")]
        public string ConfirmationMotDePasse { get; set; } = string.Empty;

        [Required(ErrorMessage = "Veuillez sélectionner un rôle.")]
        [Display(Name = "Rôle")]
        public RoleUtilisateur Role { get; set; }

        // Requis uniquement si Role == Etudiant (RG04)
        [Display(Name = "Classe")]
        public int? IdClasse { get; set; }

        // Utilisé uniquement si Role == Enseignant (RG13)
        [Range(1, 12, ErrorMessage = "Le plafond doit être compris entre 1 et 12 heures.")]
        [Display(Name = "Plafond d'heures journalières")]
        public int? PlafondHeuresJournalieres { get; set; }
    }
}
