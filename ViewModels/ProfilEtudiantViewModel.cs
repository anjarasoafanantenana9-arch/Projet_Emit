using System.ComponentModel.DataAnnotations;

namespace EMIT.ViewModels
{
    public class ProfilEtudiantViewModel
    {
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        [MaxLength(100)]
        public string Prenom { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date de naissance est obligatoire.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date de naissance")]
        public DateTime? DateNaissance { get; set; }

        [Required(ErrorMessage = "Le numéro d'inscription est obligatoire.")]
        [MaxLength(30)]
        [Display(Name = "Numéro d'inscription")]
        public string NumeroInscription { get; set; } = string.Empty;

        [Required(ErrorMessage = "La mention est obligatoire.")]
        [MaxLength(100)]
        [Display(Name = "Mention")]
        public string Mention { get; set; } = string.Empty;

        [Required(ErrorMessage = "La classe est obligatoire.")]
        [Display(Name = "Classe")]
        public int? IdClasse { get; set; }

        // Lecture seule, affiché en haut du formulaire
        public string Email { get; set; } = string.Empty;
    }
}
