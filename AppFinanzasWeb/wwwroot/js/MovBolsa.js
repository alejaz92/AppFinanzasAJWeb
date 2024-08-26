function updateGruposForm() {
    const tipoMovimiento = document.getElementById('TipoMovimiento').value;
    const tipoComercio = document.getElementById('TipoComercio').value;


    const grupoIngresos = document.getElementById('IngresoGroup');
    const grupoEgresos = document.getElementById('EgresoGroup');
    const grupoCotizacionIng = document.getElementById('CotizacionIngGroup');
    const grupoCotizacionEgr = document.getElementById('CotizacionEgrGroup');

    if (tipoMovimiento === 'Ingreso' && tipoComercio === 'AjusteSaldos') {
        grupoEgresos.style.display = 'none';
        grupoIngresos.style.display = 'flex';
        grupoCotizacionIng.style.display = 'block';
        grupoCotizacionEgr.style.display = 'none';

    }else if (tipoMovimiento === 'Ingreso' && tipoComercio === 'General') {
        grupoEgresos.style.display = 'flex';
        grupoIngresos.style.display = 'flex';
        grupoCotizacionIng.style.display = 'block';
        grupoCotizacionEgr.style.display = 'none';

    } else if (tipoMovimiento === 'Egreso' && tipoComercio === 'AjusteSaldos') {
        grupoEgresos.style.display = 'flex';
        grupoIngresos.style.display = 'none';

        grupoCotizacionIng.style.display = 'none';
        grupoCotizacionEgr.style.display = 'block';

    } else if (tipoMovimiento === 'Egreso' && tipoComercio === 'General') {
        grupoEgresos.style.display = 'flex';
        grupoIngresos.style.display = 'flex';
        grupoCotizacionIng.style.display = 'none';
        grupoCotizacionEgr.style.display = 'block';
    } else {
        grupoEgresos.style.display = 'none';
        grupoIngresos.style.display = 'none';
    }

    updateTipoActivo();
}

