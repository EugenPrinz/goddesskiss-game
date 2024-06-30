using System;

namespace ICode.Conditions
{
	[Serializable]
	[Category(Category.GameObject)]
	[Tooltip("")]
	public class IsEnabled : Condition
	{
		[SharedPersistent]
		[Tooltip("GameObject to use.")]
		public FsmGameObject gameObject;

		[Tooltip("Does the result equals this condition.")]
		public FsmBool equals;

		public override bool Validate()
		{
			UIButton component = gameObject.Value.GetComponent<UIButton>();
			return component.isEnabled == equals.Value;
		}
	}
}
