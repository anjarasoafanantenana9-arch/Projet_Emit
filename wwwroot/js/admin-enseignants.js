const API = "/api/admin";

let modeEditionEnseignant = false;
let idEnseignantEnEdition = null;

// ---------- Chargement des matières pour le select du formulaire ----------

async function chargerMatieresFormulaire(){
    const response = await fetch(`${API}/matieres`);
    const matieres = await response.json();

    const select = document.getElementById("idMatieres");
    select.innerHTML = "";
    matieres.forEach(m => {
        select.innerHTML += `<option value="${m.idMatiere}">${m.nomMatiere}</option>`;
    });
}

// ---------- Chargement et affichage du tableau des enseignants ----------

async function chargerEnseignants(){

    const response = await fetch(`${API}/enseignants`);
    const enseignants = await response.json();

    const tbody = document.getElementById("tableEnseignants");
    tbody.innerHTML = "";

    enseignants.forEach(e => {

        const matieresTexte = (e.nomsMatieres && e.nomsMatieres.length > 0)
            ? e.nomsMatieres.join(", ")
            : "<span class='text-muted'>Aucune</span>";

        tbody.innerHTML += `
        <tr>
            <td>${e.idUtilisateur}</td>
            <td>${e.nom}</td>
            <td>${e.prenom}</td>
            <td>${matieresTexte}</td>
            <td>${e.plafondHeuresJournalieres}</td>
            <td>
                <button type="button" class="btn btn-sm btn-outline-primary" onclick='ouvrirEditionEnseignant(${JSON.stringify(e)})'>
                    <i class="bi bi-pencil-square"></i>
                </button>
                <button type="button" class="btn btn-sm btn-outline-danger" onclick="supprimerEnseignant(${e.idUtilisateur})">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        </tr>`;
    });
}

// ---------- Édition ----------

function ouvrirEditionEnseignant(e){

    modeEditionEnseignant = true;
    idEnseignantEnEdition = e.idUtilisateur;

    document.getElementById("idUtilisateur").value = e.idUtilisateur;
    document.getElementById("nom").value = e.nom;
    document.getElementById("prenom").value = e.prenom;
    document.getElementById("email").value = e.email ?? "";
    document.getElementById("motDePasse").value = "";
    document.getElementById("motDePasse").placeholder = "Laisser vide pour ne pas changer";
    document.getElementById("hintMotDePasse").classList.remove("d-none");
    document.getElementById("plafond").value = e.plafondHeuresJournalieres;

    // Coche les matières déjà assignées dans le select multiple
    const select = document.getElementById("idMatieres");
    Array.from(select.options).forEach(option => {
        option.selected = e.idMatieres.includes(parseInt(option.value));
    });

    document.getElementById("titreFormulaireEnseignant").textContent = "Modifier l'enseignant";
    document.getElementById("btnSubmitEnseignant").textContent = "Enregistrer les modifications";
    document.getElementById("btnAnnulerEditionEnseignant").classList.remove("d-none");

    document.getElementById("formEnseignant").scrollIntoView({ behavior: "smooth" });
}

function annulerEditionEnseignant(){
    modeEditionEnseignant = false;
    idEnseignantEnEdition = null;

    document.getElementById("formEnseignant").reset();
    document.getElementById("idUtilisateur").value = "";
    document.getElementById("motDePasse").placeholder = "Requis à la création";
    document.getElementById("hintMotDePasse").classList.add("d-none");

    Array.from(document.getElementById("idMatieres").options).forEach(o => o.selected = false);

    document.getElementById("titreFormulaireEnseignant").textContent = "Ajouter un enseignant";
    document.getElementById("btnSubmitEnseignant").textContent = "Enregistrer";
    document.getElementById("btnAnnulerEditionEnseignant").classList.add("d-none");
}

// ---------- Suppression ----------

async function supprimerEnseignant(id){

    if (!confirm("Voulez-vous vraiment supprimer cet enseignant ?")) {
        return;
    }

    try {
        const response = await fetch(`${API}/enseignants/${id}`, {
            method: "DELETE"
        });

        const data = await response.json();

        if (response.ok) {
            alert("✅ " + data.message);
            chargerEnseignants();
        } else {
            alert("❌ " + data.message);
        }

    } catch (error) {
        alert("Erreur serveur");
        console.error(error);
    }
}

// ---------- Soumission du formulaire (création ou modification) ----------

document.getElementById("formEnseignant").addEventListener("submit", async function(e){
    e.preventDefault();

    const idMatieresSelectionnees = Array.from(document.getElementById("idMatieres").selectedOptions)
        .map(option => parseInt(option.value));

    const dto = {
        nom: document.getElementById("nom").value,
        prenom: document.getElementById("prenom").value,
        email: document.getElementById("email").value,
        motDePasse: document.getElementById("motDePasse").value || null,
        plafondHeuresJournalieres: parseInt(document.getElementById("plafond").value),
        idMatieres: idMatieresSelectionnees
    };

    const estEdition = modeEditionEnseignant && idEnseignantEnEdition !== null;

    try {
        const response = await fetch(
            estEdition ? `${API}/enseignants/${idEnseignantEnEdition}` : `${API}/enseignants`,
            {
                method: estEdition ? "PUT" : "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(dto)
            }
        );

        const data = await response.json();

        if (response.ok) {
            alert("✅ " + data.message);
            chargerEnseignants();

            if (estEdition) {
                annulerEditionEnseignant();
            } else {
                document.getElementById("formEnseignant").reset();
                Array.from(document.getElementById("idMatieres").options).forEach(o => o.selected = false);
            }
        } else {
            alert("❌ " + (data.message?.join ? data.message.join("\n") : data.message));
        }

    } catch (error) {
        alert("Erreur serveur");
        console.error(error);
    }
});

document.getElementById("btnAnnulerEditionEnseignant").addEventListener("click", annulerEditionEnseignant);

// ---------- Initialisation ----------

window.onload = async function(){
    await chargerMatieresFormulaire();
    chargerEnseignants();
};