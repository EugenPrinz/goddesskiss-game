using Shared.Regulation;
using UnityEngine;

public class UIEventRaidListItem : UIItemBase
{
	public UILabel bossName;

	public UILabel bossLevel;

	public UISprite bossThumb;

	public UISprite bg;

	public UILabel userName;

	public UITimer remainTimer;

	public UIProgressBar hpProgress;

	public UILabel hpLabel;

	public UILabel maxHpLabel;

	public GameObject shareBtn;

	public GameObject readyBtn;

	public GameObject sharingBtn;

	public GameObject rewardBtn;

	public UISprite clear;

	private readonly string shareBtnIdPrefix = "Share-";

	private readonly string sharedBtnIdPrefix = "Shared-";

	private readonly string readyBtnIdIdPrefix = "Ready-";

	private readonly string rewardBtnIdPrefix = "Reward-";

	public void Set(EventRaidTabType type, Protocols.EventRaidData data)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetGameObjectName(shareBtn, $"{shareBtnIdPrefix}{data.bossId}");
		UISetter.SetGameObjectName(sharingBtn, $"{sharedBtnIdPrefix}{data.bossId}");
		UISetter.SetGameObjectName(readyBtn, $"{readyBtnIdIdPrefix}{data.bossId}");
		UISetter.SetGameObjectName(rewardBtn, $"{rewardBtnIdPrefix}{data.bossId}");
		UISetter.SetActive(clear, type == EventRaidTabType.History);
		UISetter.SetActive(rewardBtn, type == EventRaidTabType.History && data.receive == 0 && data.clear == 1);
		UISetter.SetActive(readyBtn, type != EventRaidTabType.History);
		UISetter.SetSprite(bg, (data.isOwn != 0) ? "login_bg_sever_select" : "com_bg_popup_inside");
		UISetter.SetSprite(clear, (data.clear != 0) ? "me-clear" : "me-fail");
		UISetter.SetButtonGray(shareBtn, localUser.IsExistGuild());
		if (type != EventRaidTabType.History)
		{
			UISetter.SetActive(shareBtn, data.isOwn == 1 && data.isShare == 0);
			UISetter.SetActive(sharingBtn, data.isOwn == 0 || data.isShare == 1);
		}
		else
		{
			UISetter.SetActive(shareBtn, active: false);
			UISetter.SetActive(sharingBtn, active: true);
		}
		UISetter.SetLabel(userName, Localization.Format("6506", data.userName));
		EnemyCommanderDataRow enemyCommanderDataRow = regulation.enemyCommanderDtbl.Find((EnemyCommanderDataRow row) => row.id == data.enemy);
		RoTroop roTroop = RoTroop.CreateEventBoss(data)[0];
		UISetter.SetLabel(bossName, Localization.Get(enemyCommanderDataRow.name));
		RoTroop.Slot slot = roTroop.slots[0];
		RoUnit roUnit = RoUnit.Create(slot.unitId, slot.unitLevel, slot.unitRank, slot.unitCls, slot.unitCostume, slot.commanderId, slot.favorRewardStep, slot.marry, slot.transcendence, EBattleType.EventRaid);
		UISetter.SetSprite(bossThumb, $"{roUnit.currLevelReg.resourceName}_Front");
		UISetter.SetLabel(bossLevel, Localization.Format("1021", roUnit.level));
		UISetter.SetLabel(hpLabel, slot.health.ToString("N0"));
		UISetter.SetLabel(maxHpLabel, string.Format("/{0}", roUnit.currLevelReg.originMaxHealth.ToString("N0")));
		UISetter.SetProgress(hpProgress, (float)slot.health / (float)roUnit.currLevelReg.originMaxHealth);
		TimeData timeData = TimeData.Create();
		timeData.SetByDuration(data.remain);
		remainTimer.Set(timeData);
		remainTimer.SetLabelFormat(string.Format("{0} ", Localization.Get("6505")), string.Empty);
	}

	public void raidShared()
	{
		UISetter.SetActive(shareBtn, active: false);
		UISetter.SetActive(sharingBtn, active: true);
	}

	public void rewardReceived()
	{
		UISetter.SetActive(rewardBtn, active: false);
	}

	public int RemainTime()
	{
		return (int)remainTimer.timeData.GetRemain();
	}
}
