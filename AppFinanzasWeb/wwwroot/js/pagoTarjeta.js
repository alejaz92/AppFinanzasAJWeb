
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
        const moneda = row.cells[3];


        var checkbox = row.cells[7].getElementsByTagName('input')[0];

        
        if (checkbox.checked && (moneda.textContent == 'Peso Argentino' || monedaPago.value == 'Pesos')) {

            if (input) {
                var valorPesos = parseFloat(input.value);
                total = total + valorPesos;


            }
        }      
    }


    if (monedaPago.textContent === 'PesosDolar') {


        gastosAdm.value = parseFloat(removePoints(pagoPesos.value)) - total + parseFloat(removePoints(pagoDolar.value)) * cotizacion.value;
    }
    else {

        
        gastosAdm.value = parseFloat(removePoints(pagoPesos.value)) - total;       

    }

    if (gastosAdm.value >= 0) {

        formatearMoneda(gastosAdm);
    } else {
        gastosAdm.value = 'Datos Incorrectos';
    } 


    
}

$('form').change(function () {
    var datosTabla = [];
    
    $('#gastosTableBody tr').each(function () {

        var rawData = {};


        rawData.FechaMov = $(this).find('td:eq(0)').text();
        rawData.IdClaseMovimiento = parseInt($(this).find('input[name="idClase"]').val());
        rawData.Detalle = $(this).find('td:eq(2)').text();
        rawData.IdActivo = parseInt($(this).find('input[name="idActivo"]').val());
        rawData.CuotaTexto = $(this).find('td:eq(4)').text();
        rawData.MontoCuotaString = $(this).find('td:eq(5) input').val();
        rawData.ValorPesosString = $(this).find('td:eq(6) input').val();        
        rawData.Pagar = $(this).find('input[type="checkbox"]').prop('checked');

        datosTabla.push(rawData);
    });

    var datosTablaJson = JSON.stringify(datosTabla);
    $('#DatosTablaSerializados').val(datosTablaJson);
});

