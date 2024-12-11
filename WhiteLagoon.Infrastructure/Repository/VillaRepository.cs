using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
	public class VillaRepository : Repository<Villa> , IVillaRepository
	{
		private readonly AppDbContext _context;
		public VillaRepository(AppDbContext context):base(context)
		{
			_context = context;
		}	
		
		public void Update(Villa entity)
		{
			_context.Update(entity);	
		}
	}
}
