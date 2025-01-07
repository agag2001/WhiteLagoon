

$(document).ready(function () {
    loadCustomerAndBookingLineChart();
})

function loadCustomerAndBookingLineChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetCustomersAndBookingLineChart",
        type: 'GET',
        dataType: "json",
        success: function (data) {

            loadLineChart('newCustomerAndBookingLineChart',data)
            $('.chart-spinner').hide();
        }
    });
}

function loadLineChart(id , data) {
    var chartColors = getChartColorsArray(id);
    var options = {
        colors: chartColors,
        chart: {
            height: 350,
            //show area later
            type: 'line',
            zoom: {
                type: 'x',
                enabled: true,
                autoScaleYaxis: true
            },
        },
        stroke: {
            curve: 'smooth',
            width: 2
        },
        series: data.series,
        dataLabels: {
            enabled: false,
        },
        markers: {
            size: 6,
            strokeWidth: 0,
            hover: {
                size: 9
            }
        },
        xaxis: {
            categories: data.categories,
            labels: {
                style: {
                    colors: "#fff",
                },
            }
        },
        yaxis: {
            labels: {
                
                style: {
                    colors: "#fff",
                },
            }
        },
        legend: {
            labels: {
                colors: "#fff",
            },
        },
        tooltip: {
            theme: 'dark'
        }
    };
    var chart = new ApexCharts(document.querySelector("#" + id), options);
    chart.render();
}

function getChartColorsArray(id) {
    //parse the colors
    // colors = '["--color-primary", "red", "#20E647"]'
    // the result >> ["#20E647", "red", "#20E647"]
    if (document.getElementById(id) !== null) {
        var colors = document.getElementById(id).getAttribute("data-colors");

        if (colors) {
            colors = JSON.parse(colors);
            return colors.map(function (value) {
                var newValue = value.replace(" ", "");
                if (newValue.indexOf(",") === -1) {
                    var color = getComputedStyle(document.documentElement).getPropertyValue(newValue);
                    if (color) return color;
                    else return newValue;;
                }
            });
        }


    }
}