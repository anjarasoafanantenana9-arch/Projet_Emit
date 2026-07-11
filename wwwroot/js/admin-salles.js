const API = "/api/admin/salles";

// Chargement au démarrage
window.onload = function () {
    chargerSalles();
};

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