using System.Collections;
using Shared.Regulation;
using UnityEngine;

public class EventListItem : UIItemBase
{
	public UITexture texture;

	public GameObject badge;

	private readonly string eventImagePrefix = "Texture/UI/";

	public void Set(Protocols.EventBattleInfo data)
	{
		StartCoroutine(SetImage(data));
		UISetter.SetActive(badge, data.rewardCount > 0);
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		if (data.rewardCount > 0)
		{
			localUser.badgeEventRaidReward = true;
		}
	}

	private IEnumerator SetImage(Protocols.EventBattleInfo data)
	{
		EventBattleDataRow row = RemoteObjectManager.instance.regulation.eventBattleDtbl[data.idx];
		string textureName = Localization.Get(row.eventImg);
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
