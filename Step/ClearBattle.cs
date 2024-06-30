using UnityEngine;

namespace Step
{
	public class ClearBattle : AbstractStepAction
	{
		protected AbstractBattle main;

		protected override void OnEnter()
		{
			main = UIManager.instance.battle.Main;
			if (main.UnitRenderers != null)
			{
				for (int i = 0; i < main.UnitRenderers.Length; i++)
				{
					if (main.UnitRenderers[i] != null)
					{
						Object.DestroyImmediate(main.UnitRenderers[i].gameObject);
					}
				}
			}
			_isFinish = true;
		}
	}
}
