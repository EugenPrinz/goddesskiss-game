using System.Collections;
using Shared.Regulation;
using UnityEngine;

public class UIBuilding : UIItemBase
{
	public UISprite image;

	public UISprite shadow;

	public UILabel time;

	public UILabel cost;

	public UILabel level;

	public UILabel beforeLevel;

	public UILabel dotLevel;

	public UILabel beforeDotLevel;

	public UILabel spaceLevel;

	public UILabel beforeSpaceLevel;

	public UILabel nickname;

	public UILabel notiLabel;

	public GameObject Lock;

	public GameObject Name;

	public UILabel Locklevel;

	public UITimer timer;

	public UISprite upgradeText;

	public UISprite upgradeEnable;

	public UISprite spriteLevel;

	public UISprite spriteNextLevel;

	public GameObject notiView;

	public GameObject selectedRoot;

	public UISprite badge;

	private const string upgradePrefix = "text_upgrading";

	private const string upgradeCompletePrefix = "text_upgradcomplete";

	private GameObject openEffect;

	public GameObject buildingObject;

	private GameObject upgradeCompleteEffect;

	public EBuildingState state { get; set; }

	private EBuildingState tempState { get; set; }

	public override void SetSelection(bool selected)
	{
		UISetter.SetActive(selectedRoot, selected);
	}

	public void Set(BuildingLevelDataRow row)
	{
		if (row == null)
		{
			SetBuildingLock(999);
			return;
		}
		UISetter.SetSprite(image, row.resourceId);
		UISetter.SetSprite(shadow, row.resourceId);
		UISetter.SetLabel(level, row.level);
		UISetter.SetLabel(beforeLevel, row.level - 1);
		UISetter.SetSprite(spriteLevel, "main_num_" + (row.level - 1));
		UISetter.SetSprite(spriteNextLevel, "main_num_" + row.level);
		UISetter.SetLabel(spaceLevel, Localization.Format("1021", row.level));
		UISetter.SetLabel(beforeSpaceLevel, Localization.Format("1021", row.level - 1));
		UISetter.SetLabel(dotLevel, Localization.Format("1021", row.level));
		UISetter.SetActive(upgradeText, state == EBuildingState.Upgrading || state == EBuildingState.UpgradeComplete);
		if (state == EBuildingState.Upgrading)
		{
			UISetter.SetSprite(upgradeText, "text_upgrading", snap: true);
		}
		else if (state == EBuildingState.UpgradeComplete)
		{
			UISetter.SetSprite(upgradeText, "text_upgradcomplete", snap: true);
			if (upgradeCompleteEffect == null)
			{
				upgradeCompleteEffect = Utility.LoadAndInstantiateGameObject("Prefabs/Effect/Fx_NewBuilding", base.gameObject.transform);
				if (upgradeCompleteEffect != null)
				{
					GameObject gameObject = FindDeepOjbect(upgradeCompleteEffect, "NewText");
					if (gameObject != null)
					{
						UISetter.SetActive(gameObject, active: false);
					}
				}
			}
		}
		if (state != EBuildingState.UpgradeComplete && upgradeCompleteEffect != null)
		{
			Object.Destroy(upgradeCompleteEffect);
		}
	}

	public void Set(RoBuilding building)
	{
		if ((bool)timer)
		{
			UISetter.SetActive(timer, building.upgradeTime.IsValid());
			if (building.upgradeTime.IsValid())
			{
				UISetter.SetTimer(timer, building.upgradeTime);
			}
		}
		state = building.state;
		SetBuildingLock(building.firstLevelReg.userLevel);
		UISetter.SetLabel(Locklevel, building.firstLevelReg.userLevel);
		UISetter.SetActive(upgradeEnable, active: false);
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		if (building.type == EBuilding.WarMemorial)
		{
			UISetter.SetActive(badge, localUser.badgeMissionCount + localUser.badgeAchievementCount > 0);
		}
		else if (building.type == EBuilding.Raid)
		{
			UISetter.SetActive(badge, localUser.badgeRaidShop || localUser.raidRank > localUser.raidRewardPoint);
		}
		else if (building.type == EBuilding.Challenge)
		{
			UISetter.SetActive(badge, localUser.badgeChallenge || localUser.badgeChallengeShop || localUser.winRank != localUser.winRankIdx || localUser.badgeWaveDuelShop);
		}
		else if (building.type == EBuilding.WorldChallenge)
		{
			UISetter.SetActive(badge, localUser.worldWinRank != localUser.worldWinRankIdx);
		}
		else if (building.type == EBuilding.Headquarters)
		{
			UISetter.SetActive(badge, localUser.IsBadgeHeadQuarter());
		}
		else if (building.type == EBuilding.Gacha)
		{
			UISetter.SetActive(badge, localUser.CanOpenFreeGachaBox());
		}
		else if (building.type != EBuilding.Academy)
		{
			if (building.type == EBuilding.WaveBattle)
			{
				UISetter.SetActive(badge, localUser.badgeWaveBattle);
			}
			else if (building.type == EBuilding.Guild)
			{
				UISetter.SetActive(badge, localUser.badgeGuild);
			}
			else if (building.type == EBuilding.EventBattle)
			{
				UISetter.SetActive(badge, localUser.badgeEventRaidReward);
			}
			else if (building.type == EBuilding.InfinityBattle)
			{
				UISetter.SetActive(badge, localUser.badgeInfinityBattleReward);
			}
		}
		Set(building.reg);
	}

