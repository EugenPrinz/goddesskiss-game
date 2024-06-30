using System;

namespace ICode.Conditions
{
	[Serializable]
	public class Condition : ExecutableNode
	{
		public virtual bool Validate()
		{
			return true;
		}
	}
}
