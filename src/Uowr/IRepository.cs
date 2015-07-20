using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uowr
{
	public interface IRepository<TContext> : IDisposable
		where TContext : System.Data.Entity.DbContext, System.Data.Entity.Infrastructure.IObjectContextAdapter, new()
	{
		TContext DbContext { get; }

		T Get<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class;
		Task<T> GetAsync<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class;

		int Update<T>(T entity) where T : class;
		Task<int> UpdateAsync<T>(T entity) where T : class;

		int Insert<T>(T entity) where T : class;
		Task<int> InsertAsync<T>(T entity) where T : class;
		void BulkInsert<T>(IEnumerable<T> list) where T : class;

		int Delete<T>(T entity) where T : class;
		Task<int> DeleteAsync<T>(T entity) where T : class;
		int DeleteAll<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class;

		int ExecuteStoreCommand(string cmdText, params object[] parameters);
		Task<int> ExecuteStoreCommandAsync(string cmdText, params object[] parameters);

		ObjectResult<T> ExecuteStoreQuery<T>(string cmdText, params object[] parameters);
		Task<ObjectResult<T>> ExecuteStoreQueryAsync<T>(string cmdText, params object[] parameters);

		System.Linq.IQueryable<T> Query<T, TKey>(System.Linq.Expressions.Expression<Func<T, bool>> predicate, System.Linq.Expressions.Expression<Func<T, TKey>> orderBy) where T : class;
		System.Linq.IQueryable<T> Query<T, TKey>(System.Linq.Expressions.Expression<Func<T, TKey>> orderBy) where T : class;
		System.Linq.IQueryable<T> Query<T>() where T : class;
		System.Linq.IQueryable<T> Query<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class;

		void Dispose();
	}
}
