using System;
using System.Linq;
using UnityEngine;

namespace ICode.Actions
{
	[Serializable]
	[Category(Category.GameObject)]
	[Tooltip("Finds a list of active GameObjects tagged tag.")]
	[HelpUrl("http://docs.unity3d.com/ScriptReference/GameObject.FindGameObjectsWithTag.html")]
	public class FindGameObjectsWithTag : StateAction
	{
		[Tag]
		[Tooltip("The name of the tag to search GameObjects for.")]
		public FsmString tag;

		[NotRequired]
		[Shared]
		[Tooltip("Store the list of find game objects.")]
		public FsmArray store;

		[NotRequired]
		[Shared]
		[Tooltip("Store a random game object from the list.")]
		public FsmGameObject storeRandom;

		[Tooltip("Execute the action every frame.")]
		public bool everyFrame;

		public override void OnEnter()
		{
			DoFind();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoFind();
		}

		public void DoFind()
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(tag.Value);
			if (array.Length > 0)
			{
				if (!store.IsNone)
				{
					store.Value = array.Cast<object>().ToArray();
				}
				if (!storeRandom.IsNone && array.Length > 0)
				{
					storeRandom.Value = array.GetRandom<GameObject>();
				}
			}
		}
	}
}
