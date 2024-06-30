using System;

namespace ICode.Conditions
{
	[Serializable]
	[Category(Category.GameObject)]
	[Tooltip("Is the value of the GameObject variable null?")]
	public class IsNull : Condition
	{
		[SharedPersistent]
		[Tooltip("GameObject to use.")]
		public FsmGameObject gameObject;

		[Tooltip("Does the result equals this condition.")]
		public FsmBool equals;

		public override bool Validate()
		{
			return gameObject.Value == null == equals.Value;
		}
	}
}
