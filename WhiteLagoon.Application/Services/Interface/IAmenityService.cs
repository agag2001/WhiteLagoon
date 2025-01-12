 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entites;

namespace WhiteLagoon.Application.Services.Interface
{
    public  interface IAmenityService
    {
        IEnumerable<Amenity> GetAllAmenities(string IncludeProperties = null);
        Amenity GetAmenityById(int id, string IncludeProperties = null);
        bool CreateAmenity(Amenity amenity);
        bool UpdateAmenity(Amenity amenity);
        bool DeleteAmenity(int id);
    }
}
