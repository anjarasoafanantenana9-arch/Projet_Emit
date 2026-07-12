const API = "/api/admin/salles";

// Chargement au démarrage
window.onload = function () {
    chargerSalles();
};

// 1. APPELER CETTE FONCTION À CHAQUE FOIS QUE TU CHARGES OU REFRESH LES DONNÉES DU TABLEAU
function mettreAjourStats() {
    let lignes = document.querySelectorAll("#tableSalles tr");
    let totalSalles = lignes.length;
    let capaciteTotale = 0;

    lignes.forEach(tr => {
        // Extrait le nombre depuis le badge de la 3ème colonne
        let placesText = tr.cells[2]?.textContent || "0";
        let nbPlaces = parseInt(placesText.replace(/[^0-9]/g, "")) || 0;
        capaciteTotale += nbPlaces;
    });

    document.getElementById("statTotalSalles").innerText = totalSalles;
    document.getElementById("statCapaciteTotale").innerText = capaciteTotale + " places";
}

// 2. FONCTION DE RECHERCHE EN TEMPS RÉEL (LIÉE AU ONKEYUP DU CHAMP)
function filtrerSalles() {
    let input = document.getElementById("searchSalle").value.toLowerCase();
    let lignes = document.querySelectorAll("#tableSalles tr");
    let countVisible = 0;

    lignes.forEach(tr => {
        let textNom = tr.cells[1]?.textContent.toLowerCase() || "";
        if (textNom.includes(input)) {
            tr.style.display = "";
            countVisible++;
        } else {
            tr.style.display = "none";
        }
    });

    // Affiche un message si aucun résultat n'est trouvé
    let msgVide = document.getElementById("emptySearchMessage");
    if (countVisible === 0 && lignes.length > 0) {
        msgVide.classList.remove("d-none");
    } else {
        msgVide.classList.add("d-none");
    }
}

document.addEventListener("DOMContentLoaded", function () {
    // 1. Calculer les stats dès le premier chargement de la page
    calculerStatistiques();

    // 2. Écouter les changements dans le tableau (si ajout/suppression dynamique en AJAX)
    const cibleTableau = document.getElementById("tableSalles");
    if (cibleTableau) {
        const observateur = new MutationObserver(calculerStatistiques);
        observateur.observe(cibleTableau, { childList: true });
    }
});

/**
 * Calcule automatiquement le nombre total de salles 
 * et la somme de toutes les capacités à partir du tableau HTML
 */
function calculerStatistiques() {
    const lignes = document.querySelectorAll("#tableSalles tr");
    
    let totalSalles = 0;
    let capaciteTotale = 0;

    lignes.forEach(ligne => {
        // Ignorer les lignes d'en-tête ou de message vide si elles existent
        if (ligne.cells.length >= 3) {
            totalSalles++;

            // Récupère la valeur de la 3ème colonne (index 2 : Capacité d'accueil)
            const texteCapacite = ligne.cells[2].textContent.trim();
            const nbPlaces = parseInt(texteCapacite, 10);

            if (!isNaN(nbPlaces)) {
                capaciteTotale += nbPlaces;
            }
        }
    });

    // Mise à jour des compteurs dans les boîtes de statistiques du tableau de bord
    const elTotal = document.getElementById("statTotalSalles");
    const elCapacite = document.getElementById("statCapaciteTotale");

    if (elTotal) elTotal.innerText = totalSalles;
    if (elCapacite) elCapacite.innerText = capaciteTotale + " places";
}

// ==========================
// Charger les salles
// ==========================
async function chargerSalles() {

    const tbody = document.getElementById("tableSalles");
    tbody.innerHTML = "";

    try {

        const response = await fetch(API);

        if (!response.ok) {
            throw new Error("Erreur lors du chargement.");
        }

        const salles = await response.json();

        salles.forEach(salle => {

            tbody.innerHTML += `
                <tr>

                    <td>${salle.idSalle}</td>

                    <td>${salle.nomSalle}</td>

                    <td>${salle.capacite}</td>

                    <td>

                        <button
                            class="btn btn-warning btn-sm"
                            onclick="modifierSalle(${salle.idSalle}, '${salle.nomSalle}', ${salle.capacite})">

                            Modifier

                        </button>

                        <button
                            class="btn btn-danger btn-sm"
                            onclick="supprimerSalle(${salle.idSalle})">

                            Supprimer

                        </button>

                    </td>

                </tr>
            `;

        });

    }
    catch (error) {

        console.log(error);

    }

}


// ==========================
// Ajouter / Modifier
// ==========================

document
.getElementById("formSalle")
.addEventListener("submit", async function (e) {

    e.preventDefault();

    const id = document.getElementById("idSalle").value;

    const salle = {

        idSalle: id == "" ? 0 : parseInt(id),

        nomSalle: document.getElementById("nomSalle").value,

        capacite: parseInt(
            document.getElementById("capacite").value
        )

    };

    let url = API;
    let methode = "POST";

    if (id != "") {

        url += "/" + id;

        methode = "PUT";

    }

    try {

        const response = await fetch(url, {

            method: methode,

            headers: {

                "Content-Type": "application/json"

            },

            body: JSON.stringify(salle)

        });

        const data = await response.json();

        afficherMessage(
            response.ok,
            data.message
        );

        if (response.ok) {

            document
                .getElementById("formSalle")
                .reset();

            document
                .getElementById("idSalle")
                .value = "";

            chargerSalles();

        }

    }
    catch (error) {

        console.log(error);

    }

});


// ==========================
// Modifier
// ==========================

function modifierSalle(id, nom, capacite) {

    document.getElementById("idSalle").value = id;

    document.getElementById("nomSalle").value = nom;

    document.getElementById("capacite").value = capacite;

}



// ==========================
// Supprimer
// ==========================

async function supprimerSalle(id) {

    if (!confirm("Voulez-vous supprimer cette salle ?"))
        return;

    try {

        const response = await fetch(API + "/" + id, {

            method: "DELETE"

        });

        const data = await response.json();

        afficherMessage(
            response.ok,
            data.message
        );

        if (response.ok) {

            chargerSalles();

        }

    }
    catch (error) {

        console.log(error);

    }

}



// ==========================
// Messages
// ==========================

function afficherMessage(succes, message) {

    const alertBox = document.getElementById("alertBox");

    alertBox.classList.remove("d-none");

    if (succes) {

        alertBox.className =
            "alert alert-success";

    }
    else {

        alertBox.className =
            "alert alert-danger";

    }

    alertBox.innerHTML = message;

}