const { Alert } = require("bootstrap");

function chooseFile() {
    document.getElementById('ImageFile').click();
}

function handleImageSelect(input) {
    const file = input.files[0];

    if (file) {
        const imgElement = document.getElementById('imagePreview');

        // Display the selected image preview
        imgElement.src = URL.createObjectURL(file);
        imgElement.style.display = 'block';
    }
}

function loadContent(action) {
    $.ajax({
        url: '/Produits/' + action,
        type: 'GET',
        success: function (result) {
            $('#main-content').html(result);

            // Si l'action est 'ModifyForm', récupérer et afficher les données dans le formulaire
            if (action === 'ModifyProductForm') {
                // Appeler une nouvelle fonction pour remplir le formulaire avec les données
                loadFormData();

            }

        }
    });
}
// Déclarez une variable pour stocker l'ID
var currentProductId;
function searchAndLoadForm() {
    var searchInputValue = $('#searchInput').val();
    $.ajax({
        type: 'GET',
        url: '/Produits/CheckIfIdExists?searchId=' + searchInputValue,
        success: function (data) {
            if (data.exists) {
                // Stockez l'ID récupéré dans la variable
                currentProductId = searchInputValue;
                loadContent('ModifyProductForm');

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
    var productId = currentProductId;

    $.ajax({
        type: 'GET',
        url: '/Produits/GetProductData?id=' + productId,
        success: function (data) {
            console.log(data.idPr);
            console.log(data.name);
            console.log(data.prix);
            console.log(data.dateDepot);
            console.log(data.stock);
            console.log(data.description);
            console.log(data.iDC);
            console.log(data.iDP);

            // Remplissez le formulaire avec les données récupérées
            $('#idPr').val(data.idPr);
            $('#Name').val(data.name);
            $('#prix').val(data.prix);
            $('#stock').val(data.stock);
            $('#description').val(data.description);
            $('#Category').val(data.iDC);
            // Affichez l'image à partir de l'URL
        },
        error: function () {
            alert('An error occurred while fetching owner data.');
        }
    });
}
function saveModifiedData() {

    var selectedCategoryName = $('#categorieName').val();
    var idP = $('#iDP').val();
    var idC = $('#iDC').val();  // Assurez-vous d'avoir un champ iDC dans votre formulaire
    // Utilisation des promesses pour effectuer les deux requêtes en parallèle
    var productId = currentProductId; // Utilisez l'ID stocké dans la variable
    var formData = new FormData();
    var selectedCategoryName = $('#categorieName').val();

    getCategoryIdByName(selectedCategoryName)
        .then(function (categoryId) {
            console.log("Category ID:", categoryId);
            formData.append('IDC', categoryId);
        })
        .catch(function (error) {
            console.error("Erreur lors de la récupération de l'ID de la catégorie:", error);
        });

    var imageFile = $('#imageFile')[0].files[0];
    convertImageToBinary(imageFile)
        .then(function (binaryData) {
            // Assignez les données binaires à la propriété ImageData de l'objet updatedProduitData
            updatedProduitData.ImageData = binaryData;
        })
        .catch(function (error) {
            console.error(error);
        });


    // Vérifiez si une image est sélectionnée
    if (!imageFile) {
        alert('Please choose an image file.');
        return;
    }

    formData.append('ImageFile', imageFile);

    // Gardez la partie du formulaire que vous ne souhaitez pas modifier
    formData.append('IDP', $('#iDP').val());
    formData.append('IdPr', $('#idPr').val());
    formData.append('Name', $('#Name').val());
    formData.append('prix', $('#prix').val());
    formData.append('DateDepot', $('#date').val());
    formData.append('stock', $('#stock').val());
    formData.append('Description', $('#detail').val());


    // Envoyez la requête AJAX
    $.ajax({
        type: 'POST',
        url: '/Produits/InsertProductData?id=' + productId,
        data: formData,
        processData: false,  // N'ajoutez pas de traitement
        contentType: false,  // N'ajoutez pas d'en-tête Content-Type
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

function getCategoryIdByName(selectedCategoryName) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'GET',
            url: '/Categories/GetCategoryIdByName?categoryName=' + selectedCategoryName,
            success: function (categoryId) {
                console.log("AJAX request successful Cat");
                resolve(categoryId);
            },
            error: function () {
                console.log("AJAX request failed Cat");
                reject("Erreur lors de la récupération de l'ID de la catégorie.");
            }
        });
    });
}

function convertImageToBinary(imageUrl) {
    return new Promise(function (resolve, reject) {
        // Utilisez une requête AJAX pour récupérer les données de l'image
        $.ajax({
            url: imageUrl,
            method: 'GET',
            responseType: 'arraybuffer',
            success: function (data) {
                // Convertissez les données en un tableau d'octets
                var binaryData = new Uint8Array(data);

                // Résolvez la promesse avec les données binaires
                resolve(binaryData);
            },
            error: function (xhr, status, error) {
                // Rejetez la promesse avec l'erreur
                reject('Une erreur s\'est produite lors de la récupération des données de l\'image: ' + error);
            }
        });
    });
}