using System;

namespace ICode.Conditions
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	public class AbleStartTutorial : Condition
	{
		public override bool Validate()
		{
			return UIManager.instance.world.onStart;
		}
	}
}
