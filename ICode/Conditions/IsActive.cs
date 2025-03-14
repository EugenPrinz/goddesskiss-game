using System;

namespace ICode.Conditions
{
	[Serializable]
	[Category(Category.GameObject)]
	[Tooltip("The local active state of this GameObject.")]
	[HelpUrl("http://docs.unity3d.com/Documentation/ScriptReference/GameObject-activeSelf.html")]
	public class IsActive : Condition
	{
		[SharedPersistent]
		[Tooltip("GameObject to use.")]
		public FsmGameObject gameObject;

		[Tooltip("Does the result equals this condition.")]
		public FsmBool equals;

		public override bool Validate()
		{
			return gameObject.Value.activeSelf == equals.Value;
		}
	}
}
