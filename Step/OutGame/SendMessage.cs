using UnityEngine;

namespace Step.OutGame
{
	public class SendMessage : AbstractStepAction
	{
		public GameObjectData target;

		public string methodName;

		public GameObjectData arg;

		public override bool Enter()
		{
			if (target == null || target.value == null)
			{
				return false;
			}
			if (string.IsNullOrEmpty(methodName))
			{
				return false;
			}
			if (arg != null && arg.value == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			if (arg != null)
			{
				UIButton.current = null;
				target.value.gameObject.SendMessage(methodName, arg.value.gameObject, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				UIButton.current = null;
				target.value.gameObject.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
