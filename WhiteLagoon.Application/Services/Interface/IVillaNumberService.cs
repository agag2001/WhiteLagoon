using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entites;

namespace WhiteLagoon.Application.Services.Interface
{
    public interface IVillaNumberService
    {
        IEnumerable<VillaNumber> GetAllVillaNumbers(string includeProperties=null);
        VillaNumber GetVillaNumberById(int id, string includeProperties = null);
        bool CreateVillaNumber(VillaNumber villaNumber);
        bool UpdateVillaNumber(VillaNumber villaNumber);
        bool DeleteVillaNumber(int villa_number);
        bool CheckVillaNumberExist(int villa_number);
    }
}
