const apiDashboard = "/api/admin/dashboard";



document.addEventListener("DOMContentLoaded", () => {

    chargerDashboard();

});





async function chargerDashboard(){

    try{


        const response = await fetch(apiDashboard);


        const data = await response.json();



        // Remplir les cartes

        document.getElementById("totalEnseignants").innerText =
            data.enseignants;


        document.getElementById("totalClasses").innerText =
            data.classes;


        document.getElementById("totalSalles").innerText =
            data.salles;


        document.getElementById("totalMatieres").innerText =
            data.matieres;


        document.getElementById("totalCours").innerText =
            data.cours;


        document.getElementById("totalEtudiants").innerText =
            data.etudiants;




        // Création graphique

        creerGraphique(data);



    }
    catch(error){

        console.error(
            "Erreur chargement dashboard :",
            error
        );

    }


}

document.addEventListener("DOMContentLoaded", () => {
    initSystemChart();
});

function initSystemChart() {
    const ctx = document.getElementById('dashboardChart');
    if (!ctx) return;

    // Dégradé ultra-doux pour l'arrière-plan de la courbe (Style Vercel Analytics)
    const chartCtx = ctx.getContext('2d');
    const gradient = chartCtx.createLinearGradient(0, 0, 0, 300);
    gradient.addColorStop(0, 'rgba(37, 99, 235, 0.12)');
    gradient.addColorStop(1, 'rgba(37, 99, 235, 0.0)');

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: ['Lun', 'Mar', 'Mer', 'Jeu', 'Ven', 'Sam', 'Dim'],
            datasets: [{
                label: 'Activité globale',
                data: [65, 78, 72, 89, 85, 42, 50],
                borderColor: '#2563eb',
                borderWidth: 2.5,
                backgroundColor: gradient,
                fill: true,
                tension: 0.38,
                pointRadius: 0,
                pointHoverRadius: 5,
                pointHoverBackgroundColor: '#2563eb',
                pointHoverBorderColor: '#ffffff',
                pointHoverBorderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    backgroundColor: '#0f172a',
                    titleFont: { size: 12, weight: '600' },
                    bodyFont: { size: 12 },
                    padding: 12,
                    borderRadius: 8,
                    boxWidth: 0
                }
            },
            scales: {
                x: {
                    grid: { display: false },
                    ticks: { color: '#94a3b8', font: { size: 11, weight: '500' } }
                },
                y: {
                    grid: { color: '#f1f5f9' },
                    ticks: { color: '#94a3b8', font: { size: 11 } },
                    border: { dash: [5, 5] } // Lignes pointillées pour l'axe Y
                }
            },
            interaction: {
                mode: 'nearest',
                axis: 'x',
                intersect: false
            }
        }
    });
}





function creerGraphique(data){


    const ctx =
    document
    .getElementById("dashboardChart");



    new Chart(ctx, {


        type:"doughnut",



        data:{


            labels:[

                "Enseignants",
                "Classes",
                "Salles",
                "Matières",
                "Cours",
                "Etudiants"

            ],



            datasets:[{


                data:[

                    data.enseignants,

                    data.classes,

                    data.salles,

                    data.matieres,

                    data.cours,

                    data.etudiants

                ]

            }]

        },



        options:{


            responsive:true,


            plugins:{


                legend:{


                    position:"bottom"

                }


            }


        }


    });



}