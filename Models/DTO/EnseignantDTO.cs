namespace Projet_Emit.Models.DTO
{
    public class EnseignantDTO
    {
        public int IdUtilisateur { get; set; }

        public string Nom { get; set; } = "";

        public string Prenom { get; set; } = "";

        public int PlafondHeuresJournalieres { get; set; }
    }
}