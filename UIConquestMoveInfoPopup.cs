using Shared.Regulation;
using UnityEngine;

public class UIConquestMoveInfoPopup : UIPanelBase
{
	public UILabel description_1;

	public UILabel description_2;

	public UILabel spendDia_label;

	public UILabel tip_label;

	private int point;

	private int slot;

	private int remain;

	public void Init(int point, int slot, int remain)
	{
		this.point = point;
		this.slot = slot;
		this.remain = remain;
		string value = base.regulation.defineDtbl["GUILD_OCCUPY_QUICKMOVE"].value;
		string value2 = base.regulation.defineDtbl["GUILD_OCCUPY_QUICKMOVE_PRICE"].value;
		GuildOccupyDataRow guildOccupyDataRow = base.regulation.guildOccupyDtbl[point.ToString()];
		UISetter.SetLabel(description_1, Localization.Format("110277", Utility.GetTimeString(remain)));
		UISetter.SetLabel(description_2, Localization.Format("110279", Localization.Get(guildOccupyDataRow.s_idx)));
		UISetter.SetLabel(tip_label, Localization.Format("110335", value));
		UISetter.SetLabel(spendDia_label, value2);
	}

	public void normalMove()
	{
		if (!EnableMove(isCash: false))
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110333"));
			return;
		}
		base.network.RequestSetConquestMoveTroop(point, slot, 0);
		if (UIManager.instance.world.conquestMap.stagePopup.deckPopup != null)
		{
			UIManager.instance.world.conquestMap.stagePopup.deckPopup.ClosePopUp();
		}
		Cancle();
	}

	public void Cancle()
	{
		UISetter.SetActive(this, active: false);
	}

	public void highMove()
	{
		if (!EnableMove(isCash: true))
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110333"));
			return;
		}
		RoLocalUser roLocalUser = RemoteObjectManager.instance.localUser;
		int num = int.Parse(base.regulation.defineDtbl["GUILD_OCCUPY_QUICKMOVE_PRICE"].value);
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
		}
		else
		{
			base.network.RequestSetConquestMoveTroop(point, slot, 1);
			if (UIManager.instance.world.conquestMap.stagePopup.deckPopup != null)
			{
				UIManager.instance.world.conquestMap.stagePopup.deckPopup.ClosePopUp();
			}
			Cancle();
		}
	}

	public bool EnableMove(bool isCash)
	{
		int num = int.Parse(base.regulation.defineDtbl["GUILD_OCCUPY_QUICKMOVE"].value);
		if (isCash)
		{
			float num2 = (float)remain * (float)num * 0.01f;
			if ((double)(int)num2 <= base.uiWorld.guild.conquestTime.GetRemain())
			{
				return true;
			}
			return false;
		}
		if ((double)remain <= base.uiWorld.guild.conquestTime.GetRemain())
		{
			return true;
		}
		return false;
	}
}
