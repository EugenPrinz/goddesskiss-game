using System;
using UnityEngine;

namespace ICode.Actions
{
	[Serializable]
	[Category(Category.StateMachine)]
	[Tooltip("Sends an event to all GameObjects in array. Can be checked in condition OnCustomEvent.")]
	public class SendEventToArray : StateAction
	{
		[Shared]
		[Tooltip("Array to use.")]
		public FsmArray array;

		[Tooltip("Send event also to children state machines.")]
		public FsmBool includeChildren;

		[InspectorLabel("Event")]
		[Tooltip("Event name to send.")]
		public FsmString _event;

		[ParameterType]
		public FsmVariable parameter;

		public override void OnEnter()
		{
			for (int i = 0; i < array.Value.Length; i++)
			{
				object obj = array.Value[i];
				GameObject gameObject = null;
				if (obj is GameObject)
				{
					gameObject = obj as GameObject;
				}
				else if (obj is Component)
				{
					gameObject = (obj as Component).gameObject;
				}
				else if (obj is MonoBehaviour)
				{
					gameObject = (obj as MonoBehaviour).gameObject;
				}
				ICodeBehaviour[] behaviours = gameObject.GetBehaviours(includeChildren.Value);
				if (behaviours == null || behaviours.Length <= 0)
				{
					continue;
				}
				for (int j = 0; j < behaviours.Length; j++)
				{
					if (parameter != null)
					{
						behaviours[j].SendEvent(_event.Value, parameter.GetValue());
					}
					else
					{
						behaviours[j].SendEvent(_event.Value, null);
					}
				}
			}
			Finish();
		}
	}
}
