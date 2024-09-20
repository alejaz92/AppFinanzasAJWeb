var incomeByClassChartInstance = null;
var incomeLast6MonthsChartInstance = null;
var expenseByClassChartInstance = null;
var expenseLast6MonthsChartInstance = null;

var incomeByClassPesosChartInstance = null;
var incomeLast6MonthsPesosChartInstance = null;
var expenseByClassPesosChartInstance = null;
var expenseLast6MonthsPesosChartInstance = null;

var gastosEnPesosChartInstance = null;
var gastosEnDolaresChartInstance = null;

var porcentajeTickerChartInstance = null;
var origVsActualChartInstance = null;
var bolsaGralChartInstance = null;

document.addEventListener("DOMContentLoaded", function () {

    cargarDatosDashboard1();
    cargarDatosDashboard2();
});


function cargarDatosDashboard1() {
    var selectedMonth = document.getElementById('MesDB1').value;
    //console.log(JSON.stringify({ "selectedMonth": selectedMonth }));
    fetch('/Estadistica/getDataDB1', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(selectedMonth)
    })
        .then(response => response.json())
        .then(data => {
            updateGraficosDB1(data);
        });
}

function updateGraficosDB1(data) {

    if (incomeByClassChartInstance) {
        incomeByClassChartInstance.destroy();
    }
    if (incomeLast6MonthsChartInstance) {
        incomeLast6MonthsChartInstance.destroy();
    }
    if (expenseByClassChartInstance) {
        expenseByClassChartInstance.destroy();
    }
    if (expenseLast6MonthsChartInstance) {
        expenseLast6MonthsChartInstance.destroy();
    }

    var incomeByClassChart = document.getElementById('incomeByClassChart').getContext('2d');
    incomeByClassChartInstance = new Chart(incomeByClassChart, {
        type: 'bar',
        data: {
            labels: data.ingresosPorClase.map(item => item.claseMovimiento),
            datasets: [{
                label: 'Ingresos en USD',
                data: data.ingresosPorClase.map(item => item.total),
                backgroundColor: 'rgba(75, 192, 192, 0.2)',  // Verde claro
                borderColor: 'rgba(75, 192, 192, 1)',        // Verde oscuro
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Ingresos por Clase',
                    font: {
                        size: 18
                    },
                    padding: {
                        bottom: 10
                    }
                },
                legend: {
                    display: false
                },
                datalabels: {
                    color: '#000',
                    formatter: function (value) {
                        return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'USD' }).format(value);
                    },
                    anchor: 'end',
                    align: 'top',
                    offset: 4
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'USD' }).format(value);
                        }
                    }
                }
            }
        }
    });


    var incomeLast6MonthsChart = document.getElementById('incomeLast6MonthsChart').getContext('2d');
    incomeLast6MonthsChartInstance = new Chart(incomeLast6MonthsChart, {
        type: 'bar',
        data: {
            labels: data.ingresosUltMeses.map(item => item.mesNombre),
            datasets: [{
                label: 'Ingresos en USD',
                data: data.ingresosUltMeses.map(item => item.total),
                backgroundColor: 'rgba(75, 192, 192, 0.2)',  // Verde claro
                borderColor: 'rgba(75, 192, 192, 1)',        // Verde oscuro
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Ingresos por Mes',
                    font: {
                        size: 18
                    },
                    padding: {
                        bottom: 10
                    }
                },
                legend: {
                    display: false
                },
                datalabels: {
                    color: '#000',
                    formatter: function (value) {
                        return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'USD' }).format(value);
                    },
                    anchor: 'end',
                    align: 'top',
                    offset: 4
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'USD' }).format(value);
                        }
                    }
                }
            }
        }
    });

    
    var expenseLast6MonthsChart = document.getElementById('expenseLast6MonthsChart').getContext('2d');
    expenseLast6MonthsChartInstance = new Chart(expenseLast6MonthsChart, {
        type: 'bar',
        data: {
            labels: data.egresosUltMeses.map(item => item.mesNombre),
            datasets: [{
                label: 'Ingresos en USD',
                data: data.egresosUltMeses.map(item => item.total),
                backgroundColor: 'rgba(255, 99, 132, 0.2)',  // Rojo claro
                borderColor: 'rgba(255, 99, 132, 1)',        // Rojo oscuro
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Egresos por Mes',
                    font: {
                        size: 18
                    },
                    padding: {
                        bottom: 10
                    }
                },
                legend: {
                    display: false
                },
                datalabels: {
                    color: '#000',
                    formatter: function (value) {
                        return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'USD' }).format(value);
                    },
                    anchor: 'end',
                    align: 'top',
                    offset: 4
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'USD' }).format(value);
                        }
                    }
                }
            }
        }
    });




    var expenseByClassChart = document.getElementById('expenseByClassChart').getContext('2d');
    expenseByClassChartInstance = new Chart(expenseByClassChart, {
        type: 'bar',
        data: {
            labels: data.egresosPorClase.map(item => item.claseMovimiento),
            datasets: [{
                label: 'Egresos en USD',
                data: data.egresosPorClase.map(item => item.total),
                backgroundColor: 'rgba(255, 99, 132, 0.2)',  // Rojo claro
                borderColor: 'rgba(255, 99, 132, 1)',        // Rojo oscuro
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Egresos por Clase',
                    font: {
                        size: 18
                    },
                    padding: {
                        bottom: 10
                    }
                },
                legend: {
                    display: false
                },
                datalabels: {
                    color: '#000',
                    formatter: function (value) {
                        return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'USD' }).format(value);
                    },
                    anchor: 'end',
                    align: 'top',
                    offset: 4
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'USD' }).format(value);
                        }
                    }
                }
            }
        }
    });

}

