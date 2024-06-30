using UnityEngine;

namespace Step
{
	public class IsAnimationEnd : AbstractStepCondition
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

		public override bool Validate()
		{
			if (!animation.isPlaying)
			{
				return true;
			}
			return false;
		}
	}
}
