using System;

namespace ICode.Conditions
{
	[Serializable]
	[Obsolete("This action becomes obsolete in Unity 5.2, please use SceneManagement.LoadSceneAsync with a custom event to check if loading is done.")]
	[Category(Category.Application)]
	[Tooltip("Is some level being loaded?")]
	[HelpUrl("https://docs.unity3d.com/Documentation/ScriptReference/Application-isLoadingLevel.html")]
	public class IsLoadingLevel : Condition
	{
		[Tooltip("Does the result equals this condition.")]
		public FsmBool equals;

		public override bool Validate()
		{
			return false;
		}
	}
}
