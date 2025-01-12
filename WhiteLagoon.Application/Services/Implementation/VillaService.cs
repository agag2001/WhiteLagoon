using Microsoft.AspNetCore.Hosting;
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
    public class VillaService : IVillaService

    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VillaService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnviroment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnviroment;
        }

        public bool CreateVilla(Villa villa)
        {

           
            if (villa.Image is not null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"Images\VillaImage");

                using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                {
                    villa.Image.CopyTo(fileStream);
                    villa.ImageUrl = @"\Images\VillaImage\" + fileName;
                }
            }
            else
            {
                //default image
                villa.ImageUrl = "https://placehold.co/600x400";
            }
            _unitOfWork.Villa.Add(villa);
            _unitOfWork.Save();
            return true;
            
        }
        public bool DeleteVilla(int id)
        {

            try
            {
                Villa? objDb = _unitOfWork.Villa.Get(x => x.Id == id);
                if (objDb is not null)
                {
                    if (!string.IsNullOrEmpty(objDb.ImageUrl))
                    {
                        string oldPath = Path.Combine(_webHostEnvironment.WebRootPath, objDb.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    _unitOfWork.Villa.Remove(objDb);
                    _unitOfWork.Save();
                    return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public IEnumerable<Villa> GetAllVillas(string IncludeProperties)
        {
               return _unitOfWork.Villa.GetAll(includeProperties:IncludeProperties);
        }

        public IEnumerable<Villa> GetAvaliableVillaByDate(int nights, DateOnly checkInDate)
        {
            var villalist = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity");
            // get only the approved bookings and the checkin (Status) these books may overlapped with my Booking
            var bookings = _unitOfWork.Booking.GetAll(b => b.Status == SD.StatusApproved || b.Status == SD.StatusCheckIn).ToList();

            var villaNumbers = _unitOfWork.VillaNumber.GetAll().ToList();


            foreach (var Villa in villalist)
            {
                var availabelRooms = SD.VillaRoomAvailable_Count(Villa.Id, bookings, villaNumbers, nights, checkInDate);
                Villa.IsAvilable = availabelRooms > 0 ? true : false;

            }
            return villalist;
        }

        public Villa GetVillaById(int id, string IncludeProperties)
        {
            return _unitOfWork.Villa.Get(x=>x.Id== id,IncludeProperties); 

        }

        public bool IsVillaRoomAvailable(int villaId, int nights, DateOnly checkInDate)
        {
            var bookings = _unitOfWork.Booking.GetAll(b => b.Status == SD.StatusApproved || b.Status == SD.StatusCheckIn).ToList();

            var villaNumbers = _unitOfWork.VillaNumber.GetAll().ToList();


            var availabelRooms = SD.VillaRoomAvailable_Count(villaId, bookings, villaNumbers, nights, checkInDate);

            return availabelRooms > 0;
        }

        public bool UpdateVilla(Villa villa)
        {
            try
            {
                if (villa.Image is not null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"Images\VillaImage");
                    //delete old ImageUrl 
                    if (!string.IsNullOrEmpty(villa.ImageUrl))
                    {
                        string oldPath = Path.Combine(_webHostEnvironment.WebRootPath, villa.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    {
                        villa.Image.CopyTo(fileStream);
                        villa.ImageUrl = @"\Images\VillaImage\" + fileName;
                    }
                }

                _unitOfWork.Villa.Update(villa);
                _unitOfWork.Save();
                return true;

            }
            catch (Exception ex)
            {
                return false;   
            }
        }

        
    }
}
