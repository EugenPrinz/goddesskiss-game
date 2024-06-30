using UnityEngine;

public class UIUnitPanelDisabler : MonoBehaviour
{
	public static int enabledCnt;

	private void OnEnable()
	{
		enabledCnt++;
		if (UIManager.instance.battle != null && enabledCnt > 0)
		{
			UIManager.instance.battle.MainUI.SetUnitPanelEnable(enable: false);
		}
	}

	private void OnDisable()
	{
		enabledCnt--;
		if (UIManager.hasInstance && UIManager.instance.battle != null && enabledCnt <= 0)
		{
			enabledCnt = 0;
			if (UIManager.instance.battle.MainUI != null)
			{
				UIManager.instance.battle.MainUI.SetUnitPanelEnable(enable: true);
			}
		}
	}
}
