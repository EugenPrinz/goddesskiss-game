using System;
using UnityEngine;

public class RecordList : UIPopup
{
	public UIScrollView scrollView;

	public UIDefaultListView recordListView;

	public GameObject typeBtns;

	[HideInInspector]
	public string recordItemPrefix = "RePlay-";

	private ERePlayType type = ERePlayType.Challenge;

	private ERePlaySubType stype = ERePlaySubType.Attack;

	private new RoLocalUser localUser => RemoteObjectManager.instance.localUser;

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
	}

	protected bool CanRequest()
	{
		switch (stype)
		{
		case ERePlaySubType.Attack:
			return localUser.isUpdateAtkRecord;
		case ERePlaySubType.Defence:
			if (localUser.requestDefenceRecordTmTick != 0)
			{
				long value = DateTime.Now.Ticks - localUser.requestDefenceRecordTmTick;
				int num = (int)TimeSpan.FromTicks(value).TotalSeconds;
				int num2 = 5 - num;
				if (num2 > 0)
				{
					return false;
				}
			}
			break;
		}
		return true;
	}

	public void Init(ERePlayType type)
	{
		this.type = type;
		switch (type)
		{
		case ERePlayType.WaveDuel:
			UISetter.SetActive(typeBtns, active: true);
			if (CanRequest())
			{
				localUser.isUpdateAtkRecord = false;
				base.network.RequestGetRecordList(type, ERePlaySubType.Attack);
			}
			else
			{
				OnRefresh();
			}
			break;
		case ERePlayType.Challenge:
		case ERePlayType.WorldDuel:
			UISetter.SetActive(typeBtns, active: false);
			if (CanRequest())
			{
				localUser.isUpdateAtkRecord = false;
				base.network.RequestGetRecordList(type);
			}
			else
			{
				OnRefresh();
			}
			break;
		}
	}

	public override void OnRefresh()
	{
		switch (stype)
		{
		case ERePlaySubType.Attack:
			recordListView.Init(type, localUser.atkRecordList);
			break;
		case ERePlaySubType.Defence:
			recordListView.Init(type, localUser.defRecordList);
			break;
		}
		scrollView.ResetPosition();
	}

	public override void OnClick(GameObject sender)
	{
		if (_isClosed)
		{
			return;
		}
		switch (sender.name)
		{
		case "Close":
			Close();
			break;
		case "Attack":
			stype = ERePlaySubType.Attack;
			if (CanRequest())
			{
				if (type == ERePlayType.WaveDuel)
				{
					base.network.RequestGetRecordList(type, stype);
				}
				else
				{
					base.network.RequestGetRecordList(type);
				}
			}
			else
			{
				OnRefresh();
			}
			break;
		case "Defence":
			if (type == ERePlayType.WaveDuel)
			{
				stype = ERePlaySubType.Defence;
				if (CanRequest())
				{
					localUser.requestDefenceRecordTmTick = DateTime.Now.Ticks;
					base.network.RequestGetRecordList(type, stype);
				}
				else
				{
					OnRefresh();
				}
			}
			break;
		}
	}
}
