using DialoguerCore;
using Shared.Regulation;
using UnityEngine;

public class EventScenarioItem : UIItemBase
{
	[SerializeField]
	private UISprite BG;

	[SerializeField]
	private UILabel Order;

	[SerializeField]
	private UILabel Title;

	[SerializeField]
	private GameObject Lock;

	[SerializeField]
	private GameObject arrow;

	private bool isLock;

	private EventBattleScenarioDataRow scenarioData;

	private const string active_bg_name = "com_bg_popup_inside";

	private const string lock_bg_name = "com_bg_popup_inside2";

	private const string active_btn_img = "ma_btn_arrow";

	private const string lock_btn_img = "pvp_type_lock";

	public void Set(EventBattleScenarioDataRow _scenarioData, int lastClearIdx)
	{
		if (_scenarioData != null)
		{
			scenarioData = _scenarioData;
			isLock = ((scenarioData.mapClear > lastClearIdx) ? true : false);
			BG.spriteName = ((!isLock) ? "com_bg_popup_inside" : "com_bg_popup_inside2");
			UISetter.SetActive(Lock, isLock);
			UISetter.SetActive(arrow, !isLock);
			UISetter.SetLabel(Order, string.Format(Localization.Get("20004"), scenarioData.sort));
			UISetter.SetLabel(Title, Localization.Get(scenarioData.title));
		}
	}

	public void PlayEventScenario()
	{
		if (scenarioData != null)
		{
			ClassicRpgManager dialogMrg = UIManager.instance.world.dialogMrg;
			if (dialogMrg != null)
			{
				dialogMrg.StartEventScenario();
				dialogMrg.InitScenarioDialogue(scenarioData.scenarioIdx, DialogueType.Event);
			}
		}
	}
}
