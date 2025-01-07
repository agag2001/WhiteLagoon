using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entites;

namespace WhiteLagoon.Application.Common.Interfaces
{
	public interface IRepository<T> where T : class 
	{
		// to handel filter with villas like First or default ___ and the include properties will be included(eger loading)
		IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null,bool tracked =  false);
		T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
		void Add(T entity);
		void Remove(T entity);
		bool Any(Expression<Func<T, bool>> filter);
	}
	
}
