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
    
    public class VillaNumberService : IVillaNumberService
    {
        private readonly IUnitOfWork _unitOfWork;
        public VillaNumberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public bool CheckVillaNumberExist(int villa_number)
        {
           return _unitOfWork.VillaNumber.Any(x => x.Villa_Number == villa_number);
        }

        public bool CreateVillaNumber(VillaNumber villaNumber)
        {

            try
            {

                _unitOfWork.VillaNumber.Add(villaNumber);
                _unitOfWork.Save();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
         

        }

        public bool DeleteVillaNumber(int villa_number)
        {
            var villaNumberDb = _unitOfWork.VillaNumber.Get(x => x.Villa_Number == villa_number);
            if (villaNumberDb is not null)
            {
                _unitOfWork.VillaNumber.Remove(villaNumberDb);
                _unitOfWork.Save();
                return true;    
            }
            return false;
        }
        public IEnumerable<VillaNumber> GetAllVillaNumbers(string includeProperties = null)
        {
            return _unitOfWork.VillaNumber.GetAll(includeProperties: includeProperties);
        }

        public VillaNumber GetVillaNumberById(int id, string includeProperties = null)
        {
            return _unitOfWork.VillaNumber.Get(x=>x.Villa_Number==id, includeProperties: includeProperties);    
        }

        public bool UpdateVillaNumber(VillaNumber villa)
        {
            try
            {
                _unitOfWork.VillaNumber.Update(villa);
                _unitOfWork.Save();
                return true;
            }
            catch { return false; } 
          
        }
    }
}
