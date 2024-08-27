function checkSubmit() {
    const fecha = document.getElementById('Fecha').value;
    const IdActivoEgr = document.getElementById('IdActivoEgr');
    const IdCuentaEgr = document.getElementById('IdCuentaEgr');
    const CantidadEgr = document.getElementById('CantidadEgr');
    const IdActivoIng = document.getElementById('IdActivoIng');
    const IdCuentaIng = document.getElementById('IdCuentaIng');
    const CantidadIng = document.getElementById('CantidadIng');

    if (fecha === '') {
        alert('Seleccione una fecha válida');
        return false;
    }

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
    if (!isNaN(CantidadIng) || !isNaN(CantidadEgr) ||
        CantidadIng.value === '' || CantidadEgr.value === '') {
        alert('Debe insertar valores numericos correctos');
        return false;
    }

    if (IdActivoEgr.value === IdActivoIng.value) {
        alert('Los activos no deben coincidir');
        return false;
    }

    return true;
}