function updateListaTiposComercio() {
    const tipoMovimiento = document.getElementById('TipoMovimiento').value;

    const tipoComercio = document.getElementById('TipoComercio');

    //Limpiar el select
    tipoComercio.innerHTML = '';

    let options = [];

    if (tipoMovimiento === 'Ingreso') {
        options = [
            { value: '', text: 'Seleccione' },
            { value: 'Ajuste de Saldos', text: 'Ajuste de Saldos' },
            { value: 'Comercio Fiat/Cripto', text: 'Comercio Fiat/Cripto'}
        ];
    } else if (tipoMovimiento === 'Egreso') {
        options = [
            { value: '', text: 'Seleccione' },
            { value: 'Ajuste de Saldos', text: 'Ajuste de Saldos' },
            { value: 'Comercio Fiat/Cripto', text: 'Comercio Fiat/Cripto' }
        ];
    } else if (tipoMovimiento === 'Intercambio') {
        options = [
            { value: 'Trading', text: 'Trading' }
        ];
    }

    // agregar las opciones

    options.forEach(option => {
        const newOption = document.createElement('option');
        newOption.value = option.value;
        newOption.text = option.text;
        tipoComercio.appendChild(newOption);
    });

    updateGruposForm()
}

function updateGruposForm() {
    const tipoMovimiento = document.getElementById('TipoMovimiento').value;
    const tipoComercio = document.getElementById('TipoComercio').value;

    const grupoIngresos = document.getElementById('IngresoGroup');
    const grupoEgresos = document.getElementById('EgresoGroup');
    const grupoCotizacionIng = document.getElementById('CotizacionIngGroup');
    const grupoCotizacionEgr = document.getElementById('CotizacionEgrGroup');

    const IdActivoEgr = document.getElementById('IdActivoEgr');
    const IdCuentaEgr = document.getElementById('IdCuentaEgr');
    const CantidadEgr = document.getElementById('CantidadEgr');
    const CotizacionEgr = document.getElementById('CotizacionEgr');
    const IdActivoIng = document.getElementById('IdActivoIng');
    const IdCuentaIng = document.getElementById('IdCuentaIng');
    const CantidadIng = document.getElementById('CantidadIng');
    const CotizacionIng = document.getElementById('CotizacionIng');

    IdActivoEgr.innerHTML = '<option value="">Seleccione</option>';
    IdCuentaEgr.innerHTML = '<option value="">Seleccione</option>';
    IdActivoIng.innerHTML = '<option value="">Seleccione</option>';
    IdCuentaIng.innerHTML = '<option value="">Seleccione</option>';
    CantidadEgr.value = '';
    CotizacionEgr.value = '';
    CantidadIng.value = '';
    CotizacionIng.value = '';



    if (tipoMovimiento === 'Ingreso' && tipoComercio === 'Ajuste de Saldos') {
        grupoEgresos.style.display = 'none';
        grupoIngresos.style.display = 'flex';
        grupoCotizacionIng.style.display = 'block';

        opcionesActivoIng = JSON.parse(IdActivoIng.getAttribute('data-crypto'));
        opcionesCuentaIng = JSON.parse(IdCuentaIng.getAttribute('data-crypto'));

        opcionesActivoIng.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.activoNombre;
            IdActivoIng.appendChild(option);
        });
        opcionesCuentaIng.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.cuentaNombre;
            IdCuentaIng.appendChild(option);
        });

    } else if (tipoMovimiento === 'Ingreso' && tipoComercio === 'Comercio Fiat/Cripto') {
        grupoEgresos.style.display = 'flex';
        grupoIngresos.style.display = 'flex';
        grupoCotizacionIng.style.display = 'block';
        grupoCotizacionEgr.style.display = 'none';


        opcionesActivoIng = JSON.parse(IdActivoIng.getAttribute('data-crypto'));
        opcionesCuentaIng = JSON.parse(IdCuentaIng.getAttribute('data-crypto'));
        opcionesActivoEgr = JSON.parse(IdActivoEgr.getAttribute('data-moneda'));
        opcionesCuentaEgr = JSON.parse(IdCuentaEgr.getAttribute('data-moneda'));

        opcionesActivoIng.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.activoNombre;
            IdActivoIng.appendChild(option);
        });
        opcionesCuentaIng.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.cuentaNombre;
            IdCuentaIng.appendChild(option);
        });

        opcionesActivoEgr.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.activoNombre;
            IdActivoEgr.appendChild(option);
        });
        opcionesCuentaEgr.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.cuentaNombre;
            IdCuentaEgr.appendChild(option);
        });

    } else if (tipoMovimiento === 'Egreso' && tipoComercio === 'Ajuste de Saldos') {
        grupoEgresos.style.display = 'flex';
        grupoIngresos.style.display = 'none';
        grupoCotizacionEgr.style.display = 'block';

        opcionesActivoEgr = JSON.parse(IdActivoEgr.getAttribute('data-crypto'));
        opcionesCuentaEgr = JSON.parse(IdCuentaEgr.getAttribute('data-crypto'));

        opcionesActivoEgr.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.activoNombre;
            IdActivoEgr.appendChild(option);
        });
        opcionesCuentaEgr.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.cuentaNombre;
            IdCuentaEgr.appendChild(option);
        });
    } else if (tipoMovimiento === 'Egreso' && tipoComercio === 'Comercio Fiat/Cripto') {
        grupoEgresos.style.display = 'flex';
        grupoIngresos.style.display = 'flex';
        grupoCotizacionIng.style.display = 'none';
        grupoCotizacionEgr.style.display = 'block';

        opcionesActivoEgr = JSON.parse(IdActivoEgr.getAttribute('data-crypto'));
        opcionesCuentaEgr = JSON.parse(IdCuentaEgr.getAttribute('data-crypto'));
        opcionesActivoIng = JSON.parse(IdActivoIng.getAttribute('data-moneda'));
        opcionesCuentaIng = JSON.parse(IdCuentaIng.getAttribute('data-moneda'));

        opcionesActivoIng.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.activoNombre;
            IdActivoIng.appendChild(option);
        });
        opcionesCuentaIng.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.cuentaNombre;
            IdCuentaIng.appendChild(option);
        });

        opcionesActivoEgr.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.activoNombre;
            IdActivoEgr.appendChild(option);
        });
        opcionesCuentaEgr.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.cuentaNombre;
            IdCuentaEgr.appendChild(option);
        });
    } else if (tipoMovimiento === 'Intercambio') {
        grupoEgresos.style.display = 'flex';
        grupoIngresos.style.display = 'flex';
        grupoCotizacionIng.style.display = 'block';
        grupoCotizacionEgr.style.display = 'block';

        opcionesActivoEgr = JSON.parse(IdActivoEgr.getAttribute('data-crypto'));
        opcionesCuentaEgr = JSON.parse(IdCuentaEgr.getAttribute('data-crypto'));
        opcionesActivoIng = JSON.parse(IdActivoIng.getAttribute('data-crypto'));
        opcionesCuentaIng = JSON.parse(IdCuentaIng.getAttribute('data-crypto'));

        opcionesActivoIng.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.activoNombre;
            IdActivoIng.appendChild(option);
        });
        opcionesCuentaIng.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.cuentaNombre;
            IdCuentaIng.appendChild(option);
        });

        opcionesActivoEgr.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.activoNombre;
            IdActivoEgr.appendChild(option);
        });
        opcionesCuentaEgr.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.cuentaNombre;
            IdCuentaEgr.appendChild(option);
        });
    }
    else {
        grupoEgresos.style.display = 'none';
        grupoIngresos.style.display = 'none';
    }
}

