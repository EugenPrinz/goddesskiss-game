using UnityEngine;

namespace Step
{
	public class PlayAnimation : AbstractStepAction
	{
		public Animation animation;

		public override bool Enter()
		{
			if (animation == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			animation.Play();
		}
	}
}
