using System;
using UnityEngine;

namespace ICode.Actions
{
	[Serializable]
	[Category(Category.StateMachine)]
	[Tooltip("Enable a state machine or reset it.")]
	public class EnableStateMachine : StateAction
	{
		[SharedPersistent]
		[Tooltip("GameObject to use.")]
		public FsmGameObject gameObject;

		[Tooltip("The group value of the StateMachineBehaviour to enable.")]
		public FsmInt group;

		public override void OnEnter()
		{
			GameObject value = gameObject.Value;
			ICodeBehaviour[] components = value.GetComponents<ICodeBehaviour>();
			if (components != null && components.Length > 0)
			{
				for (int i = 0; i < components.Length; i++)
				{
					if (components[i].group == group.Value)
					{
						components[i].EnableStateMachine();
					}
				}
			}
			Finish();
		}
	}
}
