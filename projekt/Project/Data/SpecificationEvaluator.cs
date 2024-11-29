using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Interfaces;
using Project.Models;

namespace Project.Data
{
	public class SpecificationEvaluator<T> where T : BaseEntity
	{
		public static IQueryable<T> GetQuery(IQueryable<T> query, ISpecification<T> spec)
		{
			if (spec.Criteria != null)
			{
				query = query.Where(spec.Criteria);
			}

			if (spec.OrderBy != null)
			{
				query = query.OrderBy(spec.OrderBy);
			}
			if (spec.OrderByDescending != null)
			{
				query = query.OrderByDescending(spec.OrderByDescending);
			}

			query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

			// Zastosuj Include z stringami (dla ThenInclude)
			query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

			if (spec.IsDistinct)
			{
				query = query.Distinct();
			}

			if (spec.IsPagingEnabled)
			{
				query = query.Skip(spec.Skip).Take(spec.Take);
			}


			return query;
		}

		public static IQueryable<TResult> GetQuery<TSpec, TResult>(IQueryable<T> query, ISpecification<T, TResult> spec)
		{
			if (spec.Criteria != null)
			{
				query = query.Where(spec.Criteria);
			}
			if (spec.OrderBy != null)
			{
				query = query.OrderBy(spec.OrderBy);
			}
			if (spec.OrderByDescending != null)
			{
				query = query.OrderByDescending(spec.OrderByDescending);
			}

			query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

			// Zastosuj Include z stringami (dla ThenInclude)
			query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));



			var selectQuery = query as IQueryable<TResult>;

			if (spec.Select != null)
			{
				selectQuery = query.Select(spec.Select);
			}

			if (spec.IsDistinct)
			{
				selectQuery = selectQuery?.Distinct();
			}


			if (spec.IsPagingEnabled)
			{
				selectQuery = selectQuery?.Skip(spec.Skip).Take(spec.Take);
			}


			return selectQuery ?? query.Cast<TResult>();
		}
	}


}