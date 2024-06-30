using UnityEngine;

public class UIOptionController : Manager<UIOptionController>
{
	public delegate void OnClickDelegate(GameObject sender);

	public delegate void OnShow();

	public delegate void OnHide();

	public GameObject prefabOption;

	[HideInInspector]
	public bool opt_skill;

	public OnClickDelegate onClick;

	public OnShow onShow;

	public OnHide onHide;

	protected UIOptionSet option;

	protected bool bInit;

	[HideInInspector]
	public bool opt_sound
	{
		get
		{
			return GameSetting.instance.se;
		}
		set
		{
			GameSetting.instance.se = value;
			GameSetting.instance.voice = value;
		}
	}

	[HideInInspector]
	public bool opt_bgm
	{
		get
		{
			return GameSetting.instance.bgm;
		}
		set
		{
			GameSetting.instance.bgm = value;
		}
	}

	public bool isShow
	{
		get
		{
			if (option == null)
			{
				return false;
			}
			return option.gameObject.activeSelf;
		}
	}

	private void OnDisable()
	{
		bInit = false;
	}

	public void InitUI()
	{
		if (option != null)
		{
			option.gameObject.SetActive(value: false);
		}
		bInit = true;
	}

	public void Show()
	{
		if (!bInit || (UIManager.instance.state != UIManager.EState.Battle && UIManager.instance.state != UIManager.EState.Tutorial))
		{
			return;
		}
		if (option == null)
		{
			option = Object.Instantiate(prefabOption).GetComponent<UIOptionSet>();
			option.transform.parent = base.transform;
			option.transform.localPosition = Vector3.zero;
			option.transform.localEulerAngles = Vector3.zero;
			option.transform.localScale = Vector3.one;
			option.onClick = delegate(GameObject sender)
			{
				SoundManager.PlaySFX("BTN_Norma_001");
				string text = sender.name;
				if (text != null && text == "btn_play")
				{
					Hide();
				}
				else if (onClick != null)
				{
					onClick(sender);
				}
			};
		}
		else if (option.gameObject.activeSelf)
		{
			return;
		}
		if (UIManager.instance.battle.Simulator.initState.battleType == EBattleType.Duel || UIManager.instance.battle.Simulator.initState.battleType == EBattleType.WaveDuel || UIManager.instance.battle.Simulator.initState.battleType == EBattleType.WorldDuel)
		{
			if (!UIManager.instance.battle.Main.isReplayMode)
			{
				UISetter.SetActive(option.optionEnd, active: false);
			}
		}
		else if (RemoteObjectManager.instance.localUser != null)
		{
			if (RemoteObjectManager.instance.localUser.tutorialData.enable || UIManager.instance.state == UIManager.EState.Tutorial)
			{
				UISetter.SetActive(option.optionEnd, active: false);
			}
			else
			{
				UISetter.SetActive(option.optionEnd, active: true);
			}
		}
		else
		{
			UISetter.SetActive(option.optionEnd, active: true);
		}
		if (option.optionSound != null)
		{
			option.optionSound.Status = opt_sound;
		}
		if (option.optionBgm != null)
		{
			option.optionBgm.Status = opt_bgm;
		}
		if (option.optionSkill != null)
		{
			option.optionSkill.Status = opt_skill;
		}
		option.gameObject.SetActive(value: true);
		if (onShow != null)
		{
			onShow();
		}
	}

	public void Hide()
	{
		if (option != null)
		{
			option.gameObject.SetActive(value: false);
		}
		if (onHide != null)
		{
			onHide();
		}
	}
}
