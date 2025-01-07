

$(document).ready(function () {
    loadTotalBookingRadialChart();
})

function loadTotalBookingRadialChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetBookingsRedialChartData",
        type: 'GET',
        dataType: "json",
        success: function (data) {

            document.querySelector('#spanTotalBookingCount').innerHTML = data.totalCount;

            var currentCount = document.createElement('span');

            if (data.isRatioIncrease) {
                currentCount.className = 'text-success me-1';
                currentCount.innerHTML = '<i class="bi bi-arrow-up-right-circle"></i> <span> +' + data.countInCurrentMonth + '</span>';
            } else {
                currentCount.className = 'text-danger me-1';
                currentCount.innerHTML = '<i class="bi bi-arrow-down-right-circle"></i> <span>+' + data.countInCurrentMonth + '</span>';
            }

            document.querySelector('#sectionBookingCount').append(currentCount);
            document.querySelector('#sectionBookingCount').append('Since Last Month');

            loadRadialChart('totalBookingsRadialChart',data)
            $('.chart-spinner').hide();
        }
    });
}