function cargarDatosDashboard2() {
    var selectedMonth = document.getElementById('MesDB2').value;
    //console.log(JSON.stringify({ "selectedMonth": selectedMonth }));
    fetch('/Estadistica/getDataDB2', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(selectedMonth)
    })
        .then(response => response.json())
        .then(data => {
            updateGraficosDB2(data);
        });
}


function updateGraficosDB2(data) {

    if (incomeByClassPesosChartInstance) {
        incomeByClassPesosChartInstance.destroy();
    }
    if (incomeLast6MonthsPesosChartInstance) {
        incomeLast6MonthsPesosChartInstance.destroy();
    }
    if (expenseByClassPesosChartInstance) {
        expenseByClassPesosChartInstance.destroy();
    }
    if (expenseLast6MonthsPesosChartInstance) {
        expenseLast6MonthsPesosChartInstance.destroy();
    }

    var incomeByClassPesosChart = document.getElementById('incomeByClassPesosChart').getContext('2d');
    incomeByClassPesosChartInstance = new Chart(incomeByClassPesosChart, {
        type: 'bar',
        data: {
            labels: data.ingresosPorClase.map(item => item.claseMovimiento),
            datasets: [{
                label: 'Ingresos en Pesos',
                data: data.ingresosPorClase.map(item => item.total),
                backgroundColor: 'rgba(75, 192, 192, 0.2)',  // Verde claro
                borderColor: 'rgba(75, 192, 192, 1)',        // Verde oscuro
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Ingresos por Clase',
                    font: {
                        size: 18
                    },
                    padding: {
                        bottom: 10
                    }
                },
                legend: {
                    display: false
                },
                datalabels: {
                    color: '#000',
                    formatter: function (value) {
                        return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value);
                    },
                    anchor: 'end',
                    align: 'top',
                    offset: 4
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value);
                        }
                    }
                }
            }
        }
    });


    var incomeLast6MonthsPesosChart = document.getElementById('incomeLast6MonthsPesosChart').getContext('2d');
    incomeLast6MonthsPesosChartInstance = new Chart(incomeLast6MonthsPesosChart, {
        type: 'bar',
        data: {
            labels: data.ingresosUltMeses.map(item => item.mesNombre),
            datasets: [{
                label: 'Ingresos en Pesos',
                data: data.ingresosUltMeses.map(item => item.total),
                backgroundColor: 'rgba(75, 192, 192, 0.2)',  // Verde claro
                borderColor: 'rgba(75, 192, 192, 1)',        // Verde oscuro
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Ingresos por Mes',
                    font: {
                        size: 18
                    },
                    padding: {
                        bottom: 10
                    }
                },
                legend: {
                    display: false
                },
                datalabels: {
                    color: '#000',
                    formatter: function (value) {
                        return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value);
                    },
                    anchor: 'end',
                    align: 'top',
                    offset: 4
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value);
                        }
                    }
                }
            }
        }
    });


    var expenseLast6MonthsPesosChart = document.getElementById('expenseLast6MonthsPesosChart').getContext('2d');
    expenseLast6MonthsPesosChartInstance = new Chart(expenseLast6MonthsPesosChart, {
        type: 'bar',
        data: {
            labels: data.egresosUltMeses.map(item => item.mesNombre),
            datasets: [{
                label: 'Egresos en Pesos',
                data: data.egresosUltMeses.map(item => item.total),
                backgroundColor: 'rgba(255, 99, 132, 0.2)',  // Rojo claro
                borderColor: 'rgba(255, 99, 132, 1)',        // Rojo oscuro
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Egresos por Mes',
                    font: {
                        size: 18
                    },
                    padding: {
                        bottom: 10
                    }
                },
                legend: {
                    display: false
                },
                datalabels: {
                    color: '#000',
                    formatter: function (value) {
                        return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value);
                    },
                    anchor: 'end',
                    align: 'top',
                    offset: 4
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value);
                        }
                    }
                }
            }
        }
    });




    var expenseByClassPesosChart = document.getElementById('expenseByClassPesosChart').getContext('2d');
    expenseByClassPesosChartInstance = new Chart(expenseByClassPesosChart, {
        type: 'bar',
        data: {
            labels: data.egresosPorClase.map(item => item.claseMovimiento),
            datasets: [{
                label: 'Egresos en Pesos',
                data: data.egresosPorClase.map(item => item.total),
                backgroundColor: 'rgba(255, 99, 132, 0.2)',  // Rojo claro
                borderColor: 'rgba(255, 99, 132, 1)',        // Rojo oscuro
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Egresos por Clase',
                    font: {
                        size: 18
                    },
                    padding: {
                        bottom: 10
                    }
                },
                legend: {
                    display: false
                },
                datalabels: {
                    color: '#000',
                    formatter: function (value) {
                        return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value);
                    },
                    anchor: 'end',
                    align: 'top',
                    offset: 4
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value);
                        }
                    }
                }
            }
        }
    });

}


