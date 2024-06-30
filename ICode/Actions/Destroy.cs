using System;
using UnityEngine;

namespace ICode.Actions
{
	[Serializable]
	[Category(Category.GameObject)]
	[Tooltip("Removes a gameobject.")]
	[HelpUrl("http://docs.unity3d.com/Documentation/ScriptReference/Object.Destroy.html")]
	public class Destroy : StateAction
	{
		[SharedPersistent]
		[Tooltip("GameObject to use.")]
		public FsmGameObject gameObject;

		[Tooltip("Delay")]
		public FsmFloat delay;

		public override void OnEnter()
		{
			UnityEngine.Object.Destroy(gameObject.Value, delay.Value);
			Finish();
		}
	}
}
