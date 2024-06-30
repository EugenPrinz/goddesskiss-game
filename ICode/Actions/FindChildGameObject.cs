using System;
using UnityEngine;

namespace ICode.Actions
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class FindChildGameObject : StateAction
	{
		[SharedPersistent]
		[Tooltip("GameObject to use.")]
		public FsmGameObject gameObject;

		[InspectorLabel("Name")]
		[Tooltip("The name of the child game object to find.")]
		public FsmString _name;

		[Shared]
		[Tooltip("Store the result.")]
		public FsmGameObject store;

		[Tooltip("Execute the action every frame.")]
		public bool everyFrame;

		public override void OnEnter()
		{
			Transform transform = gameObject.Value.transform.Find(_name.Value);
			if (transform != null)
			{
				store.Value = transform.gameObject;
			}
			else
			{
				store.Value = null;
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			Transform transform = gameObject.Value.transform.Find(_name.Value);
			if (transform != null)
			{
				store.Value = transform.gameObject;
			}
			else
			{
				store.Value = null;
			}
		}
	}
}
