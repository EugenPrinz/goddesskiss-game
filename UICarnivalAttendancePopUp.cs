using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shared.Regulation;
using UnityEngine;

public class UICarnivalAttendancePopUp : UIPopup
{
	public UITexture texture;

	public UIDefaultListView tabListView;

	public UIDefaultListView rewardListView;

	public GameObject dontBtn;

	public GameObject receiveBtn;

	public GameObject completeBtn;

	private readonly string eventImagePrefix = "Texture/UI/";

	private CarnivalTypeDataRow typeData;

	private Dictionary<string, CarnivalDataRow> carnivalList;

	private List<string> tabList;

	private string selectTab;

	public void Init(CarnivalTypeDataRow _typeData)
	{
		carnivalList = new Dictionary<string, CarnivalDataRow>();
		typeData = _typeData;
		base.regulation.carnivalDtbl.ForEach(delegate(CarnivalDataRow row)
		{
			if (row.cTidx == typeData.idx)
			{
				carnivalList.Add(row.checkCount.ToString(), row);
			}
		});
		tabList = carnivalList.Keys.ToList();
		tabList.Sort((string sort_1, string sort_2) => (int.Parse(sort_1) > int.Parse(sort_2)) ? 1 : (-1));
		if (!string.IsNullOrEmpty(typeData.img))
		{
			StartCoroutine(SetImage(typeData.img));
		}
		firstTab();
	}

	private void firstTab()
	{
		for (int i = 0; i < tabList.Count; i++)
		{
			string text = tabList[i];
			CarnivalDataRow carnivalDataRow = carnivalList[text];
			Protocols.CarnivalList.ProcessData processData = GetProcessData(text);
			selectTab = text;
			if (processData.receive == 0)
			{
				if (processData.complete == 0)
				{
					selectTab = tabList[Mathf.Max(0, i - 1)];
				}
				break;
			}
		}
		SetTab();
		SetReward();
		SetBtn();
	}

	private void SetReward()
	{
		CarnivalDataRow carnivalData = carnivalList[selectTab];
		List<RewardDataRow> list = RemoteObjectManager.instance.regulation.rewardDtbl.FindAll((RewardDataRow item) => item.category == ERewardCategory.Carnival && item.type == int.Parse(carnivalData.idx));
		rewardListView.InitAttandRewardList(list, carnivalData.check2, ((int)carnivalData.check2Type).ToString());
	}

	private void SetTab()
	{
		tabListView.InitAttendanceDayList(tabList, carnivalList, "Day-");
		tabListView.SetSelection(selectTab, selected: true);
	}

	private void SetBtn()
	{
		Protocols.CarnivalList.ProcessData processData = GetProcessData(selectTab);
		UISetter.SetActive(dontBtn, processData == null || (processData.complete == 0 && processData.receive == 0));
		UISetter.SetActive(receiveBtn, processData != null && processData.complete == 1 && processData.receive == 0);
		UISetter.SetActive(completeBtn, processData != null && processData.complete == 1 && processData.receive == 1);
	}

	private Protocols.CarnivalList.ProcessData GetProcessData(string tabId)
	{
		Protocols.CarnivalList.ProcessData result = null;
		CarnivalDataRow carnivalDataRow = carnivalList[tabId];
		if (base.localUser.carnivalList.carnivalProcessList.ContainsKey(typeData.idx) && base.localUser.carnivalList.carnivalProcessList[typeData.idx].ContainsKey(carnivalDataRow.idx))
		{
			result = base.localUser.carnivalList.carnivalProcessList[typeData.idx][carnivalDataRow.idx];
		}
		return result;
	}

	public override void OnRefresh()
	{
		firstTab();
	}

	public override void OnClick(GameObject sender)
	{
		string text = sender.name;
		if (text.StartsWith("Day-"))
		{
			string id = text.Substring(text.IndexOf("-") + 1);
			tabListView.SetSelection(id, selected: true);
			selectTab = id;
			SetReward();
			SetBtn();
		}
		else if (text == "ReceiveBtn")
		{
			CarnivalDataRow carnivalDataRow = carnivalList[selectTab];
			base.network.RequestCarnivalComplete(int.Parse(typeData.idx), carnivalDataRow.idx);
		}
		else if (text == "DontBtn")
		{
			NetworkAnimation.Instance.CreateFloatingText(Localization.Format("1057", selectTab));
		}
	}

	private IEnumerator SetImage(string texName)
	{
		string textureName = Localization.Get(texName);
		if (!AssetBundleManager.HasAssetBundle(textureName + ".assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(textureName + ".assetbundle", ECacheType.Texture));
		}
		if (AssetBundleManager.HasAssetBundle(textureName + ".assetbundle"))
		{
			Texture texture = AssetBundleManager.GetAssetBundle(textureName + ".assetbundle").LoadAsset($"{textureName}.png") as Texture;
			UISetter.SetTexture(this.texture, texture);
		}
		else
		{
			UISetter.SetTexture(this.texture, Utility.LoadTexture(string.Format(eventImagePrefix + "{0}", textureName)));
		}
		yield return true;
	}
}
