using System;
using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UILoading : MonoBehaviour
{
	public UITexture backGround;

	public UILabel type;

	public UILabel commanderName;

	public UILabel unitName;

	[SerializeField]
	private UILabel loadingTip_txt;

	public TweenAlpha inTweener;

	public TweenAlpha outTweener;

	private Regulation regulation;

	public static readonly string loadingImagePrefix = "Texture/Loading/";

	private List<string> resourceList;

	private DataTable<LoadingTipDataRow> Dt_LoadingTip;

	private List<LoadingTipDataRow> list_LoadingDataTip = new List<LoadingTipDataRow>();

	[SerializeField]
	private GameObject OriginLoading;

	[SerializeField]
	private GameObject NewLoadingBG;

	private IEnumerator Start()
	{
		regulation = RemoteObjectManager.instance.regulation;
		Dt_LoadingTip = regulation.loadingTipDtbl;
		LoadingTipDataRow row = GetLoadigDataTip();
		if (row == null)
		{
			yield break;
		}
		if (row.type == 100)
		{
			string textureName = row.backgroundimg;
			if (!AssetBundleManager.HasAssetBundle(textureName + ".assetbundle"))
			{
				yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(textureName + ".assetbundle", ECacheType.Texture));
			}
			if (AssetBundleManager.HasAssetBundle(textureName + ".assetbundle"))
			{
				UISetter.SetTexture(backGround, AssetBundleManager.GetAssetBundle(textureName + ".assetbundle").mainAsset as Texture);
			}
			else
			{
				UISetter.SetTexture(backGround, Utility.LoadTexture(string.Format(loadingImagePrefix + "{0}", row.backgroundimg)));
			}
			UISetter.SetActive(OriginLoading, active: true);
			UISetter.SetActive(loadingTip_txt.gameObject, active: false);
			UISetter.SetLabel(type, Localization.Get(row.comjobsidx));
			UISetter.SetLabel(commanderName, Localization.Get(row.comnamesidx));
			UISetter.SetLabel(unitName, Localization.Get(row.comunitnamesidx));
		}
		else
		{
			UISetter.SetActive(backGround, active: false);
			UISetter.SetActive(OriginLoading, active: false);
			UISetter.SetActive(NewLoadingBG.transform.Find(row.backgroundimg).gameObject, active: true);
			UISetter.SetActive(loadingTip_txt, active: true);
			UISetter.SetLabel(loadingTip_txt, Localization.Get(row.comjobsidx));
		}
		base.gameObject.transform.localScale = UIRoot.list[0].transform.localScale;
	}

	public void In()
	{
		_Play(inTweener);
	}

	public void Out()
	{
		_Play(outTweener);
	}

	private void _Play(UITweener tweener)
	{
		tweener.enabled = true;
		tweener.ResetToBeginning();
		tweener.PlayForward();
	}

	public void Close()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		if (backGround != null)
		{
			UnityEngine.Object.DestroyImmediate(backGround);
			backGround = null;
		}
		Resources.UnloadUnusedAssets();
		GC.Collect();
		GC.WaitForPendingFinalizers();
	}

	private LoadingTipDataRow GetLoadigDataTip()
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		if (localUser == null)
		{
			return null;
		}
		list_LoadingDataTip.Clear();
		BattleData battleData = BattleData.Get();
		BattleData.Set(battleData);
		for (int i = 0; i < Dt_LoadingTip.length; i++)
		{
			if (localUser.level < Dt_LoadingTip[i].userstartlevel || localUser.level > Dt_LoadingTip[i].userendlevel)
			{
				continue;
			}
			if (battleData == null)
			{
				if (Dt_LoadingTip[i].type == 100)
				{
					list_LoadingDataTip.Add(Dt_LoadingTip[i]);
				}
			}
			else if (Dt_LoadingTip[i].type == 100 || battleData.type == (EBattleType)Dt_LoadingTip[i].type)
			{
				list_LoadingDataTip.Add(Dt_LoadingTip[i]);
			}
		}
		int index = UnityEngine.Random.Range(0, list_LoadingDataTip.Count);
		return list_LoadingDataTip[index];
	}
}
