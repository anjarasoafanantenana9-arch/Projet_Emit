using EMIT.Data;
using EMIT.Models;
using Microsoft.EntityFrameworkCore;

namespace EMIT.Services
{
    public class CoursValidationResult
    {
        public bool EstValide => Erreurs.Count == 0;
        public List<string> Erreurs { get; } = new();
    }

    public interface ICoursValidationService
    {
        Task<CoursValidationResult> ValiderAsync(Cours cours);
    }

    // Centralise les règles de gestion RG09 à RG14 du cahier des charges EMIT.
    public class CoursValidationService : ICoursValidationService
    {
        private readonly ApplicationDbContext _context;
        private const int PlafondHeuresParDefaut = 6;

        public CoursValidationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CoursValidationResult> ValiderAsync(Cours cours)
        {
            var result = new CoursValidationResult();

            // RG11 : l'heure de début doit être strictement inférieure à l'heure de fin
            if (cours.HeureDebut >= cours.HeureFin)
            {
                result.Erreurs.Add("L'heure de début doit être strictement inférieure à l'heure de fin.");
                return result; // inutile de continuer si le créneau lui-même est invalide
            }

            // RG12 : cours autorisés du lundi au vendredi, mercredi après-midi bloqué par défaut
            if (cours.Jour == JourSemaine.Mercredi
                && cours.HeureDebut >= new TimeSpan(12, 0, 0)
                && !cours.DerogationExceptionnelle)
            {
                result.Erreurs.Add("Le mercredi après-midi est bloqué par défaut. Cochez la dérogation exceptionnelle si autorisé.");
            }

            // RG09 : l'effectif de la classe ne doit pas dépasser la capacité de la salle
            var salle = await _context.Salles.FindAsync(cours.IdSalle);
            var classe = await _context.Classes.FindAsync(cours.IdClasse);

            if (salle == null)
                result.Erreurs.Add("La salle sélectionnée est introuvable.");
            if (classe == null)
                result.Erreurs.Add("La classe sélectionnée est introuvable.");

            if (salle != null && classe != null && classe.Effectif > salle.Capacite)
            {
                result.Erreurs.Add(
                    $"La salle '{salle.NomSalle}' (capacité {salle.Capacite}) ne peut pas accueillir " +
                    $"la classe '{classe.NomClasse}' (effectif {classe.Effectif}).");
            }

            // RG14 : triple blocage des doublons sur tout créneau qui chevauche celui demandé
            var coursSurCreneau = await _context.Cours
                .Where(c => c.IdCours != cours.IdCours
                            && c.Jour == cours.Jour
                            && c.HeureDebut < cours.HeureFin
                            && cours.HeureDebut < c.HeureFin)
                .ToListAsync();

            if (coursSurCreneau.Any(c => c.IdSalle == cours.IdSalle))
                result.Erreurs.Add("Doublon détecté : cette salle est déjà occupée sur ce créneau.");

            if (coursSurCreneau.Any(c => c.IdEnseignant == cours.IdEnseignant))
                result.Erreurs.Add("Doublon détecté : cet enseignant dispense déjà un cours sur ce créneau.");

            if (coursSurCreneau.Any(c => c.IdClasse == cours.IdClasse))
                result.Erreurs.Add("Doublon détecté : cette classe assiste déjà à un cours sur ce créneau.");

            // RG13 : plafond d'heures de cours journalières pour l'enseignant
            var enseignant = await _context.Enseignants.FindAsync(cours.IdEnseignant);
            if (enseignant != null)
            {
                var autresCoursDuJour = await _context.Cours
                    .Where(c => c.IdCours != cours.IdCours
                                && c.IdEnseignant == cours.IdEnseignant
                                && c.Jour == cours.Jour)
                    .ToListAsync();

                double heuresExistantes = autresCoursDuJour.Sum(c => (c.HeureFin - c.HeureDebut).TotalHours);
                double heuresNouvelles = (cours.HeureFin - cours.HeureDebut).TotalHours;
                int plafond = enseignant.PlafondHeuresJournalieres > 0
                    ? enseignant.PlafondHeuresJournalieres
                    : PlafondHeuresParDefaut;

                if (heuresExistantes + heuresNouvelles > plafond)
                {
                    result.Erreurs.Add(
                        $"Le plafond de {plafond}h de cours par jour pour cet enseignant serait dépassé " +
                        $"({heuresExistantes + heuresNouvelles:0.##}h au total).");
                }
            }

            return result;
        }
    }
}
