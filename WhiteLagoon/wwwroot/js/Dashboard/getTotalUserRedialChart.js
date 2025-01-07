

$(document).ready(function () {
    loadTotalUserRadialChart();
})

function loadTotalUserRadialChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetUsersRedialChartData",
        type: 'GET',
        dataType: "json",
        success: function (data) {

            document.querySelector('#spanTotalUserCount').innerHTML = data.totalCount;

            var currentCount = document.createElement('span');

            if (data.isRatioIncrease) {
                currentCount.className = 'text-success me-1';
                currentCount.innerHTML = '<i class="bi bi-arrow-up-right-circle"></i> <span> +' + data.countInCurrentMonth + '</span>';
            } else {
                currentCount.className = 'text-danger me-1';
                currentCount.innerHTML = '<i class="bi bi-arrow-down-right-circle"></i> <span>+' + data.countInCurrentMonth + '</span>';
            }

            document.querySelector('#sectionUserCount').append(currentCount);
            document.querySelector('#sectionUserCount').append('Since Last Month');

            loadRadialChart('totalUsersRadialChart',data)
            $('.chart-spinner').hide();
        }
    });
}