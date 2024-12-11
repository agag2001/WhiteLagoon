using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entites;

namespace WhiteLagoon.Application.Common.Interfaces
{
	public interface IVillaNumberRepository:IRepository<VillaNumber>
	{
		 void Update( VillaNumber entity);
	}
}
