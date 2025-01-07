using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.DTO;


namespace WhiteLagoon.Application.Services.Interface
{
    public interface IDashboardService
    {
        Task<RedialChartDto> GetBookingsRedialChartData();
        Task<RedialChartDto> GetUsersRedialChartData();
        Task<RedialChartDto> GetRevenuesRedialChartData();
        Task<PieChartDto> GetCustomerBookingsPieChartData();
        Task<LineChartDto> GetCustomersAndBookingLineChart();
    }
}