function checkSubmit() {
    const tipoMovimiento = document.getElementById('TipoMovimiento').value;
    const tipoComercio = document.getElementById('TipoComercio').value;
    const fecha = document.getElementById('Fecha').value;
    const grupoIngresos = document.getElementById('IngresoGroup');
    const grupoEgresos = document.getElementById('EgresoGroup');

    const IdActivoEgr = document.getElementById('IdActivoEgr');
    const IdCuentaEgr = document.getElementById('IdCuentaEgr');
    const CantidadEgr = document.getElementById('CantidadEgr');
    const CotizacionEgr = document.getElementById('CotizacionEgr');
    const IdActivoIng = document.getElementById('IdActivoIng');
    const IdCuentaIng = document.getElementById('IdCuentaIng');
    const CantidadIng = document.getElementById('CantidadIng');
    const CotizacionIng = document.getElementById('CotizacionIng');


    if (fecha === "") {
        alert('Seleccione una fecha válida');
        return false;
    }

    if (tipoMovimiento === 'Ingreso' && tipoComercio === 'Ajuste de Saldos') {

        if (IdActivoIng.value === '') {
            alert('Debe seleccionar un activo correcto');
            return false;
        }
        if (IdCuentaIng.value === '') {
            alert('Debe seleccionar una cuenta correcta');
            return false;
        }
        if (!isNaN(CantidadIng) || !isNaN(CotizacionIng) || CantidadIng.value === '' || CotizacionIng.value === '') {
            alert('Debe insertar valores numericos correctos');
            return false;
        }
        return true;

    } else if (tipoMovimiento === 'Ingreso' && tipoComercio === 'Comercio Fiat/Cripto') {

        if (IdActivoIng.value === '') {
            alert('Debe seleccionar un activo correcto');
            return false;
        }
        if (IdCuentaIng.value === '') {
            alert('Debe seleccionar una cuenta correcta');
            return false;
        }
        if (IdActivoEgr.value === '') {
            alert('Debe seleccionar un activo correcto');
            return false;
        }
        if (IdCuentaEgr.value === '') {
            alert('Debe seleccionar una cuenta correcta');
            return false;
        }
        if (!isNaN(CantidadIng) || !isNaN(CotizacionIng) || !isNaN(CantidadEgr) ||
            CantidadIng.value === '' || CotizacionIng.value === '' || CantidadEgr.value === '' ) {
            alert('Debe insertar valores numericos correctos');
            return false;
        }
        return true;
    } else if (tipoMovimiento === 'Egreso' && tipoComercio === 'Ajuste de Saldos') {
        if (IdActivoEgr.value === '') {
            alert('Debe seleccionar un activo correcto');
            return false;
        }
        if (IdCuentaEgr.value === '') {
            alert('Debe seleccionar una cuenta correcta');
            return false;
        }
        if (!isNaN(CantidadEgr) || !isNaN(CotizacionEgr) || CantidadEgr.value === '' || CotizacionEgr.value === '') {
            alert('Debe insertar valores numericos correctos');
            return false;
        }
        return true;
    } else if (tipoMovimiento === 'Egreso' && tipoComercio === 'Comercio Fiat/Cripto') {


        if (IdActivoIng.value === '') {
            alert('Debe seleccionar un activo correcto');
            return false;
        }
        if (IdCuentaIng.value === '') {
            alert('Debe seleccionar una cuenta correcta');
            return false;
        }
        if (IdActivoEgr.value === '') {
            alert('Debe seleccionar un activo correcto');
            return false;
        }
        if (IdCuentaEgr.value === '') {
            alert('Debe seleccionar una cuenta correcta');
            return false;
        }

        if (!isNaN(CantidadIng) || !isNaN(CantidadEgr) || !isNaN(CotizacionEgr) ||
            CantidadIng.value === '' || CantidadEgr.value === '' || CotizacionEgr.value === '') {
            alert('Debe insertar valores numericos correctos');
            return false;
        }
        return true;
    } else if (tipoMovimiento === 'Intercambio') {

        if (IdActivoIng.value === '') {
            alert('Debe seleccionar un activo correcto');
            return false;
        }
        if (IdCuentaIng.value === '') {
            alert('Debe seleccionar una cuenta correcta');
            return false;
        }
        if (IdActivoEgr.value === '') {
            alert('Debe seleccionar un activo correcto');
            return false;
        }
        if (IdCuentaEgr.value === '') {
            alert('Debe seleccionar una cuenta correcta');
            return false;
        }
        // 

        if (!isNaN(CantidadIng) || !isNaN(CotizacionIng) || !isNaN(CantidadEgr) || !isNaN(CotizacionEgr) ||
            CantidadIng.value === '' || CotizacionIng.value === '' || CantidadEgr.value === '' || CotizacionEgr.value === '') {
            alert('Debe insertar valores numericos correctos');
            return false;
        }
        return true;
    }
    else {
        alert('Seleccione tipo de movimiento y tipo de comercio');
        return false;
    }
}