	public void Set(EBuilding type)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		RoBuilding roBuilding = localUser.FindBuilding(type);
		UISetter.SetSprite(image, roBuilding.reg.resourceId);
	}

	public void SetBuildingLock(int level)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		if (base.name == "Building-Laboratory")
		{
			return;
		}
		if (level == 999)
		{
			UISetter.SetActive(buildingObject, active: false);
			UISetter.SetActive(Lock, active: false);
			UISetter.SetActive(Name, active: false);
			state = EBuildingState.Lock;
			tempState = EBuildingState.Lock;
			return;
		}
		if (localUser.level < level)
		{
			UISetter.SetActive(Lock, active: true);
			UISetter.SetActive(Name, active: false);
			state = EBuildingState.Lock;
			tempState = EBuildingState.Lock;
			if (PlayerPrefs.GetInt(localUser.uno + ":" + base.name, 0) == 0)
			{
				PlayerPrefs.SetInt(localUser.uno + ":" + base.name, 1);
			}
		}
		else if (base.name == "Building-VipShop")
		{
			RoBuilding roBuilding = localUser.FindBuilding(EBuilding.VipShop);
			UISetter.SetActive(this, active: true);
			UISetter.SetActive(Name, active: true);
			if (localUser.statistics.vipShop == 0)
			{
				if (localUser.vipLevel < roBuilding.firstLevelReg.vipLevel)
				{
					UISetter.SetActive(this, active: false);
				}
				else
				{
					UISetter.SetActive(Lock, active: true);
					UISetter.SetLabel(cost, int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["SKY_SHOP_CREATE_CASH"].value));
					state = EBuildingState.Lock;
					tempState = EBuildingState.Lock;
				}
			}
			else
			{
				UISetter.SetActive(Lock, active: false);
				state = EBuildingState.Undefined;
			}
		}
		else if (base.name == "Building-VipGacha")
		{
			RoBuilding roBuilding2 = localUser.FindBuilding(EBuilding.VipGacha);
			if (roBuilding2 != null)
			{
				if (localUser.vipLevel >= roBuilding2.reg.vipLevel)
				{
					UISetter.SetActive(this, active: true);
				}
				else
				{
					UISetter.SetActive(this, active: false);
				}
			}
		}
		else
		{
			UISetter.SetActive(Lock, active: false);
			UISetter.SetActive(Name, active: true);
			if (state == EBuildingState.Lock)
			{
				state = EBuildingState.Undefined;
			}
			if (PlayerPrefs.GetInt(localUser.uno + ":" + base.name, 0) == 1)
			{
				PlayerPrefs.SetInt(localUser.uno + ":" + base.name, 2);
			}
		}
		if (PlayerPrefs.GetInt(localUser.uno + ":" + base.name, 0) == 2)
		{
			state = EBuildingState.Open;
			if (openEffect == null)
			{
				openEffect = Utility.LoadAndInstantiateGameObject("Prefabs/Effect/Fx_NewBuilding", base.gameObject.transform);
			}
		}
		UISetter.SetActive(buildingObject, !Lock.activeSelf && openEffect == null);
	}

	public void StartProgress(TimeData _data)
	{
		timer.Set(_data);
	}

	public void OpenNotiView(EBuildingNotiType _type)
	{
		UISetter.SetActive(notiView, active: true);
	}

	public void CloseNotiView()
	{
		UISetter.SetActive(notiView, active: false);
	}

	public void PlayAnimation()
	{
		UIPlayTween component = GetComponent<UIPlayTween>();
		component.resetOnPlay = true;
		component.tweenGroup = 1;
		component.Play(forward: true);
	}

	public IEnumerator PlayOpenEffect()
	{
		state = EBuildingState.Undefined;
		tempState = EBuildingState.Undefined;
		PlayerPrefs.SetInt(RemoteObjectManager.instance.localUser.uno + ":" + base.name, 3);
		if (openEffect != null)
		{
			Object.Destroy(openEffect);
		}
		Object.Destroy(Utility.LoadAndInstantiateGameObject("Prefabs/Effect/Fx_BuildingAppear", base.gameObject.transform), 1.3f);
		SoundManager.PlaySFX("SE_OpenObject_001");
		yield return new WaitForSeconds(0.2f);
		UISetter.SetActive(buildingObject, active: true);
	}

	public void DestroyOpenEffect()
	{
		state = EBuildingState.Undefined;
		tempState = EBuildingState.Undefined;
		PlayerPrefs.SetInt(RemoteObjectManager.instance.localUser.uno + ":" + base.name, 3);
		if (openEffect != null)
		{
			Object.Destroy(openEffect);
		}
		UISetter.SetActive(buildingObject, active: true);
	}

	private GameObject FindDeepOjbect(GameObject parent, string name)
	{
		Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>(includeInactive: true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name == name)
			{
				return transform.gameObject;
			}
		}
		return null;
	}
}
