using UnityEngine;

namespace ICode
{
	public class HeaderAttribute : PropertyAttribute
	{
		public readonly string header;

		public HeaderAttribute(string header)
		{
			this.header = header;
		}
	}
}
