namespace Projet_Emit.Models.DTO
{
    public class ClasseListeDTO
    {
        public int IdClasse { get; set; }

        public string NomClasse { get; set; } = "";

        // Gardé pour compatibilité avec le reste de l'app qui utilise déjà "niveau"
        public string Niveau { get; set; } = "";

        // Nouveaux champs nécessaires pour le planning (filtrage par parcours + regroupement par niveau)
        public int IdNiveau { get; set; }

        public string NomNiveau { get; set; } = "";

        public string Parcours { get; set; } = "";

        public int Effectif { get; set; }
    }
}