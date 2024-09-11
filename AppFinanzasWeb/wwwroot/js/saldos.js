function updateTipoActivo() {
    const tipoActivo = document.getElementById('TipoActivo');
    var selectedIndex = tipoActivo.selectedIndex;
    var tipoActivoText = tipoActivo.options[selectedIndex].text;

    const activo = document.getElementById('activo');

    var table = $('#tablaSaldos');
    table.empty();

    activo.innerHTML = '<option value="">Seleccione</option>';

    var opcionesActivos;

    if (tipoActivoText === 'Accion Argentina') {
        opcionesActivos = JSON.parse(activo.getAttribute('data-accion'));
    } else if (tipoActivoText === 'CEDEAR') {
        opcionesActivos = JSON.parse(activo.getAttribute('data-cedear'));

    } else if (tipoActivoText === 'FCI') {
        opcionesActivos = JSON.parse(activo.getAttribute('data-fci'));

    } else if (tipoActivoText === 'Bonos') {
        opcionesActivos = JSON.parse(activo.getAttribute('data-bonos'));
    } else if (tipoActivoText === 'Moneda') {
        opcionesActivos = JSON.parse(activo.getAttribute('data-moneda'));
    } else if (tipoActivoText === 'Criptomoneda') {
        opcionesActivos = JSON.parse(activo.getAttribute('data-cripto'));
    }

    opcionesActivos.forEach(function (opcion) {
        var option = document.createElement("option");
        option.value = opcion.id;
        option.text = opcion.activoNombre;
        activo.appendChild(option);
    })
}

