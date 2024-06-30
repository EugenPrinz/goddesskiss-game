using System;
using UnityEngine;

namespace ICode.Actions
{
	[Serializable]
	[Category(Category.GameObject)]
	[Tooltip("Finds a game object by name.")]
	[HelpUrl("https://docs.unity3d.com/Documentation/ScriptReference/GameObject.Find.html")]
	public class Find : StateAction
	{
		[InspectorLabel("Name")]
		[Tooltip("The name of the game object to find.")]
		public FsmString _name;

		[Tooltip("Should inactive GameObjects be included into the search?")]
		public FsmBool includeInactive;

		[Shared]
		[Tooltip("Store the result.")]
		public FsmGameObject store;

		[Tooltip("Execute the action every frame.")]
		public bool everyFrame;

		public override void OnEnter()
		{
			store.Value = DoFind();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			store.Value = DoFind();
		}

		private GameObject DoFind()
		{
			GameObject gameObject = GameObject.Find(_name.Value);
			if (includeInactive.Value && gameObject == null)
			{
				Transform[] array = FsmUtility.FindAll<Transform>(includeInactive: true);
				Transform[] array2 = array;
				foreach (Transform transform in array2)
				{
					if (transform.name == _name.Value)
					{
						return transform.gameObject;
					}
				}
			}
			return gameObject;
		}
	}
}
