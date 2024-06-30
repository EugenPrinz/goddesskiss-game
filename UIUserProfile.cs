using Shared.Regulation;
using UnityEngine;

public class UIUserProfile : UIPanelBase
{
	public UILabel nickname;

	public UISprite picture;

	public UILabel level;

	public UILabel exp;

	public UIProgressBar expProgress;

	public UILabel vipLevel;

	public UILabel guildName;

	public UILabel uno;

	public UILabel serverInfo;

	private RoLocalUser _currUser;

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string key = sender.name;
		SoundManager.PlaySFX("BTN_Norma_001");
		if (key == "BtnVip")
		{
			base.uiWorld.mainCommand.OpenVipInfo(isShop: false);
		}
		else if (key == "ChangeNickname")
		{
			DefineDataRow defineDataRow = base.regulation.defineDtbl["USER_CHANGE_NICKNAME_CASH"];
			int cash = int.Parse(defineDataRow.value);
			UIReceiveUserString recv = UIPopup.Create<UIReceiveUserString>("InputUserString");
			recv.SetDefault(base.localUser.nickname);
			recv.SetLimitLength(10);
			recv.Set(localization: false, Localization.Get("7013"), Localization.Format("5208", cash), null, Localization.Get("1006"), Localization.Get("1000"), null);
			recv.onClick = delegate(GameObject popupSender)
			{
				string text2 = popupSender.name;
				if (text2 == "OK")
				{
					string text3 = recv.inputLabel.text;
					if (text3.Length < 2 || text3.Length > 10 || string.IsNullOrEmpty(text3))
					{
						NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7143"));
					}
					else if (!Utility.PossibleChar(text3))
					{
						NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7144"));
					}
					else if (text3 == base.localUser.nickname)
					{
						NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("7145"));
					}
					else if (_currUser.cash < cash)
					{
						UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject go)
						{
							string text4 = go.name;
							if (text4 == "OK")
							{
								base.uiWorld.mainCommand.OpenDiamonShop();
							}
							else if (!(key == "Cancel"))
							{
							}
						};
					}
					else
					{
						base.network.RequestChangeNickname(text3);
						recv.Close();
					}
				}
			};
		}
		else
		{
			if (!(key == "ChangeImage"))
			{
				return;
			}
			UIImageListPopup uIImageListPopup = UIPopup.Create<UIImageListPopup>("ImageListPopup");
			uIImageListPopup.Set(localization: true, "7012", null, null, null, "1000", null);
			uIImageListPopup.SetItemProfileImage();
			uIImageListPopup.onClick = delegate(GameObject popupSender)
			{
				string text = popupSender.name;
				if (text.StartsWith("thumbnail_"))
				{
					string idx = text.Substring(text.IndexOf("_") + 1);
					base.network.RequestChangeUserThumbnail(idx);
				}
			};
		}
	}

	public void Set(RoLocalUser user)
	{
		_currUser = user;
		Set((RoUser)user);
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UserLevelDataRow userLevelDataRow = regulation.GetUserLevelDataRow(user.level);
		if (userLevelDataRow != null)
		{
			UserLevelDataRow userLevelDataRow2 = regulation.GetUserLevelDataRow(user.level + 1);
			int num = userLevelDataRow.exp;
			int num2 = 0;
			num2 = userLevelDataRow2?.exp ?? num;
			int num3 = user.exp - userLevelDataRow.uExp;
			UISetter.SetProgress(expProgress, (!user.isMaxLevel) ? ((float)num3 / (float)num2) : 1f);
			UISetter.SetLabel(exp, Localization.Format("7010", (!user.isMaxLevel) ? num3.ToString() : "MAX"));
		}
		UISetter.SetLabel(vipLevel, user.vipLevel);
		if (GameSetting.instance.guildName)
		{
			if (string.IsNullOrEmpty(user.guildName))
			{
				UISetter.SetLabel(guildName, Localization.Get("7179"));
			}
			else
			{
				UISetter.SetLabel(guildName, Localization.Format("19067", user.guildInfo.world) + "\n" + user.guildName);
			}
		}
		else
		{
			UISetter.SetLabel(guildName, string.Empty);
		}
		CommanderCostumeDataRow commanderCostumeDataRow = regulation.FindCostumeData(int.Parse(base.localUser.thumbnailId));
		if (commanderCostumeDataRow != null)
		{
			RoCommander roCommander = base.localUser.FindCommander(commanderCostumeDataRow.cid.ToString());
			if (roCommander != null && roCommander.isBasicCostume)
			{
				UISetter.SetSprite(picture, roCommander.resourceId + "_" + roCommander.currentViewCostume);
			}
			else
			{
				UISetter.SetSprite(picture, regulation.GetCostumeThumbnailName(int.Parse(base.localUser.thumbnailId)));
			}
		}
	}

	public void Set(RoUser user)
	{
		if (user != null)
		{
			UISetter.SetLabel(nickname, user.nickname);
			UISetter.SetLabel(level, user.level);
			UISetter.SetLabel(uno, user.uno);
			UISetter.SetLabel(serverInfo, Localization.Get((27919 + user.channel).ToString()) + " - " + user.world);
		}
	}
}
