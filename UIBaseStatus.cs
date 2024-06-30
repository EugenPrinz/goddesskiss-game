using UnityEngine;

public class UIBaseStatus : UIPanelBase
{
	public UILabel timeMachine;

	public UILabel traningTicket1;

	public UILabel traningTicket2;

	public UILabel traningTicket3;

	public UILabel traningTicket4;

	public UILabel munitions;

	public UILabel explorationTicket;

	public UILabel sweepTicket;

	public UILabel opener;

	public UILabel challenge;

	public UILabel level;

	public GameObject[] objBuilding = new GameObject[13];

	private RoLocalUser _currUser;

	public GameObject buildings;

	public GameObject resources;

	public UIScrollView haveResource;

	public void Set(RoLocalUser user, bool isBuilding)
	{
		_currUser = user;
		UISetter.SetActive(buildings, isBuilding);
		UISetter.SetActive(resources, !isBuilding);
		UISetter.SetLabel(traningTicket1, user.resourceList[ETrainingTicketType.ctt1.ToString()]);
		UISetter.SetLabel(traningTicket2, user.resourceList[ETrainingTicketType.ctt2.ToString()]);
		UISetter.SetLabel(traningTicket3, user.resourceList[ETrainingTicketType.ctt3.ToString()]);
		UISetter.SetLabel(traningTicket4, user.resourceList[ETrainingTicketType.ctt4.ToString()]);
		UISetter.SetLabel(munitions, user.munitions);
		UISetter.SetLabel(explorationTicket, user.explorationTicket);
		UISetter.SetLabel(sweepTicket, user.sweepTicket);
		UISetter.SetLabel(opener, user.opener);
		UISetter.SetLabel(challenge, user.challenge);
		UISetter.SetLabel(level, "Lv" + base.localUser.FindBuilding(EBuilding.Headquarters).level);
		objBuilding[0].transform.Find("Value").GetComponent<UILabel>().text = "Lv" + base.localUser.FindBuilding(EBuilding.Headquarters).level;
		objBuilding[1].transform.Find("Value").GetComponent<UILabel>().text = "Lv" + base.localUser.FindBuilding(EBuilding.MetroBank).level;
		objBuilding[2].transform.Find("Value").GetComponent<UILabel>().text = "Lv" + base.localUser.FindBuilding(EBuilding.Storage).level;
		objBuilding[3].transform.Find("Value").GetComponent<UILabel>().text = "Lv" + base.localUser.FindBuilding(EBuilding.Academy).level;
		objBuilding[4].transform.Find("Value").GetComponent<UILabel>().text = "Lv" + base.localUser.FindBuilding(EBuilding.SituationRoom).level;
		objBuilding[6].transform.Find("Value").GetComponent<UILabel>().text = "Lv" + base.localUser.FindBuilding(EBuilding.Gacha).level;
		objBuilding[7].transform.Find("Value").GetComponent<UILabel>().text = "Lv" + base.localUser.FindBuilding(EBuilding.WarMemorial).level;
		objBuilding[8].transform.Find("Value").GetComponent<UILabel>().text = "Lv" + base.localUser.FindBuilding(EBuilding.Challenge).level;
		objBuilding[9].transform.Find("Value").GetComponent<UILabel>().text = "Lv" + base.localUser.FindBuilding(EBuilding.Raid).level;
		objBuilding[10].transform.Find("Value").GetComponent<UILabel>().text = "Lv" + base.localUser.FindBuilding(EBuilding.Guild).level;
		objBuilding[11].transform.Find("Value").GetComponent<UILabel>().text = "Lv" + base.localUser.FindBuilding(EBuilding.BlackMarket).level;
		objBuilding[12].transform.Find("Value").GetComponent<UILabel>().text = "Lv" + base.localUser.FindBuilding(EBuilding.Loot).level;
		SetBuildingLock(objBuilding[0], base.localUser.FindBuilding(EBuilding.Headquarters).firstLevelReg.userLevel);
		SetBuildingLock(objBuilding[1], base.localUser.FindBuilding(EBuilding.MetroBank).firstLevelReg.userLevel);
		SetBuildingLock(objBuilding[2], base.localUser.FindBuilding(EBuilding.Storage).firstLevelReg.userLevel);
		SetBuildingLock(objBuilding[3], base.localUser.FindBuilding(EBuilding.Academy).firstLevelReg.userLevel);
		SetBuildingLock(objBuilding[4], base.localUser.FindBuilding(EBuilding.SituationRoom).firstLevelReg.userLevel);
		SetBuildingLock(objBuilding[6], base.localUser.FindBuilding(EBuilding.Gacha).firstLevelReg.userLevel);
		SetBuildingLock(objBuilding[7], base.localUser.FindBuilding(EBuilding.WarMemorial).firstLevelReg.userLevel);
		SetBuildingLock(objBuilding[8], base.localUser.FindBuilding(EBuilding.Challenge).firstLevelReg.userLevel);
		SetBuildingLock(objBuilding[9], base.localUser.FindBuilding(EBuilding.Raid).firstLevelReg.userLevel);
		SetBuildingLock(objBuilding[10], base.localUser.FindBuilding(EBuilding.Guild).firstLevelReg.userLevel);
		SetBuildingLock(objBuilding[11], base.localUser.FindBuilding(EBuilding.BlackMarket).firstLevelReg.userLevel);
		SetBuildingLock(objBuilding[12], base.localUser.FindBuilding(EBuilding.Loot).firstLevelReg.userLevel);
	}

	public void SetBuildingLock(GameObject obj, int level)
	{
		RoLocalUser roLocalUser = RemoteObjectManager.instance.localUser;
		if (roLocalUser.level < level || base.name == "Building-Guild" || base.name == "Building-Loot" || base.name == "Building-BlackMarket")
		{
			obj.transform.Find("Value").GetComponent<UILabel>().text = "Lock";
			obj.transform.Find("Icon").GetComponent<UISprite>().color = Color.gray;
			UISetter.SetActive(obj.transform.Find("Lock").GetComponent<UISprite>(), active: true);
		}
		else
		{
			obj.transform.Find("Icon").GetComponent<UISprite>().color = Color.white;
			UISetter.SetActive(obj.transform.Find("Lock").GetComponent<UISprite>(), active: false);
		}
	}
}
