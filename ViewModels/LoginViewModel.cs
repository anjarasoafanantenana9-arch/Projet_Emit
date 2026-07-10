using System.ComponentModel.DataAnnotations;

namespace EMIT.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string MotDePasse { get; set; } = string.Empty;

        [Display(Name = "Se souvenir de moi")]
        public bool SeSouvenirDeMoi { get; set; }
    }
}
