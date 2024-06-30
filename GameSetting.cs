using System.Collections.Generic;
using UnityEngine;

public class GameSetting
{
	private static GameSetting _singltone;

	private bool _bgm = true;

	private bool _se = true;

	private bool _voice = true;

	private bool _effect = true;

	private bool _autoSkill;

	private bool _speedUp;

	private bool _guildName = true;

	private bool _repeatBattle;

	private bool _notification = true;

	private string _language = "S_Kr";

	private bool _pushBullet;

	private bool _pushSkillPoint;

	private bool _pushPremium;

	private bool _pushVipShop;

	private bool _chatReplayBattle = true;

	public static GameSetting instance
	{
		get
		{
			if (_singltone == null)
			{
				_singltone = _CreateFromLocal();
			}
			return _singltone;
		}
	}

	public bool bgm
	{
		get
		{
			return _bgm;
		}
		set
		{
			if (_bgm != value)
			{
				float num = 1f;
				if (PlayerPrefs.HasKey("bgm_Volume"))
				{
					num = PlayerPrefs.GetFloat("bgm_Volume");
				}
				PlayerPrefs.SetString("Setting-BGM", value.ToString());
				SoundPlayer.SetBGMVolume((!value) ? 0f : num);
				SoundManager.SetVolumeMusic((!value) ? 0f : num);
			}
			_bgm = value;
		}
	}

	public bool se
	{
		get
		{
			return _se;
		}
		set
		{
			if (_se != value)
			{
				PlayerPrefs.SetString("Setting-SE", value.ToString());
			}
			_se = value;
		}
	}

	public bool voice
	{
		get
		{
			return _voice;
		}
		set
		{
			if (_voice != value)
			{
				PlayerPrefs.SetString("Setting-VOICE", value.ToString());
			}
			_voice = value;
		}
	}

	public bool guildName
	{
		get
		{
			return _guildName;
		}
		set
		{
			if (_guildName != value)
			{
				PlayerPrefs.SetString("Setting-GuildName", value.ToString());
			}
			_guildName = value;
		}
	}

	public bool chatReplayBattle
	{
		get
		{
			return _chatReplayBattle;
		}
		set
		{
			if (_chatReplayBattle != value)
			{
				PlayerPrefs.SetString("Setting-ChatReplayBattle", value.ToString());
			}
			_chatReplayBattle = value;
		}
	}

	public bool Notification
	{
		get
		{
			return _notification;
		}
		set
		{
			if (_notification != value)
			{
				PlayerPrefs.SetString("Setting-Notification", value.ToString());
			}
			_notification = value;
		}
	}

	public bool PushBullet
	{
		get
		{
			return _pushBullet;
		}
		set
		{
			if (_pushBullet != value)
			{
				PlayerPrefs.SetString("Setting-PushBullet", value.ToString());
			}
			_pushBullet = value;
		}
	}

	public bool PushSkillPoint
	{
		get
		{
			return _pushSkillPoint;
		}
		set
		{
			if (_pushSkillPoint != value)
			{
				PlayerPrefs.SetString("Setting-PushSkillPoint", value.ToString());
			}
			_pushSkillPoint = value;
		}
	}

	public bool PushPremium
	{
		get
		{
			return _pushPremium;
		}
		set
		{
			if (_pushPremium != value)
			{
				PlayerPrefs.SetString("Setting-PushPremium", value.ToString());
			}
			_pushPremium = value;
		}
	}

	public bool PushVipShop
	{
		get
		{
			return _pushVipShop;
		}
		set
		{
			if (_pushVipShop != value)
			{
				PlayerPrefs.SetString("Setting-PushVipShop", value.ToString());
			}
			_pushVipShop = value;
		}
	}

	public string language
	{
		get
		{
			return _language;
		}
		set
		{
			_language = value;
			if (Localization.language != value)
			{
				Localization.language = value;
				PlayerPrefs.GetString("Language", _language);
				Toast.Show("언어가 " + languageLocalizedString + "로 바뀌었습니다.(" + _language + ")");
			}
		}
	}

	public string languageLocalizedString
	{
		get
		{
			return language;
		}
		set
		{
			language = value;
		}
	}

	public bool effect
	{
		get
		{
			return _effect;
		}
		set
		{
			if (_effect != value)
			{
				PlayerPrefs.SetString("Setting-Effect", value.ToString());
			}
			_effect = value;
		}
	}

	public bool autoSkill
	{
		get
		{
			return _autoSkill;
		}
		set
		{
			if (_autoSkill != value)
			{
				PlayerPrefs.SetString("Setting-AutoSkill", value.ToString());
			}
			_autoSkill = value;
		}
	}

	public bool speedUp
	{
		get
		{
			return _speedUp;
		}
		set
		{
			if (_speedUp != value)
			{
				PlayerPrefs.SetString("Setting-SpeedUp3", value.ToString());
			}
			_speedUp = value;
		}
	}

	public bool repeatBattle
	{
		get
		{
			return _repeatBattle;
		}
		set
		{
			_repeatBattle = value;
		}
	}

	public List<string> GetSupportLanguageList()
	{
		return null;
	}

	private static GameSetting _CreateFromLocal()
	{
		GameSetting gameSetting = new GameSetting();
		gameSetting.se = bool.Parse(PlayerPrefs.GetString("Setting-SE", bool.TrueString));
		gameSetting.bgm = bool.Parse(PlayerPrefs.GetString("Setting-BGM", bool.TrueString));
		gameSetting.voice = bool.Parse(PlayerPrefs.GetString("Setting-VOICE", bool.TrueString));
		gameSetting.language = PlayerPrefs.GetString("Language", "S_Kr");
		gameSetting.effect = bool.Parse(PlayerPrefs.GetString("Setting-Effect", bool.TrueString));
		gameSetting.autoSkill = bool.Parse(PlayerPrefs.GetString("Setting-AutoSkill", bool.FalseString));
		gameSetting.speedUp = bool.Parse(PlayerPrefs.GetString("Setting-SpeedUp3", bool.FalseString));
		gameSetting.Notification = bool.Parse(PlayerPrefs.GetString("Setting-Notification", bool.TrueString));
		gameSetting.PushBullet = bool.Parse(PlayerPrefs.GetString("Setting-PushBullet", bool.FalseString));
		gameSetting.PushSkillPoint = bool.Parse(PlayerPrefs.GetString("Setting-PushSkillPoint", bool.FalseString));
		gameSetting.PushPremium = bool.Parse(PlayerPrefs.GetString("Setting-PushPremium", bool.FalseString));
		gameSetting.PushVipShop = bool.Parse(PlayerPrefs.GetString("Setting-PushVipShop", bool.FalseString));
		gameSetting.guildName = bool.Parse(PlayerPrefs.GetString("Setting-GuildName", bool.TrueString));
		gameSetting.chatReplayBattle = bool.Parse(PlayerPrefs.GetString("Setting-ChatReplayBattle", bool.TrueString));
		return gameSetting;
	}
}
