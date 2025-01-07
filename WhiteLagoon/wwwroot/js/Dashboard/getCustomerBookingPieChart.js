

$(document).ready(function () {
    loadCustomerBookingPieChart();
})

function loadCustomerBookingPieChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetCustomerBookingsPieChartData",
        type: 'GET',
        dataType: "json",
        success: function (data) {

            loadPieChart('CustomerBookingPieChart',data)
            $('.chart-spinner').hide();
        }
    });
}

function loadPieChart(id , data) {
    var chartColors = getChartColorsArray(id);
    var options = {
        colors: chartColors,
        series: data.series,
        labels: data.labels,
        chart: {
            width: 380,
            type: 'pie',
        },
        stroke: {
            show: false
        },
        legend: {
            position: 'bottom',
            horizontalAlign: 'center',
            labels: {
                colors: "#fff",
                useSeriesColors: true
            },
        },
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