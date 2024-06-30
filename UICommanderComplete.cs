using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UICommanderComplete : UIPopup
{
	public GameObject promotion;

	public GameObject transmission;

	public GameObject recruit;

	public GameObject commanderUnitInfo;

	public GameObject classUp;

	public GameObject commanderNameRoot;

	public GameObject NextRoot;

	public GameObject MaxRoot;

	public UICommander uiCommander;

	public UICommander prevCommander;

	public UICommander nowCommander;

	public UISprite unitThumbnail1;

	public Animation openAnimation;

	private readonly string openAnimationName = "GachaBoxCommander-Popup_Open";

	private readonly string loopAnimationName = "GachaBoxCommander-Popup_Looping";

	private readonly string boardAnimationName = "GachaBoxCommander-Popup_Boarding";

	private readonly string rankUpAnimationName = "RankUp_Commander";

	private readonly string rankUpCloseAnimationName = "RankUp_Commander_Close";

	private readonly string starAnimation = "RankUpStar";

	public UIButton closeButton;

	public UILabel closeLabel;

	public UILabel transDescription;

	public GameObject newIcon;

	public List<GameObject> starEffectList;

	public List<Animation> starAnimationList;

	public UICommanderSkill commanderSkill1;

	public UICommanderSkill commanderSkill2;

	public UICommanderSkill OpenCommanderSkill;

	public UIUnit uiUnit;

	public UIStatus prevStatus;

	public UIStatus nowStatus;

	public UILabel increaseHealth;

	public UILabel increaseAttack;

	public UILabel increaseDefence;

	public UILabel increaseLuck;

	public UILabel increaseAccur;

	public GameObject unitEffect;

	public ForwardBackKeyEventIgnore backIgnorEvent;

	private CommanderCompleteType type;

	private RoCommander commander;

	private UIPanel aniPanel;

	[SerializeField]
	private UIKissAnimation new_kissAnim;

	public IEnumerator Init(CommanderCompleteType _type, RoCommander _commander)
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: false);
		SetAutoDestroy(autoDestory: true);
		type = _type;
		commander = _commander;
		aniPanel = openAnimation.gameObject.GetComponent<UIPanel>();
		aniPanel.alpha = 0f;
		UISetter.SetActive(promotion, type == CommanderCompleteType.Promotion);
		UISetter.SetActive(transmission, type == CommanderCompleteType.Transmission);
		UISetter.SetActive(recruit, type == CommanderCompleteType.Recruit || type == CommanderCompleteType.WorldMapReward);
		UISetter.SetActive(commanderUnitInfo, type != CommanderCompleteType.Promotion && type != CommanderCompleteType.ClassUp);
		UISetter.SetActive(newIcon, type == CommanderCompleteType.Recruit);
		UISetter.SetActive(classUp, type == CommanderCompleteType.ClassUp);
		UISetter.SetActive(uiUnit, type != CommanderCompleteType.Promotion);
		UISetter.SetActive(commanderNameRoot, type != CommanderCompleteType.Promotion);
		yield return StartCoroutine(SetCommander(uiCommander, commander));
		UISetter.SetSprite(unitThumbnail1, string.Format(commander.unitReg.resourceName + UIUnit.thumbnailFrontIdPostfix));
		RoUnit unit = RoUnit.Create(commander.unitId, commander.level, commander.rank, commander.cls, commander.currentCostume, commander.id, commander.favorRewardStep, commander.marry, commander.transcendence);
		uiUnit.Set(unit);
		setActiveStarEffect(state: false);
		if (type == CommanderCompleteType.Promotion)
		{
			UISetter.SetLabel(title, Localization.Get("10049"));
			UISetter.SetLabel(closeLabel, Localization.Get("1001"));
			RoCommander beforeCommander = commander.CreateBeforeRank();
			if (beforeCommander != null)
			{
				yield return StartCoroutine(SetCommander(prevCommander, beforeCommander));
			}
			yield return StartCoroutine(SetCommander(nowCommander, commander));
			aniPanel.alpha = 1f;
			StartCoroutine(PlayRankUpAnimation());
			StartCoroutine(PlayStarAnimation());
		}
		else if (type == CommanderCompleteType.Recruit)
		{
			UISetter.SetLabel(title, Localization.Get("10052"));
			UISetter.SetLabel(closeLabel, Localization.Get("1308"));
			SkillDataRow skillDataRow = base.regulation.skillDtbl[commander.currLevelUnitReg.skillDrks[0]];
			SkillDataRow skillDataRow2 = base.regulation.skillDtbl[commander.currLevelUnitReg.skillDrks[1]];
			UISetter.SetGameObjectName(commanderSkill1.gameObject, $"Skill-{skillDataRow.key}");
			UISetter.SetGameObjectName(commanderSkill2.gameObject, $"Skill-{skillDataRow2.key}");
			commanderSkill1.Set(skillDataRow);
			commanderSkill2.Set(skillDataRow2);
			StartCoroutine(PlayOpenAnimation());
		}
		else if (type == CommanderCompleteType.Transmission)
		{
			UISetter.SetLabel(title, Localization.Get("10054"));
			UISetter.SetLabel(closeLabel, Localization.Get("1001"));
			UISetter.SetLabel(transDescription, Localization.Format("10055", commander.reg.overlapReward));
			StartCoroutine(PlayOpenAnimation());
		}
		else if (type == CommanderCompleteType.WorldMapReward || type == CommanderCompleteType.KissReplay)
		{
			UISetter.SetLabel(title, Localization.Get("10052"));
			UISetter.SetLabel(closeLabel, Localization.Get("1308"));
			SkillDataRow skillDataRow3 = base.regulation.skillDtbl[commander.currLevelUnitReg.skillDrks[0]];
			SkillDataRow skillDataRow4 = base.regulation.skillDtbl[commander.currLevelUnitReg.skillDrks[1]];
			UISetter.SetGameObjectName(commanderSkill1.gameObject, $"Skill-{skillDataRow3.key}");
			UISetter.SetGameObjectName(commanderSkill2.gameObject, $"Skill-{skillDataRow4.key}");
			commanderSkill1.Set(skillDataRow3);
			commanderSkill2.Set(skillDataRow4);
			UISetter.SetActive(openAnimation.gameObject, active: false);
			UISetter.SetActive(new_kissAnim.gameObject, active: true);
			new_kissAnim.SetKissAnim(commander);
		}
		else if (type == CommanderCompleteType.ClassUp)
		{
			UISetter.SetLabel(title, Localization.Get("8009"));
			UISetter.SetLabel(closeLabel, Localization.Get("1001"));
			UnitDataRow prevClsReg = unit.prevClsReg;
			UISetter.SetLabel(increaseHealth, "+" + (unit.currClsReg.maxHealth - prevClsReg.maxHealth));
			UISetter.SetLabel(increaseAttack, "+" + (unit.currClsReg.attackDamage - prevClsReg.attackDamage));
			UISetter.SetLabel(increaseDefence, "+" + (unit.currClsReg.defense - prevClsReg.defense));
			UISetter.SetLabel(increaseLuck, "+" + (unit.currClsReg.luck - prevClsReg.luck));
			UISetter.SetLabel(increaseAccur, "+" + (unit.currClsReg.accuracy - prevClsReg.accuracy));
			prevStatus.Set(prevClsReg);
			nowStatus.Set(unit.currClsReg);
			SkillDataRow openCommnaderSkill = GetOpenCommnaderSkill(unit.currClsReg.skillDrks, unit.cls);
			UISetter.SetActive(OpenCommanderSkill, openCommnaderSkill != null);
			if (openCommnaderSkill != null)
			{
				OpenCommanderSkill.Set(openCommnaderSkill);
				UISetter.SetGameObjectName(OpenCommanderSkill.gameObject, $"Skill-{openCommnaderSkill.key}");
			}
			UISetter.SetActive(unitEffect, active: true);
			StartCoroutine(PlayOpenAnimation());
		}
		Open();
	}

	private SkillDataRow GetOpenCommnaderSkill(IList<string> skillList, int c_class)
	{
		for (int i = 0; i < skillList.Count; i++)
		{
			SkillDataRow skillDataRow = base.regulation.skillDtbl[skillList[i]];
			if (skillDataRow.openGrade == c_class)
			{
				return skillDataRow;
			}
		}
		return null;
	}

	private IEnumerator SetCommander(UICommander commander, RoCommander commanderData)
	{
		yield return StartCoroutine(commander.SetCommander(commanderData));
		if (commander.spine != null && commander.spine.target != null)
		{
			commander.spine.target.GetComponent<UIInteraction>().idleVoice = false;
		}
		yield return null;
	}

	public void Init(CommanderCompleteType type, string id)
	{
		RoCommander roCommander = null;
		if (type == CommanderCompleteType.Promotion || type == CommanderCompleteType.ClassUp)
		{
			roCommander = base.localUser.FindCommander(id);
		}
		else
		{
			CommanderDataRow commanderDataRow = base.regulation.commanderDtbl[id];
			roCommander = RoCommander.Create(id, 1, commanderDataRow.grade, 1, 0, 0, 0, new List<int>());
			roCommander.SetBaseCostume();
		}
		StartCoroutine(Init(type, roCommander));
	}

	public IEnumerator PlayOpenAnimation()
	{
		SoundManager.PlaySFX("SE_PilotGet_001");
		UISetter.SetActive(openAnimation.gameObject, active: true);
		aniPanel.alpha = 1f;
		UISetter.SetActive(closeButton, active: false);
		openAnimation.Play(openAnimationName);
		while (openAnimation.IsPlaying(openAnimationName))
		{
			yield return null;
		}
		backIgnorEvent.enabled = false;
		UISetter.SetActive(closeButton, active: true);
		openAnimation.Play(loopAnimationName);
	}

	public IEnumerator PlayBoardAnimation()
	{
		SoundManager.PlaySFX("SE_PilotGetOn_001");
		backIgnorEvent.enabled = true;
		openAnimation.Play(boardAnimationName);
		UISetter.SetActive(newIcon, active: false);
		while (openAnimation.IsPlaying(boardAnimationName))
		{
			yield return null;
		}
		Close();
	}

	private IEnumerator PlayRankUpAnimation()
	{
		UISetter.SetActive(closeButton, active: true);
		openAnimation.Play(rankUpAnimationName);
		while (openAnimation.IsPlaying(rankUpAnimationName))
		{
			yield return null;
		}
	}

	private IEnumerator PlayRankUpCloseAnimation()
	{
		openAnimation.Play(rankUpCloseAnimationName);
		while (openAnimation.IsPlaying(rankUpCloseAnimationName))
		{
			yield return null;
		}
		Close();
	}

	public void TestCode()
	{
		RoCommander roCommander = base.localUser.FindCommander("3");
		StartCoroutine(Init(CommanderCompleteType.Recruit, roCommander));
	}

	private IEnumerator PlayStarAnimation()
	{
		UISetter.SetActive(starEffectList[(int)commander.rank - 1], active: false);
		yield return new WaitForSeconds(0.5f);
		backIgnorEvent.enabled = false;
		UISetter.SetActive(starEffectList[(int)commander.rank - 1], active: true);
		starAnimationList[(int)commander.rank - 1].Play(starAnimation);
	}

	private void setActiveStarEffect(bool state)
	{
		for (int i = 0; i < starEffectList.Count; i++)
		{
			UISetter.SetActive(starEffectList[i], state);
		}
	}

	private void SetSkillInfo(string id)
	{
		UISkillInfoPopup uISkillInfoPopup = UIPopup.Create<UISkillInfoPopup>("SkillInfoPopup");
		uISkillInfoPopup.Set(localization: true, "10056", null, null, null, Localization.Get("10048"), null);
		uISkillInfoPopup.SetInfo(commander, id);
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text == "Close")
		{
			if (type == CommanderCompleteType.Recruit || type == CommanderCompleteType.WorldMapReward)
			{
				StartCoroutine(PlayBoardAnimation());
			}
			else if (type == CommanderCompleteType.Promotion)
			{
				StartCoroutine(PlayRankUpCloseAnimation());
			}
			else
			{
				Close();
			}
		}
		else if (text.StartsWith("Skill-"))
		{
			string skillInfo = text.Substring(text.IndexOf("-") + 1);
			SetSkillInfo(skillInfo);
		}
	}

	public void OpenAnimation()
	{
		iTween.MoveFrom(base.gameObject, iTween.Hash("y", -1000, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeOutBack));
	}

	public void CloseAnimation()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("y", -1000, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeInBack, "oncomplete", "Close", "oncompletetarget", base.gameObject));
	}

	public override void Open()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: true);
		base.Open();
	}

	public override void Close()
	{
		base.Close();
	}

	public void StartOpenAnimation()
	{
		if (type == CommanderCompleteType.WorldMapReward)
		{
			StartCoroutine(PlayOpenAnimation());
		}
		else if (type == CommanderCompleteType.KissReplay)
		{
			Close();
		}
	}
}
