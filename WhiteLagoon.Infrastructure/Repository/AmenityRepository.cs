using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class AmenityRepository : Repository<Amenity>, IAmenityRepository
    {
        private readonly AppDbContext _context;

        public AmenityRepository(AppDbContext context):base(context)
        {
            _context = context;
        }
        public void Update(Amenity entity)
        {
            _context.Update(entity);    
        }
    }
}
