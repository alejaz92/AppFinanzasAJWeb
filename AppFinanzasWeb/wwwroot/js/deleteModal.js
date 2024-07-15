
function openDeleteModal(id, deleteURL, esUsadoURL) {
   // console.log(id);

    $.ajax({
        url: esUsadoURL,
        type: 'GET',
        data: { id: id },
        success: function (response) {
            if (response.result) {
                
                $('#errorDeleteModal').modal('show');
            } else {
                $('#deleteModal').modal('show');
                $('#confirmDeleteButton').attr('onclick', `confirmDelete(${id}, '${deleteURL}')`);    
            }
        }

    })

    //$('#deleteModal').modal('show');
    //$('#confirmDeleteButton').attr('onclick', `confirmDelete(${id}, '${deleteUrl}')`);
}

async function confirmDelete(id, deleteUrl) {
    try {
        const response = await fetch(deleteUrl + '/' + id, {
            method: 'POST'
        });

        if (response.ok) {
            $('#deleteModal').modal('hide');
            window.location.reload();
        } else {
            console.log('Error al eliminar el elemento: ', response.statusText);
            $('#deleteModal').modal('hide');
        }
    } catch (error) {
        console.log('Error al eliminar el elemento: ', error);
        $('#deleteModal').modal('hide');
    }
}

document.getElementById('cancelButton').addEventListener('click', function () {
    $('#deleteModal').modal('hide');
});

document.getElementById('closeButton').addEventListener('click', function () {
    $('#deleteModal').modal('hide');
});


document.getElementById('closeButton2').addEventListener('click', function () {
    $('#errorDeleteModal').modal('hide');
});
document.getElementById('acceptButton2').addEventListener('click', function () {
    $('#errorDeleteModal').modal('hide');
});

function checkDelete(controlador, id) {
    $.ajax({
        url: '/' + controlador + '/checkEsUsado',
        type: 'GET',
        data: {id: id},
        success: function (response) {
            if (response.result) {
                $('#deleteModal').modal('show');
                $('#confirmDeleteButton').attr('onclick', `confirmDelete(${id}, 'Borrar')`);
            } else {
                $('#errorDeleteModal').modal('show');
            }
        }

    })
}