using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Uowr.Tests
{
	[TestClass]
	public class SqlTests
	{
		[TestInitialize]
		public void Initialize()
		{
			var migrationStrategy = new MigrateDatabaseToLatestVersion<TestDbContext, TestDbMigration>();
			Database.SetInitializer(migrationStrategy);
		}


		[TestMethod]
		public async Task CRUD()
		{
			var dbContext = new Uowr.SqlRepository<TestDbContext>();

			var category = new Category();
			category.Id = Guid.NewGuid().ToString().Replace("-", "");
			category.Title = "Cat1";

			await dbContext.InsertAsync(category);

			var product = new Product();
			product.Id = Guid.NewGuid().ToString().Replace("-", "");
			product.Code = "SKU1";
			product.Title = "Product1";
			product.Category = category;
			product.CreationDate = DateTime.Now;

			await dbContext.InsertAsync(product);

			// dbContext = new Uowr.SqlRepository<TestDbContext>();
			product = await dbContext.GetAsync<Product>(i => i.Id == product.Id);

			var category2 = new Category();
			category2.Id = Guid.NewGuid().ToString().Replace("-", "");
			category2.Title = "Cat2";

			await dbContext.InsertAsync(category2);

			product.Category = category2;

			await dbContext.UpdateAsync(product);
		}
	}
}
