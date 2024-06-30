using System;
using UnityEngine;

namespace ICode.Actions
{
	[Serializable]
	[Category(Category.StateMachine)]
	[Tooltip("Sends an event to all attached state machines. Can be checked in condition OnCustomEvent.")]
	public class SendEvent : StateAction
	{
		[SharedPersistent]
		[Tooltip("GameObject to use.")]
		public FsmGameObject gameObject;

		[Tooltip("Send event also to children state machines.")]
		public FsmBool includeChildren;

		[InspectorLabel("Event")]
		[Tooltip("Event name to send.")]
		public FsmString _event;

		[ParameterType]
		public FsmVariable parameter;

		public override void OnEnter()
		{
			GameObject value = gameObject.Value;
			if (value != null)
			{
				ICodeBehaviour[] behaviours = value.GetBehaviours(includeChildren.Value);
				if (behaviours != null && behaviours.Length > 0)
				{
					for (int i = 0; i < behaviours.Length; i++)
					{
						if (parameter != null)
						{
							behaviours[i].SendEvent(_event.Value, parameter.GetValue());
						}
						else
						{
							behaviours[i].SendEvent(_event.Value, null);
						}
					}
				}
			}
			Finish();
		}
	}
}
