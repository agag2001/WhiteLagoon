using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.DTO;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Application.Utilities;


namespace WhiteLagoon.Application.Services.Implementation
{
    public class DashboardService : IDashboardService
    {
        static int lastMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        static int lastYear = lastMonth == 12 ? DateTime.Now.Year - 1 : DateTime.Now.Year;

        // current month  1/1/2025  >>>lastmonth =12
        // last Year  =  2024 because the last month is 12 
        readonly DateTime previousMonth = new DateTime(lastYear, lastMonth, 1);
        readonly DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        private readonly IUnitOfWork _unitOfWork;


        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<RedialChartDto> GetBookingsRedialChartData()
        {
            var bookings = _unitOfWork.Booking.GetAll(b => b.Status != SD.StatusPending && b.Status != SD.StatusCancelled);
            var currentMonthBookings_count = bookings.Where(b => b.BookingDate >= currentMonth && b.BookingDate <= DateTime.Now).Count();
            var lastMonthBookings_count = bookings.Where(b => b.BookingDate >= previousMonth && b.BookingDate <= currentMonth).Count();

            return (SD.GetRedialChartDataModel(bookings.Count(), currentMonthBookings_count, lastMonthBookings_count));
        }

        public async Task<PieChartDto> GetCustomerBookingsPieChartData()
        {

            var bookings = _unitOfWork.Booking.GetAll(b => b.Status != SD.StatusPending && b.Status != SD.StatusCancelled
                                                     && b.BookingDate >= DateTime.Now.AddDays(-30));

            // customer who book only one time
            var customerWithOneBooking = bookings.GroupBy(x => x.UserId).Where(x => x.Count() == 1).Select(x => x.Key).ToList();
            // number of customers that only booked one time in my villas
            var newCustomersBooking = customerWithOneBooking.Count;
            // customers that return to book again >> they liked my services
            var bookingAgainCustomers = bookings.Count() - newCustomersBooking;

            PieChartDto PieChartDto = new()
            {
                Series = [newCustomersBooking, bookingAgainCustomers],
                Labels = ["New Customer Booking", "Returning Customer booking"]
            };


            return PieChartDto;

        }

        public async Task<LineChartDto> GetCustomersAndBookingLineChart()
        {
            var numOfBookingInLast30Days = _unitOfWork.Booking.GetAll(b => b.BookingDate >= DateTime.Now.AddDays(-30) && b.BookingDate <= DateTime.Now && b.Status != SD.StatusCancelled).
                                             GroupBy(b => b.BookingDate.Date).Select(x => new
                                             {
                                                 bookingDate = x.Key,
                                                 count = x.Count()
                                             });
            var numOfCustomersInLast30Days = _unitOfWork.User.GetAll(b => b.CreatedAt >= DateTime.Now.AddDays(-30)).
                                           GroupBy(b => b.CreatedAt.Date).Select(x => new
                                           {
                                               bookingDate = x.Key,
                                               count = x.Count()
                                           });
            // left join between the bookin and the new customers on the BookingDate
            // ex 1/1/2025 >> booking =6 cutomer =1 

            var bookingLeftJoinCustomer = from b in numOfBookingInLast30Days
                                          join c in numOfCustomersInLast30Days on b.bookingDate equals c.bookingDate into bookingLeftJoinCusotmer
                                          from BC in bookingLeftJoinCusotmer.DefaultIfEmpty()
                                          select new
                                          {
                                              b.bookingDate,
                                              bookingCount = b.count,
                                              newCustomerCount = BC != null ? BC.count : 0
                                          };
            var cutomerLeftJoinBooking = from c in numOfCustomersInLast30Days
                                         join b in numOfBookingInLast30Days on c.bookingDate equals b.bookingDate into customerLeftJoinbooking
                                         from BC in customerLeftJoinbooking.DefaultIfEmpty()
                                         select new
                                         {
                                             c.bookingDate,
                                             bookingCount = BC != null ? BC.count : 0,
                                             newCustomerCount = c.count
                                         };

            // remove duplicate records by Union
            var mergedCustomerAndBooking = bookingLeftJoinCustomer.Union(cutomerLeftJoinBooking).OrderBy(x => x.bookingDate).ToList();

            // retrive the data for the view model

            var newBooking = mergedCustomerAndBooking.Select(x => x.bookingCount).ToArray();
            var newCustomer = mergedCustomerAndBooking.Select(x => x.newCustomerCount).ToArray();
            string[] catigores = mergedCustomerAndBooking.Select(x => x.bookingDate.ToString("MM/dd/yyyy")).ToArray();
            List<LineItem> series = new()
            {
                new LineItem
                {
                    Name = "New Bookings",
                    Data = newBooking
                },
                new LineItem
                {
                    Name = "New Clients",
                    Data = newCustomer
                }
            };

            LineChartDto LineChartDto = new()
            {
                Series = series,
                Categories = catigores
            };

            return LineChartDto;
        }

        public async Task<RedialChartDto> GetRevenuesRedialChartData()
        {
            // money money money عاوز فلوووووووس
            var bookings = _unitOfWork.Booking.GetAll();
            var totalRevenues = Convert.ToInt32(bookings.Sum(x => x.TotalCost));
            var currentMonthBookings_count = bookings.Where(b => b.BookingDate >= currentMonth && b.BookingDate <= DateTime.Now).Sum(x => x.TotalCost);
            var lastMonthBookings_count = bookings.Where(b => b.BookingDate >= previousMonth && b.BookingDate <= currentMonth).Sum(x => x.TotalCost);


            return SD.GetRedialChartDataModel(totalRevenues, currentMonthBookings_count, lastMonthBookings_count);
        }
        
        public async Task<RedialChartDto> GetUsersRedialChartData()
        {
            var users = _unitOfWork.User.GetAll();
            var currentMonthBookings_count = users.Where(u => u.CreatedAt >= currentMonth && u.CreatedAt <= DateTime.Now).Count();
            var lastMonthBookings_count = users.Where(u => u.CreatedAt >= previousMonth && u.CreatedAt <= currentMonth).Count();


            return SD.GetRedialChartDataModel(users.Count(), currentMonthBookings_count, lastMonthBookings_count);
        }

       
    }
}
