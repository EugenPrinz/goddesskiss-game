using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIEventBattleGacha : UIPanelBase
{
	public List<UISprite> eventIcons;

	public UILabel eventPoint;

	public UILabel curSeason;

	public UILabel remainTotalCount;

	public UILabel one_requireResCount;

	public UILabel one_possibleGachaCount;

	public UILabel many_requireResCount;

	public UILabel many_possibleGachaCount;

	public GameObject btnReset;

	public UIDefaultListView gacheListView;

	public GameObject openAnimationRoot;

	public Animation normalBoxAnimation;

	public Animation premiumBoxAnimation;

	public Animation rainbowBoxAnimation;

	private UISimplePopup _infoPopUp;

	private int _eventId;

	private int _eventPoint;

	private int _gachaOneTimeAmount;

	private int _remainPickupCount;

	private int _remainTotalCount;

	private int _sel_one_possibleGachaCount = 1;

	private int _sel_one_requireResCount;

	private int _sel_many_possibleGachaCount;

	private int _sel_many_requireResCount;

	private string _eventResName;

	private Animation _sel_openAnimation;

	private Protocols.EventBattleGachaInfo _data;

	protected override void OnEnable()
	{
		base.OnEnable();
		openAnimationRoot.SetActive(value: false);
		normalBoxAnimation.gameObject.SetActive(value: false);
		premiumBoxAnimation.gameObject.SetActive(value: false);
		rainbowBoxAnimation.gameObject.SetActive(value: false);
		_sel_openAnimation = null;
		gacheListView.ResetPosition();
	}

	public void Refresh()
	{
		if (_data == null)
		{
			return;
		}
		EventBattleDataRow eventBattleDataRow = base.regulation.eventBattleDtbl[_eventId.ToString()];
		if (int.Parse(eventBattleDataRow.eventPointIdx) > 0)
		{
			GoodsDataRow goodsDataRow = base.regulation.goodsDtbl[eventBattleDataRow.eventPointIdx];
			int num = base.localUser.resourceList[goodsDataRow.serverFieldName];
			if (num != _eventPoint)
			{
				Set(_eventId, _data);
			}
		}
	}

	public void Set(int eventId, Protocols.EventBattleGachaInfo data)
	{
		_data = data;
		_eventId = eventId;
		EventBattleDataRow eventBattleDataRow = base.regulation.eventBattleDtbl[_eventId.ToString()];
		if (int.Parse(eventBattleDataRow.eventPointIdx) <= 0)
		{
			return;
		}
		_remainTotalCount = 0;
		_remainPickupCount = 0;
		_gachaOneTimeAmount = eventBattleDataRow.gachaOneTimeAmount;
		_sel_one_requireResCount = _gachaOneTimeAmount;
		_sel_many_requireResCount = 0;
		_sel_many_possibleGachaCount = 0;
		List<EventBattleGachaRewardDataRow> list = base.regulation.eventBattleGachaRewardDtbl.FindAll((EventBattleGachaRewardDataRow x) => x.eventIdx == _eventId && x.count == _data.season);
		if (gacheListView != null)
		{
			gacheListView.ResizeItemList(list.Count);
		}
		for (int i = 0; i < list.Count; i++)
		{
			EventBattleGachaRewardDataRow eventBattleGachaRewardDataRow = list[i];
			int num = 0;
			if (_data.info.ContainsKey(eventBattleGachaRewardDataRow.idx))
			{
				num = _data.info[eventBattleGachaRewardDataRow.idx];
			}
			int num2 = eventBattleGachaRewardDataRow.rewardCount - num;
			_remainTotalCount += num2;
			if (num2 > 0 && eventBattleGachaRewardDataRow.mainReward == 1)
			{
				_remainPickupCount += num2;
			}
			if (gacheListView != null)
			{
				UIEventBattleGachaItem uIEventBattleGachaItem = gacheListView.itemList[i] as UIEventBattleGachaItem;
				uIEventBattleGachaItem.Set(eventBattleGachaRewardDataRow, num2);
			}
		}
		GoodsDataRow goodsDataRow = base.regulation.goodsDtbl[eventBattleDataRow.eventPointIdx];
		_eventPoint = base.localUser.resourceList[goodsDataRow.serverFieldName];
		_eventResName = goodsDataRow.name;
		int num3 = int.Parse(base.regulation.defineDtbl["EVENTBATTLE_GACHA_MIN_COUNT"].value);
		int num4 = int.Parse(base.regulation.defineDtbl["EVENTBATTLE_GACHA_MAX_COUNT"].value);
		int num5 = ((_eventPoint > _gachaOneTimeAmount) ? (_eventPoint / _gachaOneTimeAmount) : 0);
		if (num5 > _remainTotalCount)
		{
			num5 = _remainTotalCount;
		}
		if (num5 < num3)
		{
			_sel_many_possibleGachaCount = num3;
		}
		else if (num5 > num4)
		{
			_sel_many_possibleGachaCount = num4;
		}
		else
		{
			_sel_many_possibleGachaCount = num5;
		}
		_sel_many_requireResCount = _gachaOneTimeAmount * _sel_many_possibleGachaCount;
		UISetter.SetLabel(curSeason, Localization.Format("10999995", _data.season));
		UISetter.SetLabel(remainTotalCount, Localization.Format("11000002", _remainTotalCount));
		UISetter.SetActive(btnReset, _data.reset == 1);
		for (int j = 0; j < eventIcons.Count; j++)
		{
			UISetter.SetSprite(eventIcons[j], goodsDataRow.iconId);
		}
		UISetter.SetLabel(eventPoint, _eventPoint);
		UISetter.SetLabel(one_possibleGachaCount, Localization.Format("17018", _sel_one_possibleGachaCount));
		UISetter.SetLabel(one_requireResCount, _sel_one_requireResCount);
		UISetter.SetLabel(many_possibleGachaCount, Localization.Format("17018", _sel_many_possibleGachaCount));
		UISetter.SetLabel(many_requireResCount, _sel_many_requireResCount);
	}

	public Coroutine OpenGacha(Protocols.EventBattleGachaInfo data)
	{
		int num = 0;
		Dictionary<int, int>.Enumerator enumerator = data.info.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (!_data.info.ContainsKey(enumerator.Current.Key) || _data.info[enumerator.Current.Key] != enumerator.Current.Value)
			{
				string key = $"{_eventId}_{data.season}_{enumerator.Current.Key}";
				EventBattleGachaRewardDataRow eventBattleGachaRewardDataRow = base.regulation.eventBattleGachaRewardDtbl[key];
				if (num < eventBattleGachaRewardDataRow.effect)
				{
					num = eventBattleGachaRewardDataRow.effect;
				}
			}
		}
		Set(_eventId, data);
		return StartCoroutine(_OpenGachaAnimation(num));
	}

	private IEnumerator _OpenGachaAnimation(int effect)
	{
		openAnimationRoot.SetActive(value: true);
		switch (effect)
		{
		case 1:
			_sel_openAnimation = normalBoxAnimation;
			break;
		case 2:
			_sel_openAnimation = premiumBoxAnimation;
			break;
		case 3:
			_sel_openAnimation = rainbowBoxAnimation;
			break;
		default:
			_sel_openAnimation = normalBoxAnimation;
			break;
		}
		_sel_openAnimation.gameObject.SetActive(value: true);
		yield return _sel_openAnimation.Play();
		while (_sel_openAnimation.isPlaying)
		{
			yield return null;
		}
		_sel_openAnimation.gameObject.SetActive(value: false);
		openAnimationRoot.SetActive(value: false);
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		switch (sender.name)
		{
		case "StartGacha-One":
			_StartOpenGacha(_sel_one_possibleGachaCount);
			break;
		case "StartGacha-Many":
			_StartOpenGacha(_sel_many_possibleGachaCount);
			break;
		case "ResetGacha":
			_StartResetGacha();
			break;
		case "InfoGachaBtn":
			if (_infoPopUp == null)
			{
				_infoPopUp = UISimplePopup.CreateOK("InformationPopup");
				_infoPopUp.Set(localization: true, "10999997", "10999998", string.Empty, "1001", string.Empty, string.Empty);
			}
			break;
		case "OpenAnimationSkip":
			if (_sel_openAnimation != null && _sel_openAnimation.isPlaying)
			{
				_sel_openAnimation.Stop();
			}
			break;
		}
	}

	private void _StartOpenGacha(int gachaCount)
	{
		if (_eventPoint < gachaCount * _gachaOneTimeAmount)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Format("11000005", Localization.Get(_eventResName)));
		}
		else if (_remainTotalCount < gachaCount)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("11000006"));
		}
		else
		{
			base.network.RequestEventBattleGachaOpen(_eventId, gachaCount);
		}
	}

	private void _StartResetGacha()
	{
		if (_remainPickupCount > 0)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("11000003"));
		}
		else if (_remainTotalCount > 0)
		{
			UISimplePopup.CreateBool(localization: true, "11000000", "10999999", null, "1304", "1305").onClick = delegate(GameObject obj)
			{
				string text = obj.name;
				if (text == "OK")
				{
					base.network.RequestEventBattleGachaReset(_eventId);
				}
			};
		}
		else
		{
			base.network.RequestEventBattleGachaReset(_eventId);
		}
	}
}
