

function visibleFields() {
    var monedaPago = document.getElementById('MonedaPago');

    dolarField = document.getElementById('DolarField');
    dolarInput = document.getElementById('TotalDolaresString');

    if (monedaPago.value == 'Pesos') {
        dolarField.style.display = 'none';
        dolarInput.value = '';
    } else {
        dolarField.style.display = 'block';
    }
}

function actualizarGastosAdm() {
    var monedaPago = document.getElementById('MonedaPago');
    var pagoPesos = document.getElementById('TotalPesosString');
    var pagoDolar = document.getElementById('TotalDolaresString');
    const gastosAdm = document.getElementById('TotalGastosString');
    const cotizacion = document.getElementById('Cotizacion');

    var tabla = document.getElementById('gastosTableBody');
    var rows = tabla.getElementsByTagName('tr');

    var total = 0;

    for (var i = 0; i < rows.length; i++) {
        var row = rows[i];

        const input = row.cells[6].querySelector('input');

        var checkbox = row.cells[7].getElementsByTagName('input')[0];


        if (checkbox.checked) {
            if (input) {
                var valorPesos = parseFloat(input.value);
                total = total + valorPesos;
            }
        }      
    }

    if (monedaPago === 'PesosDolar') {
        gastosAdm.value = parseFloat(pagoPesos.value) - total + parseFloat(pagoDolar.value) * cotizacion.value;
    }
    else {
        gastosAdm.value = parseFloat(pagoPesos.value) - total;       
    }
    formatearMoneda(gastosAdm);
}

$('form').change(function () {
    var test = document.getElementById('gastosTable');
    console.log(test);
    $('#gastosTableBody tr').each(function () {


        var fechaMov = $(this).find('td:eq(0)').text();
        var idClaseMov = $(this).find('input[name="idClase"]').val();
        var detalle = $(this).find('td:eq(2)').text();
        var idActivo = $(this).find('input[name="idActivo"]').val();
        var cuotaTexto = $(this).find('td:eq(4)').text();
        var montoCuota = $(this).find('td:eq(5) input').text();
        var valorPesos = $(this).find('td:eq(6) input').text();        
        var pagar = $(this).find('input[type="checkbox"]').prop('checked');


    });
});

