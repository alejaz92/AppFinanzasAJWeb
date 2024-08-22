function updateGruposForm() {
    const tipoMovimiento = document.getElementById('TipoMovimiento').value;
    const tipoComercio = document.getElementById('TipoComercio').value;

    const grupoIngresos = document.getElementById('IngresoGroup');
    const grupoEgresos = document.getElementById('EgresoGroup');

    if (tipoMovimiento === 'Ingreso' && tipoComercio === 'AjusteSaldos') {
        grupoEgresos.style.display = 'none';
        grupoIngresos.style.display = 'flex';
    }else if (tipoMovimiento === 'Ingreso' && tipoComercio === 'General') {
        grupoEgresos.style.display = 'flex';
        grupoIngresos.style.display = 'flex';
    } else if (tipoMovimiento === 'Egreso' && tipoComercio === 'AjusteSaldos') {
        grupoEgresos.style.display = 'flex';
        grupoIngresos.style.display = 'none';
    } else if (tipoMovimiento === 'Egreso' && tipoComercio === 'General') {
        grupoEgresos.style.display = 'flex';
        grupoIngresos.style.display = 'flex';
    } else {
        grupoEgresos.style.display = 'none';
        grupoIngresos.style.display = 'none';
    }

    updateTipoActivo();
}

function uptdateTipoActivo() {
    const tipoMovimiento = document.getElementById('TipoMovimiento').value;
    const tipoComercio = document.getElementById('TipoComercio').value;
    const tipoActivo = document.getElementById('TipoActivo').value;

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
        if(tipoActivo === )
    } else if (tipoMovimiento === 'Egreso') {

    }
}