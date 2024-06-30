using Shared.Regulation;
using UnityEngine;

public class WorldDuelBuffItem : UIItemBase
{
	public UISprite icon;

	public UILabel costCount;

	public UILabel level;

	public UILabel activeType;

	public UILabel description;

	public EWorldDuelBuffEffect buffType;

	public GameObject empty;

	public GameObject upgradeLabel;

	public GameObject upgradeDisableLabel;

	public GameObject resourceRoot;

	public GameObject btn;

	public void Set(EWorldDuelBuff type, bool upgrade)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		string text = $"{type.ToString()}{buffType.ToString()}";
		int level = Mathf.Max(localUser.worldDuelBuff[text], 1);
		StrongestBuffBattleDataRow strongestBuffBattleDataRow = regulation.strongestBuffBattleDtbl.Find((StrongestBuffBattleDataRow row) => row.buffTarget == type && row.buffEffectType == buffType && row.buffLevel == level);
		StrongestBuffBattleDataRow strongestBuffBattleDataRow2 = regulation.strongestBuffBattleDtbl.Find((StrongestBuffBattleDataRow row) => row.buffTarget == type && row.buffEffectType == buffType && row.buffLevel == level + 1);
		switch (text)
		{
		case "attb":
			UISetter.SetSprite(icon, "group_buff_1001");
			UISetter.SetLabel(activeType, Localization.Get("400005"));
			if (upgrade)
			{
				UISetter.SetActive(empty, strongestBuffBattleDataRow2 == null);
				UISetter.SetActive(resourceRoot, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(this.level, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(description, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(upgradeLabel, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(upgradeDisableLabel, strongestBuffBattleDataRow2 == null);
				UISetter.SetButtonEnable(btn, strongestBuffBattleDataRow2 != null);
				if (strongestBuffBattleDataRow2 != null)
				{
					UISetter.SetLabel(this.level, $"[FC6120FF]{level + 1}[-][76480CFF]LV[-]");
					UISetter.SetLabel(costCount, strongestBuffBattleDataRow.upgradeCoin);
					UISetter.SetLabel(description, $"[6AA635FF]+{(float)(strongestBuffBattleDataRow2.buffAdd - strongestBuffBattleDataRow.buffAdd) / 100f:0.##}%[-]");
					UISetter.SetGameObjectName(btn, $"Upgrade-{text}");
				}
			}
			else
			{
				UISetter.SetLabel(this.level, $"[FC6120FF]{level}[-][76480CFF]LV[-]");
				UISetter.SetLabel(description, string.Format("[76480CFF]{0}[-] [6AA635FF]+{1:0.##}%[-]", Localization.Get("400011"), (float)strongestBuffBattleDataRow.buffAdd / 100f));
				UISetter.SetGameObjectName(btn, $"Setting-{text}");
			}
			break;
		case "attd":
			UISetter.SetSprite(icon, "group_buff_1001_off");
			UISetter.SetLabel(activeType, Localization.Get("400008"));
			if (upgrade)
			{
				UISetter.SetActive(empty, strongestBuffBattleDataRow2 == null);
				UISetter.SetActive(resourceRoot, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(this.level, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(description, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(upgradeLabel, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(upgradeDisableLabel, strongestBuffBattleDataRow2 == null);
				UISetter.SetButtonEnable(btn, strongestBuffBattleDataRow2 != null);
				if (strongestBuffBattleDataRow2 != null)
				{
					UISetter.SetLabel(this.level, $"[FC6120FF]{level + 1}[-][76480CFF]LV[-]");
					UISetter.SetLabel(costCount, strongestBuffBattleDataRow.upgradeCoin);
					UISetter.SetLabel(description, $"[6AA635FF]-{(float)(strongestBuffBattleDataRow2.buffAdd - strongestBuffBattleDataRow.buffAdd) / 100f:0.##}%[-]");
					UISetter.SetGameObjectName(btn, $"Upgrade-{text}");
				}
			}
			else
			{
				UISetter.SetLabel(this.level, $"[FC6120FF]{level}[-][76480CFF]LV[-]");
				UISetter.SetLabel(description, string.Format("[76480CFF]{0}[-] [6AA635FF]-{1:0.##}%[-]", Localization.Get("400011"), (float)strongestBuffBattleDataRow.buffAdd / 100f));
				UISetter.SetGameObjectName(btn, $"Setting-{text}");
			}
			break;
		case "defb":
			UISetter.SetSprite(icon, "group_buff_1002");
			UISetter.SetLabel(activeType, Localization.Get("400006"));
			if (upgrade)
			{
				UISetter.SetActive(empty, strongestBuffBattleDataRow2 == null);
				UISetter.SetActive(resourceRoot, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(this.level, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(description, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(upgradeLabel, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(upgradeDisableLabel, strongestBuffBattleDataRow2 == null);
				UISetter.SetButtonEnable(btn, strongestBuffBattleDataRow2 != null);
				if (strongestBuffBattleDataRow2 != null)
				{
					UISetter.SetLabel(this.level, $"[FC6120FF]{level + 1}[-][76480CFF]LV[-]");
					UISetter.SetLabel(costCount, strongestBuffBattleDataRow.upgradeCoin);
					UISetter.SetLabel(description, $"[6AA635FF]+{(float)(strongestBuffBattleDataRow2.buffAdd - strongestBuffBattleDataRow.buffAdd) / 100f:0.##}%[-]");
					UISetter.SetGameObjectName(btn, $"Upgrade-{text}");
				}
			}
			else
			{
				UISetter.SetLabel(this.level, $"[FC6120FF]{level}[-][76480CFF]LV[-]");
				UISetter.SetLabel(description, string.Format("[76480CFF]{0}[-] [6AA635FF]+{1:0.##}%[-]", Localization.Get("400012"), (float)strongestBuffBattleDataRow.buffAdd / 100f));
				UISetter.SetGameObjectName(btn, $"Setting-{text}");
			}
			break;
		case "defd":
			UISetter.SetSprite(icon, "group_buff_1002_off");
			UISetter.SetLabel(activeType, Localization.Get("400009"));
			if (upgrade)
			{
				UISetter.SetActive(empty, strongestBuffBattleDataRow2 == null);
				UISetter.SetActive(resourceRoot, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(this.level, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(description, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(upgradeLabel, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(upgradeDisableLabel, strongestBuffBattleDataRow2 == null);
				UISetter.SetButtonEnable(btn, strongestBuffBattleDataRow2 != null);
				if (strongestBuffBattleDataRow2 != null)
				{
					UISetter.SetLabel(this.level, $"[FC6120FF]{level + 1}[-][76480CFF]LV[-]");
					UISetter.SetLabel(costCount, strongestBuffBattleDataRow.upgradeCoin);
					UISetter.SetLabel(description, $"[6AA635FF]-{(float)(strongestBuffBattleDataRow2.buffAdd - strongestBuffBattleDataRow.buffAdd) / 100f:0.##}%[-]");
					UISetter.SetGameObjectName(btn, $"Upgrade-{text}");
				}
			}
			else
			{
				UISetter.SetLabel(this.level, $"[FC6120FF]{level}[-][76480CFF]LV[-]");
				UISetter.SetLabel(description, string.Format("[76480CFF]{0}[-] [6AA635FF]-{1:0.##}%[-]", Localization.Get("400012"), (float)strongestBuffBattleDataRow.buffAdd / 100f));
				UISetter.SetGameObjectName(btn, $"Setting-{text}");
			}
			break;
		case "supb":
			UISetter.SetSprite(icon, "group_buff_1003");
			UISetter.SetLabel(activeType, Localization.Get("400007"));
			if (upgrade)
			{
				UISetter.SetActive(empty, strongestBuffBattleDataRow2 == null);
				UISetter.SetActive(resourceRoot, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(this.level, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(description, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(upgradeLabel, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(upgradeDisableLabel, strongestBuffBattleDataRow2 == null);
				UISetter.SetButtonEnable(btn, strongestBuffBattleDataRow2 != null);
				if (strongestBuffBattleDataRow2 != null)
				{
					UISetter.SetLabel(this.level, $"[FC6120FF]{level + 1}[-][76480CFF]LV[-]");
					UISetter.SetLabel(costCount, strongestBuffBattleDataRow.upgradeCoin);
					UISetter.SetLabel(description, $"[6AA635FF]+{(float)(strongestBuffBattleDataRow2.buffAdd - strongestBuffBattleDataRow.buffAdd) / 100f:0.##}%[-]");
					UISetter.SetGameObjectName(btn, $"Upgrade-{text}");
				}
			}
			else
			{
				UISetter.SetLabel(this.level, $"[FC6120FF]{level}[-][76480CFF]LV[-]");
				UISetter.SetLabel(description, string.Format("[76480CFF]{0}[-] [6AA635FF]+{1:0.##}%[-]", Localization.Get("400011"), (float)strongestBuffBattleDataRow.buffAdd / 100f));
				UISetter.SetGameObjectName(btn, $"Setting-{text}");
			}
			break;
		case "supd":
			UISetter.SetSprite(icon, "group_buff_1003_off");
			UISetter.SetLabel(activeType, Localization.Get("400010"));
			if (upgrade)
			{
				UISetter.SetActive(empty, strongestBuffBattleDataRow2 == null);
				UISetter.SetActive(resourceRoot, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(this.level, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(description, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(upgradeLabel, strongestBuffBattleDataRow2 != null);
				UISetter.SetActive(upgradeDisableLabel, strongestBuffBattleDataRow2 == null);
				UISetter.SetButtonEnable(btn, strongestBuffBattleDataRow2 != null);
				if (strongestBuffBattleDataRow2 != null)
				{
					UISetter.SetLabel(this.level, $"[FC6120FF]{level + 1}[-][76480CFF]LV[-]");
					UISetter.SetLabel(costCount, strongestBuffBattleDataRow.upgradeCoin);
					UISetter.SetLabel(description, $"[6AA635FF]-{(float)(strongestBuffBattleDataRow2.buffAdd - strongestBuffBattleDataRow.buffAdd) / 100f:0.##}%[-]");
					UISetter.SetGameObjectName(btn, $"Upgrade-{text}");
				}
			}
			else
			{
				UISetter.SetLabel(this.level, $"[FC6120FF]{level}[-][76480CFF]LV[-]");
				UISetter.SetLabel(description, string.Format("[76480CFF]{0}[-] [6AA635FF]-{1:0.##}%[-]", Localization.Get("400011"), (float)strongestBuffBattleDataRow.buffAdd / 100f));
				UISetter.SetGameObjectName(btn, $"Setting-{text}");
			}
			break;
		}
	}
}
