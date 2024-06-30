using System;

namespace ICode.Conditions
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	public class IsPopupStateEqual : Condition
	{
		[Shared]
		public FsmPopupState first;

		public FsmPopupState second;

		public FsmBool equals;

		public override bool Validate()
		{
			return first.Value == second.Value == equals.Value;
		}
	}
}