function cargarDatosDashboard3() {
    var selectedCard = document.getElementById('idTarjeta').value;
    if (selectedCard != null) {
        fetch('/Estadistica/getDataDB3', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(selectedCard)
        })
            .then(response => response.json())
            .then(data => {
                updateGraficosDB3(data);
            });
    }    
}

function updateGraficosDB3(data) {
    if (gastosEnPesosChartInstance) {
        gastosEnPesosChartInstance.destroy();
    }
    var gastosEnPesosChart = document.getElementById('gastosEnPesosChart').getContext('2d');
    //console.log(data)
    var currentMonthIndex = 6;

    var labels = data.tarjetaPesos.map(item => item.mesNombre);

    var gastos = data.tarjetaPesos.map(item => item.total);
    

    var backgroundColors = gastos.map((gasto, index) => {
        return index < currentMonthIndex ? 'rgba(255, 99, 132, 0.2)' : 'rgba(75, 192, 192, 0.2)'; // rojo para meses anteriores, verde para posteriores
    });

    var borderColors = gastos.map((gasto, index) => {
        return index < currentMonthIndex ? 'rgba(255, 99, 132, 1)' : 'rgba(75, 192, 192, 1)'; // bordes en rojo y verde
    });

    gastosEnPesosChartInstance = new Chart(gastosEnPesosChart, {
        
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Gastos en Pesos',
                    data: gastos,
                    backgroundColor: backgroundColors,
                    borderColor: borderColors,
                    borderWidth: 1
                }]
            },
            options: {
                plugins: {
                    title: {
                        display: true,
                        text: 'Gastos en Pesos'
                    },
                    tooltip: {
                        callbacks: {
                            label: function (tooltipItem) {
                                return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(tooltipItem.raw);
                            }
                        }
                    },
                    legend: {
                        display: true,
                        labels: {
                            generateLabels: function (chart) {
                                return [
                                    {
                                        text: '6 Meses Anteriores',
                                        fillStyle: 'rgba(255, 99, 132, 0.2)',
                                        strokeStyle: 'rgba(255, 99, 132, 1)',
                                        lineWidth: 1
                                    },
                                    {
                                        text: '6 Meses Posteriores',
                                        fillStyle: 'rgba(75, 192, 192, 0.2)',
                                        strokeStyle: 'rgba(75, 192, 192, 1)',
                                        lineWidth: 1
                                    }
                                ];
                            }
                        }
                    },
                    datalabels: {
                        display: true,
                        color: '#000',
                        formatter: function (value) {
                            return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value);
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function (value) {
                                return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value);
                            }
                        }
                    }
                }
            }
    });

    if (gastosEnDolaresChartInstance) {
        gastosEnDolaresChartInstance.destroy();
    }
    var gastosEnDolaresChart = document.getElementById('gastosEnDolaresChart').getContext('2d');
    //console.log(data)
    var currentMonthIndex = 6;

    var labels = data.tarjetaDolares.map(item => item.mesNombre);

    var gastos = data.tarjetaDolares.map(item => item.total);


    var backgroundColors = gastos.map((gasto, index) => {
        return index < currentMonthIndex ? 'rgba(255, 99, 132, 0.2)' : 'rgba(75, 192, 192, 0.2)'; // rojo para meses anteriores, verde para posteriores
    });

    var borderColors = gastos.map((gasto, index) => {
        return index < currentMonthIndex ? 'rgba(255, 99, 132, 1)' : 'rgba(75, 192, 192, 1)'; // bordes en rojo y verde
    });

    gastosEnDolaresChartInstance = new Chart(gastosEnDolaresChart, {

        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Gastos en Dolares',
                data: gastos,
                backgroundColor: backgroundColors,
                borderColor: borderColors,
                borderWidth: 1
            }]
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    text: 'Gastos en Dolares'
                },
                tooltip: {
                    callbacks: {
                        label: function (tooltipItem) {
                            return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'USD' }).format(tooltipItem.raw);
                        }
                    }
                },
                legend: {
                    display: true,
                    labels: {
                        generateLabels: function (chart) {
                            return [
                                {
                                    text: '6 Meses Anteriores',
                                    fillStyle: 'rgba(255, 99, 132, 0.2)',
                                    strokeStyle: 'rgba(255, 99, 132, 1)',
                                    lineWidth: 1
                                },
                                {
                                    text: '6 Meses Posteriores',
                                    fillStyle: 'rgba(75, 192, 192, 0.2)',
                                    strokeStyle: 'rgba(75, 192, 192, 1)',
                                    lineWidth: 1
                                }
                            ];
                        }
                    }
                },
                datalabels: {
                    display: true,
                    color: '#000',
                    formatter: function (value) {
                        return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'USD' }).format(value);
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'USD' }).format(value);
                        }
                    }
                }
            }
        }
    });

    var tableGastosMes = document.getElementById('tablaGastos');
    tableGastosMes.innerHTML = '';

    var tableTotal = '';
    var baseTable = '<div class="card"> ' +
        '<div class="card-header"> ' +
        '<h5>Gastos en el mes Actual </h5>' +
        '   </div>' +
        '<div class="card-body"> ' +
        '<div class="table-responsive">' +
        '   <table class="table table-bordered table-hover">' +
        '    <thead>' +
        '   <tr>' +
        '   <th>Tarjeta </th>' +
        ' <th> Moneda </th>' +
        '   <th>Fecha </th>' +
        '  <th> Tipo Movimiento </th>' +
        '   <th> Detalle </th>' +
        ' <th> Cuota </th>' +
        ' <th> Monto Cuota </th>' +
        ' <th> Valor Pesos </th>' +
        '   </tr>' +
        '  </thead>' +
        '<tbody id="gastosTableBody" >';

    tableTotal = baseTable;

    $.each(data.gastosTarjetaMes, function (i, movimiento) {
        var row = '<tr>' +
            '<td>' + movimiento.nombreTarj + '</td>' +
            '<td>' + movimiento.nombreMoneda + '</td>' +
            '<td>' + movimiento.fechaMov.toString("yyyy-MM-dd") + '</td>' +
            '<td>' + movimiento.tipoMov + '</td>' +
            '<td>' + movimiento.detalle + '</td>' +
            '<td>' + movimiento.cuotaTexto + '</td>' +
            '<td>' + movimiento.montoCuota + '</td>' +
            '<td>' + movimiento.valorPesos + '</td>' +
            '</tr>';
        tableTotal = tableTotal + row;
    });
    baseTable = '</tbody>' +
        '</table>' +
        '</div>' +
        '</div>' +
        '</div>';

    tableTotal = tableTotal + baseTable;


    tableGastosMes.innerHTML = tableTotal ;
}

