using System.Collections.Generic;
using UnityEngine;

public class UIEventBattleListPopup : UISimplePopup
{
	public UIDefaultListView eventListView;

	public void Init(List<Protocols.EventBattleInfo> list)
	{
		eventListView.Init(list, "Event-");
		UpdateEventBattleDeck(list);
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text.StartsWith("Event-"))
		{
			string s = text.Substring(text.IndexOf("-") + 1);
			base.network.RequestGetEventBattleData(int.Parse(s));
		}
		base.OnClick(sender);
	}

	private void UpdateEventBattleDeck(List<Protocols.EventBattleInfo> bannerList)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < bannerList.Count; i++)
		{
			Protocols.EventBattleInfo eventBattleInfo = bannerList[i];
			list.Add(int.Parse(eventBattleInfo.idx));
		}
		Utility.UpdateEventBattleDeck(list);
	}
}
