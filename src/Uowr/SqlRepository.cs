using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Reflection;

namespace Uowr
{
	public class SqlRepository<TContext> : IDisposable, IRepository<TContext>
		where TContext : System.Data.Entity.DbContext, IObjectContextAdapter, new()
	{
		public SqlRepository()
		{
			DbRepositoryId = Guid.NewGuid().ToString();
		}

		public string DbRepositoryId { get; private set; }

		public virtual TContext GetDbContext()
		{
			var result = new TContext();
			return result;
		}

		public virtual T Get<T>(Expression<Func<T, bool>> predicate) where T : class
		{
			if (predicate == null)
			{
				throw new ArgumentException("predicate does not be null.");
			}
			var dbContext = GetDbContext();
			var query = dbContext.Set<T>().Where(predicate);
			var result = query.FirstOrDefault();
			return result;
		}

		public virtual async Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate) where T : class
		{
			if (predicate == null)
			{
				throw new ArgumentException("predicate does not be null.");
			}
			var dbContext = GetDbContext();
			var query = dbContext.Set<T>().Where(predicate);
			var result = await query.FirstOrDefaultAsync();
			return result;
		}

		public virtual IQueryable<T> Query<T>(Expression<Func<T, bool>> predicate) where T : class
		{
			var dbContext = GetDbContext();
			var query = dbContext.Set<T>().Where(predicate);
			return query;
		}

		public virtual IQueryable<T> Query<T, TKey>(Expression<Func<T, bool>> predicate,
			Expression<Func<T, TKey>> orderBy) where T : class
		{
			var query = Query(predicate).OrderBy(orderBy);
			return query;
		}

		public virtual IQueryable<T> Query<T, TKey>(Expression<Func<T, TKey>> orderBy) where T : class
		{
			var query = Query<T>().OrderBy(orderBy);
			return query;
		}

		public virtual IQueryable<T> Query<T>() where T : class
		{
			var dbContext = GetDbContext();
			return dbContext.Set<T>();
		}

		public virtual int Insert<T>(T entity) where T : class
		{
			int result = 0;
			var dbContext = GetDbContext();
			try
			{
				var dbSet = dbContext.Set<T>();
				var entry = dbContext.Entry(entity);
				if (entry.State == EntityState.Detached)
				{
					dbSet.Attach(entity);
				}
				dbSet.Add(entity);
				dbContext.ObjectContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Added);
				result = dbContext.SaveChanges();
			}
			catch (System.Data.Entity.Validation.DbEntityValidationException vex)
			{
				int errorId = 0;
				foreach (var item in vex.EntityValidationErrors)
				{
					foreach (var error in item.ValidationErrors)
					{
						errorId++;
						string key = string.Format("{0}{1}", error.PropertyName, errorId);
						vex.Data.Add(key, error.ErrorMessage);
					}
				}
				throw;
			}
			catch (Exception exp)
			{
				Console.WriteLine(exp);
				exp.Data.Add("SqlRepository:Insert:Entity", entity.ToString());
				throw;
			}

