using System.Collections;
using System.Collections.Generic;
using System.Text;
using Shared.Regulation;
using UnityEngine;

public class UIGuildManagePopup : UIPopup
{
	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimBlock;

	public UIDefaultListView guildSkillListView;

	public UIFlipSwitch tapUpgrade;

	public UIFlipSwitch tapSetting;

	public GameObject upgradePage;

	public GameObject settingPage;

	public UILabel guildName;

	public UISprite guildEmblem;

	public UILabel guildLevel;

	public UILabel guildWorld;

	public UILabel guildPoint;

	public UILabel guildCount;

	public UILabel guildUpgradeCost;

	public UILabel guildUpgradeEffect;

	public UILabel changeUpgradeTapLabel;

	public UILabel guildChangeName;

	public UISprite guildChangEmblem;

	public UILabel guildChangMinLevel;

	public UIToggle toggleFree;

	public UIToggle toggleApprove;

	public UIButton btnChangeName;

	public UIButton btnChangeEmblem;

	public UIButton btnChangeMinLevel;

	public UIButton btnChangeType;

	public UIButton btnGuildUpgrad;

	public UIButton btnLeave;

	public UILabel nameChangeCostLabel;

	public UILabel emblemChangeCostLabel;

	public GameObject buttons;

	private int page;

	private int tempMinLevel;

	private int tempGuildType;

	public static readonly string guildSkillItemIdPrefix = "GuildSkillItem-";

	private int nameChangeCost;

	private int emblemChangeCost;

	private int currentMaxMemberCount;

	private int nextMaxMemberCount;

	private int upgradeCost;

