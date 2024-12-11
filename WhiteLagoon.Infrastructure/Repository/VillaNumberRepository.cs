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
	public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
	{
		private readonly AppDbContext _context;

		public VillaNumberRepository(AppDbContext context):base(context) 
        {
			_context = context;
		}
        public void Update(VillaNumber entity)
		{
			_context.Update<VillaNumber>(entity);
		}
	}
}