			return result;
		}

		public virtual async Task<int> InsertAsync<T>(T entity) where T : class
		{
			int result = 0;
			var dbContext = GetDbContext();
			try
			{
				var dbSet = dbContext.Set<T>();
				var entry = dbContext.Entry(entity);
				if (entry.State == EntityState.Detached)
				{
					dbSet.Attach(entity);
				}
				dbSet.Add(entity);
				dbContext.ObjectContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Added);
				//var attachedEntities = GetAttachedEntityPropertyList(entity);
				//foreach (var property in attachedEntities)
				//{
				//	var value = property.GetValue(entity);
				//	if (value != null)
				//	{
				//		var subDbSet = dbContext.Set(property.PropertyType);
				//		var subEntry = dbContext.Entry(value);
				//		var ose = dbContext.ObjectContext.ObjectStateManager.GetObjectStateEntry(value);
				//		if (ose.IsRelationship)
				//		{
				//			subDbSet.Attach(value);
				//		}
				//	}
				//}
				result = await dbContext.SaveChangesAsync();
			}
			catch (System.Data.Entity.Validation.DbEntityValidationException vex)
			{
				int errorId = 0;
				foreach (var item in vex.EntityValidationErrors)
				{
					foreach (var error in item.ValidationErrors)
					{
						errorId++;
						string key = string.Format("{0}{1}", error.PropertyName, errorId);
						vex.Data.Add(key, error.ErrorMessage);
					}
				}
				throw;
			}
			catch (Exception exp)
			{
				Console.WriteLine(exp);
				exp.Data.Add("SqlRepository:Insert:Entity", entity.ToString());
				throw;
			}

			return result;
		}

		public virtual void BulkInsert<T>(IEnumerable<T> list) where T : class
		{
			var dbContext = GetDbContext();
			var transactionOptions = new System.Transactions.TransactionOptions
			{
				IsolationLevel = System.Transactions.IsolationLevel.Serializable,
				Timeout = TimeSpan.FromSeconds(0)
			};
			try
			{
				using (var transactionScope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, transactionOptions))
				{
					var dbSet = dbContext.Set<T>();
					foreach (var entity in list)
					{
						if (dbContext.Entry(entity).State == EntityState.Detached)
						{
							dbSet.Attach(entity);
						}
						dbSet.Add(entity);
						dbContext.ObjectContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Added);
					}

					dbContext.SaveChanges();
					transactionScope.Complete();
				}
			}
			catch (System.Data.Entity.Validation.DbEntityValidationException vex)
			{
				int errorId = 0;
				foreach (var item in vex.EntityValidationErrors)
				{
					foreach (var error in item.ValidationErrors)
					{
						errorId++;
						string key = string.Format("{0}{1}", error.PropertyName, errorId);
						vex.Data.Add(key, error.ErrorMessage);
					}
				}
				throw;
			}
			catch (Exception exp)
			{
				throw;
			}
		}

		public virtual int Update<T>(T entity) where T : class
		{
			int result = 0;
			var dbContext = GetDbContext();
			try
			{
				if (dbContext.Entry(entity).State == EntityState.Detached)
				{
					dbContext.Set<T>().Attach(entity);
				}
				dbContext.ObjectContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);

				result = dbContext.SaveChanges();
			}
			catch (Exception exp)
			{
				exp.Data.Add("SqlRepository:Update:Entity", entity.ToString());
				throw;
			}

			return result;
		}

		public virtual async Task<int> UpdateAsync<T>(T entity) where T : class
		{
			int result = 0;
			var dbContext = GetDbContext();
			try
			{
				if (dbContext.Entry(entity).State == EntityState.Detached)
				{
					dbContext.Set<T>().Attach(entity);
				}
				dbContext.ObjectContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);

				result = await dbContext.SaveChangesAsync();
			}
			catch (Exception exp)
			{
				exp.Data.Add("SqlRepository:Update:Entity", entity.ToString());
				throw;
			}

			return result;
		}

		public virtual int Delete<T>(T entity) where T : class
		{
			int result = 0;
			var dbContext = GetDbContext();
			var loop = 0;

			while (true)
			{
				try
				{
					if (dbContext.Entry(entity).State == EntityState.Detached)
					{
						dbContext.Set<T>().Attach(entity);
					}
					dbContext.ObjectContext.DeleteObject(entity);

					result = dbContext.SaveChanges();
					break;
				}
				catch (DbUpdateConcurrencyException)
				{
					dbContext.ObjectContext.Refresh(RefreshMode.StoreWins, entity);
					if (loop > 2)
					{
						break;
					}
					loop++;
					System.Threading.Thread.Sleep(100);
				}
				catch (Exception exp)
				{
					exp.Data.Add("SqlRepository:Delete:Entity", entity.ToString());
					throw;
				}
			}

			return result;
		}

		public virtual async Task<int> DeleteAsync<T>(T entity) where T : class
		{
			int result = 0;
			var dbContext = GetDbContext();
			var loop = 0;

			while (true)
			{
				try
				{
					if (dbContext.Entry(entity).State == EntityState.Detached)
					{
						dbContext.Set<T>().Attach(entity);
					}
					dbContext.ObjectContext.DeleteObject(entity);

					result = await dbContext.SaveChangesAsync();
					break;
				}
				catch (DbUpdateConcurrencyException)
				{
					dbContext.ObjectContext.Refresh(RefreshMode.StoreWins, entity);
					if (loop > 2)
					{
						break;
					}
					loop++;
					System.Threading.Thread.Sleep(100);
				}
				catch (Exception exp)
				{
					exp.Data.Add("SqlRepository:Delete:Entity", entity.ToString());
					throw;
				}
			}

			return result;
		}

		public virtual int DeleteAll<T>(Expression<Func<T, bool>> predicate) where T : class
		{
			int result = 0;
			var dbContext = GetDbContext();

			var query = Query<T>(predicate);
			foreach (var entity in query)
			{
				var loop = 0;
			retry:
				try
				{
					if (dbContext.Entry(entity).State == EntityState.Detached)
					{
						dbContext.Set<T>().Attach(entity);
					}
					dbContext.ObjectContext.DeleteObject(entity);
					result = dbContext.SaveChanges();
				}
				catch (DbUpdateConcurrencyException)
				{
					dbContext.ObjectContext.Refresh(RefreshMode.StoreWins, entity);
					loop++;
					if (loop < 2)
					{
						System.Threading.Thread.Sleep(100);
						goto retry;
					}
				}
				catch (Exception exp)
				{
					exp.Data.Add("SqlRepository:Delete:Entity", entity.ToString());
					throw;
				}
			}

			return result;
		}

		public int ExecuteStoreCommand(string cmdText, params object[] parameters)
		{
			int result = 0;
			try
			{
				var dbContext = GetDbContext();
				result = dbContext.Database.ExecuteSqlCommand(cmdText, parameters);
			}
			catch (Exception exp)
			{
				exp.Data.Add("SqlRepository:ExecuteStoreCommand:script", cmdText);
				if (parameters != null)
				{
					for (int i = 0; i < parameters.Length; i++)
					{
						exp.Data.Add(string.Format("SqlRepository:ExecuteStoreCommand:script:P{0}", i), parameters[i]);
					}
				}
				throw;
			}
			return result;
		}

		public async Task<int> ExecuteStoreCommandAsync(string cmdText, params object[] parameters)
		{
			int result = 0;
			try
			{
				var dbContext = GetDbContext();
				result = await dbContext.Database.ExecuteSqlCommandAsync(cmdText, parameters);
			}
			catch (Exception exp)
			{
				exp.Data.Add("SqlRepository:ExecuteStoreCommand:script", cmdText);
				if (parameters != null)
				{
					for (int i = 0; i < parameters.Length; i++)
					{
						exp.Data.Add(string.Format("SqlRepository:ExecuteStoreCommand:script:P{0}", i), parameters[i]);
					}
				}
				throw;
			}
			return result;
		}

		public ObjectResult<T> ExecuteStoreQuery<T>(string cmdText, params object[] parameters)
		{
			var dbContext = GetDbContext();
			var result = dbContext.ObjectContext.ExecuteStoreQuery<T>(cmdText, parameters);
			return result;
		}

		public async Task<ObjectResult<T>> ExecuteStoreQueryAsync<T>(string cmdText, params object[] parameters)
		{
			var dbContext = GetDbContext();
			var result = await dbContext.ObjectContext.ExecuteStoreQueryAsync<T>(cmdText, parameters);
			return result;
		}

		private bool IsEntityKeyGenerated<T>(System.Data.Entity.DbContext dbContext, T entity) where T : class
		{
			var objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
			var set = objectContext.CreateObjectSet<T>();
			var isKeyGenerated = set.EntitySet.ElementType.KeyMembers.Any(k => k.IsStoreGeneratedComputed | k.IsStoreGeneratedIdentity);
			return isKeyGenerated;
		}

		private IEnumerable<PropertyInfo> GetAttachedEntityPropertyList<T>(T entity)
		{
			var nameSpace = entity.GetType().Namespace;
			var properties = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(i => i.PropertyType.Namespace == nameSpace);
			return properties;
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
