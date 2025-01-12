using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Domain.Entites;

namespace WhiteLagoon.Application.Services.Implementation
{
    public class AmenityService : IAmenityService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AmenityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public bool CreateAmenity(Amenity amenity)
        {
            try
            {
                _unitOfWork.Amenity.Add(amenity);
                _unitOfWork.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
          
        }

        public bool DeleteAmenity(int id)
        {
            var amenityDB = _unitOfWork.Amenity.Get(x=>x.Id == id);
            try
            {
                if (amenityDB is not null)
                {
                    _unitOfWork.Amenity.Remove(amenityDB);
                    _unitOfWork.Save();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
          
        }

        public IEnumerable<Amenity> GetAllAmenities(string IncludeProperties = null)
        {
            return _unitOfWork.Amenity.GetAll(includeProperties:IncludeProperties);
        }

        public Amenity GetAmenityById(int id, string IncludeProperties = null)
        {
            return _unitOfWork.Amenity.Get(x=>x.Id == id, includeProperties:IncludeProperties);
        }

        public bool UpdateAmenity(Amenity amenity)
        {
            try
            {
                _unitOfWork.Amenity.Update(amenity);
                _unitOfWork.Save();
                return true; 
            }
            catch(Exception ex)
            {
                return false;
            }
 
        }
    }
}
