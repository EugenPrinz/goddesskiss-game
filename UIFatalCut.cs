using Shared.Battle;
using Shared.Regulation;
using UnityEngine;

public class UIFatalCut : MonoBehaviour
{
	public GameObject fatalCut;

	public Animation animation;

	public UISpineAnimation uiSpine;

	public UISpineAnimation uiDamegedSpine;

	public bool isNewFatalCut;

	[Range(0f, 100f)]
	public int showHealthRate;

	private const string _fatlAnimationName = "a_05_lose";

	private void OnEnable()
	{
		UISetter.SetActive(fatalCut, active: false);
	}

	public void Show(Unit unit)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		CommanderDataRow commanderDataRow = regulation.commanderDtbl[unit._cdri];
		unit._bFatal = true;
		string skinName = "1";
		if (unit._ctdri >= 0)
		{
			RoCommander roCommander = RemoteObjectManager.instance.localUser.FindCommander(unit.cid);
			if (roCommander != null && roCommander.isBasicCostume)
			{
				skinName = roCommander.currentViewCostume;
			}
			else
			{
				CommanderCostumeDataRow commanderCostumeDataRow = regulation.commanderCostumeDtbl[unit._ctdri];
				skinName = commanderCostumeDataRow.skinName;
			}
		}
		if (Init(commanderDataRow.resourceId, skinName))
		{
			Show();
			SoundManager.PlayVoiceEvent(commanderDataRow, ECommanderVoiceEventType.Fatal, looping: false, 0f, float.MaxValue, 1f);
		}
	}

	public void Show()
	{
		UISetter.SetActive(fatalCut, active: true);
		animation.Play();
	}

	public bool Init(string commanderId, string skinName = null)
	{
		uiSpine.spinePrefabName = commanderId;
		if (uiSpine.skeletonAnimation == null)
		{
			return false;
		}
		if (!string.IsNullOrEmpty(skinName))
		{
			uiSpine.SetSkin(skinName);
		}
		UIInteraction component = uiSpine.skeletonAnimation.GetComponent<UIInteraction>();
		if (component != null)
		{
			component.EnableInteration = false;
			component.enabled = false;
		}
		if (isNewFatalCut)
		{
			uiSpine.skeletonAnimation.state.SetAnimation(0, "a_06_damage", loop: false);
			animation.Rewind();
			return true;
		}
		uiDamegedSpine.spinePrefabName = commanderId;
		uiDamegedSpine.SetSkin(RemoteObjectManager.instance.localUser.FindCommander(commanderId).getCurrentCostumeName());
		UIInteraction component2 = uiDamegedSpine.skeletonAnimation.GetComponent<UIInteraction>();
		if (component2 != null)
		{
			component2.EnableInteration = false;
			component2.enabled = false;
		}
		uiDamegedSpine.skeletonAnimation.state.SetAnimation(0, "a_05_lose", loop: false);
		animation.Rewind();
		return true;
	}

	public bool CanFatalCut(Unit unit)
	{
		if (unit == null)
		{
			return false;
		}
		if (!GameSetting.instance.effect)
		{
			return false;
		}
		if (!unit._readyFatal)
		{
			return false;
		}
		if (unit._bFatal)
		{
			return false;
		}
		if (unit._cdri < 0)
		{
			return false;
		}
		if (unit.side == EBattleSide.Right)
		{
			return false;
		}
		if (unit.isDead)
		{
			return false;
		}
		if (fatalCut == null || fatalCut.activeSelf)
		{
			return false;
		}
		float num = (float)unit.health / (float)unit.maxHealth;
		if (num <= (float)showHealthRate * 0.01f)
		{
			return true;
		}
		return false;
	}
}
