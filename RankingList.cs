using System.Collections.Generic;
using UnityEngine;

public class RankingList : UIPopup
{
	public UIDefaultListView rankingListView;

	public void Set(EBattleType type, List<RoUser> list)
	{
		SetAutoDestroy(autoDestory: true);
		rankingListView.InitRankingList(type, list);
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "Close")
		{
			Close();
		}
	}
}
