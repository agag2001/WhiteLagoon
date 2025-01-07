using Microsoft.AspNetCore.Identity;
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
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly AppDbContext _context;
		internal DbSet<T> _dbSet;
		public Repository(AppDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<T>();
		}
		public void Add(T entity)
		{
			_dbSet.Add(entity);
		}

		public bool Any(Expression<Func<T, bool>> filter)
		{
			return _dbSet.Any(filter);
		}

		public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
		{
			IQueryable<T> query ;
			if (tracked)
			{
				query = _dbSet;
			}
			else
			{
				query = _dbSet.AsNoTracking();
			}
			if (filter is not null)
			{
				query = query.Where(filter);
			}
			// handel inlcude many navigation properties in the code
			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var includeprop in includeProperties.Split(",", StringSplitOptions.RemoveEmptyEntries))
				{
					// use trim remove white spaces >>Include(" Category ") 
					// if the input like above given to efcore it will fail because of white spaces
					query = query.Include(includeprop.Trim());
				}

			}
			return query.FirstOrDefault();

		}

		public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = false)
		{
			IQueryable<T> query;

			if (tracked)
            {
                query = _dbSet;
            }
            else
            {
                query = _dbSet.AsNoTracking();
            }
            if (filter is not null)
			{
				query = query.Where(filter);
			}
			// handle include of many navigation properties in the code
			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var includeprop in includeProperties.Split(",", StringSplitOptions.RemoveEmptyEntries))
				{
					// use trim to remove white spaces >>Include(" Category ") 
					// if the input like above is given to efcore it will fail because of white spaces
					query = query.Include(includeprop.Trim());
				}
			}
			return query.ToList();
		}
		public void Remove(T entity)
		{
			_dbSet.Remove(entity);
		}

		
	}

}
