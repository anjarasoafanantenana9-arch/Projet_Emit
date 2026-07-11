using System.ComponentModel.DataAnnotations;
using EMIT.Models;

namespace Projet_Emit.Models.DTO
{
    public class ClasseDTO
    {
        [Required(ErrorMessage = "Le nom de la classe est obligatoire.")]
        [MaxLength(50)]
        public string NomClasse { get; set; } = "";

        [Required(ErrorMessage = "Le niveau est obligatoire.")]
        public int IdNiveau { get; set; }

        // AJOUTÉ : sans ce champ, le parcours n'était jamais enregistré à la création/modification
        [Required(ErrorMessage = "Le parcours est obligatoire.")]
        public ParcoursType Parcours { get; set; }

        [Range(1, 500, ErrorMessage = "L'effectif doit être supérieur à 0.")]
        public int Effectif { get; set; }
    }
}