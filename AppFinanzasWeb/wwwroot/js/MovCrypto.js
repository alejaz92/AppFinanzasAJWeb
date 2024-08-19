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

    if (tipoMovimiento === 'Ingreso' && tipoComercio === 'Ajuste de Saldos') {
        grupoEgresos.style.display = 'none';
        grupoIngresos.style.display = 'flex';


    } else if (tipoMovimiento === 'Ingreso' && tipoComercio === 'Comercio Fiat/Cripto') {
        grupoEgresos.style.display = 'flex';
        grupoIngresos.style.display = 'flex';
    } else if (tipoMovimiento === 'Egreso' && tipoComercio === 'Ajuste de Saldos') {
        grupoEgresos.style.display = 'flex';
        grupoIngresos.style.display = 'none';
    } else if (tipoMovimiento === 'Egreso' && tipoComercio === 'Comercio Fiat/Cripto') {
        grupoEgresos.style.display = 'flex';
        grupoIngresos.style.display = 'flex';
    } else if (tipoMovimiento === 'Intercambio') {
        grupoEgresos.style.display = 'flex';
        grupoIngresos.style.display = 'flex';
    }
    else {
        grupoEgresos.style.display = 'none';
        grupoIngresos.style.display = 'none';
    }

}