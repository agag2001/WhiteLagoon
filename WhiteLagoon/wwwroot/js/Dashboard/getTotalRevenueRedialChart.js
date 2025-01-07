
$(document).ready(function () {
    loadTotalRevenueRadialChart();
})

function loadTotalRevenueRadialChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: '/Dashboard/GetRevenuesRedialChartData',
        type: 'GET',
        dataType: "json",
        success: function (data) {

            document.querySelector('#spanTotalRevenueCount').innerHTML = data.totalCount + '<i class="bi bi-currency-dollar"></i>';

            var currentCount = document.createElement('span');

            if (data.isRatioIncrease) {
                currentCount.className = 'text-success me-1';
                currentCount.innerHTML = '<i class="bi bi-arrow-up-right-circle"></i> <span> +' + data.countInCurrentMonth + '</span>';
            } else {
                currentCount.className = 'text-danger me-1';
                currentCount.innerHTML = '<i class="bi bi-arrow-down-right-circle"></i> <span>+' + data.countInCurrentMonth + '</span>';
            }

            document.querySelector('#sectionRevenueCount').append(currentCount);
            document.querySelector('#sectionRevenueCount').append('Since Last Month');

            loadRadialChart('totalRevenuesRadialChart', data)
            $('.chart-spinner').hide();
        }
    });
}