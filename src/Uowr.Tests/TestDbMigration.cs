using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uowr.Tests
{
	class TestDbMigration : DbMigrationsConfiguration<TestDbContext>
	{
		public TestDbMigration()
		{
			AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
			ContextKey = "FluxCenter";
		}
	}
}
