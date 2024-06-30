using System;
using UnityEngine.SceneManagement;

namespace ICode.Actions.UnityApplication
{
	[Serializable]
	[Obsolete("This action is obsolete. Please use SceneManagement.LoadScene")]
	[Category(Category.Application)]
	[Tooltip("Loads the level by its name.")]
	[HelpUrl("https://docs.unity3d.com/Documentation/ScriptReference/Application.LoadLevel.html")]
	public class LoadLevel : StateAction
	{
		[Tooltip("The name of the level to load.")]
		public FsmString level;

		public override void OnEnter()
		{
			SceneManager.LoadScene(level.Value);
			Finish();
		}
	}
}
