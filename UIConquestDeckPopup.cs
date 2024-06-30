using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConquestDeckPopup : UIPopup
{
	public UIDefaultListView deckListView;

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	public void Start()
	{
		SetAutoDestroy(autoDestory: true);
		OpenPopup();
	}

	public void Init(EConquestStageInfoType type)
	{
		deckListView.Init(base.localUser.conquestDeck, type, "Deck_");
	}

	public override void OnClick(GameObject sender)
	{
		SoundManager.PlaySFX("EFM_SelectButton_001");
		string text = sender.name;
		if (text == "Close")
		{
			if (!bBackKeyEnable)
			{
				ClosePopUp();
			}
		}
		else if (text == "MovingTroop")
		{
			UIConquestMap conquestMap = UIManager.instance.world.conquestMap;
			if (conquestMap.selectPoint == conquestMap.mainStageId)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110319"));
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110275"));
			}
		}
		else if (text.StartsWith("StandByTroop"))
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110319"));
		}
		else if (text.StartsWith("StandByPosition"))
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Get("110346"));
		}
		else if (text.StartsWith("AddSlot-"))
		{
			string s = text.Substring(text.IndexOf("-") + 1);
			OnBuySlotClicked(int.Parse(s));
		}
		else if (text.StartsWith("DeleteSlot-"))
		{
			string s2 = text.Substring(text.IndexOf("-") + 1);
			base.network.RequestDeleteConquestTroop(int.Parse(s2));
		}
		else if (text.StartsWith("SettingTroop-"))
		{
			string s3 = text.Substring(text.IndexOf("-") + 1);
			OnSettingTroopClicked(int.Parse(s3));
		}
		else if (text.StartsWith("MoveTroop-"))
		{
			string s4 = text.Substring(text.IndexOf("-") + 1);
			OnMoveTroopClicked(int.Parse(s4));
		}
	}

	private void OnMoveTroopClicked(int id)
	{
		base.network.RequestGetConquestMovePath(UIManager.instance.world.conquestMap.selectPoint, id);
	}

	private void OnBuySlotClicked(int id)
	{
		int num = int.Parse(base.regulation.defineDtbl["GUILD_OCCUPY_PREMIUM_TEAM_PRICE"].value);
		if (base.localUser.cash >= num)
		{
			UISimplePopup.CreateBool(localization: false, Localization.Get("110321"), Localization.Format("110324", num), Localization.Get("110325"), Localization.Get("110326"), Localization.Get("1000")).onClick = delegate(GameObject sender)
			{
				string text2 = sender.name;
				if (text2 == "OK")
				{
					base.network.RequestBuyConquestTroopSlot(id);
				}
			};
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				UIManager.instance.world.mainCommand.OpenDiamonShop();
			}
		};
	}

	private void OnSettingTroopClicked(int id)
	{
		BattleData battleData = BattleData.Create(EBattleType.Conquest);
		battleData.conquestDeckId = id;
		RoTroop item = RoTroop.Create(base.localUser.id);
		RoTroop item2 = RoTroop.Create(base.localUser.id);
		battleData.attacker = base.localUser.CreateForBattle(new List<RoTroop> { item });
		battleData.defender = base.localUser.CreateForBattle(new List<RoTroop> { item2 });
		base.uiWorld.readyBattle.InitAndOpenReadyBattle(battleData);
	}

	public void OpenPopup()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopUp()
	{
		bBackKeyEnable = true;
		if (base.uiWorld.existReadyBattle && base.uiWorld.readyBattle.isActive)
		{
			base.uiWorld.readyBattle.CloseAnimation();
		}
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0f);
		base.Close();
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0f);
		OnAnimOpen();
	}

	private void HidePopup()
	{
		OnAnimClose();
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
	}

	private void OnAnimClose()
	{
	}
}
