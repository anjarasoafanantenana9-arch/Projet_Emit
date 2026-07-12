const API = "/api/admin";

let modeEditionClasse = false;
let idClasseEnEdition = null;

// ---------- Chargement des niveaux pour le select du formulaire ----------

async function chargerNiveauxFormulaire(){
    const response = await fetch(`${API}/niveaux`);
    const niveaux = await response.json();

    const select = document.getElementById("idNiveau");
    select.innerHTML = `<option value="">-- Choisir un niveau --</option>`;
    niveaux.forEach(n => {
        select.innerHTML += `<option value="${n.idNiveau}">${n.nomNiveau}</option>`;
    });
}

// ---------- Chargement et affichage du tableau des classes ----------

async function chargerClasses(){

    const response = await fetch(`${API}/classes`);
    const classes = await response.json();

    const tbody = document.getElementById("tableClasses");
    tbody.innerHTML = "";

   classes.forEach(c => {

    tbody.innerHTML += `
    <tr>
        <td>${c.idClasse}</td>
        <td>${c.nomClasse}</td>
        <td>${c.nomNiveau ?? ""}</td>
        <td>${c.parcours ?? ""}</td>
        <td>${c.effectif}</td>

        <td>
            <button type="button" 
                    class="btn btn-sm btn-outline-primary"
                    onclick='ouvrirEditionClasse(${JSON.stringify(c)})'>
                <i class="bi bi-pencil-square"></i>
            </button>

            <button type="button"
                    class="btn btn-sm btn-outline-danger"
                    onclick="supprimerClasse(${c.idClasse})">
                <i class="bi bi-trash"></i>
            </button>
        </td>
    </tr>`;
});
}

// ---------- Édition ----------

async function ouvrirEditionClasse(c){

    // On récupère le détail complet (idNiveau, parcours en enum) via GET /api/admin/classes/{id}
    // car la liste renvoie le parcours déjà en texte, ce qui suffit ici
    modeEditionClasse = true;
    idClasseEnEdition = c.idClasse;

    document.getElementById("idClasse").value = c.idClasse;
    document.getElementById("nomClasse").value = c.nomClasse;
    document.getElementById("idNiveau").value = c.idNiveau;
    document.getElementById("parcours").value = c.parcours;
    document.getElementById("effectif").value = c.effectif;

    document.getElementById("titreFormulaireClasse").textContent = "Modifier la classe";
    document.getElementById("btnSubmitClasse").textContent = "Enregistrer les modifications";
    document.getElementById("btnAnnulerEditionClasse").classList.remove("d-none");

    document.getElementById("formClasse").scrollIntoView({ behavior: "smooth" });
}

function annulerEditionClasse(){
    modeEditionClasse = false;
    idClasseEnEdition = null;

    document.getElementById("formClasse").reset();
    document.getElementById("idClasse").value = "";

    document.getElementById("titreFormulaireClasse").textContent = "Ajouter une classe";
    document.getElementById("btnSubmitClasse").textContent = "Enregistrer";
    document.getElementById("btnAnnulerEditionClasse").classList.add("d-none");
}

// ---------- Suppression ----------

async function supprimerClasse(idClasse){

    if (!confirm("Voulez-vous vraiment supprimer cette classe ?")) {
        return;
    }

    try {
        const response = await fetch(`${API}/classes/${idClasse}`, {
            method: "DELETE"
        });

        const data = await response.json();

        if (response.ok) {
            alert("✅ " + data.message);
            chargerClasses();
        } else {
            alert("❌ " + data.message);
        }

    } catch (error) {
        alert("Erreur serveur");
        console.error(error);
    }
}

// ---------- Soumission du formulaire (création ou modification) ----------

document.getElementById("formClasse").addEventListener("submit", async function(e){
    e.preventDefault();

    const dto = {
        nomClasse: document.getElementById("nomClasse").value,
        idNiveau: parseInt(document.getElementById("idNiveau").value),
        parcours: document.getElementById("parcours").value,
        effectif: parseInt(document.getElementById("effectif").value)
    };

    console.log(dto);

    const estEdition = modeEditionClasse && idClasseEnEdition !== null;

    try {
        const response = await fetch(
            estEdition ? `${API}/classes/${idClasseEnEdition}` : `${API}/classes`,
            {
                method: estEdition ? "PUT" : "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(dto)
            }
        );

        const data = await response.json();

        if (response.ok) {
            console.log(data);
alert("Classe ajoutée");
            chargerClasses();

            if (estEdition) {
                annulerEditionClasse();
            } else {
                document.getElementById("formClasse").reset();
            }
        } else {
            alert("❌ " + (data.message?.join ? data.message.join("\n") : data.message));
        }

    } catch (error) {
        alert("Erreur serveur");
        console.error(error);
    }
});

document.getElementById("btnAnnulerEditionClasse").addEventListener("click", annulerEditionClasse);

// ---------- Initialisation ----------

window.onload = function(){
    chargerNiveauxFormulaire();
    chargerClasses();
};