function cargarDatosDashboard4() {
    var selectedTipoActivo = document.getElementById('idTipoActivo').value;
    //console.log(JSON.stringify({ "selectedMonth": selectedMonth }));
    fetch('/Estadistica/getDataDB4', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(selectedTipoActivo)
    })
        .then(response => response.json())
        .then(data => {
            updateGraficosDB4(data);
        });
}

function updateGraficosDB4(data) {

    //graph 1 porcentaje
    if (porcentajeTickerChartInstance) {
        porcentajeTickerChartInstance.destroy();
    }

    var porcentajeTickersChart = document.getElementById('porcentajeTickersChart').getContext('2d'); 
    //console.log(data.porcentajeTicker)
    var tickers = data.bolsaEstadistica1.map(ticker => ticker.nombreActivo);
    var simbolos = data.bolsaEstadistica1.map(ticker => ticker.simbolo);
    var valoresActuales = data.bolsaEstadistica1.map(ticker => ticker.valorActual);

    // Generar colores controlados para evitar problemas de contraste
    var coloresControlados = generarColoresControlados(valoresActuales.length);

       porcentajeTickerChartInstance = new Chart(porcentajeTickersChart, {
        type: 'pie',
        data: {
            labels: simbolos,
            datasets: [{
                data: valoresActuales,
                backgroundColor: coloresControlados,
                hoverOffset: 4
            }]
        },
        options: {
            plugins: {
                legend: {
                    display: true,
                    position: 'right'
                },
                title: {
                    display: true,
                    text: 'Distribución por Ticker (En Dólares)'
                },
                tooltip: {
                    callbacks: {
                        label: function (tooltipItem) {
                            let total = tooltipItem.dataset.data.reduce((a, b) => a + b, 0);
                            let percentage = (tooltipItem.raw / total * 100).toFixed(2);
                            let nombreLargo = tickers[tooltipItem.dataIndex];  // Nombre largo del ticker
                            let simbolo = simbolos[tooltipItem.dataIndex];  // Símbolo del ticker
                            return `${nombreLargo} (${simbolo}): ${new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(tooltipItem.raw)} (${percentage}%)`;
                        }
                    }
                },
                datalabels: {
                    display: true,
                    color: 'white',
                    align: 'center',
                    anchor: 'center',
                    font: {
                        weight: 'bold'
                    },
                    formatter: function (value, context) {
                        // Calcula el porcentaje de la porción
                        let total = context.chart.data.datasets[0].data.reduce((a, b) => a + b, 0);
                        let porcentaje = (value / total * 100).toFixed(2);

                        // Solo muestra la etiqueta si el porcentaje es mayor que 5%
                        if (porcentaje > 5) {
                            return context.chart.data.labels[context.dataIndex];  // Mostrar símbolo solo si la porción es mayor al 5%
                        } else {
                            return '';  // No muestra etiqueta si la porción es pequeña
                        }
                    }
                }
            }
           },
           plugins: [ChartDataLabels]  // Asegúrate de habilitar el plugin
       });

    //graph 2 orig vs actual
    if (origVsActualChartInstance) {
        origVsActualChartInstance.destroy();
    }

    var origVsActualChart = document.getElementById('origVsActualChart').getContext('2d');

    origVsActualChartInstance = new Chart(origVsActualChart, {
        type: 'bar',
        data: {
            labels: data.bolsaEstadistica1.map(item => item.simbolo),
            datasets: [{
                label: 'Valores Originales',
                data: data.bolsaEstadistica1.map(item => item.valorOrigen),
                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                borderColor: 'rgba(255, 99, 132, 1)',
                borderWidth: 1
            },
                {
                    label: 'Valores Actuales',
                    data: valoresActuales,
                    backgroundColor: 'rgba(75, 192, 192, 0.2)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1
                }]
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    text: 'Comparación Valor Original vs Valor Actual (En Dólares)'
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value);
                        }
                    }
                }
            }
        }
    });


    //tabla bolsa
    var tableBolsa = document.getElementById('tablaBolsa');
    tableBolsa.innerHTML = '';

    var tableTotal = '';
    var baseTable = '<div class="card"> ' +
        '<div class="card-header"> ' +
        '<h5>Gastos en el mes Actual </h5>' +
        '   </div>' +
        '<div class="card-body"> ' +
        '<div class="table-responsive">' +
        '   <table class="table table-bordered table-hover">' +
        '    <thead>' +
        '   <tr>' +
        '   <th>Ticker</th>' +
        '   <th>Nombre</th>' +
        ' <th> Cantidad</th>' +
        '   <th>Valor Origen</th>' +
        '  <th>Valor Actual</th>' +
        '   </tr>' +
        '  </thead>' +
        '<tbody id="bolsaTableBody" >';

    tableTotal = baseTable;

    $.each(data.bolsaEstadistica1, function (i, ticker) {
        var row = '<tr>' +
            '<td>' + ticker.simbolo + '</td>' +
            '<td>' + ticker.nombreActivo + '</td>' +
            '<td>' + ticker.cantidad + '</td>' +
            '<td>' + ticker.valorOrigen + '</td>' +
            '<td>' + ticker.valorActual + '</td>' +
            '</tr>';
        tableTotal = tableTotal + row;
    });
    baseTable = '</tbody>' +
        '</table>' +
        '</div>' +
        '</div>' +
        '</div>';

    tableTotal = tableTotal + baseTable;


    tableBolsa.innerHTML = tableTotal;


    //graph 4 bolsa gral
    if (bolsaGralChartInstance) {
        bolsaGralChartInstance.destroy();
    }

    var bolsaGralChart = document.getElementById('bolsaGralChart').getContext('2d');
    //console.log(data.porcentajeTicker)
    var tiposActivo = data.bolsaEstadistica2.map(ticker => ticker.tipoActivo);
    var valoresActuales = data.bolsaEstadistica2.map(ticker => ticker.valorActual);

    // Generar colores controlados para evitar problemas de contraste
    var coloresControlados = generarColoresControlados(valoresActuales.length);

    bolsaGralChartInstance = new Chart(bolsaGralChart, {
        type: 'pie',
        data: {
            labels: tiposActivo,
            datasets: [{
                data: valoresActuales,
                backgroundColor: coloresControlados,
                hoverOffset: 4
            }]
        },
        options: {
            plugins: {
                legend: {
                    display: true,
                    position: 'right'
                },
                title: {
                    display: true,
                    text: 'Distribución por Tipo de Activo Bolsa (En Dólares)'
                },
                tooltip: {
                    callbacks: {
                        label: function (tooltipItem) {
                            let total = tooltipItem.dataset.data.reduce((a, b) => a + b, 0);
                            let percentage = (tooltipItem.raw / total * 100).toFixed(2);
                            return `${new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(tooltipItem.raw)} (${percentage}%)`;
                        }
                    }
                },
                datalabels: {
                    display: true,
                    color: 'white',
                    align: 'center',
                    anchor: 'center',
                    font: {
                        weight: 'bold'
                    },
                    formatter: function (value, context) {
                        // Calcula el porcentaje de la porción
                        let total = context.chart.data.datasets[0].data.reduce((a, b) => a + b, 0);
                        let porcentaje = (value / total * 100).toFixed(2);

                        // Solo muestra la etiqueta si el porcentaje es mayor que 5%
                        if (porcentaje > 5) {
                            return context.chart.data.labels[context.dataIndex];  // Mostrar símbolo solo si la porción es mayor al 5%
                        } else {
                            return '';  // No muestra etiqueta si la porción es pequeña
                        }
                    }
                }
            }
        },
        plugins: [ChartDataLabels]  // Asegúrate de habilitar el plugin
    });
}

function generarColoresControlados(cantidad) {
    var colores = [];
    for (var i = 0; i < cantidad; i++) {
        var hue = Math.floor(Math.random() * 360);  // Varía el tono entre 0 y 360 grados (todos los colores)
        var saturation = Math.floor(Math.random() * (100 - 60) + 60);  // Saturación alta (60% a 100%) para colores vivos
        var lightness = Math.floor(Math.random() * (70 - 40) + 40);  // Evita colores muy oscuros o muy claros (40% a 70%)

        var color = `hsl(${hue}, ${saturation}%, ${lightness}%)`;
        colores.push(color);
    }
    return colores;
}