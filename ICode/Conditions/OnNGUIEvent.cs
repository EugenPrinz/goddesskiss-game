using System;

namespace ICode.Conditions
{
	[Serializable]
	[Category("NGUI")]
	public class OnNGUIEvent : Condition
	{
		[SharedPersistent]
		[Tooltip("GameObject to use.")]
		public FsmGameObject gameObject;

		public NGUIEventType type;

		private NGUIEventHandler handler;

		private bool isTrigger;

		public override void OnEnter()
		{
			handler = gameObject.Value.GetComponent<NGUIEventHandler>();
			if (handler == null)
			{
				handler = gameObject.Value.AddComponent<NGUIEventHandler>();
			}
			handler.type = type;
			handler.onTrigger += OnTrigger;
		}

		public override void OnExit()
		{
			if (isTrigger)
			{
				handler.onTrigger -= OnTrigger;
			}
			isTrigger = false;
		}

		private void OnTrigger()
		{
			isTrigger = true;
		}

		public override bool Validate()
		{
			if (isTrigger)
			{
				isTrigger = false;
				return true;
			}
			return isTrigger;
		}
	}
}
