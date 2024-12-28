using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly AppDbContext _context;
		public IVillaRepository Villa { get; private set; }

		public IVillaNumberRepository VillaNumber { get; private set; }

		public IAmenityRepository Amenity { get; private set; }

		public IBookingRepository Booking { get; private set; }

		public IAppUserRepository User { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context; 
            Villa =  new VillaRepository(context);
			VillaNumber = new VillaNumberRepository(context);
			User = new AppUserRepository(context);
			Amenity = new AmenityRepository(context);	
			Booking = new BookingRepository(context);		
        }

		public void Save()
		{
			_context.SaveChanges();	
		}
	}
}
