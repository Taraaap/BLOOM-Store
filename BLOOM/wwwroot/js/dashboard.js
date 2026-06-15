const chartColors = ['#FF6384', '#FF9F40', '#FFCD56', '#4BC0C0', '#36A2EB', '#9966FF', '#C9CBCF'];

const statusColorMap = {
    'Pending': '#FFCD56',
    'Approved': '#4BC0C0',
    'Processing': '#36A2EB',
    'Shipped': '#9966FF',
    'Cancelled': '#FF6384',
    'Refunded': '#FF9F40',
    'Unknown': '#C9CBCF'
};


$.getJSON('/Admin/Dashboard/GetChartData', function (data) {

    new Chart(document.getElementById('revenueChart'), {
        type: 'line',
        data: {
            labels: data.monthlyRevenue.map(r => r.label),
            datasets: [{
                label: 'Revenue',
                data: data.monthlyRevenue.map(r => r.revenue),
                fill: true,
                borderColor: chartColors[3],
                backgroundColor: chartColors[3] + '20',
                tension: 0.4,
                borderWidth: 1,
                pointRadius: 3,
                pointHoverRadius: 6,
                pointBackgroundcolor: chartColors[3]
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    grid: {
                        color: 'rgba(0,0,0,0.05)',
                        drawBorder: false
                    },
                    ticks: {
                        callbacks: v => '$' + v,
                        padding: 10
                    }
                },
                x: {
                    grid: { display: false },
                    ticks: { padding: 10 }
                }
            }
        }
    });


    new Chart(document.getElementById('ordersChart'), {
        type: 'bar',
        data: {
            labels: data.monthlyOrders.map(r => r.label),
            datasets: [{
                label: 'Order',
                data: data.monthlyOrders.map(r => r.count),
                borderColor: chartColors[0],
                backgroundColor: chartColors[0] + '80',
                borderWidth: 1,
                borderRadius: 8,
                hoverBackgroundColor: chartColors[0]
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: {
                        color: 'rgba(0,0,0,0.05)',
                        drawBorder: false
                    },
                    ticks: {
                        stepSize: 1,
                        padding: 10
                    }
                },
                x: {
                    grid: { display: false },
                    ticks: { padding: 10 }
                }
            }
        }   
    });     


    new Chart(document.getElementById('statusChart'), {
        type: 'doughnut',
        data: {
            labels: data.statusBreakdown.map(r => r.status),
            datasets: [{
                label: 'Order Status',
                data: data.statusBreakdown.map(r => r.count),
                backgroundColor: data.statusBreakdown.map(s => statusColorMap[s.status] || '#999'),
                borderWidth: 2,
                borderColor: '#fff',
                hoverBorderWidth: 3,
                hoverOffset: 10
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: '70',
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        padding: 15,
                        usePointStyle: true,
                        pointStyle: 'circle',
                        font: { size: 12 }
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(0,0,0,0.8)',
                    padding: 12,
                    cornerRadius: 8
                }
            }
        }
    });


    new Chart(document.getElementById('categoryChart'), {
        type: 'bar',
        data: {
            labels: data.productPerCategory.map(r => r.category),
            datasets: [{
                label: 'Products by Category',
                data: data.productPerCategory.map(r => r.count),
                backgroundColor: chartColors.map(c => c + '80'),
                borderColor: chartColors,
                borderWidth: 1,
                borderRadius: 8,
                hoverBackgroundColor: chartColors
            }]
        },
        options: {
            indexAxis: "y",
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: 'rgba(0,0,0,0.8)',
                    padding: 12,
                    cornerRadius: 8,
                    displayColors: false
                }
            },
            scales: {
                x: {
                    beginAtZero: true,
                    grid: {
                        color: 'rgba(0,0,0,0.05)',
                        drawBorder: false
                    },
                    ticks: {
                        stepSize: 1,
                        padding: 10
                    }
                },
                y: {
                    grid: { display: false },
                    ticks: { padding: 10 }
                }
            }
        }
    });

}); 