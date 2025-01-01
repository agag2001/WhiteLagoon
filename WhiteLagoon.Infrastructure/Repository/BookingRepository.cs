using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Utilities;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private AppDbContext _context;
        public BookingRepository(AppDbContext context):base(context)
        {
            _context = context;
        }
        public void Update(Booking entity)
        {
            _context.Bookings.Update(entity);    
            
        }

        public void UpdateStauts(int bookingId, string bookingStatus,int villaNumber=0)
        {
           var bookingDB = _context.Bookings.FirstOrDefault(b=>b.Id==  bookingId);    
           if(bookingDB is not null)
           {
                if (!string.IsNullOrEmpty(bookingStatus))
                {
                    bookingDB.Status = bookingStatus;   
                    if(bookingStatus== SD.StatusCheckIn)
                    {
                        bookingDB.VillaNumber = villaNumber;
                         bookingDB.ActualCheckInDate = DateTime.Now;       
                    }
                    if(bookingStatus == SD.StatusCompleted)
                    {
                        bookingDB.ActualCheckOutDate = DateTime.Now;    
                    }
                }
                
           }
           
        }

        public void UpdateStripePaymentId(int bookingId, string sessionId, string paymentIntentId)
        {
            var bookingDB = _context.Bookings.FirstOrDefault(b => b.Id == bookingId);
            if (bookingDB is not null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    bookingDB.StripSessionId = sessionId;   
                }
                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    bookingDB.StripPaymentIntentId = paymentIntentId;   
                    // if the intentId is not null  mean the customer is complete the payment
                    bookingDB.PaymentDate = DateTime.Now;   
                    bookingDB.IsPaymentSuccessful = true;   
                }
            }
        }
    }
}