function updateTipoActivo() {
    const tipoMovimiento = document.getElementById('TipoMovimiento').value;
    const tipoComercio = document.getElementById('TipoComercio').value;
    const tipoActivo = document.getElementById('TipoActivo');

    var selectedIndex = tipoActivo.selectedIndex;

    var tipoActivoText = tipoActivo.options[selectedIndex].text;

    const IdActivoEgr = document.getElementById('IdActivoEgr');
    const CantidadEgr = document.getElementById('CantidadEgr');
    const CotizacionEgr = document.getElementById('CotizacionEgr');
    const IdActivoIng = document.getElementById('IdActivoIng');
    const CantidadIng = document.getElementById('CantidadIng');
    const CotizacionIng = document.getElementById('CotizacionIng');

    IdActivoEgr.innerHTML = '<option value="">Seleccione</option>';
    IdActivoIng.innerHTML = '<option value="">Seleccione</option>';
    CantidadEgr.value = '';
    CotizacionEgr.value = '';
    CantidadIng.value = '';
    CotizacionIng.value = '';

    if (tipoMovimiento === 'Ingreso') {

        var opcionesActivoIng;

        if (tipoActivoText === 'Accion Argentina') {

            opcionesActivoIng = JSON.parse(IdActivoIng.getAttribute('data-accion'));

            
        } else if (tipoActivoText === 'CEDEAR') {
            opcionesActivoIng = JSON.parse(IdActivoIng.getAttribute('data-cedear'));

        } else if (tipoActivoText === 'FCI') {
            opcionesActivoIng = JSON.parse(IdActivoIng.getAttribute('data-fci'));


        } else if (tipoActivoText === 'Bonos') {
            opcionesActivoIng = JSON.parse(IdActivoIng.getAttribute('data-bonos'));


        }

        opcionesActivoIng.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.activoNombre;
            IdActivoIng.appendChild(option);
        });

        

        if (tipoComercio === 'General') {
            opcionesActivoEgr = JSON.parse(IdActivoIng.getAttribute('data-moneda'));

            opcionesActivoEgr.forEach(function (opcion) {
                var option = document.createElement("option");
                option.value = opcion.id;
                option.text = opcion.activoNombre;
                IdActivoEgr.appendChild(option);
            });
        }
    } else if (tipoMovimiento === 'Egreso') {

        var opcionesActivoEgr;
        if (tipoActivoText === 'Accion Argentina') {
            opcionesActivoEgr = JSON.parse(IdActivoIng.getAttribute('data-accion'));
        } else if (tipoActivoText === 'CEDEAR') {
            opcionesActivoEgr = JSON.parse(IdActivoIng.getAttribute('data-cedear'));
        } else if (tipoActivoText === 'FCI') {
            opcionesActivoEgr = JSON.parse(IdActivoIng.getAttribute('data-fci'));
        } else if (tipoActivoText === 'Bonos') {
            opcionesActivoEgr = JSON.parse(IdActivoIng.getAttribute('data-bonos'));
        }


        opcionesActivoEgr.forEach(function (opcion) {
            var option = document.createElement("option");
            option.value = opcion.id;
            option.text = opcion.activoNombre;
            IdActivoEgr.appendChild(option);
        });

        if (tipoComercio === 'General') {
            opcionesActivoIng = JSON.parse(IdActivoIng.getAttribute('data-moneda'));

            opcionesActivoIng.forEach(function (opcion) {
                var option = document.createElement("option");
                option.value = opcion.id;
                option.text = opcion.activoNombre;
                IdActivoIng.appendChild(option);
            });
        }
    }
}
function checkSubmit() {
    const tipoMovimiento = document.getElementById('TipoMovimiento').value;
    const tipoComercio = document.getElementById('TipoComercio').value;
    const fecha = document.getElementById('Fecha').value;
    const tipoActivo = document.getElementById('TipoActivo').value;


    const IdActivoEgr = document.getElementById('IdActivoEgr');
    const IdCuentaEgr = document.getElementById('IdCuentaEgr');
    const CantidadEgr = document.getElementById('CantidadEgr');
    const CotizacionEgr = document.getElementById('CotizacionEgr');
    const IdActivoIng = document.getElementById('IdActivoIng');
    const IdCuentaIng = document.getElementById('IdCuentaIng');
    const CantidadIng = document.getElementById('CantidadIng');
    const CotizacionIng = document.getElementById('CotizacionIng');


    if (fecha === '') {
        alert('Seleccione una fecha válida');
        return false;
    }


    if (tipoActivo === '' || tipoActivo === 'Seleccione') {
        alert('Seleccione un Tipo de Activo');
        return false;
    }



    if (tipoMovimiento === 'Ingreso' && tipoComercio === 'AjusteSaldos') {
        if (IdActivoIng.value === '' || IdActivoIng === 'Seleccione') {
            alert('Debe seleccionar un activo correcto');
            return false;
        }
        if (IdCuentaIng.value === '' || IdCuentaIng.value === 'Seleccione') {
            alert('Debe seleccionar una cuenta correcta');
            return false;
        }
        if (!isNaN(CantidadIng) || !isNaN(CotizacionIng) || CantidadIng.value === '' || CotizacionIng.value === '') {
            alert('Debe insertar valores numericos correctos');
            return false;
        }
        return true;
    } else if (tipoMovimiento === 'Ingreso' && tipoComercio === 'General') {
        if (IdActivoIng.value === '' || IdActivoIng.value === 'Seleccione') {
            alert('Debe seleccionar un activo correcto');
            return false;
        }
        if (IdCuentaIng.value === '' || IdCuentaIng.value === 'Seleccione') {
            alert('Debe seleccionar una cuenta correcta');
            return false;
        }
        if (IdActivoEgr.value === '' || IdActivoEgr.value === 'Seleccione') {
            alert('Debe seleccionar un activo correcto');
            return false;
        }
        if (IdCuentaEgr.value === '' || IdCuentaEgr === 'Seleccione') {
            alert('Debe seleccionar una cuenta correcta');
            return false;
        }
        if (!isNaN(CantidadIng) || !isNaN(CotizacionIng) || !isNaN(CantidadEgr) ||
            CantidadIng.value === '' || CotizacionIng.value === '' || CantidadEgr.value === '') {
            alert('Debe insertar valores numericos correctos');
            return false;
        }
        return true;
    } else if (tipoMovimiento === 'Egreso' && tipoComercio === 'AjusteSaldos') {
        if (IdActivoEgr.value === '' || IdActivoEgr.value === 'Seleccione') {
            alert('Debe seleccionar un activo correcto');
            return false;
        }
        if (IdCuentaEgr.value === '' || IdCuentaEgr.value === 'Seleccione') {
            alert('Debe seleccionar una cuenta correcta');
            return false;
        }
        if (!isNaN(CantidadEgr) || !isNaN(CotizacionEgr) || CantidadEgr.value === '' || CotizacionEgr.value === '') {
            alert('Debe insertar valores numericos correctos');
            return false;
        }
        return true;
    } else if (tipoMovimiento === 'Egreso' && tipoComercio === 'General') {
        if (IdActivoIng.value === '' || IdActivoIng.value === 'Seleccione') {
            alert('Debe seleccionar un activo correcto');
            return false;
        }
        if (IdCuentaIng.value === '' || IdCuentaIng.value === 'Seleccione') {
            alert('Debe seleccionar una cuenta correcta');
            return false;
        }
        if (IdActivoEgr.value === '' || IdActivoEgr.value === 'Seleccione') {
            alert('Debe seleccionar un activo correcto');
            return false;
        }
        if (IdCuentaEgr.value === '' || IdCuentaEgr.value === 'Seleccione') {
            alert('Debe seleccionar una cuenta correcta');
            return false;
        }

        if (!isNaN(CantidadIng) || !isNaN(CantidadEgr) || !isNaN(CotizacionEgr) ||
            CantidadIng.value === '' || CantidadEgr.value === '' || CotizacionEgr.value === '') {
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