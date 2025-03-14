using System;

namespace ICode
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class CategoryAttribute : Attribute
	{
		private readonly string category;

		public string Category => category;

		public CategoryAttribute(string category)
		{
			this.category = category;
		}

		public CategoryAttribute(Category category)
		{
			this.category = category.ToString();
		}
	}
}
