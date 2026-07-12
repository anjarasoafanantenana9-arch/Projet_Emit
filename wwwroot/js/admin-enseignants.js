const api = "/api/admin/enseignants";


// Chargement après ouverture de la page
document.addEventListener("DOMContentLoaded", () => {

    chargerEnseignants();


    document
    .getElementById("formEnseignant")
    .addEventListener("submit", async function(e){

        e.preventDefault();


        const id = document
            .getElementById("idUtilisateur")
            .value;



        const enseignant = {

            nom: document
                .getElementById("nom")
                .value,


            prenom: document
                .getElementById("prenom")
                .value,


            plafondHeuresJournalieres:
                parseInt(
                    document.getElementById("plafond").value
                )

        };



        let response;



        try {


            // AJOUT
            if(id === "")
            {

                response = await fetch(api, {

                    method:"POST",

                    headers:{
                        "Content-Type":"application/json"
                    },

                    body: JSON.stringify(enseignant)

                });


            }


            // MODIFICATION
            else
            {


                



                response = await fetch(api + "/" + id, {

                    method:"PUT",

                    headers:{
                        "Content-Type":"application/json"
                    },


                    body: JSON.stringify(enseignant)

                });


            }



            const data = await response.json();


            alert(data.message);



            this.reset();


            document
            .getElementById("idUtilisateur")
            .value="";



            chargerEnseignants();



        }
        catch(error)
        {

            console.error(error);

            alert("Erreur serveur");

        }


    });


});





// =============================
// AFFICHER LA LISTE
// =============================


async function chargerEnseignants(){


    try {


        const res = await fetch(api);


        const data = await res.json();



        let html="";



        data.forEach(e => {



            html += `

            <tr>

                <td>${e.idUtilisateur}</td>

                <td>${e.nom}</td>

                <td>${e.prenom}</td>


                <td>


                    <button 
                    class="btn btn-warning btn-sm"
                    onclick="modifierEnseignant(${e.idUtilisateur})">

                        Modifier

                    </button>



                    <button 
                    class="btn btn-danger btn-sm"
                    onclick="supprimerEnseignant(${e.idUtilisateur})">

                        Supprimer

                    </button>


                </td>


            </tr>


            `;


        });



        document
        .getElementById("tableEnseignants")
        .innerHTML = html;



    }
    catch(error)
    {

        console.error(error);

    }


}







// =============================
// SUPPRESSION
// =============================


async function supprimerEnseignant(id){


    if(!confirm("Supprimer cet enseignant ?"))
        return;



    const res = await fetch(api+"/"+id, {


        method:"DELETE"


    });



    const data = await res.json();



    alert(data.message);



    chargerEnseignants();


}







// =============================
// MODIFICATION
// =============================


async function modifierEnseignant(id){


    const res = await fetch(api+"/"+id);


    const e = await res.json();



    document
    .getElementById("idUtilisateur")
    .value = e.idUtilisateur;



    document
    .getElementById("nom")
    .value = e.nom;



    document
    .getElementById("prenom")
    .value = e.prenom;



    document
    .getElementById("plafond")
    .value = e.plafondHeuresJournalieres ?? 6;



}