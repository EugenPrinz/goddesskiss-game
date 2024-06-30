using UnityEngine;

namespace Unitycoding
{
	public class HeaderLineAttribute : PropertyAttribute
	{
		public readonly string header;

		public HeaderLineAttribute(string header)
		{
			this.header = header;
		}
	}
}
