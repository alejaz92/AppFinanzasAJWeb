// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function formatearMoneda(input) {
    // Reemplazar punto o coma por punto para el parseo
    let valor = removePoints(input.value)

    valor = valor.replace(/[^0-9.,]/g, '').replace(',', '.');

    // Convertir el valor a número flotante
    let numero = parseFloat(valor);


    if (!isNaN(numero)) {
        // Formatear el número con puntos y comas
        let valorFormateado = numero.toLocaleString('es-ES', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
            //useGrouping: true
        });


        // Actualizar el valor del campo input visible
        input.value = valorFormateado;

    } else {
        // Si no es un número válido, limpiar los valores
        input.value = '';
    }
}

function removePoints(str) {
    //posicion del coma decimal
    const lastIndex = str.lastIndexOf('.');

    // Construye una nueva cadena con los puntos
    let result = '';
    for (let i = 0; i < str.length; i++) {
        if (str[i] === '.' && i != lastIndex) {
            // Omitir el punto si no es el de la antepenúltima posición
            continue;
        }
        result += str[i];
    }
    return result;
}

