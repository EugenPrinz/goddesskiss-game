using System;
using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIWorldDuelInfoPopup : UIPopup
{
	[Serializable]
	public class WorldDuelUser : UIInnerPartBase
	{
		public UISpineAnimation spineAnimation;

		public List<UISprite> buffList;

		public UISprite gradeIcon;

		public UILabel server;

		public UILabel level;

		public UILabel nickName;

		public UILabel record;

		public UILabel score;

		public UILabel gradeNum;

		private UIPanelBase parent;

		public void Set(RoUser user, UIPanelBase parent)
		{
			this.parent = parent;
			for (int i = 0; i < buffList.Count; i++)
			{
				UISprite sprite = buffList[i];
				EWorldDuelBuff key = (EWorldDuelBuff)(i + 1);
				switch ($"{key.ToString()}{user.activeBuff[key].ToString()}")
				{
				case "attb":
					UISetter.SetSprite(sprite, "group_buff_1001");
					break;
				case "attd":
					UISetter.SetSprite(sprite, "group_buff_1001_off");
					break;
				case "defb":
					UISetter.SetSprite(sprite, "group_buff_1002");
					break;
				case "defd":
					UISetter.SetSprite(sprite, "group_buff_1002_off");
					break;
				case "supb":
					UISetter.SetSprite(sprite, "group_buff_1003");
					break;
				case "supd":
					UISetter.SetSprite(sprite, "group_buff_1003_off");
					break;
				}
			}
			UISetter.SetLabel(server, Localization.Format("19067", user.world));
			UISetter.SetLabel(score, Localization.Format("17014", user.worldDuelScore));
			UISetter.SetLabel(level, user.level);
			UISetter.SetLabel(record, Localization.Format("17016", user.worldWinCount, user.worldLoseCount));
			UISetter.SetLabel(nickName, user.nickname);
			UISetter.SetSprite(gradeIcon, user.GetDuelGradeIcon());
			SetGradeLabel(user.duelGradeIdx);
			SetSpine(user.thumbnailId);
		}

		private void SetGradeLabel(int duelGradeIdx)
		{
			if (base.regulation.rankingDtbl.ContainsKey(duelGradeIdx.ToString()))
			{
				RankingDataRow data = base.regulation.rankingDtbl[duelGradeIdx.ToString()];
				List<RankingDataRow> list = base.regulation.rankingDtbl.FindAll((RankingDataRow row) => row.type == data.type && row.icon == data.icon);
				UISetter.SetActive(gradeNum, list.Count > 1);
				int num = list.IndexOf(data);
				UISetter.SetLabel(gradeNum, num + 1);
			}
		}

		public void SetSpine(string thumbnailId)
		{
			Regulation regulation = RemoteObjectManager.instance.regulation;
			string costumeThumbnailName = regulation.GetCostumeThumbnailName(int.Parse(thumbnailId));
			string[] array = costumeThumbnailName.Split('_');
			string resourceId = array[0] + "_" + array[1];
			parent.StartCoroutine(CreateSpineFromCache(resourceId, array[2]));
		}

		private IEnumerator CreateSpineFromCache(string resourceId, string costumeName)
		{
			if (spineAnimation != null)
			{
				if (!AssetBundleManager.HasAssetBundle(resourceId + ".assetbundle"))
				{
					yield return parent.StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(resourceId + ".assetbundle"));
				}
				UISetter.SetSpine(spineAnimation, resourceId, costumeName);
				spineAnimation.BroadcastMessage("MarkAsChanged", SendMessageOptions.DontRequireReceiver);
			}
			yield return null;
		}
	}

	public WorldDuelUser attackUser;

	public WorldDuelUser defenderUser;

	public int delay = 5;

	public bool battleEnable;

	private void Start()
	{
		Open();
		SetAutoDestroy(autoDestory: true);
	}

	public void Init(RoUser attacker, RoUser defender)
	{
		battleEnable = false;
		attackUser.Set(attacker, this);
		defenderUser.Set(defender, this);
	}

	public void StartGameObjectDestroy()
	{
		StartCoroutine("GameObjectDestroy");
	}

	private IEnumerator GameObjectDestroy()
	{
		yield return new WaitForSeconds(delay);
		battleEnable = true;
	}
}
