using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uowr.Tests
{
	public class TestDbContext : DbContext
	{
		public TestDbContext()
			: base("Name=UowrTestConnectionString")
		{
			Configuration.ProxyCreationEnabled = true;
			Configuration.AutoDetectChangesEnabled = true;
			Configuration.ValidateOnSaveEnabled = false;
			Configuration.LazyLoadingEnabled = true;
		}

		public virtual IDbSet<Product> Products { get; set; }
		public virtual IDbSet<Category> Categories { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Properties<string>()
				.Where(p => p.Name == "Id")
				.Configure(p => { p.IsKey(); p.HasMaxLength(32); p.IsRequired(); });

			modelBuilder.Properties<byte[]>()
				.Where(p => p.Name == "Version")
				.Configure(p => {
						p.IsRequired();
						p.IsRowVersion();
				});

			modelBuilder.Properties<string>()
				.Where(p => p.Name == "Code")
				.Configure(p => { p.HasMaxLength(50); p.IsRequired(); });

			modelBuilder.Properties<string>().Configure(c => c.HasColumnType("varchar"));
			modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
			modelBuilder.Properties<DateTime?>().Configure(c => c.HasColumnType("datetime2"));

			// Turn off cascading deletes
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
		}
	}
}
