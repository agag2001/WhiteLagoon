using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entites;

namespace WhiteLagoon.Application.Services.Interface
{
    public  interface IVillaService
    {
        IEnumerable<Villa> GetAllVillas(string IncludeProperties=null);
        Villa GetVillaById(int id, string IncludeProperties=null);     
        bool CreateVilla(Villa villa);  
        bool UpdateVilla(Villa villa);  
        bool DeleteVilla(int id); 
        IEnumerable<Villa> GetAvaliableVillaByDate(int nights, DateOnly checkInDate);
        bool IsVillaRoomAvailable(int villaId, int nights,DateOnly checkInDate);    
    }
}
