using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entites;

namespace WhiteLagoon.Application.Services.Interface
{
    public interface IBookingService
    {
         IEnumerable<Booking> GetAllBooking(string userId="", string? statusFilterList  ="" );
        Booking GetBookingById(int bookingId);
        void CreateBooking(Booking booking);
        void UpdateStripePaymentId(int bookingId, string sessionId, string paymentIntentId);
        void UpdateStauts(int bookingId, string bookingStatus, int villaNumber = 0);
        IEnumerable<int> GetBookedRoomInVilla(int villaId );




    }
}
