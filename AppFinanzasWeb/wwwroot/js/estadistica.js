var incomeByClassChartInstance = null;
var expenseByClassChartInstance = null;

function cargarDatosDashboard1() {
    var selectedMonth = document.getElementById('MesDB1').value;
    console.log(JSON.stringify({ "selectedMonth": selectedMonth }));
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
    if (expenseByClassChartInstance) {
        expenseByClassChartInstance.destroy();
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