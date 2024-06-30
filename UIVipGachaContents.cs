using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIVipGachaContents : UIPanelBase
{
	private List<Protocols.VipGacha.VipGachaInfo> currentGachaList;

	[SerializeField]
	private UIDefaultListView gachaListView;

	[SerializeField]
	private UILabel vipGachaCash;

	[SerializeField]
	private UILabel RemainCount;

	[SerializeField]
	private Animation rulletAnimation;

	[SerializeField]
	private UITimer gachaRemainTime;

	private int gachaCach;

	private int MaxGachaCount;

	public void Init(List<Protocols.VipGacha.VipGachaInfo> _list)
	{
		if (_list != null)
		{
			currentGachaList = _list;
			gachaListView.Init(_list);
			gachaCach = int.Parse(base.regulation.defineDtbl["VIP_GACHA_CASH"].value);
			MaxGachaCount = int.Parse(base.regulation.defineDtbl["VIP_GACHA_COUNT"].value);
			vipGachaCash.text = gachaCach.ToString();
			RemainCount.text = $"{MaxGachaCount - base.localUser.vipGachaCount} / {MaxGachaCount}";
			UISetter.SetTimer(gachaRemainTime, base.localUser.vipGachaRefreshTime);
			UIManager.instance.world.vipGacha.SetMainCommander();
		}
	}

	public void RegisterEndPopup()
	{
		gachaRemainTime.RegisterOnFinished(delegate
		{
			SetVipGachaResetAlarmPopup();
		});
	}

	public void UnRegisterEndPopup()
	{
		gachaRemainTime.AllReset();
	}

	public override void OnClick(GameObject sender)
	{
		if (!(sender == null))
		{
			string text = sender.name;
			if (text != null && text == "BuyBtn")
			{
				BuyVipGacha();
			}
			if (gachaListView.Contains(sender.name))
			{
				gachaListView.SetSelection(sender.name, selected: true);
			}
		}
	}

	private void BuyVipGacha()
	{
		RoLocalUser roLocalUser = RemoteObjectManager.instance.localUser;
		if (roLocalUser.cash < gachaCach)
		{
			UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "1001", "1000").onClick = delegate(GameObject sender)
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
			StartCoroutine(StartAnimation());
		}
	}

	private IEnumerator StartAnimation()
	{
		UISetter.SetActive(rulletAnimation.gameObject, active: true);
		while (rulletAnimation.IsPlaying("VipGacha"))
		{
			yield return null;
		}
		UISetter.SetActive(rulletAnimation.gameObject, active: false);
		RemoteObjectManager.instance.RequestBuyVipGacha();
	}

	private void SetVipGachaResetAlarmPopup()
	{
		UISimplePopup uISimplePopup = UISimplePopup.CreateOK(localization: true, "1303", "22008", null, "1001");
		if (!(uISimplePopup == null))
		{
			uISimplePopup.onClick = delegate
			{
				UIManager.instance.world.vipGacha.Close();
			};
			uISimplePopup.onClose = delegate
			{
				UIManager.instance.world.vipGacha.Close();
			};
		}
	}
}
