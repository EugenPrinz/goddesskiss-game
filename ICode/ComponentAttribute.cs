using System;
using UnityEngine;

namespace ICode
{
	public class ComponentAttribute : PropertyAttribute
	{
		private readonly Type type;

		public Type Type => type;

		public ComponentAttribute(Type type)
		{
			this.type = type;
		}

		public ComponentAttribute()
		{
		}
	}
}
