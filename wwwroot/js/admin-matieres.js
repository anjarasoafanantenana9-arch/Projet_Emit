const api = "/api/admin/matieres";


// Chargement au démarrage
document.addEventListener("DOMContentLoaded", () => {

    chargerMatieres();


    document
    .getElementById("formMatiere")
    .addEventListener("submit", enregistrerMatiere);

});




// ============================
// CHARGER LES MATIERES
// ============================

async function chargerMatieres(){

    try {

        const response = await fetch(api);

        const matieres = await response.json();


        let html = "";


        matieres.forEach(m => {

            html += `

            <tr>

                <td>
                    ${m.idMatiere}
                </td>


                <td>
                    ${m.nomMatiere}
                </td>


                <td>


                    <button 
                    class="btn btn-warning btn-sm"
                    onclick="modifierMatiere(${m.idMatiere})">

                        Modifier

                    </button>



                    <button 
                    class="btn btn-danger btn-sm"
                    onclick="supprimerMatiere(${m.idMatiere})">

                        Supprimer

                    </button>


                </td>


            </tr>

            `;

        });



        document
        .getElementById("tableMatieres")
        .innerHTML = html;


    }
    catch(error){

        console.error(
            "Erreur chargement matières :",
            error
        );

    }

}






// ============================
// AJOUT / MODIFICATION
// ============================

async function enregistrerMatiere(e){


    e.preventDefault();



    const id = document
        .getElementById("idMatiere")
        .value;



    const matiere = {

        nomMatiere:
        document
        .getElementById("nomMatiere")
        .value

    };



    let response;



    try {


        if(id === "")
        {

            response = await fetch(api,{

                method:"POST",

                headers:{

                    "Content-Type":"application/json"

                },


                body:JSON.stringify(matiere)

            });


        }

        else
        {


            response = await fetch(api+"/"+id,{

                method:"PUT",

                headers:{

                    "Content-Type":"application/json"

                },


                body:JSON.stringify(matiere)

            });


        }



        const data = await response.json();



        alert(data.message);



        document
        .getElementById("formMatiere")
        .reset();



        document
        .getElementById("idMatiere")
        .value = "";



        chargerMatieres();


    }
    catch(error){

        console.error(error);

        alert("Erreur serveur");

    }


}







// ============================
// MODIFIER
// ============================

async function modifierMatiere(id){


    try{


        const response =
        await fetch(api+"/"+id);



        const m =
        await response.json();




        document
        .getElementById("idMatiere")
        .value =
        m.idMatiere;



        document
        .getElementById("nomMatiere")
        .value =
        m.nomMatiere;



    }
    catch(error){

        console.error(error);

    }


}

// ── GESTION DE LA VISIBILITÉ DU TOKEN (ICÔNES VECTORIELLES) ───
document.getElementById('togglePassword').addEventListener('click', function () {
  const tokenInput = document.getElementById('loginToken');
  const eyeIcon = document.getElementById('eyeIcon');
  
  if (tokenInput.type === 'password') {
    tokenInput.type = 'text';
    // Mutation vers l'icône de l'œil barré (Masquer)
    eyeIcon.innerHTML = `
      <path stroke-linecap="round" stroke-linejoin="round" d="M3.98 8.223A10.477 10.477 0 0 0 1.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.451 10.451 0 0 1 12 4.5c4.756 0 8.773 3.162 10.065 7.498a10.522 10.522 0 0 1-4.293 5.774M6.228 6.228 3 3m3.228 3.228 3.65 3.65m7.894 7.894L21 21m-3.228-3.228-3.65-3.65m0 0a3 3 0 1 0-4.243-4.243m4.242 4.242L9.88 9.88" />
    `;
  } else {
    tokenInput.type = 'password';
    // Retour à l'icône de l'œil ouvert (Afficher)
    eyeIcon.innerHTML = `
      <path stroke-linecap="round" stroke-linejoin="round" d="M2.036 12.322a1.012 1.012 0 0 1 0-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178Z" />
      <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z" />
    `;
  }
});





// ============================
// SUPPRIMER
// ============================

async function supprimerMatiere(id){


    if(!confirm(
        "Supprimer cette matière ?"
    ))
    {
        return;
    }




    try{


        const response =
        await fetch(api+"/"+id,{

            method:"DELETE"

        });



        const data =
        await response.json();



        alert(data.message);



        chargerMatieres();



    }
    catch(error){

        console.error(error);

    }


}