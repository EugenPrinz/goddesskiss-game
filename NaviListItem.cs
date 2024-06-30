using UnityEngine;

public class NaviListItem : UIItemBase
{
	public UISprite icon;

	public UISprite buildingIcon;

	public UISprite commonIcon;

	public UILabel title;

	public UILabel discription;

	public UILabel btnLabel;

	public GameObject btn;

	public GameObject lockRoot;

	public void Set(Protocols.NavigatorInfo row)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		bool flag = false;
		UISetter.SetActive(icon, active: false);
		UISetter.SetActive(buildingIcon, active: false);
		UISetter.SetActive(commonIcon, active: false);
		if (row.type == ENavigatorType.Stage)
		{
			UISetter.SetActive(icon, active: true);
			RoWorldMap roWorldMap = localUser.FindWorldMapByStage(row.stageIdx.ToString());
			RoWorldMap.Stage stage = localUser.FindWorldMapStage(row.stageIdx.ToString());
			UISetter.SetSprite(icon, stage.typeData.resourceId, snap: false);
			UISetter.SetLabel(title, Localization.Get(roWorldMap.name));
			UISetter.SetLabel(discription, Localization.Get(stage.data.title));
			flag = localUser.lastClearStage < row.stageIdx - 1;
		}
		else if (row.type == ENavigatorType.Challenge)
		{
			UISetter.SetActive(buildingIcon, active: true);
			UISetter.SetSprite(buildingIcon, "icon_Challenge", snap: false);
			UISetter.SetLabel(title, Localization.Get("1908"));
			UISetter.SetLabel(discription, Localization.Get("20062"));
			RoBuilding roBuilding = localUser.FindBuilding(EBuilding.Challenge);
			flag = localUser.level < roBuilding.firstLevelReg.userLevel;
		}
		else if (row.type == ENavigatorType.Raid)
		{
			UISetter.SetActive(buildingIcon, active: true);
			UISetter.SetSprite(buildingIcon, "icon_Raid", snap: false);
			UISetter.SetLabel(title, Localization.Get("1909"));
			UISetter.SetLabel(discription, Localization.Get("20063"));
			RoBuilding roBuilding2 = localUser.FindBuilding(EBuilding.Raid);
			flag = localUser.level < roBuilding2.firstLevelReg.userLevel;
		}
		else if (row.type == ENavigatorType.GuildShop)
		{
			UISetter.SetActive(buildingIcon, active: true);
			UISetter.SetSprite(buildingIcon, "icon_BlackMarket", snap: false);
			UISetter.SetLabel(title, Localization.Get("1912"));
			UISetter.SetLabel(discription, Localization.Format("1339", Localization.Get("1910")));
			RoBuilding roBuilding3 = localUser.FindBuilding(EBuilding.Guild);
			flag = localUser.level < roBuilding3.firstLevelReg.userLevel;
		}
		else if (row.type == ENavigatorType.WarHome)
		{
			UISetter.SetActive(buildingIcon, active: true);
			UISetter.SetSprite(buildingIcon, "icon_WarMemorial", snap: false);
			UISetter.SetLabel(title, Localization.Get("1907"));
			UISetter.SetLabel(discription, Localization.Get("1343"));
			RoBuilding roBuilding4 = localUser.FindBuilding(EBuilding.WarMemorial);
			flag = localUser.level < roBuilding4.firstLevelReg.userLevel;
		}
		else if (row.type == ENavigatorType.Gacha)
		{
			UISetter.SetActive(buildingIcon, active: true);
			UISetter.SetSprite(buildingIcon, "icon_Gacha", snap: false);
			UISetter.SetLabel(title, Localization.Get("1911"));
			UISetter.SetLabel(discription, Localization.Get("1344"));
			RoBuilding roBuilding5 = localUser.FindBuilding(EBuilding.Gacha);
			flag = localUser.level < roBuilding5.firstLevelReg.userLevel;
		}
		else if (row.type == ENavigatorType.Event)
		{
			UISetter.SetActive(icon, active: true);
			UISetter.SetActive(btn, active: false);
			UISetter.SetSprite(icon, "ng-event-icon", snap: false);
			UISetter.SetLabel(title, Localization.Get("1341"));
			UISetter.SetLabel(discription, Localization.Get("1342"));
		}
		else if (row.type == ENavigatorType.Annihilation)
		{
			UISetter.SetActive(buildingIcon, active: true);
			UISetter.SetSprite(buildingIcon, "icon_Challenge2", snap: false);
			UISetter.SetLabel(title, Localization.Get("1913"));
			UISetter.SetLabel(discription, Localization.Get("20050"));
			RoBuilding roBuilding6 = localUser.FindBuilding(EBuilding.Loot);
			flag = localUser.level < roBuilding6.firstLevelReg.userLevel;
		}
		else if (row.type == ENavigatorType.StageGroup)
		{
			UISetter.SetActive(icon, active: true);
			RoWorldMap roWorldMap2 = localUser.FindWorldMap(row.stageIdx.ToString());
			UISetter.SetLabel(title, Localization.Get(roWorldMap2.name));
			UISetter.SetLabel(discription, Localization.Get("1346"));
			int num = int.Parse(localUser.FindLastOpenedWorldMap().id);
			flag = num < row.stageIdx;
		}
		else if (row.type == ENavigatorType.DailyBattle)
		{
			UISetter.SetActive(buildingIcon, active: true);
			UISetter.SetSprite(buildingIcon, "icon_SituationRoom", snap: false);
			UISetter.SetLabel(title, Localization.Get("1904"));
			UISetter.SetLabel(discription, Localization.Get("20042"));
			RoBuilding roBuilding7 = localUser.FindBuilding(EBuilding.SituationRoom);
			flag = localUser.level < roBuilding7.firstLevelReg.userLevel;
		}
		else if (row.type == ENavigatorType.Shop)
		{
			UISetter.SetActive(buildingIcon, active: true);
			UISetter.SetSprite(buildingIcon, "icon_BlackMarket", snap: false);
			UISetter.SetLabel(title, Localization.Get("1912"));
			UISetter.SetLabel(discription, Localization.Get("20043"));
			RoBuilding roBuilding8 = localUser.FindBuilding(EBuilding.BlackMarket);
			flag = localUser.level < roBuilding8.firstLevelReg.userLevel;
		}
		else if (row.type == ENavigatorType.LastStageMap)
		{
			UISetter.SetActive(icon, active: true);
			UISetter.SetSprite(icon, "stage_boss_01", snap: false);
			UISetter.SetLabel(title, Localization.Get("1275"));
			UISetter.SetLabel(discription, Localization.Get("20064"));
			flag = false;
		}
		else if (row.type == ENavigatorType.WaveDuel)
		{
			UISetter.SetActive(buildingIcon, active: true);
			UISetter.SetSprite(buildingIcon, "icon_Challenge", snap: false);
			UISetter.SetLabel(title, Localization.Get("1908"));
			UISetter.SetLabel(discription, Localization.Get("20062"));
			RoBuilding roBuilding9 = localUser.FindBuilding(EBuilding.Challenge);
			flag = localUser.level < roBuilding9.firstLevelReg.userLevel;
			if (!flag)
			{
				int num2 = int.Parse(RemoteObjectManager.instance.regulation.defineDtbl["ARENA_3WAVE_OPEN_LEVEL"].value);
				flag = localUser.level < num2;
			}
		}
		else if (row.type == ENavigatorType.HeadQuarter)
		{
			UISetter.SetActive(buildingIcon, active: true);
			UISetter.SetSprite(buildingIcon, "icon_Headquarters", snap: false);
			UISetter.SetLabel(title, Localization.Get("1900"));
			UISetter.SetLabel(discription, Localization.Get("20066"));
			RoBuilding roBuilding10 = localUser.FindBuilding(EBuilding.Headquarters);
			flag = localUser.level < roBuilding10.firstLevelReg.userLevel;
		}
		else if (row.type == ENavigatorType.Carnival)
		{
			UISetter.SetActive(commonIcon, active: true);
			UISetter.SetSprite(buildingIcon, "qm-carnival-icon", snap: false);
			UISetter.SetLabel(title, Localization.Get("1918"));
			UISetter.SetLabel(discription, Localization.Get("1381"));
			flag = false;
		}
		else if (row.type == ENavigatorType.VipShop)
		{
			UISetter.SetActive(icon, active: true);
			UISetter.SetActive(btn, active: false);
			UISetter.SetSprite(icon, "icon_FloatingShop", snap: false);
			UISetter.SetLabel(title, Localization.Get("1917"));
			UISetter.SetLabel(discription, Localization.Get("20067"));
		}
		else if (row.type == ENavigatorType.VipGacha)
		{
			UISetter.SetActive(icon, active: true);
			UISetter.SetActive(btn, active: false);
			UISetter.SetSprite(icon, "icon_CruiseHall", snap: false);
			UISetter.SetLabel(title, Localization.Get("1916"));
			UISetter.SetLabel(discription, Localization.Get("20068"));
		}
		else if (row.type == ENavigatorType.VipShop_1)
		{
			UISetter.SetActive(icon, active: true);
			UISetter.SetActive(btn, active: false);
			UISetter.SetSprite(icon, "icon_FloatingShop", snap: false);
			UISetter.SetLabel(title, Localization.Get("1917"));
			UISetter.SetLabel(discription, Localization.Get("20069"));
		}
		else if (row.type == ENavigatorType.VipGacha_1)
		{
			UISetter.SetActive(icon, active: true);
			UISetter.SetActive(btn, active: false);
			UISetter.SetSprite(icon, "icon_CruiseHall", snap: false);
			UISetter.SetLabel(title, Localization.Get("1916"));
			UISetter.SetLabel(discription, Localization.Get("20070"));
		}
		if (flag)
		{
			UISetter.SetLabel(btnLabel, Localization.Get("1340"));
		}
		else
		{
			UISetter.SetLabel(btnLabel, Localization.Get("1004"));
		}
		UISetter.SetButtonGray(btn, !flag);
	}
}
