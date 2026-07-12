const API = "/api/admin";
const API_COURS = `${API}/cours`;

let modeEdition = false;
let idCoursEnEdition = null;

let listeNiveaux = [];      // [{idNiveau, nomNiveau}, ...]
let toutesLesClasses = [];  // [{idClasse, nomClasse, parcours, idNiveau}, ...]
let toutLesEnseignants = []; // [{idUtilisateur, nom, prenom, idMatieres}, ...]

const JOURS_LABELS = ["Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi"];
const joursIndexMap = { "Lundi":0, "Mardi":1, "Mercredi":2, "Jeudi":3, "Vendredi":4 };

// ---------- Chargement des listes de base ----------

async function chargerMatieres(){
    let response = await fetch(`${API}/matieres`);
    let data = await response.json();
    let select = document.getElementById("idMat");
    data.forEach(m=>{
        select.innerHTML += `<option value="${m.idMatiere}">${m.nomMatiere}</option>`;
    });
}

// Charge la liste des niveaux (utilisée uniquement pour construire les grilles de planning,
// le formulaire n'a plus de champ Niveau : il est hérité de la classe choisie)
async function chargerNiveauxPourGrilles(){
    const response = await fetch(`${API}/niveaux`);
    listeNiveaux = await response.json();
}

async function chargerClasses(){
    const response = await fetch(`${API}/classes`);
    toutesLesClasses = await response.json();
    // toutesLesClasses attendu: [{idClasse, nomClasse, parcours, idNiveau, nomNiveau}, ...]
    filtrerClassesDuFormulaire();
}

// Filtre le select #idClasse selon le Parcours choisi dans le FORMULAIRE
// (le niveau n'est plus demandé ici : il est hérité automatiquement de la classe choisie)
function filtrerClassesDuFormulaire(){
    const parcours = document.getElementById("filtreParcoursForm").value;
    const select = document.getElementById("idClasse");

    if (!parcours) {
        select.innerHTML = `<option value="">-- Choisir d'abord un parcours --</option>`;
        return;
    }

    const classesFiltrees = toutesLesClasses.filter(c => c.parcours === parcours);

    if (classesFiltrees.length === 0) {
        select.innerHTML = `<option value="">-- Aucune classe pour ce parcours --</option>`;
        return;
    }

    select.innerHTML = `<option value="">-- Choisir une classe --</option>`;
    classesFiltrees.forEach(c => {
        const niveauLabel = c.nomNiveau ? ` (${c.nomNiveau})` : "";
        select.innerHTML += `<option value="${c.idClasse}">${c.nomClasse}${niveauLabel}</option>`;
    });
}

async function chargerEnseignants(){
    let response = await fetch(`${API}/enseignants`);
    let data = await response.json();
    toutLesEnseignants = data;
    remplirSelectEnseignants(data);
}

// Remplit le select #idProf avec la liste fournie (tous, ou filtrés par matière)
function remplirSelectEnseignants(liste){
    const select = document.getElementById("idProf");
    const valeurActuelle = select.value;

    if (liste.length === 0) {
        select.innerHTML = `<option value="">-- Aucun enseignant pour cette matière --</option>`;
        return;
    }

    select.innerHTML = `<option value="">-- Choisir un enseignant --</option>`;
    liste.forEach(e=>{
        select.innerHTML += `<option value="${e.idUtilisateur}">${e.nom} ${e.prenom}</option>`;
    });

    // Conserve la sélection précédente si elle est toujours valide dans la nouvelle liste
    if (liste.some(e => String(e.idUtilisateur) === valeurActuelle)) {
        select.value = valeurActuelle;
    }
}

// Quand la matière change dans le formulaire : ne propose que les enseignants habilités
function filtrerEnseignantsParMatiere(){
    const idMat = document.getElementById("idMat").value;

    if (!idMat) {
        remplirSelectEnseignants(toutLesEnseignants);
        return;
    }

    const enseignantsFiltres = toutLesEnseignants.filter(e =>
        e.idMatieres && e.idMatieres.includes(parseInt(idMat))
    );

    remplirSelectEnseignants(enseignantsFiltres);
}

async function chargerSalles(){
    let response = await fetch(`${API}/salles`);
    let data = await response.json();
    let select = document.getElementById("idSalle");
    data.forEach(s=>{
        select.innerHTML += `<option value="${s.idSalle}">${s.nomSalle}</option>`;
    });
}

// ---------- Construction dynamique d'une grille par niveau ----------

