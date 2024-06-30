using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ICode.Actions.UnityApplication
{
	[Serializable]
	[Obsolete("This action is obsolete. Please use SceneManagement.LoadSceneAsync")]
	[Category(Category.Application)]
	[Tooltip("Loads the level asynchronously in the background.")]
	[HelpUrl("http://docs.unity3d.com/ScriptReference/Application.LoadLevelAsync.html")]
	public class LoadLevelAsync : StateAction
	{
		[Tooltip("The name of the level to load.")]
		public FsmString level;

		[NotRequired]
		[Tooltip("Send event when loading is done. Check with OnCustomEvent")]
		public FsmString loadedEvent;

		[NotRequired]
		[Shared]
		[Tooltip("Store the progress of loading.")]
		public FsmFloat progress;

		private AsyncOperation asyncOperation;

		public override void OnUpdate()
		{
			if (asyncOperation == null)
			{
				asyncOperation = SceneManager.LoadSceneAsync(level.Value);
				asyncOperation.allowSceneActivation = false;
				return;
			}
			if (!progress.IsNone)
			{
				progress.Value = asyncOperation.progress;
			}
			asyncOperation.allowSceneActivation = !(asyncOperation.progress < 0.9f);
			if (asyncOperation.isDone)
			{
				if (!loadedEvent.IsNone && !string.IsNullOrEmpty(loadedEvent.Value))
				{
					base.Root.Owner.SendEvent(loadedEvent.Value, null);
				}
				Finish();
			}
		}
	}
}
