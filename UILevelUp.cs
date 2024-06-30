using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UILevelUp : UIPopup
{
	public UILabel level;

	public UILabel commanderLevel;

	public UILabel bullet;

	private string imageStr = "lv_";

	public UILabel NextLevel;

	public UILabel NextCommanderLevel;

	public UILabel NextBullet;

	public UILabel count;

	public Animation popupAnimation;

	private const string inAnimationName = "LevelUoPopUp_In";

	private const string outAnimationName = "LevelUoPopUp_Out";

	public UISpineAnimation spineAnimation;

	[SerializeField]
	private UILabel description;

	[SerializeField]
	private UISprite buildingImg;

	[SerializeField]
	private GameObject Item;

	[SerializeField]
	private GameObject ItemWithBuilding;

	[SerializeField]
	private UILabel BulletCnt;

	private const string strprefix = "icon_";

	private void Start()
	{
		UISetter.SetSpine(spineAnimation, "n_010");
	}

	public void InitData(int _beforeLevel, int _level)
	{
		UISetter.SetLabel(level, _beforeLevel.ToString("D2"));
		UISetter.SetLabel(NextLevel, _level.ToString("D2"));
		UISetter.SetLabel(commanderLevel, _beforeLevel.ToString("D2"));
		UISetter.SetLabel(NextCommanderLevel, _level.ToString("D2"));
		UserLevelDataRow userLevelDataRow = base.regulation.userLevelDtbl.Find((UserLevelDataRow row) => row.level == _beforeLevel);
		bullet.text = userLevelDataRow.maxBullet.ToString("D2");
		int num = 0;
		int i;
		for (i = _beforeLevel + 1; i <= _level; i++)
		{
			userLevelDataRow = base.regulation.userLevelDtbl.Find((UserLevelDataRow row) => row.level == i);
			num += userLevelDataRow.rewardBullet;
		}
		NextBullet.text = userLevelDataRow.maxBullet.ToString();
		UISetter.SetLabel(count, num);
		EBuilding eBuilding = BuildingType(_beforeLevel, _level);
		SetBuildingResource(eBuilding);
		bool flag = ((eBuilding != 0) ? true : false);
		string text = Localization.Get("5730");
		if (flag)
		{
			text = Localization.Get("5729");
			UISetter.SetLabel(BulletCnt, num);
		}
		UISetter.SetLabel(description, text);
		UISetter.SetActive(Item, !flag);
		UISetter.SetActive(ItemWithBuilding, flag);
		base.localUser.beforeLevel = base.localUser.level;
		Open();
		popupAnimation.Play("LevelUoPopUp_In");
	}

	public void PlayCloseAnimation()
	{
		StartCoroutine(CloseAnimation());
	}

	private IEnumerator CloseAnimation()
	{
		popupAnimation.Play("LevelUoPopUp_Out");
		while (popupAnimation.IsPlaying("LevelUoPopUp_Out"))
		{
			yield return null;
		}
		Close();
	}

	private EBuilding BuildingType(int beforeLv, int level)
	{
		List<BuildingLevelDataRow> openBuildingDataList = base.regulation.GetOpenBuildingDataList();
		for (int i = 0; i < openBuildingDataList.Count; i++)
		{
			if (beforeLv < openBuildingDataList[i].userLevel && level >= openBuildingDataList[i].userLevel && openBuildingDataList[i].vipLevel == 0)
			{
				return openBuildingDataList[i].type;
			}
		}
		return EBuilding.Undefined;
	}

	private void SetBuildingResource(EBuilding _type)
	{
		string spriteName = string.Empty;
		switch (_type)
		{
		case EBuilding.MetroBank:
			spriteName = "icon_MetroBank";
			break;
		case EBuilding.SituationRoom:
			spriteName = "icon_SituationRoom";
			break;
		case EBuilding.Raid:
			spriteName = "icon_Raid";
			break;
		case EBuilding.Guild:
			spriteName = "icon_Guild";
			break;
		case EBuilding.BlackMarket:
			spriteName = "icon_BlackMarket";
			break;
		case EBuilding.Challenge:
			spriteName = "icon_Challenge";
			break;
		case EBuilding.Loot:
			spriteName = "icon_Challenge2";
			break;
		case EBuilding.WaveBattle:
			spriteName = "icon_Wave_battle";
			break;
		case EBuilding.Laboratory:
			spriteName = "icon_UnitLab";
			break;
		}
		UISetter.SetSprite(buildingImg, spriteName);
	}
}
