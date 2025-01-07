
function loadRadialChart(id, data) {
    var chartColors = getChartColorArray(id);
    var options = {
        chart: {
            height: 120,
            width: 120,
            type: "radialBar",
        },

        series: data.series,
        colors: [chartColors[0]],
        plotOptions: {
            radialBar: {

                track: {
                    dropShadow: {
                        enabled: true,
                        top: 2,
                        left: 0,
                        blur: 4,
                        opacity: 0.15
                    }
                },
                dataLabels: {
                    name: {
                       show:false
                    },
                    value: {
                        offsetY: 6,
                        color: "#fff",
                        fontSize: "15px",
                        show: true
                    }
                }
            }
        },
        fill: {
            type: "gradient",
            gradient: {
                shade: "dark",
                type: "vertical",
                gradientToColors: [chartColors[1]],
                stops: [0, 100]
            }
        },
        stroke: {
            lineCap: "round"
        }
    };

    var chart = new ApexCharts(document.querySelector("#" + id), options);

    chart.render();

}
function getChartColorArray(id) {
    //parse the colors
    // colors = '["--color-primary", "red", "#20E647"]'
    // the result >> ["#20E647", "red", "#20E647"]
    if (document.getElementById(id) !== null)
    {
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