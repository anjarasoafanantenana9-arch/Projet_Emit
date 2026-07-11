using System.ComponentModel.DataAnnotations;

namespace EMIT.ViewModels
{
    public class ChangerMotDePasseViewModel
    {
        [Required(ErrorMessage = "Le mot de passe actuel est obligatoire.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe actuel")]
        public string MotDePasseActuel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nouveau mot de passe est obligatoire.")]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe")]
        public string NouveauMotDePasse { get; set; } = string.Empty;

        [Required(ErrorMessage = "Veuillez confirmer le nouveau mot de passe.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NouveauMotDePasse), ErrorMessage = "Les mots de passe ne correspondent pas.")]
        [Display(Name = "Confirmer le nouveau mot de passe")]
        public string ConfirmationMotDePasse { get; set; } = string.Empty;
    }
}