function construireGrillesVides(){
    const container = document.getElementById("planningsParNiveau");
    const parcours = document.getElementById("filtreParcours").value;

    if (!parcours) {
        container.innerHTML = `<div class="text-muted small">Sélectionnez un parcours pour afficher les emplois du temps.</div>`;
        return;
    }

    if (listeNiveaux.length === 0) {
        container.innerHTML = `<div class="text-muted small">Aucun niveau trouvé.</div>`;
        return;
    }

    let html = "";

    listeNiveaux.forEach(niveau => {
        html += `
        <div class="card-custom mb-3">
            <div class="card-title-custom m-0 mb-2">
                ${parcours} — ${niveau.nomNiveau}
            </div>
            <div class="table-responsive">
                <table class="table table-timetable m-0">
                    <thead>
                        <tr>
                            <th></th>
                            <th>Lundi</th>
                            <th>Mardi</th>
                            <th>Mercredi</th>
                            <th>Jeudi</th>
                            <th>Vendredi</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="periode-label">Matin</td>
                            <td id="day-${niveau.idNiveau}-0-matin"></td>
                            <td id="day-${niveau.idNiveau}-1-matin"></td>
                            <td id="day-${niveau.idNiveau}-2-matin"></td>
                            <td id="day-${niveau.idNiveau}-3-matin"></td>
                            <td id="day-${niveau.idNiveau}-4-matin"></td>
                        </tr>
                        <tr>
                            <td class="periode-label">Après-midi</td>
                            <td id="day-${niveau.idNiveau}-0-am"></td>
                            <td id="day-${niveau.idNiveau}-1-am"></td>
                            <td id="day-${niveau.idNiveau}-2-am" class="cell-bloquee">
                                <span class="cell-bloquee-texte">Bloqué<br>(sauf dérogation)</span>
                            </td>
                            <td id="day-${niveau.idNiveau}-3-am"></td>
                            <td id="day-${niveau.idNiveau}-4-am"></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>`;
    });

    container.innerHTML = html;
}

// ---------- Chargement et affichage du planning (réparti par niveau) ----------

async function chargerPlanning() {

    const parcours = document.getElementById("filtreParcours").value;

    // Reconstruit les grilles vides (une par niveau) à chaque actualisation
    construireGrillesVides();

    if (!parcours) {
        return; // rien à charger tant qu'aucun parcours n'est choisi
    }

    try {
        const response = await fetch(`${API_COURS}?parcours=${parcours}`);
        if (!response.ok) throw new Error("Erreur chargement cours");
        const cours = await response.json();

        cours.forEach(c => {

            const jourIndex = typeof c.jour === "number" ? c.jour : joursIndexMap[c.jour];
            const periode = c.heureDebut < "12:00:00" ? "matin" : "am";
            const idNiveauCours = c.classe.idNiveau;

            const idCible = `day-${idNiveauCours}-${jourIndex}-${periode}`;
            let cellule = document.getElementById(idCible);

            if (cellule) {
                cellule.innerHTML += `
                <div class="course-slot" id="cours-${c.idCours}">
                    <div class="course-time">
                        <i class="bi bi-clock"></i> ${c.heureDebut} - ${c.heureFin}
                    </div>
                    <strong>${c.matiere.nomMatiere}</strong><br>
                    Classe : ${c.classe.nomClasse}<br>
                    Prof : ${c.enseignant.nom}<br>
                    <span class="badge-salle">Salle ${c.salle.nomSalle}</span>
                    <div class="course-actions">
                        <button type="button" class="btn-action btn-edit" onclick='ouvrirEdition(${JSON.stringify(c)})' title="Modifier">
                            <i class="bi bi-pencil-square"></i>
                        </button>
                        <button type="button" class="btn-action btn-delete" onclick="supprimerCours(${c.idCours})" title="Supprimer">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </div>`;
            }
        });

    } catch (error) {
        console.error("Erreur chargement planning : ", error);
    }
}

// ---------- Édition ----------

