function loadContent(action) {
    $.ajax({
        url: '/Proprietaires/' + action,
        type: 'GET',
        success: function (result) {
            $('#main-content').html(result);

            // Si l'action est 'ModifyForm', récupérer et afficher les données dans le formulaire
            if (action === 'ModifyForm') {
                // Appeler une nouvelle fonction pour remplir le formulaire avec les données
                loadFormData();

            }

        }
    });
}
// Déclarez une variable pour stocker l'ID
var currentOwnerId;
function searchAndLoadForm() {
    var searchInputValue = $('#searchInput').val();
    $.ajax({
        type: 'GET',
        url: '/Proprietaires/CheckIfIdExists?searchId=' + searchInputValue,
        success: function (data) {
            if (data.exists) {
                // Stockez l'ID récupéré dans la variable
                currentOwnerId = searchInputValue;
                loadContent('ModifyForm');

            } else {
                alert('ID not found. Please try again.');
            }
        },
        error: function () {
            alert('An error occurred while checking the ID.');
        }
    });
}
function loadFormData() {
    // Utilisez l'ID stocké dans la variable
    var ownerId = currentOwnerId;

    $.ajax({
        type: 'GET',
        url: '/Proprietaires/GetOwnerData?id=' + ownerId,
        success: function (data) {
            // Remplissez le formulaire avec les données récupérées
            $('#LastName').val(data.nom);
            $('#FirstName').val(data.prenom);
            $('#Adresse').val(data.adresseEntreprise);
            $('#Phone').val(data.tele);
            $('#Passport').val(data.iNterID);
            $('#Company').val(data.nomEntreprise);
            $('#Password').val(data.password);
            $('#Email').val(data.email);
        },
        error: function () {
            alert('An error occurred while fetching owner data.');
        }
    });

}
function saveModifiedData() {
    var ownerId = currentOwnerId; // Utilisez l'ID stocké dans la variable

    var formData = {
        INterID: $('#Passport').val(),
        Nom: $('#LastName').val(),
        Prenom: $('#FirstName').val(),
        AdresseEntreprise: $('#Adresse').val(),
        Tele: $('#Phone').val(),
        NomEntreprise: $('#Company').val(),
        Password: $('#Password').val(),
        Email: $('#Email').val()
    };

    $.ajax({
        type: 'POST',
        url: '/Proprietaires/InsertOwnerData?id=' + ownerId,
        contentType: 'application/json',
        data: JSON.stringify(formData),
        success: function (response) {
            if (response.success) {
                alert('Les modifications ont été enregistrées avec succès.');
                // Vous pouvez également mettre à jour l'affichage sur la page si nécessaire
            } else {
                alert('Une erreur s\'est produite lors de la sauvegarde des modifications: ' + response.message);
            }
        },
        error: function (xhr) {
            alert('Une erreur s\'est produite lors de la sauvegarde des modifications: ' + xhr.responseText);
        }
    });
}
