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

       
    }
}