	private string guildEffectInfo;

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string key = sender.name;
		if (key == "Close")
		{
			Close();
		}
		else if (key == "Upgrade")
		{
			SetPage(1);
		}
		else if (key == "Setting")
		{
			SetPage(2);
		}
		else if (key == "BtnChangeName")
		{
			if (base.localUser.cash < nameChangeCost)
			{
				NotEnough(MultiplePopUpType.NOTENOUGH_CASH);
				return;
			}
			UIReceiveUserString recv2 = UIPopup.Create<UIReceiveUserString>("InputUserString");
			recv2.SetDefault(string.Empty);
			recv2.Set(localization: true, Localization.Get("110237"), Localization.Get("110238"), null, Localization.Get("1006"), Localization.Get("1000"), null);
			recv2.SetLimitLength(10);
			recv2.onClick = delegate(GameObject popupSender)
			{
				string text7 = popupSender.name;
				if (text7 == "OK")
				{
					string text8 = recv2.inputLabel.text;
					if (text8.Length < 2 || text8.Length > 10 || string.IsNullOrEmpty(text8))
					{
						NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("80030"));
					}
					else if (!Utility.PossibleChar(text8))
					{
						NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7144"));
					}
					else if (text8 == base.localUser.guildInfo.name)
					{
						NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110021"));
					}
					else
					{
						recv2.ClosePopup();
						UISimplePopup.CreateBool(localization: true, Localization.Get("110239"), base.localUser.guildInfo.name + "\n↓\n" + recv2.input.value, string.Format(Localization.Get("110240"), nameChangeCost), Localization.Get("1006"), Localization.Get("1000")).onClick = delegate(GameObject popupSender_1)
						{
							string text9 = popupSender_1.name;
							if (text9 == "OK")
							{
								base.network.RequestUpdateGuildInfo(0, recv2.input.value);
							}
							else if (!(text9 == "Cancel"))
							{
							}
						};
					}
				}
				else if (!(text7 == "Close"))
				{
				}
			};
		}
		else if (key == "BtnChangeEmblem")
		{
			if (base.localUser.cash < emblemChangeCost)
			{
				NotEnough(MultiplePopUpType.NOTENOUGH_CASH);
				return;
			}
			UIImageListPopup recv = UIPopup.Create<UIImageListPopup>("ImageListPopup");
			recv.Set(localization: true, Localization.Get("110020"), null, null, Localization.Get("1006"), null, Localization.Get("1000"));
			recv.SetItemGuildEmblem(base.localUser.guildInfo.emblem);
			recv.onClick = delegate(GameObject popupSender)
			{
				string text5 = popupSender.name;
				if (text5 == "OK")
				{
					if (recv.emblemIdx == base.localUser.guildInfo.emblem)
					{
						UISimplePopup.CreateOK(localization: true, "1303", "110270", null, Localization.Get("1001"));
					}
					else
					{
						UIEmblemChangePopup chaPopup = UIPopup.Create<UIEmblemChangePopup>("EmblemChangePopup");
						chaPopup.Set(localization: true, Localization.Get("110098"), null, string.Format(Localization.Get("110099"), emblemChangeCost), Localization.Get("1006"), Localization.Get("1000"), null);
						chaPopup.SetEmblem(recv.emblemIdx);
						chaPopup.onClick = delegate(GameObject popupSender_1)
						{
							string text6 = popupSender_1.name;
							if (text6 == "OK")
							{
								chaPopup.Close();
								base.network.RequestUpdateGuildInfo(1, recv.emblemIdx.ToString());
							}
							else if (!(text6 == "Cancel"))
							{
							}
						};
					}
				}
			};
		}
		else if (key == "BtnChangeLevel")
		{
			base.network.RequestUpdateGuildInfo(2, tempMinLevel.ToString());
		}
		else if (key == "BtnChangeType")
		{
			base.network.RequestUpdateGuildInfo(3, tempGuildType.ToString());
		}
		else if (key == "BtnPlus")
		{
			if (tempMinLevel < base.localUser.level)
			{
				SetChangeMinLevel(++tempMinLevel);
			}
		}
		else if (key == "BtnMinus")
		{
			if (tempMinLevel > base.localUser.FindBuilding(EBuilding.Guild).firstLevelReg.userLevel)
			{
				SetChangeMinLevel(--tempMinLevel);
			}
		}
		else if (key == "BtnCloseDown")
		{
			UISimplePopup.CreateBool(localization: true, Localization.Get("110096"), Localization.Get("110103"), Localization.Get("110241"), Localization.Get("110105"), Localization.Get("1000")).onClick = delegate(GameObject popupSender)
			{
				string text4 = popupSender.name;
				if (text4 == "OK")
				{
					base.network.RequestGuildCloseDown();
				}
				else if (!(text4 == "Cancel"))
				{
				}
			};
		}
		else if (key == "BtnGuildUpgrade")
		{
			if (base.localUser.guildInfo.point < upgradeCost)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110235"));
				return;
			}
			string[] array = guildEffectInfo.Split('\n');
			string infoName = array[0];
			string infoText = ((array.Length <= 1) ? null : guildEffectInfo.Remove(0, guildEffectInfo.IndexOf('\n') + 1));
			UIGuildUpgradePopup uIGuildUpgradePopup = UIPopup.Create<UIGuildUpgradePopup>("GuildUpgradePopup");
			uIGuildUpgradePopup.Set(localization: true, Localization.Get("110090"), null, string.Format(Localization.Get("110091"), upgradeCost), Localization.Get("1007"), Localization.Get("1000"), null);
			uIGuildUpgradePopup.SetGuildUpgrade(infoName, base.localUser.guildInfo.level, infoText, string.Format(Localization.Get("110262"), currentMaxMemberCount), string.Format(Localization.Get("110262"), nextMaxMemberCount));
			uIGuildUpgradePopup.onClick = delegate(GameObject popupSender)
			{
				string text3 = popupSender.name;
				if (text3 == "OK")
				{
					base.network.RequestUpgradeGuildLevel();
				}
				else if (!(text3 == "Cancel"))
				{
				}
			};
		}
		else if (key.StartsWith("BtnSkillUpgrade-"))
		{
			RoGuildSkill skill = base.localUser.guildSkillList.Find((RoGuildSkill list) => list.idx.ToString() == key.Replace("BtnSkillUpgrade-", string.Empty));
			if (skill.isMaxLevel)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110295"));
				return;
			}
			if (base.localUser.guildInfo.level < skill.nextLevelReg.level)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110259"));
				return;
			}
			if (base.localUser.guildInfo.point < skill.nextLevelReg.cost)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("110235"));
				return;
			}
			UIGuildUpgradePopup uIGuildUpgradePopup2 = UIPopup.Create<UIGuildUpgradePopup>("GuildUpgradePopup");
			uIGuildUpgradePopup2.Set(localization: true, Localization.Get("110094"), null, string.Format(Localization.Get("110095"), skill.nextLevelReg.cost), Localization.Get("1007"), Localization.Get("1000"), null);
			uIGuildUpgradePopup2.SetSkillUpgrade(skill.idx, Localization.Format(skill.name), skill.skillLevel, Localization.Format(skill.description, skill.reg.value), Localization.Format(skill.description, skill.nextLevelReg.value));
			uIGuildUpgradePopup2.onClick = delegate(GameObject popupSender)
			{
				string text2 = popupSender.name;
				if (text2 == "OK")
				{
					base.network.RequestUpgradeGuildSkill(skill.idx);
				}
				else if (!(text2 == "Cancel"))
				{
				}
			};
		}
		else
		{
			if (!(key == "BtnLeave"))
			{
				return;
			}
			UISimplePopup.CreateBool(localization: true, Localization.Get("110109"), (base.localUser.guildInfo.state != 0) ? Localization.Get("110113") : Localization.Get("110110"), Localization.Get("110244"), Localization.Get("110115"), Localization.Get("1000")).onClick = delegate(GameObject popupSender)
			{
				string text = popupSender.name;
				if (text == "OK")
				{
					base.network.RequestLeaveGuild();
				}
				else if (!(text == "Cancel"))
				{
				}
			};
		}
	}

	public void OnValueChande(GameObject sender)
	{
		if (toggleFree.value)
		{
			SetChangeType(1);
		}
		else
		{
			SetChangeType(2);
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		SetManage();
	}

	public void InitAndOpenGuildManage()
	{
		SetPage(1);
		SetManage();
		if (guildSkillListView != null)
		{
			guildSkillListView.scrollView.SetDragAmount(0f, 0f, updateScrollbars: false);
		}
		OpenPopup();
		SetRecyclable(recyclable: false);
	}

	private void SetManage()
	{
		SetGuildInof();
		SetChangeInfo();
		SetUpgradeInfo();
	}

	public void SetGuildInof()
	{
		UISetter.SetLabel(guildCount, $"[FF6000FF]{base.localUser.guildInfo.count}[-][4E1F00FF]/{base.localUser.guildInfo.maxCount}[-]");
		UISetter.SetLabel(guildName, base.localUser.guildInfo.name);
		UISetter.SetLabel(guildLevel, "Lv " + base.localUser.guildInfo.level);
		UISetter.SetLabel(guildWorld, Localization.Format("19067", base.localUser.guildInfo.world));
		UISetter.SetLabel(guildPoint, base.localUser.guildInfo.point);
		UISetter.SetSpriteWithSnap(guildEmblem, "union_mark_" + $"{base.localUser.guildInfo.emblem:00}");
		UISetter.SetActive(tapUpgrade, base.localUser.guildInfo.memberGrade == 1);
		UISetter.SetActive(tapSetting, base.localUser.guildInfo.memberGrade == 1);
		UISetter.SetActive(buttons, base.localUser.guildInfo.memberGrade == 1);
		UISetter.SetActive(btnLeave, base.localUser.guildInfo.memberGrade != 1);
		UISetter.SetLabel(changeUpgradeTapLabel, (base.localUser.guildInfo.memberGrade != 1) ? Localization.Get("110227") : Localization.Get("1007"));
	}

	private void SetChangeInfo()
	{
		nameChangeCost = int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["GUILD_NAME_CHANGE_PRICE"].value);
		emblemChangeCost = int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["GUILD_EMBLEM_CHANGE_PRICE"].value);
		UISetter.SetLabel(nameChangeCostLabel, nameChangeCost);
		UISetter.SetLabel(emblemChangeCostLabel, emblemChangeCost);
		SetChangeName(base.localUser.guildInfo.name);
		SetChangeEmblem(base.localUser.guildInfo.emblem);
		SetChangeMinLevel(base.localUser.guildInfo.limitLevel);
		SetChangeType(base.localUser.guildInfo.guildType);
	}

	public void SetChangeName(string name)
	{
		UISetter.SetLabel(guildChangeName, name);
	}

	public void SetChangeEmblem(int emblem)
	{
		UISetter.SetSpriteWithSnap(guildChangEmblem, "union_mark_" + string.Format("{0:00}", emblem, true));
	}

	public void SetChangeMinLevel(int level)
	{
		UISetter.SetLabel(guildChangMinLevel, "Lv " + level);
		UISetter.SetActive(btnChangeMinLevel, level != base.localUser.guildInfo.limitLevel);
		tempMinLevel = level;
	}

	public void SetChangeType(int type)
	{
		UISetter.SetToggle(toggleFree, type == 1);
		UISetter.SetToggle(toggleApprove, type != 1);
		UISetter.SetActive(btnChangeType, type != base.localUser.guildInfo.guildType);
		tempGuildType = type;
	}

	private void SetUpgradeInfo()
	{
		SetGuildUpgradeInfo();
		SetGuildSkillList();
	}

	public void SetGuildUpgradeInfo()
	{
		GuildLevelInfoDataRow guildLevelInfoDataRow = RemoteObjectManager.instance.regulation.FindGuildLevelInfoData(base.localUser.guildInfo.level);
		GuildLevelInfoDataRow guildLevelInfoDataRow2 = RemoteObjectManager.instance.regulation.FindGuildLevelInfoData(base.localUser.guildInfo.level + 1);
		UISetter.SetActive(btnGuildUpgrad, guildLevelInfoDataRow2 != null);
		if (guildLevelInfoDataRow2 != null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format(Localization.Get("110233"), guildLevelInfoDataRow2.maxcount));
			List<GuildSkillDataRow> list = RemoteObjectManager.instance.regulation.FindGuildSkillDataLevel(base.localUser.guildInfo.level + 1);
			foreach (GuildSkillDataRow item in list)
			{
				stringBuilder.Append("\n");
				stringBuilder.Append(string.Format(Localization.Get("110234"), Localization.Get(item.name), item.skilllevel));
			}
			UISetter.SetLabel(guildUpgradeCost, guildLevelInfoDataRow2.cost);
			if (base.localUser.guildInfo.point < guildLevelInfoDataRow2.cost)
			{
				guildUpgradeCost.color = new Color(1f, 0f, 0f);
			}
			UISetter.SetLabel(guildUpgradeEffect, stringBuilder);
			upgradeCost = guildLevelInfoDataRow2.cost;
			currentMaxMemberCount = guildLevelInfoDataRow.maxcount;
			nextMaxMemberCount = guildLevelInfoDataRow2.maxcount;
			guildEffectInfo = stringBuilder.ToString();
		}
		else
		{
			UISetter.SetLabel(guildUpgradeEffect, Localization.Get("110236"));
		}
	}

	public void SetGuildSkillList()
	{
		if (guildSkillListView != null)
		{
			guildSkillListView.Init(base.localUser.guildSkillList, guildSkillItemIdPrefix);
			guildSkillListView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
		}
	}

	private void NotEnough(MultiplePopUpType type)
	{
		switch (type)
		{
		case MultiplePopUpType.NOTENOUGH_CASH:
			UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject sender)
			{
				string text = sender.name;
				if (text == "OK")
				{
					UIManager.instance.world.mainCommand.OpenDiamonShop();
				}
			};
			break;
		case MultiplePopUpType.NOTENOUGH_GOLD:
			UISimplePopup.CreateBool(localization: true, "1029", "1030", null, "1001", "1000").onClick = delegate(GameObject sender)
			{
				string text2 = sender.name;
				if (text2 == "OK")
				{
					UIManager.instance.world.camp.GoNavigation("MetroBank");
				}
			};
			break;
		}
	}

	private void SetPage(int _page)
	{
		page = _page;
		UISetter.SetActive(upgradePage, page == 1);
		UISetter.SetActive(settingPage, page == 2);
	}

	private string _GetOriginalName(GameObject go)
	{
		if (go == null)
		{
			return string.Empty;
		}
		string text = go.name;
		if (!text.Contains("-"))
		{
			return text;
		}
		return text.Remove(text.IndexOf("-"));
	}

	public void OpenPopup()
	{
		base.Open();
		AnimBG.Reset();
		AnimBlock.Reset();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
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
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}
}