function ouvrirEdition(c) {

    modeEdition = true;
    idCoursEnEdition = c.idCours;

    document.getElementById("jour").value = c.jour;
    document.getElementById("idMat").value = c.idMatiere;
    filtrerEnseignantsParMatiere();
    document.getElementById("heureD").value = c.heureDebut.substring(0, 5);
    document.getElementById("heureFin").value = c.heureFin.substring(0, 5);

    document.getElementById("filtreParcoursForm").value = c.classe.parcours;
    filtrerClassesDuFormulaire();

    document.getElementById("idClasse").value = c.idClasse;
    document.getElementById("idProf").value = c.idEnseignant;
    document.getElementById("idSalle").value = c.idSalle;
    document.getElementById("derogation").checked = c.derogationExceptionnelle;

    const btn = document.querySelector("#formPlanification button[type='submit']");
    btn.textContent = "Enregistrer les modifications";
    btn.classList.add("btn-mode-edition");

    document.getElementById("btnAnnulerEdition").classList.remove("d-none");

    document.getElementById("formPlanification").scrollIntoView({ behavior: "smooth" });
}

function annulerEdition() {
    modeEdition = false;
    idCoursEnEdition = null;

    document.getElementById("formPlanification").reset();
    filtrerClassesDuFormulaire();

    const btn = document.querySelector("#formPlanification button[type='submit']");
    btn.textContent = "Ajouter au planning";
    btn.classList.remove("btn-mode-edition");

    document.getElementById("btnAnnulerEdition").classList.add("d-none");
}

// ---------- Suppression ----------

async function supprimerCours(idCours) {

    if (!confirm("Voulez-vous vraiment supprimer ce cours ?")) {
        return;
    }

    const alertBox = document.getElementById("alertBox");

    try {
        const response = await fetch(`${API_COURS}/${idCours}`, {
            method: "DELETE"
        });

        const data = await response.json();

        if (response.ok) {
            alertBox.className = "alert alert-success";
            alertBox.innerHTML = "✅ " + data.message;
            chargerPlanning();
        } else {
            alertBox.className = "alert alert-danger";
            alertBox.innerHTML = "❌ " + (Array.isArray(data.message) ? data.message.join("<br>") : data.message);
        }

    } catch (error) {
        alertBox.className = "alert alert-danger";
        alertBox.innerHTML = "Erreur serveur";
        console.error(error);
    }

    alertBox.classList.remove("d-none");
}

// ---------- Initialisation ----------

window.onload = async function(){
    await chargerNiveauxPourGrilles();
    await chargerClasses();
    chargerMatieres();
    chargerEnseignants();
    chargerSalles();
    construireGrillesVides();
};

document.getElementById("filtreParcours")?.addEventListener("change", chargerPlanning);
document.getElementById("btnAnnulerEdition")?.addEventListener("click", annulerEdition);
document.getElementById("filtreParcoursForm")?.addEventListener("change", filtrerClassesDuFormulaire);
document.getElementById("idMat")?.addEventListener("change", filtrerEnseignantsParMatiere);

document.getElementById("formPlanification").addEventListener("submit", async function(e){
    e.preventDefault();

    const alertBox = document.getElementById("alertBox");

    const cours = {
        idClasse: parseInt(document.getElementById("idClasse").value),
        idSalle: parseInt(document.getElementById("idSalle").value),
        idEnseignant: parseInt(document.getElementById("idProf").value),
        idMatiere: parseInt(document.getElementById("idMat").value),
        jour: parseInt(document.getElementById("jour").value),
        heureDebut: document.getElementById("heureD").value + ":00",
        heureFin: document.getElementById("heureFin").value + ":00",
        derogationExceptionnelle: document.getElementById("derogation").checked
    };

    const estEdition = modeEdition && idCoursEnEdition !== null;

    if (estEdition) {
        cours.idCours = idCoursEnEdition;
    }

    try {
        let response = await fetch(
            estEdition ? `${API_COURS}/${idCoursEnEdition}` : API_COURS,
            {
                method: estEdition ? "PUT" : "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(cours)
            }
        );

        let data = await response.json();

        if (response.ok) {
            alertBox.className = "alert alert-success";
            alertBox.innerHTML = "✅ " + data.message;

            // Aligne automatiquement le filtre d'affichage sur le parcours du cours ajouté/modifié
            const parcoursForm = document.getElementById("filtreParcoursForm").value;
            document.getElementById("filtreParcours").value = parcoursForm;

            chargerPlanning();

            if (estEdition) {
                annulerEdition();
            } else {
                document.getElementById("formPlanification").reset();
                filtrerClassesDuFormulaire();
            }
        } else {
            alertBox.className = "alert alert-danger";
            alertBox.innerHTML = "❌ " + (Array.isArray(data.message) ? data.message.join("<br>") : data.message);
        }

    } catch (error) {
        alertBox.className = "alert alert-danger";
        alertBox.innerHTML = "Erreur serveur";
        console.log(error);
    }

    alertBox.classList.remove("d-none");
});