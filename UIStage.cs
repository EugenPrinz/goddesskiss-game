using Shared.Regulation;
using UnityEngine;

public class UIStage : UIItemBase
{
	public UISprite icon;

	public UISprite slot;

	public UILabel lbStage;

	public GameObject openRoot;

	public GameObject lockRoot;

	public GameObject selectRoot;

	public GameObject productRoot;

	public GameObject badge;

	public GameObject unUse;

	public UISprite flag;

	public UISprite productCommanderThumbnail;

	public UISprite productCommanderThumbnailBg;

	public UITimer productTimer;

	public GameObject starRoot;

	public UIProgressBar hpProgress;

	private string commanderHit = "_t3";

	private string commanderIdle = "_t2";

	public override void SetSelection(bool selected)
	{
		base.SetSelection(selected);
		UISetter.SetActive(selectRoot, selected);
	}

	public override void SetLock(bool locked)
	{
		base.SetLock(locked);
		UISetter.SetActive(lockRoot, locked);
	}

	public void Set(RoWorldMap.Stage stage)
	{
		WorldMapStageTypeDataRow typeData = stage.typeData;
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetActive(badge, active: false);
		UISetter.SetSprite(icon, typeData.resourceId, snap: true);
		UISetter.SetActive(openRoot, active: true);
		UISetter.SetColor(icon, ((!stage.canBattle && (stage.data.type == EStageTypeIdRange.GuardPost || stage.data.type == EStageTypeIdRange.Storage)) || !stage.isOpen) ? Color.gray : Color.white);
		UISetter.SetColor(lbStage, (!stage.isOpen) ? new Color(0.88235295f, 32f / 85f, 0f, 1f) : Color.white);
		UISetter.SetActive(unUse, (stage.data.type == EStageTypeIdRange.GuardPost || stage.data.type == EStageTypeIdRange.Storage) && stage.isOpen && !stage.canBattle);
		UISetter.SetActive(icon, stage.data.type != EStageTypeIdRange.GuardPost && stage.data.type != EStageTypeIdRange.Storage);
		if (icon.gameObject.activeSelf)
		{
			selectRoot.transform.localPosition = new Vector3(0f, 63f, 0f);
		}
		else
		{
			selectRoot.transform.localPosition = new Vector3(0f, -15f, 0f);
		}
		UISetter.SetActive(starRoot, stage.data.type == EStageTypeIdRange.Factory || stage.data.type == EStageTypeIdRange.Terrorist || stage.data.type == EStageTypeIdRange.Storage);
		UISetter.SetRank(starRoot, stage.star);
		UISetter.SetActive(flag, (stage.data.type == EStageTypeIdRange.Factory || stage.data.type == EStageTypeIdRange.Terrorist) && localUser.lastClearStage >= int.Parse(stage.id));
		UISetter.SetSprite(flag, (stage.typeData.battleCount != 3) ? "stage_nomal_skyblue" : "stage_nomal_blue");
		bool isProduct = stage.isProduct;
		string worldMapId = stage.data.worldMapId;
		string arg = stage.data.order.ToString();
		lbStage.text = $"{worldMapId} - {arg}";
	}

	public void Set(RoScramble.Stage stage)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetActive(openRoot, active: true);
		UISetter.SetActive(lockRoot, !stage.isOpen);
		UISetter.SetColor(icon, (stage.clear || !stage.isOpen) ? Color.gray : Color.white);
		UISetter.SetActive(hpProgress, stage.isOpen);
		UISetter.SetProgress(hpProgress, (float)stage.hp / 100f);
		UISetter.SetActive(unUse, stage.clear);
	}
}
