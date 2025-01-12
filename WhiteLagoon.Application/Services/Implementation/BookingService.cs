using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Application.Utilities;
using WhiteLagoon.Domain.Entites;

namespace WhiteLagoon.Application.Services.Implementation
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void CreateBooking(Booking booking)
        {
            _unitOfWork.Booking.Add(booking);
            _unitOfWork.Save();

                
        }

        public IEnumerable<Booking> GetAllBooking(string userId = "", string? statusFilterList = "")
        {
            var statusList =  statusFilterList.ToLower().Split(',');    
            if(!string.IsNullOrEmpty(userId)&& !string.IsNullOrEmpty(statusFilterList))
            {
                 return _unitOfWork.Booking.GetAll(b=> statusList.Contains(b.Status.ToLower().Trim()) && b.UserId == userId,includeProperties: "Villa,User");       

            }
            else
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    return _unitOfWork.Booking.GetAll(b => b.UserId == userId);
                }
                else if (!string.IsNullOrEmpty(statusFilterList)) 
                {
                    return _unitOfWork.Booking.GetAll(b => statusList.Contains(b.Status.ToLower().Trim()),includeProperties: "Villa,User");
                }
            }
            // no filter return all booking 
            return _unitOfWork.Booking.GetAll();        
        }

        public IEnumerable<int> GetBookedRoomInVilla(int villaId)
        {
            return _unitOfWork.Booking.GetAll(b => b.Status == SD.StatusCheckIn && b.VillaId == villaId)
                                                    .Select(x => x.VillaNumber);
        }

        public Booking GetBookingById(int bookingId)
        {
           return _unitOfWork.Booking.Get(b=>b.Id == bookingId, includeProperties: "Villa,User");       
        }

        public void UpdateStauts(int bookingId, string bookingStatus, int villaNumber = 0)
        {
            var bookingDB = _unitOfWork.Booking.Get(b => b.Id == bookingId, tracked: true);
            if (bookingDB is not null)
            {
                if (!string.IsNullOrEmpty(bookingStatus))
                {
                    bookingDB.Status = bookingStatus;
                    if (bookingStatus == SD.StatusCheckIn)
                    {
                        bookingDB.VillaNumber = villaNumber;
                        bookingDB.ActualCheckInDate = DateTime.Now;
                    }
                    if (bookingStatus == SD.StatusCompleted)
                    {
                        bookingDB.ActualCheckOutDate = DateTime.Now;
                    }
                }

            }
            _unitOfWork.Save();

        }

        public void UpdateStripePaymentId(int bookingId, string sessionId, string paymentIntentId)
        {
            var bookingDB = _unitOfWork.Booking.Get(b => b.Id == bookingId, tracked: true);
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
            _unitOfWork.Save();
        }

    }
}
