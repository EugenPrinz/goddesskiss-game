using System;
using UnityEngine.SceneManagement;

namespace ICode.Actions.UnityApplication
{
	[Serializable]
	[Obsolete("This action is obsolete. Please use SceneManager.GetActiveScene")]
	[Category(Category.Application)]
	[Tooltip("The name of the level that was last loaded.")]
	[HelpUrl("http://docs.unity3d.com/ScriptReference/Application-loadedLevelName.html")]
	public class GetLoadedLevel : StateAction
	{
		[NotRequired]
		[InspectorLabel("Name")]
		[Shared]
		[Tooltip("Store the level name.")]
		public FsmString _name;

		[NotRequired]
		[Shared]
		[Tooltip("Store the index of the level.")]
		public FsmInt index;

		[Tooltip("Execute the action every frame.")]
		public bool everyFrame;

		public override void OnEnter()
		{
			GetLevelInfo();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			GetLevelInfo();
		}

		private void GetLevelInfo()
		{
			_name.Value = SceneManager.GetActiveScene().name;
			index.Value = SceneManager.GetActiveScene().buildIndex;
		}
	}
}
