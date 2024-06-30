using System.Collections;
using UnityEngine;

public class UIAnnihilationStage : UIItemBase
{
	public UISprite icon;

	public UILabel stageName;

	public GameObject flagAnimation;

	public GameObject effectRoot;

	private AnnihilationStageType annihilationStageType;

	public void Set(EAnnihilationStageType type)
	{
		SetIcon(type);
	}

	public void SetIcon(EAnnihilationStageType type)
	{
		int num = int.Parse(base.name.Substring(base.name.IndexOf("-") + 1));
		if (num == 21)
		{
			annihilationStageType = AnnihilationStageType.Boss;
		}
		else if (num % 5 == 0)
		{
			annihilationStageType = AnnihilationStageType.Special;
		}
		else
		{
			annihilationStageType = AnnihilationStageType.Basic;
		}
		switch (type)
		{
		case EAnnihilationStageType.Play:
			if (annihilationStageType == AnnihilationStageType.Basic)
			{
				UISetter.SetSprite(icon, "ab-clear-icon02", snap: true);
			}
			else if (annihilationStageType == AnnihilationStageType.Special)
			{
				UISetter.SetSprite(icon, "ab-clear-icon05", snap: true);
			}
			else if (annihilationStageType == AnnihilationStageType.Boss)
			{
				UISetter.SetSprite(icon, "ab-clear-icon07", snap: true);
			}
			break;
		case EAnnihilationStageType.Clear:
			if (annihilationStageType == AnnihilationStageType.Basic)
			{
				UISetter.SetSprite(icon, "ab-clear-icon01", snap: true);
			}
			else if (annihilationStageType == AnnihilationStageType.Special)
			{
				UISetter.SetSprite(icon, "ab-clear-icon04", snap: true);
			}
			else if (annihilationStageType == AnnihilationStageType.Boss)
			{
				UISetter.SetSprite(icon, "ab-clear-icon08", snap: true);
			}
			UISetter.SetActive(effectRoot, active: false);
			break;
		default:
			if (annihilationStageType == AnnihilationStageType.Basic)
			{
				UISetter.SetSprite(icon, "ab-clear-icon02", snap: true);
			}
			else if (annihilationStageType == AnnihilationStageType.Special)
			{
				UISetter.SetSprite(icon, "ab-clear-icon05", snap: true);
			}
			else if (annihilationStageType == AnnihilationStageType.Boss)
			{
				UISetter.SetSprite(icon, "ab-clear-icon07", snap: true);
			}
			break;
		}
	}

	public void PlayFlagAnimation()
	{
		StartCoroutine("StartFlagAnimation");
	}

	private IEnumerator StartFlagAnimation()
	{
		UISetter.SetActive(flagAnimation, active: true);
		yield return new WaitForSeconds(1.5f);
		UISetter.SetActive(flagAnimation, active: false);
	}

	public void Reset()
	{
		if (annihilationStageType == AnnihilationStageType.Basic)
		{
			UISetter.SetSprite(icon, "ab-clear-icon02", snap: true);
		}
		else if (annihilationStageType == AnnihilationStageType.Special)
		{
			UISetter.SetSprite(icon, "ab-clear-icon05", snap: true);
		}
		else if (annihilationStageType == AnnihilationStageType.Boss)
		{
			UISetter.SetSprite(icon, "ab-clear-icon07", snap: true);
		}
		UISetter.SetActive(flagAnimation, active: false);
		UISetter.SetActive(effectRoot, active: true);
	}

	private void OnClick()
	{
		CreateRewardPopUp();
	}

	public void CreateRewardPopUp()
	{
		int num = int.Parse(base.gameObject.name.Substring(base.gameObject.name.IndexOf("-") + 1));
		AnnihilationMode annihilationMode = AnnihilationMode.NONE;
		if (UIManager.instance.world.existAnnihilationMap && UIManager.instance.world.annihilationMap.isActive)
		{
			annihilationMode = UIManager.instance.world.annihilationMap.GetCurSelectMode();
		}
		switch (annihilationMode)
		{
		case AnnihilationMode.HARD:
			num += 100;
			break;
		case AnnihilationMode.HELL:
			num += 200;
			break;
		}
		UIAnnihilationStageInformation uIAnnihilationStageInformation = UIPopup.Create<UIAnnihilationStageInformation>("AnnihilationStageInformation");
		uIAnnihilationStageInformation.Init(num);
	}

	private void OnDisable()
	{
		StopCoroutine("StartFlagAnimation");
	}
}
