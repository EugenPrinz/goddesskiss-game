using System.Collections;
using UnityEngine;

public class UIGameSetting : UIPanelBase
{
	public UISprite bgm;

	public UISprite bgmOn;

	public UISprite bgmOff;

	public UISprite se;

	public UISprite seOn;

	public UISprite seOff;

	public UISprite voice;

	public UISprite voiceOn;

	public UISprite voiceOff;

	public UISprite skill;

	public UILabel skillOn;

	public UILabel skillOff;

	public UISprite push;

	public UILabel pushOn;

	public UILabel pushOff;

	public UISprite push_Bullet;

	public UILabel push_Bullet_On;

	public UILabel push_Bullet_Off;

	public UISprite push_SkillPoint;

	public UILabel push_SkillPoint_On;

	public UILabel push_SkillPoint_Off;

	public UISprite push_Premium;

	public UILabel push_Premium_On;

	public UILabel push_Premium_Off;

	public UISprite push_VipShop;

	public UILabel push_VipShop_On;

	public UILabel push_VipShop_Off;

	public UISprite guildName;

	public UILabel guildNameOn;

	public UILabel guildNameOff;

	public UISprite chat_ReplayBattle;

	public UILabel chat_ReplayBattle_On;

	public UILabel chat_ReplayBattle_Off;

	public UILabel version;

	public UIPopupList popupList;

	private GameSetting _currentSetting;

	public UIButton Btn_Facebook;

	public UIButton Btn_Google;

	public UIButton Btn_Voice;

	public UIButton Btn_DownLoad;

	public GameObject patch;

	public UILabel progressLabel;

	public UIProgressBar progressBar;

	public UISelectLanguagePopup selLangPopup;

	public UISlider bgmSlider;

	public UISlider voiceSlider;

	public UILabel bgmValue;

	public UILabel voiceValue;

	private int bgmVolume;

	private int voiceVolume;

	public BoxCollider bgmVolume_coll;

	public BoxCollider voiceVolume_coll;

	public void Set(GameSetting setting)
	{
		_currentSetting = setting;
		SetOption(setting);
		if (popupList != null)
		{
			popupList.items = setting.GetSupportLanguageList();
			popupList.value = setting.languageLocalizedString;
		}
		UISetter.SetActive(Btn_Voice, active: true);
		UISetter.SetActive(Btn_DownLoad, active: false);
		selLangPopup.SetSelLanguage(Localization.language);
	}

