using Shared.Regulation;
using UnityEngine;

public class UIResourcePurchasePopup : UISimplePopup
{
	public UILabel btnLabel;

	private EVipRechargeType type;

	private string stageId;

	private RechargeState IsRecharge;

	private int step;

	public void initData(VipRechargeDataRow vipRow)
	{
		int num = (int)(type = (EVipRechargeType)int.Parse(vipRow.idx));
		IsRecharge = RechargeState.None;
		int vipLevel = base.localUser.vipLevel;
		int num2 = num;
		int num3 = 0;
		int maxRechargeCount = vipRow.GetMaxRechargeCount(vipLevel);
		if (type == EVipRechargeType.SweepTicket)
		{
			if (base.localUser.resourceRechargeList.ContainsKey(num2.ToString()))
			{
				step = base.localUser.resourceRechargeList[num2.ToString()];
			}
			int num4 = maxRechargeCount - step;
			num3 = vipRow.startRechargePrice * (int)Mathf.Pow(vipRow.priceAddPercent / 100, Mathf.Floor(step / vipRow.numberMeasure));
			UISetter.SetLabel(subMessage, Localization.Format("1028", num4, maxRechargeCount));
			UISetter.SetLabel(title, Localization.Format("1023", Localization.Get("4008")));
			UISetter.SetLabel(message, Localization.Format("1024", num3, Localization.Get("4008"), vipRow.rechargeAmount));
			UISetter.SetLabel(btnLabel, Localization.Get("1003"));
			if (num4 < 1)
			{
				IsRecharge = RechargeState.NotEnoughCount;
			}
		}
		else if (type == EVipRechargeType.Challenge)
		{
			if (base.localUser.resourceRechargeList.ContainsKey(num2.ToString()))
			{
				step = base.localUser.resourceRechargeList[num2.ToString()];
			}
			int num5 = maxRechargeCount - step;
			num3 = vipRow.startRechargePrice * (int)Mathf.Pow(vipRow.priceAddPercent / 100, Mathf.Floor(step / vipRow.numberMeasure));
			UISetter.SetLabel(subMessage, Localization.Format("1028", num5, maxRechargeCount));
			UISetter.SetLabel(title, Localization.Format("1023", Localization.Get("4010")));
			UISetter.SetLabel(message, Localization.Format("1024", num3, Localization.Get("4010"), vipRow.rechargeAmount));
			UISetter.SetLabel(btnLabel, Localization.Get("1003"));
			if (num5 < 1)
			{
				IsRecharge = RechargeState.NotEnoughCount;
			}
		}
		else if (type == EVipRechargeType.Skill)
		{
			if (base.localUser.resourceRechargeList.ContainsKey(num2.ToString()))
			{
				step = base.localUser.resourceRechargeList[num2.ToString()];
			}
			int num6 = maxRechargeCount - step;
			num3 = vipRow.startRechargePrice * (int)Mathf.Pow(vipRow.priceAddPercent / 100, Mathf.Floor(step / vipRow.numberMeasure));
			UISetter.SetLabel(subMessage, Localization.Format("1028", num6, maxRechargeCount));
			UISetter.SetLabel(title, Localization.Format("1023", Localization.Get("4029")));
			UISetter.SetLabel(message, Localization.Format("1024", num3, Localization.Get("4029"), vipRow.rechargeAmount));
			UISetter.SetLabel(btnLabel, Localization.Get("1003"));
		}
		else if (type == EVipRechargeType.Key)
		{
			if (base.localUser.resourceRechargeList.ContainsKey(num2.ToString()))
			{
				step = base.localUser.resourceRechargeList[num2.ToString()];
			}
			int num7 = maxRechargeCount - step;
			num3 = vipRow.startRechargePrice * (int)Mathf.Pow(vipRow.priceAddPercent / 100, Mathf.Floor(step / vipRow.numberMeasure));
			UISetter.SetLabel(subMessage, Localization.Format("1028", num7, maxRechargeCount));
			UISetter.SetLabel(title, Localization.Format("1023", Localization.Get("4011")));
			UISetter.SetLabel(message, Localization.Format("1024", num3, Localization.Get("4011"), vipRow.rechargeAmount));
			UISetter.SetLabel(btnLabel, Localization.Get("1003"));
			if (num7 < 1)
			{
				IsRecharge = RechargeState.NotEnoughCount;
			}
		}
		else if (type == EVipRechargeType.Bullet)
		{
			if (base.localUser.resourceRechargeList.ContainsKey(num2.ToString()))
			{
				step = base.localUser.resourceRechargeList[num2.ToString()];
			}
			int num8 = maxRechargeCount - step;
			num3 = vipRow.startRechargePrice * (int)Mathf.Pow(vipRow.priceAddPercent / 100, Mathf.Floor(step / vipRow.numberMeasure));
			UISetter.SetLabel(subMessage, Localization.Format("1028", num8, maxRechargeCount));
			UISetter.SetLabel(title, Localization.Format("1023", Localization.Get("4006")));
			UISetter.SetLabel(message, Localization.Format("1024", num3, Localization.Get("4006"), vipRow.rechargeAmount));
			UISetter.SetLabel(btnLabel, Localization.Get("1003"));
			if (num8 < 1)
			{
				IsRecharge = RechargeState.NotEnoughCount;
			}
		}
		else if (type == EVipRechargeType.Speak)
		{
			if (base.localUser.resourceRechargeList.ContainsKey(num2.ToString()))
			{
				step = base.localUser.resourceRechargeList[num2.ToString()];
			}
			int num9 = maxRechargeCount - step;
			num3 = vipRow.startRechargePrice * (int)Mathf.Pow(vipRow.priceAddPercent / 100, Mathf.Floor(step / vipRow.numberMeasure));
			UISetter.SetLabel(subMessage, Localization.Format("1028", num9, maxRechargeCount));
			UISetter.SetLabel(title, Localization.Format("1023", Localization.Get("4301")));
			UISetter.SetLabel(message, Localization.Format("1024", num3, Localization.Get("4301"), vipRow.rechargeAmount));
			UISetter.SetLabel(btnLabel, Localization.Get("1003"));
			if (num9 < 1)
			{
				IsRecharge = RechargeState.NotEnoughCount;
			}
		}
		else if (type == EVipRechargeType.StageType1 || type == EVipRechargeType.StageType2)
		{
			stageId = base.uiWorld.readyBattle.battleData.stageId;
			if (base.localUser.stageRechargeList.ContainsKey(stageId))
			{
				step = base.localUser.stageRechargeList[stageId];
			}
			int num10 = maxRechargeCount - step;
			num3 = vipRow.startRechargePrice * (int)Mathf.Pow(vipRow.priceAddPercent / 100, Mathf.Floor(step / vipRow.numberMeasure));
			stageId = base.uiWorld.readyBattle.battleData.stageId;
			UISetter.SetLabel(subMessage, Localization.Format("1028", num10, maxRechargeCount));
			UISetter.SetLabel(title, Localization.Get("15043"));
			UISetter.SetLabel(message, string.Format(Localization.Get("15042"), num3));
			UISetter.SetLabel(btnLabel, Localization.Get("1003"));
			if (num10 < 1)
			{
				IsRecharge = RechargeState.NotEnoughCount;
			}
		}
		else if (type == EVipRechargeType.ResetStackScore)
		{
			if (base.localUser.resourceRechargeList.ContainsKey(num2.ToString()))
			{
				step = base.localUser.resourceRechargeList[num2.ToString()];
			}
			int num11 = maxRechargeCount - step;
			num3 = vipRow.startRechargePrice * (int)Mathf.Pow(vipRow.priceAddPercent / 100, Mathf.Floor(step / vipRow.numberMeasure));
			UISetter.SetLabel(subMessage, Localization.Format("1028", num11, maxRechargeCount));
			UISetter.SetLabel(title, Localization.Get("1294"));
			UISetter.SetLabel(message, string.Format(Localization.Get("1295"), num3));
			UISetter.SetLabel(btnLabel, Localization.Get("1001"));
			if (num11 < 1)
			{
				IsRecharge = RechargeState.NotEnoughCount;
			}
		}
		else if (type == EVipRechargeType.CommandersGift)
		{
			if (base.localUser.resourceRechargeList.ContainsKey(num2.ToString()))
			{
				step = base.localUser.resourceRechargeList[num2.ToString()];
			}
			int num12 = maxRechargeCount - step;
			num3 = vipRow.startRechargePrice * (int)Mathf.Pow(vipRow.priceAddPercent / 100, Mathf.Floor(step / vipRow.numberMeasure));
			UISetter.SetLabel(subMessage, Localization.Format("1028", num12, maxRechargeCount));
			UISetter.SetLabel(title, Localization.Format("1023", Localization.Get("20041")));
			UISetter.SetLabel(message, Localization.Format("1024", num3, Localization.Get("20041"), vipRow.rechargeAmount));
			UISetter.SetLabel(btnLabel, Localization.Get("1003"));
		}
		else if (type == EVipRechargeType.waveDuelTicket)
		{
			if (base.localUser.resourceRechargeList.ContainsKey(num2.ToString()))
			{
				step = base.localUser.resourceRechargeList[num2.ToString()];
			}
			int num13 = maxRechargeCount - step;
			num3 = vipRow.startRechargePrice * (int)Mathf.Pow(vipRow.priceAddPercent / 100, Mathf.Floor(step / vipRow.numberMeasure));
			UISetter.SetLabel(subMessage, Localization.Format("1028", num13, maxRechargeCount));
			UISetter.SetLabel(title, Localization.Format("1023", Localization.Get("5050001")));
			UISetter.SetLabel(message, Localization.Format("1024", num3, Localization.Get("5050001"), vipRow.rechargeAmount));
			UISetter.SetLabel(btnLabel, Localization.Get("1003"));
			if (num13 < 1)
			{
				IsRecharge = RechargeState.NotEnoughCount;
			}
		}
		else if (type == EVipRechargeType.Oil)
		{
			if (base.localUser.resourceRechargeList.ContainsKey(num2.ToString()))
			{
				step = base.localUser.resourceRechargeList[num2.ToString()];
			}
			int num14 = maxRechargeCount - step;
			num3 = vipRow.startRechargePrice * (int)Mathf.Pow(vipRow.priceAddPercent / 100, Mathf.Floor(step / vipRow.numberMeasure));
			UISetter.SetLabel(subMessage, Localization.Format("1028", num14, maxRechargeCount));
			UISetter.SetLabel(title, Localization.Format("1023", Localization.Get("4381")));
			UISetter.SetLabel(message, Localization.Format("1024", num3, Localization.Get("4381"), vipRow.rechargeAmount));
			UISetter.SetLabel(btnLabel, Localization.Get("1003"));
			if (num14 < 1)
			{
				IsRecharge = RechargeState.NotEnoughCount;
			}
		}
		else if (type == EVipRechargeType.EventRaidTicket)
		{
			if (base.localUser.resourceRechargeList.ContainsKey(num2.ToString()))
			{
				step = base.localUser.resourceRechargeList[num2.ToString()];
			}
			int num15 = maxRechargeCount - step;
			num3 = vipRow.startRechargePrice * (int)Mathf.Pow(vipRow.priceAddPercent / 100, Mathf.Floor(step / vipRow.numberMeasure));
			UISetter.SetLabel(subMessage, Localization.Format("1028", num15, maxRechargeCount));
			UISetter.SetLabel(title, Localization.Format("1023", Localization.Get("10001001")));
			UISetter.SetLabel(message, Localization.Format("1024", num3, Localization.Get("10001001"), vipRow.rechargeAmount));
			UISetter.SetLabel(btnLabel, Localization.Get("1003"));
			if (num15 < 1)
			{
				IsRecharge = RechargeState.NotEnoughCount;
			}
		}
		else
		{
			if (type != EVipRechargeType.WorldDuelTicket)
			{
				return;
			}
			if (base.localUser.resourceRechargeList.ContainsKey(num2.ToString()))
			{
				step = base.localUser.resourceRechargeList[num2.ToString()];
			}
			int num16 = maxRechargeCount - step;
			num3 = vipRow.startRechargePrice * (int)Mathf.Pow(vipRow.priceAddPercent / 100, Mathf.Floor(step / vipRow.numberMeasure));
			UISetter.SetLabel(subMessage, Localization.Format("1028", num16, maxRechargeCount));
			UISetter.SetLabel(title, Localization.Format("1023", Localization.Get("21012")));
			UISetter.SetLabel(message, Localization.Format("1024", num3, Localization.Get("21012"), vipRow.rechargeAmount));
			UISetter.SetLabel(btnLabel, Localization.Get("1003"));
			if (num16 < 1)
			{
				IsRecharge = RechargeState.NotEnoughCount;
			}
		}
		if (base.localUser.cash < num3)
		{
			IsRecharge = RechargeState.NotEnoughCash;
		}
	}

	private void goCashShop()
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

	private void goCashShop2()
	{
		UISimplePopup.CreateBool(localization: true, "5635", "12006", null, "5348", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				UIManager.instance.world.mainCommand.OpenDiamonShop();
			}
		};
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (!(text == "Close"))
		{
			if (IsRecharge == RechargeState.NotEnoughCount)
			{
				goCashShop2();
				base.OnClick(sender);
				return;
			}
			if (IsRecharge == RechargeState.NotEnoughCash)
			{
				if (type == EVipRechargeType.ResetStackScore)
				{
					UISimplePopup.CreateOK(localization: true, "1303", "1044", null, "1001");
				}
				else
				{
					goCashShop();
				}
				base.OnClick(sender);
				return;
			}
			if (type == EVipRechargeType.StageType1 || type == EVipRechargeType.StageType2)
			{
				base.network.RequestResourceRecharge((int)type, int.Parse(stageId), step);
			}
			else
			{
				base.network.RequestResourceRecharge((int)type, 0, step);
			}
		}
		base.OnClick(sender);
	}
}
