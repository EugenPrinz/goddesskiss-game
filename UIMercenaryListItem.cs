using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIMercenaryListItem : UIItemBase
{
	[SerializeField]
	private UISprite userThumbnail;

	[SerializeField]
	private UILabel userLevel;

	[SerializeField]
	private UILabel userNickName;

	[SerializeField]
	private List<UICommanderSelectItem> commanderList;

	[SerializeField]
	private GameObject UserImg;

	[SerializeField]
	private GameObject NpcIcon;

	[SerializeField]
	private GameObject checkEngage;

	public void Init(RoLocalUser.MercynaryUserList mercenaryInfo, RoTroop troop, BattleData battleData)
	{
		UISetter.SetActive(UserImg, active: false);
		UISetter.SetActive(NpcIcon, active: false);
		UISetter.SetActive(checkEngage, !mercenaryInfo.isEngagePossible);
		for (int i = 0; i < commanderList.Count; i++)
		{
			UISetter.SetActive(commanderList[i], active: false);
		}
		if (mercenaryInfo == null)
		{
			return;
		}
		if (mercenaryInfo.isNpc)
		{
			NPCMercenaryDataRow nPCMercenaryDataRow = RemoteObjectManager.instance.regulation.FindNpcMercenary(mercenaryInfo.npcId);
			CommanderDataRow commanderByUnitId = RemoteObjectManager.instance.regulation.GetCommanderByUnitId(nPCMercenaryDataRow.unitId);
			if (nPCMercenaryDataRow != null)
			{
				RoCommander commander = RemoteObjectManager.instance.localUser.FindMercenaryCommander(commanderByUnitId.id, -1, ECharacterType.NPCMercenary);
				UISetter.SetLabel(userNickName, Localization.Get(nPCMercenaryDataRow.explanation));
				UISetter.SetActive(commanderList[0], active: true);
				commanderList[0].Set(commander, troop, battleData, ECharacterType.NPCMercenary);
				UISetter.SetActive(NpcIcon, active: true);
				UISetter.SetSprite(userThumbnail, "group_buff_1005");
				UISetter.SetLabel(userLevel, string.Empty);
			}
		}
		else
		{
			UISetter.SetLabel(userLevel, string.Format(Localization.Get("1021"), mercenaryInfo.userLevel));
			UISetter.SetLabel(userNickName, mercenaryInfo.commanderList[0].userName);
			for (int j = 0; j < mercenaryInfo.commanderList.Count; j++)
			{
				RoCommander roCommander = RemoteObjectManager.instance.localUser.FindMercenaryCommander(mercenaryInfo.commanderList[j].cid.ToString(), mercenaryInfo.commanderList[j].userIdx, ECharacterType.Mercenary);
				UISetter.SetActive(commanderList[j], active: true);
				commanderList[j].Set(roCommander, troop, battleData, ECharacterType.Mercenary);
				UISetter.SetActive(UserImg, active: true);
				UISetter.SetSprite(userThumbnail, roCommander.getCurrentCostumeThumbnailName());
			}
		}
	}
}
