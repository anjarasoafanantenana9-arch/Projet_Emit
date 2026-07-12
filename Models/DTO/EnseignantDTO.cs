namespace Projet_Emit.Models.DTO
{
    public class EnseignantDTO
    {
        public int IdUtilisateur { get; set; }

        public string Nom { get; set; } = "";

        public string Prenom { get; set; } = "";

        public string Email { get; set; } = "";

        // Requis à la création ; laisser vide en modification pour ne pas changer le mot de passe
        public string? MotDePasse { get; set; }

        public int PlafondHeuresJournalieres { get; set; } = 6;

        public List<int> IdMatieres { get; set; } = new();

        public List<string>? NomsMatieres { get; set; }
    }
}