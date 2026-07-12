using EMIT.Models;

namespace Projet_Emit.Models.DTO
{
    public class ClasseDetailDTO
    {
        public int IdClasse { get; set; }

        public string NomClasse { get; set; } = "";

        public int IdNiveau { get; set; }

        public int Effectif { get; set; }

        public ParcoursType Parcours { get; set; }
    }
}