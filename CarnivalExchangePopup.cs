using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class CarnivalExchangePopup : MonoBehaviour
{
	[SerializeField]
	private List<UIGoods> curItem;

	[SerializeField]
	private List<UIGoods> target;

	[SerializeField]
	private UILabel exchange_count;

	private int count = 1;

	private const int ITEM_MAX_COUNT = 2;

	private List<CurCarnivalItemInfo> curCarnivalList;

	private List<RewardDataRow> rewardList;

	private int ExchangeMaxCount;

	private RoLocalUser localUser;

	private Regulation regulation;

	private int carnivalTypeIdx;

	private bool isPressCnt;

	private int firstPress;

	private bool isMax;

	private string carnivalIdx = string.Empty;

	public void InitPopup(List<CurCarnivalItemInfo> carnivalItem, List<RewardDataRow> _rewardList, int MaxCount, int _carnivalTypeIdx, string cid)
	{
		for (int i = 0; i < 2; i++)
		{
			UISetter.SetActive(curItem[i], active: false);
			UISetter.SetActive(target[i], active: false);
		}
		localUser = RemoteObjectManager.instance.localUser;
		regulation = RemoteObjectManager.instance.regulation;
		carnivalIdx = cid;
		isMax = false;
		isPressCnt = false;
		count = 1;
		carnivalTypeIdx = _carnivalTypeIdx;
		curCarnivalList = carnivalItem;
		rewardList = _rewardList;
		ExchangeMaxCount = MaxCount;
		SetPopup();
	}

	private void SetPopup()
	{
		if (curCarnivalList != null && curCarnivalList.Count > 0)
		{
			for (int i = 0; i < curCarnivalList.Count; i++)
			{
				UISetter.SetActive(curItem[i], active: true);
				curItem[i].SetCarnivalItem(curCarnivalList[i]);
			}
		}
		if (rewardList != null && rewardList.Count > 0)
		{
			for (int j = 0; j < rewardList.Count; j++)
			{
				UISetter.SetActive(target[j], active: true);
				target[j].Set(rewardList[j]);
			}
		}
		UISetter.SetLabel(exchange_count, count);
	}

	private void SetCount()
	{
		if (curCarnivalList != null && curCarnivalList.Count > 0)
		{
			for (int i = 0; i < curCarnivalList.Count; i++)
			{
				curItem[i].SetCarvnivalItemNeedCount(curCarnivalList[i], count);
			}
		}
		if (rewardList != null && rewardList.Count > 0)
		{
			for (int j = 0; j < rewardList.Count; j++)
			{
				target[j].SetCarvnivalRewardCount(rewardList[j], count);
			}
		}
		UISetter.SetLabel(exchange_count, count);
	}

	public void PressCountBtn(GameObject sender)
	{
		isPressCnt = true;
		string text = sender.name;
		firstPress = 1;
		switch (text)
		{
		case "DecreaseBtn":
			StartCoroutine(MinusCount());
			break;
		case "AddcreaseBtn":
			StartCoroutine(PlusCount());
			break;
		}
		SoundManager.PlaySFX("BTN_Positive_001");
	}

	public void ReleasePressCountBtn()
	{
		isPressCnt = false;
		firstPress = 0;
	}

	private IEnumerator PlusCount()
	{
		while (isPressCnt && count < ExchangeMaxCount && !isMax)
		{
			int needCount = 0;
			string idx2 = string.Empty;
			int tempCount = count + 1;
			for (int i = 0; i < curCarnivalList.Count; i++)
			{
				needCount = curCarnivalList[i].needCount * tempCount;
				idx2 = curCarnivalList[i].idx;
				switch (curCarnivalList[i].type)
				{
				case ERewardType.Goods:
				case ERewardType.Favor:
				case ERewardType.EventItem:
				{
					GoodsDataRow goodsDataRow = regulation.goodsDtbl[idx2];
					if (goodsDataRow != null && localUser.resourceList[goodsDataRow.serverFieldName] < needCount)
					{
						isMax = true;
					}
					break;
				}
				case ERewardType.Medal:
				{
					RoCommander roCommander = localUser.FindCommander(idx2);
					if (roCommander != null && roCommander.medal < needCount)
					{
						isMax = true;
					}
					break;
				}
				case ERewardType.UnitMaterial:
				{
					RoPart roPart = localUser.FindPart(idx2);
					if (roPart != null && roPart.count < needCount)
					{
						isMax = true;
					}
					break;
				}
				}
			}
			if (count < ExchangeMaxCount && !isMax)
			{
				count = tempCount;
			}
			SetCount();
			if (firstPress == 0)
			{
				firstPress = 1;
				yield return new WaitForSeconds(0.8f);
			}
			else
			{
				yield return new WaitForSeconds(0.15f);
			}
		}
	}

	private IEnumerator MinusCount()
	{
		while (isPressCnt && count > 1)
		{
			isMax = false;
			count--;
			SetCount();
			if (firstPress == 0)
			{
				firstPress = 1;
				yield return new WaitForSeconds(0.8f);
			}
			else
			{
				yield return new WaitForSeconds(0.15f);
			}
		}
	}

	public void MaxCount()
	{
		count = GetMaxCount();
		SetCount();
		isMax = true;
		SoundManager.PlaySFX("BTN_Positive_001");
	}

	public void ExchangeOK()
	{
		SoundManager.PlaySFX("BTN_Positive_001");
		int eventID = UIManager.instance.world.carnival.exchange.GetEventID();
		RemoteObjectManager.instance.RequestCarnivalComplete(carnivalTypeIdx, carnivalIdx, eventID, count);
		UISetter.SetActive(this, active: false);
	}

	public void ExchangeCancle()
	{
		SoundManager.PlaySFX("BTN_Negative_001");
		UISetter.SetActive(this, active: false);
	}

	private int GetMaxCount()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		string empty = string.Empty;
		for (int i = 0; i < curCarnivalList.Count; i++)
		{
			num3 = curCarnivalList[i].needCount;
			empty = curCarnivalList[i].idx;
			switch (curCarnivalList[i].type)
			{
			case ERewardType.Goods:
			case ERewardType.Favor:
			case ERewardType.EventItem:
			{
				GoodsDataRow goodsDataRow = regulation.goodsDtbl[empty];
				if (goodsDataRow != null && localUser.resourceList[goodsDataRow.serverFieldName] >= num3)
				{
					num2 = localUser.resourceList[goodsDataRow.serverFieldName] / num3;
					if (num > num2 || num == 0)
					{
						num = num2;
					}
				}
				break;
			}
			case ERewardType.Medal:
			{
				RoCommander roCommander = localUser.FindCommander(empty);
				if (roCommander != null && roCommander.medal >= num3)
				{
					num2 = roCommander.aMedal / num3;
					if (num > num2 || num == 0)
					{
						num = num2;
					}
				}
				break;
			}
			case ERewardType.UnitMaterial:
			{
				RoPart roPart = localUser.FindPart(empty);
				if (roPart != null && roPart.count >= num3)
				{
					num2 = roPart.count / num3;
					if (num > num2 || num == 0)
					{
						num = num2;
					}
				}
				break;
			}
			}
		}
		if (num > ExchangeMaxCount)
		{
			num = ExchangeMaxCount;
		}
		return num;
	}
}
