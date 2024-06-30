using Shared;
using UnityEngine;

public class UIEngageInfoPopup : UIPanelBase
{
	[SerializeField]
	private UILabel userName_1;

	[SerializeField]
	private UILabel userName_2;

	[SerializeField]
	private UILabel spendGold_label;

	[SerializeField]
	private UILabel spendDia_labe;

	private int spendGold = -1;

	private BattleData battleData;

	private RoCommander commander;

	public void SetEngagedPopup()
	{
		UIReadyBattle readyBattle = base.uiWorld.readyBattle;
		battleData = readyBattle.battleData;
		RoTroop roTroop = new RoTroop();
		roTroop = readyBattle.selectedTroop;
		for (int i = 0; i < roTroop.slots.Length; i++)
		{
			if (roTroop.slots[i].charType != ECharacterType.Mercenary && roTroop.slots[i].charType != ECharacterType.NPCMercenary)
			{
				continue;
			}
			commander = base.localUser.FindMercenaryCommander(roTroop.slots[i].commanderId, roTroop.slots[i].userIdx, roTroop.slots[i].charType);
			if (commander != null)
			{
				UISetter.SetActive(this, active: true);
				if (roTroop.slots[i].charType == ECharacterType.Mercenary)
				{
					userName_1.text = string.Format(Localization.Get("1369"), roTroop.slots[i].userName, commander.reg.nickname);
					userName_2.text = string.Format(Localization.Get("1370"), roTroop.slots[i].userName);
					UISetter.SetActive(userName_2, active: true);
				}
				else
				{
					userName_1.text = string.Format(Localization.Get("99994"), commander.reg.nickname);
					UISetter.SetActive(userName_2, active: false);
				}
				spendGold = (int)(commander.GetdispatchFloatGold * 10f);
				UISetter.SetLabel(spendGold_label, spendGold.ToString("N0"));
				UISetter.SetLabel(spendDia_labe, base.regulation.defineDtbl["GUILD_PREMIUM_EMPLOY"].value);
			}
			break;
		}
	}

	public void StartBattle_withNormalEngage()
	{
		RoLocalUser roLocalUser = RemoteObjectManager.instance.localUser;
		if (roLocalUser.gold < spendGold)
		{
			UISimplePopup.CreateBool(localization: true, "1029", "1030", null, "1001", "1000").onClick = delegate(GameObject sender)
			{
				string text = sender.name;
				if (text == "OK")
				{
					UIManager.instance.world.camp.GoNavigation("MetroBank");
				}
			};
		}
		else
		{
			base.uiWorld.readyBattle.StartBattle_withMercenary(0);
		}
	}

	public void Cancle()
	{
		UISetter.SetActive(this, active: false);
	}

	public void StartBattle_withHighEngage()
	{
		RoLocalUser roLocalUser = RemoteObjectManager.instance.localUser;
		int num = int.Parse(base.regulation.defineDtbl["GUILD_PREMIUM_EMPLOY"].value);
		if (roLocalUser.cash < num)
		{
			UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject sender)
			{
				string text = sender.name;
				if (text == "OK")
				{
					UIManager.instance.world.mainCommand.OpenDiamonShop();
				}
			};
			return;
		}
		UIReadyBattle readyBattle = base.uiWorld.readyBattle;
		RoTroop selectedTroop = readyBattle.selectedTroop;
		for (int i = 0; i < selectedTroop.slots.Length; i++)
		{
			if ((selectedTroop.slots[i].charType == ECharacterType.Mercenary || selectedTroop.slots[i].charType == ECharacterType.NPCMercenary) && commander != null)
			{
				Troop.Slot.Skill skill = selectedTroop.slots[i].skills[1];
				skill.sp = commander.maxSp;
				selectedTroop.slots[i].skills[1] = skill;
				selectedTroop.slots[i].charType = ((selectedTroop.slots[i].charType != ECharacterType.Mercenary) ? ECharacterType.SuperNPCMercenary : ECharacterType.SuperMercenary);
			}
		}
		base.uiWorld.readyBattle.StartBattle_withMercenary(1);
	}
}
