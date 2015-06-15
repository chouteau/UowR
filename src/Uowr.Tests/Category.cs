using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uowr.Tests
{
	public class Category
	{
		public string Id { get; set; }
		[MaxLength(256)]
		public string Title { get; set; }
	}
}
