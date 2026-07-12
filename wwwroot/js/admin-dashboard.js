const apiDashboard = "/api/admin/dashboard";

document.addEventListener("DOMContentLoaded", () => {
    chargerDashboard();
});

async function chargerDashboard(){

    try{

        const response = await fetch(apiDashboard);
        const data = await response.json();

        // Remplir les cartes
        document.getElementById("totalEnseignants").innerText = data.enseignants;
        document.getElementById("totalClasses").innerText = data.classes;
        document.getElementById("totalSalles").innerText = data.salles;
        document.getElementById("totalMatieres").innerText = data.matieres;
        document.getElementById("totalCours").innerText = data.cours;
        document.getElementById("totalEtudiants").innerText = data.etudiants;

        // Création du graphique (une seule fois, sur le seul canvas de la page)
        creerGraphique(data);

    }
    catch(error){
        console.error("Erreur chargement dashboard :", error);
    }
}

function creerGraphique(data){

    const ctx = document.getElementById("dashboardChart");
    if (!ctx) return;

    new Chart(ctx, {
        type: "doughnut",
        data: {
            labels: [
                "Enseignants",
                "Classes",
                "Salles",
                "Matières",
                "Cours",
                "Etudiants"
            ],
            datasets: [{
                data: [
                    data.enseignants,
                    data.classes,
                    data.salles,
                    data.matieres,
                    data.cours,
                    data.etudiants
                ]
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: "bottom"
                }
            }
        }
    });
}