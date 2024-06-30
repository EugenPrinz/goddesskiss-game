using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class InfinityTabListItem : UIItemBase
{
	public UISprite bg;

	public UILabel title;

	public GameObject star;

	public GameObject selectBg;

	public GameObject lockRoot;

	private string id;

	private RoLocalUser localUser;

	public override void SetSelection(bool selected)
	{
		base.SetSelection(selected);
		UISetter.SetActive(selectBg, selected);
	}

	public void Set(string idx, Dictionary<int, int> mission)
	{
		id = idx;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		InfinityFieldDataRow infinityFieldDataRow = regulation.infinityFieldDtbl[idx];
		UISetter.SetLabel(title, Localization.Get(infinityFieldDataRow.name));
		UISetter.SetActive(star, EnableReward(mission));
		UISetter.SetActive(lockRoot, isLock());
		if (infinityFieldDataRow.type == EInfinityStageType.Stage)
		{
			UISetter.SetSprite(bg, "Ar-bar01");
		}
		else if (infinityFieldDataRow.type == EInfinityStageType.Scenario)
		{
			UISetter.SetSprite(bg, "Ar-bar02");
		}
		if (infinityFieldDataRow.type == EInfinityStageType.Boss)
		{
			UISetter.SetSprite(bg, "Ar-bar03");
		}
	}

	private bool EnableReward(Dictionary<int, int> mission)
	{
		foreach (KeyValuePair<int, int> item in mission)
		{
			if (item.Value == 1)
			{
				return true;
			}
		}
		return false;
	}

	private bool isLock()
	{
		string currentIdx = UIManager.instance.world.infinityBattle.currentIdx;
		return int.Parse(id) > int.Parse(currentIdx);
	}
}
