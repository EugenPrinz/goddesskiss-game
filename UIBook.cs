using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIBook : UIPanelBase
{
	public UIDraggablePanel2 commanderListView;

	private UIBookDetail uiBooksDetail;

	public void InitData()
	{
		List<RoCommander> list = base.localUser.commanderList;
		commanderListView.Init(list.Count, delegate(UIListItem item, int index)
		{
			GameObject target = item.Target;
			UISetter.SetActive(target, active: true);
			UICommander component = target.GetComponent<UICommander>();
			component.Set(list[index]);
			component.gameObject.name = list[index].id;
		});
		commanderListView.ResetPosition();
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string key = sender.name;
		if (!commanderListView.Contains(key))
		{
			return;
		}
		RoCommander roCommander = base.localUser.FindCommander(key);
		if (roCommander.state == ECommanderState.Nomal)
		{
			if (uiBooksDetail == null)
			{
				uiBooksDetail = UIPopup.Create<UIBookDetail>("BookDetail");
				uiBooksDetail.Set(key);
			}
			return;
		}
		CommanderRankDataRow commanderRankDataRow = base.regulation.FindCommanderRankData(1);
		int num = roCommander.medal + base.localUser.medal;
		if (num < commanderRankDataRow.medal)
		{
			ItemExchangeDataRow item = base.regulation.itemExchangeDtbl.Find((ItemExchangeDataRow row) => row.type == EStorageType.Medal && row.typeidx == key);
			UINavigationPopUp uINavigationPopUp = UIPopup.Create<UINavigationPopUp>("NavigationPopUp");
			uINavigationPopUp.Init(EStorageType.Medal, roCommander.id, item);
			uINavigationPopUp.title.text = Localization.Get("5608");
		}
		else
		{
			CommanderRecruit(roCommander, commanderRankDataRow);
		}
	}

	public void CommanderRecruit(RoCommander commander, CommanderRankDataRow row)
	{
		UISimplePopup.CreateBool(localization: false, "지휘관 임명", $"골드 {row.gold}개를 사용하여\n지휘관 {commander.nickname}을 임명하시겠습니까?", string.Empty, "임명", "취소").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				if (base.localUser.gold < row.gold)
				{
					UISimplePopup.CreateOK(localization: true, "1029", "5062", null, "1001");
				}
				else
				{
					RemoteObjectManager.instance.RequestCommanderRankUp(commander.id);
					SoundManager.PlaySFX("EFM_PromotionCommander_001");
				}
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}
}
