using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMIT.Models
{
    // RG12 : jours autorisés du lundi au vendredi
    public enum JourSemaine
    {
        Lundi,
        Mardi,
        Mercredi,
        Jeudi,
        Vendredi
    }

    // RG10 : chaque Cours centralise obligatoirement classe, salle, enseignant,
    // matière, jour, heure de début et heure de fin.
    public class Cours
    {
        [Key]
        public int IdCours { get; set; }

        [Required]
        public int IdClasse { get; set; }
        [ForeignKey(nameof(IdClasse))]
        public Classe? Classe { get; set; }

        [Required]
        public int IdSalle { get; set; }
        [ForeignKey(nameof(IdSalle))]
        public Salle? Salle { get; set; }

        [Required]
        public int IdEnseignant { get; set; }
        [ForeignKey(nameof(IdEnseignant))]
        public Enseignant? Enseignant { get; set; }

        [Required]
        public int IdMatiere { get; set; }
        [ForeignKey(nameof(IdMatiere))]
        public Matiere? Matiere { get; set; }

        [Required]
        public JourSemaine Jour { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan HeureDebut { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan HeureFin { get; set; }

        // RG12 : permet une dérogation exceptionnelle (ex: mercredi après-midi)
        public bool DerogationExceptionnelle { get; set; } = false;
    }
}
