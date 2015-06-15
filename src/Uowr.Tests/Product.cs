using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uowr.Tests
{
	public class Product
	{
		public string Id { get; set; }
		public string Code { get; set; }
		public string Title { get; set; }
		public DateTime CreationDate { get; set; }
		public virtual Category Category { get; set; }
	}
}
