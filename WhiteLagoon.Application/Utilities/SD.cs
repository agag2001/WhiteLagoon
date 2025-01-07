using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.DTO;
using WhiteLagoon.Domain.Entites;

namespace WhiteLagoon.Application.Utilities
{
    public static class SD
    {
        public const string Role_Admin = "Admin";
        public const string Role_Customer = "Customer";
               
        public const string StatusPending   = "Pending";
        public const string StatusApproved  = "Approved";
        public const string StatusCheckIn   = "CheckIn";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefused   = "Refused";
        

        // function that check the avaliabliity of villa
        public static int VillaRoomAvailable_Count(int villaId,List<Booking> bookings,List<VillaNumber> villaNumbers
                    , int nights, DateOnly checkInDate)
        {
            // use this to store the booking id of the overlapped with our booking
            List<int> bookedId = new();
            var roomsInVilla = villaNumbers.Where(v => v.Villa_id == villaId).Count();
            int FinalAvailabelRooms =  int.MinValue;

            for(int i= 0; i<nights; i++)
            {
                // overlapped booking with my booking
                var bookedVillas = bookings.Where(b => b.CheckInDate <= checkInDate.AddDays(i) && b.CheckOutDate > checkInDate.AddDays(i)
                                                  && b.VillaId == villaId);
                foreach(var booking in bookedVillas)
                {
                    if(!bookedId.Contains(booking.Id))
                        bookedId.Add(booking.Id);   

                    // now i have all the overlaped booking with my villa and it's booking id is stored in bookedid
                    // this over lapped for the first night and we loop for alll the nights we need to stay

                }
                // bookedId contains all of the id for (i)  night
                int availableRoomsForNight =  roomsInVilla - bookedId.Count;
                if(availableRoomsForNight <= 0)
                {
                    return 0;
                }
                // now we want to store the lowest available rooms for all nights
                // for ex if i have <2 ,1 ,1> so the availble rooms in my villa in your Stays will be only One
                else
                {
                    FinalAvailabelRooms  = availableRoomsForNight;
                }

                
            }
            return FinalAvailabelRooms;

        }

        public static RedialChartDto GetRedialChartDataModel(int ModelCount, double currentMonthCount, double lastMonthCount)
        {
            int increaseDecreaseRatio = 100;
            if (lastMonthCount != 0)
            {
                // Use floating-point division to avoid truncation
                increaseDecreaseRatio = (int)(((double)(currentMonthCount - lastMonthCount) / lastMonthCount) * 100);
            }
            RedialChartDto RedialChartDto = new RedialChartDto()
            {
                TotalCount = ModelCount,
                CountInCurrentMonth = currentMonthCount,
                IsRatioIncrease = currentMonthCount > lastMonthCount,
                Series = new int[] { increaseDecreaseRatio }

            };
            return RedialChartDto;
        }

    }
}