	public void SetOption(GameSetting setting)
	{
		if (setting.bgm)
		{
			bgmOn.gameObject.SetActive(value: true);
			bgmOff.gameObject.SetActive(value: false);
			bgmVolume_coll.enabled = true;
			if (!PlayerPrefs.HasKey("bgm_Volume"))
			{
				bgmSlider.value = 1f;
			}
			else
			{
				bgmSlider.value = PlayerPrefs.GetFloat("bgm_Volume");
			}
		}
		else
		{
			bgmOff.gameObject.SetActive(value: true);
			bgmOn.gameObject.SetActive(value: false);
			bgmVolume_coll.enabled = false;
			bgmSlider.value = 0f;
		}
		if (setting.voice)
		{
			voiceOn.gameObject.SetActive(value: true);
			voiceOff.gameObject.SetActive(value: false);
			voiceVolume_coll.enabled = true;
			if (!PlayerPrefs.HasKey("voice_Volume"))
			{
				voiceSlider.value = 1f;
			}
			else
			{
				voiceSlider.value = PlayerPrefs.GetFloat("voice_Volume");
			}
		}
		else
		{
			voiceOff.gameObject.SetActive(value: true);
			voiceOn.gameObject.SetActive(value: false);
			voiceVolume_coll.enabled = false;
			voiceSlider.value = 0f;
		}
		if (setting.effect)
		{
			if (skill != null)
			{
				skill.spriteName = "com_bt_circle_on";
				skillOn.gameObject.SetActive(value: true);
				skillOff.gameObject.SetActive(value: false);
			}
		}
		else if (skill != null)
		{
			skill.spriteName = "com_bt_circle_off";
			skillOff.gameObject.SetActive(value: true);
			skillOn.gameObject.SetActive(value: false);
		}
		if (setting.Notification)
		{
			if (push != null)
			{
				push.spriteName = "com_bt_circle_on";
				pushOn.gameObject.SetActive(value: true);
				pushOff.gameObject.SetActive(value: false);
			}
		}
		else if (push != null)
		{
			push.spriteName = "com_bt_circle_off";
			pushOff.gameObject.SetActive(value: true);
			pushOn.gameObject.SetActive(value: false);
		}
		if (setting.guildName)
		{
			if (guildName != null)
			{
				guildName.spriteName = "com_bt_circle_on";
				guildNameOn.gameObject.SetActive(value: true);
				guildNameOff.gameObject.SetActive(value: false);
			}
		}
		else if (guildName != null)
		{
			guildName.spriteName = "com_bt_circle_off";
			guildNameOff.gameObject.SetActive(value: true);
			guildNameOn.gameObject.SetActive(value: false);
		}
		if (setting.PushBullet)
		{
			if (push_Bullet != null)
			{
				push_Bullet.spriteName = "com_bt_circle_on";
				push_Bullet_On.gameObject.SetActive(value: true);
				push_Bullet_Off.gameObject.SetActive(value: false);
			}
		}
		else if (push_Bullet != null)
		{
			push_Bullet.spriteName = "com_bt_circle_off";
			push_Bullet_Off.gameObject.SetActive(value: true);
			push_Bullet_On.gameObject.SetActive(value: false);
		}
		if (setting.PushSkillPoint)
		{
			if (push_SkillPoint != null)
			{
				push_SkillPoint.spriteName = "com_bt_circle_on";
				push_SkillPoint_On.gameObject.SetActive(value: true);
				push_SkillPoint_Off.gameObject.SetActive(value: false);
			}
		}
		else if (push_SkillPoint != null)
		{
			push_SkillPoint.spriteName = "com_bt_circle_off";
			push_SkillPoint_Off.gameObject.SetActive(value: true);
			push_SkillPoint_On.gameObject.SetActive(value: false);
		}
		if (setting.PushPremium)
		{
			if (push_Premium != null)
			{
				push_Premium.spriteName = "com_bt_circle_on";
				push_Premium_On.gameObject.SetActive(value: true);
				push_Premium_Off.gameObject.SetActive(value: false);
			}
		}
		else if (push_Premium != null)
		{
			push_Premium.spriteName = "com_bt_circle_off";
			push_Premium_Off.gameObject.SetActive(value: true);
			push_Premium_On.gameObject.SetActive(value: false);
		}
		if (setting.PushVipShop)
		{
			if (push_VipShop != null)
			{
				push_VipShop.spriteName = "com_bt_circle_on";
				push_VipShop_On.gameObject.SetActive(value: true);
				push_VipShop_Off.gameObject.SetActive(value: false);
			}
		}
		else if (push_VipShop != null)
		{
			push_VipShop.spriteName = "com_bt_circle_off";
			push_VipShop_Off.gameObject.SetActive(value: true);
			push_VipShop_On.gameObject.SetActive(value: false);
		}
		if (setting.chatReplayBattle)
		{
			if (chat_ReplayBattle != null)
			{
				chat_ReplayBattle.spriteName = "com_bt_circle_on";
				chat_ReplayBattle_On.gameObject.SetActive(value: true);
				chat_ReplayBattle_Off.gameObject.SetActive(value: false);
			}
		}
		else if (chat_ReplayBattle != null)
		{
			chat_ReplayBattle.spriteName = "com_bt_circle_off";
			chat_ReplayBattle_On.gameObject.SetActive(value: false);
			chat_ReplayBattle_Off.gameObject.SetActive(value: true);
		}
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (_currentSetting == null)
		{
			return;
		}
		if (text != "Language")
		{
			SoundManager.PlaySFX("BTN_Norma_001");
		}
		switch (text)
		{
		case "BGM-On":
			_currentSetting.bgm = true;
			break;
		case "BGM-Off":
			_currentSetting.bgm = false;
			break;
		case "Notification-On":
			_currentSetting.Notification = true;
			break;
		case "Notification-Off":
			_currentSetting.Notification = false;
			break;
		case "SE-On":
			_currentSetting.se = true;
			break;
		case "SE-Off":
			_currentSetting.se = false;
			break;
		case "Btn_BGM":
			_currentSetting.bgm = !_currentSetting.bgm;
			SetOption(_currentSetting);
			break;
		case "Btn_SE":
			_currentSetting.se = !_currentSetting.se;
			SetOption(_currentSetting);
			break;
		case "Btn_Voice":
			_currentSetting.voice = !_currentSetting.voice;
			_currentSetting.se = _currentSetting.voice;
			SetOption(_currentSetting);
			break;
		case "Btn_DownLoad":
			if (PlayerPrefs.GetInt("VoiceDownState") == 2)
			{
				StartCoroutine(StartPatch());
			}
			break;
		case "Btn_Skill":
			_currentSetting.effect = !_currentSetting.effect;
			SetOption(_currentSetting);
			break;
		case "Btn_Push":
			_currentSetting.Notification = !_currentSetting.Notification;
			SetOption(_currentSetting);
			if (_currentSetting.Notification)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7195"));
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7196"));
			}
			break;
		case "Btn_PushBullet":
			_currentSetting.PushBullet = !_currentSetting.PushBullet;
			SetOption(_currentSetting);
			if (_currentSetting.PushBullet)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7194"));
			}
			else
			{
				base.network.CancelLocalPush(ELocalPushType.BulletFullCharge);
			}
			break;
		case "Btn_PushSkillPoint":
			_currentSetting.PushSkillPoint = !_currentSetting.PushSkillPoint;
			SetOption(_currentSetting);
			if (_currentSetting.PushSkillPoint)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7194"));
			}
			else
			{
				base.network.CancelLocalPush(ELocalPushType.SkillPointFullCharge);
			}
			break;
		case "Btn_PushPremium":
			_currentSetting.PushPremium = !_currentSetting.PushPremium;
			SetOption(_currentSetting);
			if (_currentSetting.PushPremium)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7194"));
			}
			else
			{
				base.network.CancelLocalPush(ELocalPushType.PremiumGachaFree);
			}
			break;
		case "Btn_PushVipShop":
			_currentSetting.PushVipShop = !_currentSetting.PushVipShop;
			SetOption(_currentSetting);
			if (_currentSetting.PushVipShop)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("7194"));
			}
			else
			{
				base.network.CancelLocalPush(ELocalPushType.LeaveVipShop);
			}
			break;
		case "Language":
			break;
		case "Korea":
			Localization.language = "S_Kr";
			break;
		case "English":
			Localization.language = "S_En";
			break;
		case "China":
			Localization.language = "S_Beon";
			break;
		case "SelectedLanguage":
			if (!selLangPopup.isActive)
			{
				selLangPopup.Open();
			}
			break;
		case "Btn_GuildName":
			_currentSetting.guildName = !_currentSetting.guildName;
			SetOption(_currentSetting);
			if (UIManager.instance.world != null && UIManager.instance.world.mainCommand != null)
			{
				UIManager.instance.world.mainCommand.ResetGuildName();
			}
			break;
		case "Btn_ChatReplayBattle":
			_currentSetting.chatReplayBattle = !_currentSetting.chatReplayBattle;
			SetOption(_currentSetting);
			break;
		case "bgm-plus":
			if (_currentSetting.bgm)
			{
				bgmSlider.value += 0.01f;
				bgmVolume = (int)Mathf.Round(bgmSlider.value * 100f);
				UISetter.SetLabel(bgmValue, bgmVolume);
				bgmSlider.value = (float)bgmVolume / 100f;
			}
			break;
		case "bgm-minus":
			if (_currentSetting.bgm)
			{
				bgmSlider.value -= 0.01f;
				bgmVolume = (int)Mathf.Round(bgmSlider.value * 100f);
				UISetter.SetLabel(bgmValue, bgmVolume);
				bgmSlider.value = (float)bgmVolume / 100f;
				if (bgmSlider.value <= 0f)
				{
					_currentSetting.bgm = false;
					SetOption(_currentSetting);
				}
			}
			break;
		case "voice-plus":
			if (_currentSetting.voice)
			{
				voiceSlider.value += 0.01f;
				voiceVolume = (int)Mathf.Round(voiceSlider.value * 100f);
				UISetter.SetLabel(voiceValue, voiceVolume);
				voiceSlider.value = (float)voiceVolume / 100f;
			}
			break;
		case "voice-minus":
			if (_currentSetting.voice)
			{
				voiceSlider.value -= 0.01f;
				voiceVolume = (int)Mathf.Round(voiceSlider.value * 100f);
				UISetter.SetLabel(voiceValue, voiceVolume);
				voiceSlider.value = (float)voiceVolume / 100f;
				if (voiceSlider.value <= 0f)
				{
					_currentSetting.voice = false;
					_currentSetting.se = false;
					SetOption(_currentSetting);
				}
			}
			else
			{
				base.OnClick(sender);
			}
			break;
		}
	}

	private IEnumerator StartPatch()
	{
		patch.SetActive(value: true);
		yield return StartCoroutine(PatchManager.Instance.RunPatch(progressLabel, progressBar));
		patch.SetActive(value: false);
		UISetter.SetActive(Btn_Voice, PlayerPrefs.GetInt("VoiceDownState") != 2);
		UISetter.SetActive(Btn_DownLoad, PlayerPrefs.GetInt("VoiceDownState") == 2);
	}

	public void BgmSliderValueChange()
	{
		bgmVolume = (int)Mathf.Round(bgmSlider.value * 100f);
		bgmSlider.value = (float)bgmVolume / 100f;
		UISetter.SetLabel(bgmValue, bgmVolume);
		if (_currentSetting.bgm)
		{
			PlayerPrefs.SetFloat("bgm_Volume", bgmSlider.value);
			PlayerPrefs.Save();
		}
		SoundPlayer.SetBGMVolume(bgmSlider.value);
		SoundManager.SetVolumeMusic(bgmSlider.value);
	}

	public void VoiceSliderValueChange()
	{
		voiceVolume = (int)Mathf.Round(voiceSlider.value * 100f);
		voiceSlider.value = (float)voiceVolume / 100f;
		UISetter.SetLabel(voiceValue, voiceVolume);
		if (_currentSetting.voice)
		{
			PlayerPrefs.SetFloat("voice_Volume", voiceSlider.value);
			PlayerPrefs.Save();
		}
		SoundPlayer.SetSEVolume(voiceSlider.value);
		SoundManager.SetVolumeSFX(voiceSlider.value);
	}
}
