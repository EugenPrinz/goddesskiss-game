using System;

namespace ICode
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class HelpUrlAttribute : Attribute
	{
		private readonly string url;

		public string Url => url;

		public HelpUrlAttribute(string url)
		{
			this.url = url;
		}
	}
}
