using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainbanner : MonoBehaviour
{
	[SerializeField]
	private SwipeScript swipe;

	[SerializeField]
	private DownloadTexture downloadTexture;

	[HideInInspector]
	public List<BannerInfo> bannerInfoList = new List<BannerInfo>();

	private List<BannerInfo> remainBannerList = new List<BannerInfo>();

	private int currUrlIndex;

	private float maxBannerRotationTime = 10f;

	private float bannerTime;

	private bool bannerPress;

	[SerializeField]
	private GameObject bannerPoint;

	[SerializeField]
	private UIDefaultListView bannerPointListView;

	[SerializeField]
	private UITexture bannerTexture;

	private int wrapOffset = 4;

	public void InitUrlList(Protocols.RotationBanner bannerInfo)
	{
		if (bannerInfoList != null && bannerInfoList.Count > 0)
		{
			bannerInfoList.Clear();
		}
		if (remainBannerList != null && remainBannerList.Count > 0)
		{
			remainBannerList.Clear();
		}
		maxBannerRotationTime = bannerInfo.roataionTime;
		if (bannerInfo.bannerList != null && bannerInfo.bannerList.Count > 0)
		{
			double currentTime = RemoteObjectManager.instance.GetCurrentTime();
			double num = 0.0;
			double num2 = 0.0;
			for (int i = 0; i < bannerInfo.bannerList.Count; i++)
			{
				num = double.Parse(bannerInfo.bannerList[i].startDate);
				num2 = double.Parse(bannerInfo.bannerList[i].endDate);
				if (currentTime >= num && currentTime < num2)
				{
					BannerInfo bannerInfo2 = new BannerInfo();
					bannerInfo2.url = bannerInfo.bannerList[i].ImgUrl;
					bannerInfo2.linkType = bannerInfo.bannerList[i].linkType;
					bannerInfo2.linkIdx = bannerInfo.bannerList[i].linkIdx;
					bannerInfo2.eventIdx = bannerInfo.bannerList[i].eventIdx;
					bannerInfo2.startDate = num;
					bannerInfo2.endDate = num2;
					bannerInfo2.order = i;
					bannerInfoList.Add(bannerInfo2);
				}
				else if (currentTime < num)
				{
					BannerInfo bannerInfo3 = new BannerInfo();
					bannerInfo3.url = bannerInfo.bannerList[i].ImgUrl;
					bannerInfo3.linkType = bannerInfo.bannerList[i].linkType;
					bannerInfo3.linkIdx = bannerInfo.bannerList[i].linkIdx;
					bannerInfo3.eventIdx = bannerInfo.bannerList[i].eventIdx;
					bannerInfo3.startDate = num;
					bannerInfo3.endDate = num2;
					bannerInfo3.order = i;
					remainBannerList.Add(bannerInfo3);
				}
			}
		}
		DeleteDiffBannerInfo();
		currUrlIndex = 0;
		ResetBannerPoint();
		OpenBanner(0);
		UpdateEventBattleDeck(bannerInfo.bannerList);
	}

	private void UpdateEventBattleDeck(List<Protocols.RotationBanner.BannerList> bannerList)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < bannerList.Count; i++)
		{
			Protocols.RotationBanner.BannerList bannerList2 = bannerList[i];
			if (bannerList2.linkType == BannerListType.BattleEvent)
			{
				list.Add(bannerList2.eventIdx);
			}
		}
		Utility.UpdateEventBattleDeck(list);
	}

	private void OpenBanner(int urlIdx)
	{
		if (bannerInfoList != null && bannerInfoList.Count > 0)
		{
			double currentTime = RemoteObjectManager.instance.GetCurrentTime();
			BannerInfo banner = bannerInfoList[urlIdx];
			bool isTextureDownLoad = false;
			if (banner != null)
			{
				List<BannerInfo> originBannerList = RemoteObjectManager.instance.localUser.OriginBannerList;
				BannerInfo bannerInfo = originBannerList.Find((BannerInfo row) => row.url == banner.url);
				if (bannerInfo == null)
				{
					isTextureDownLoad = true;
					downloadTexture.MainBannerSetUrl(urlIdx);
				}
				else if (bannerInfo.texture == null)
				{
					isTextureDownLoad = true;
					downloadTexture.MainBannerSetUrl(urlIdx);
				}
				else
				{
					UISetter.SetTexture(bannerTexture, bannerInfo.texture);
				}
			}
			StartCoroutine(ActiveBannerPoint(urlIdx, isTextureDownLoad));
		}
		else
		{
			UISetter.SetActive(base.gameObject, active: false);
		}
	}

	public void PressBanner()
	{
		bannerPress = true;
	}

	public void ReleaseBanner()
	{
		bannerPress = false;
	}

	private void Update()
	{
		if (bannerInfoList == null || bannerInfoList.Count <= 0)
		{
			return;
		}
		if (bannerPress)
		{
			switch (swipe.GetCurrentSwipeType())
			{
			case SwipeType.LEFT:
				currUrlIndex++;
				Next();
				break;
			case SwipeType.RIGHT:
				currUrlIndex--;
				Previous();
				break;
			}
			if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
			{
				bannerTime = 0f;
			}
		}
		if (Input.touchCount != 0)
		{
			return;
		}
		if (bannerTime < maxBannerRotationTime)
		{
			bannerTime += Time.deltaTime;
			return;
		}
		bannerTime = 0f;
		currUrlIndex++;
		for (int i = 0; i < remainBannerList.Count; i++)
		{
			if (remainBannerList[i].startDate <= RemoteObjectManager.instance.GetCurrentTime())
			{
				bannerInfoList.Insert(remainBannerList[i].order, remainBannerList[i]);
				remainBannerList.Remove(remainBannerList[i]);
				ResetBannerPoint();
			}
		}
		Next();
	}

	public void ClickPrevious()
	{
		currUrlIndex--;
		Previous();
	}

	public void ClickNext()
	{
		currUrlIndex++;
		Next();
	}

	private void Previous()
	{
		swipe.SetSwipteStateNone();
		if (currUrlIndex < 0)
		{
			currUrlIndex = bannerInfoList.Count - 1;
		}
		OpenBanner(currUrlIndex);
		bannerPress = false;
	}

	private void Next()
	{
		swipe.SetSwipteStateNone();
		if (currUrlIndex > bannerInfoList.Count - 1)
		{
			currUrlIndex = 0;
		}
		OpenBanner(currUrlIndex);
		bannerPress = false;
	}

	private IEnumerator ActiveBannerPoint(int order, bool isTextureDownLoad)
	{
		if (isTextureDownLoad)
		{
			while (!downloadTexture.isTextureDownLoadDone)
			{
				yield return null;
			}
		}
		UISetter.SetActive(bannerPoint, active: true);
		for (int i = 0; i < bannerInfoList.Count; i++)
		{
			if (order == i)
			{
				bannerPointListView.SetSelection(i.ToString(), selected: true);
			}
			else
			{
				bannerPointListView.SetSelection(i.ToString(), selected: false);
			}
		}
	}

	private void ResetBannerPoint()
	{
		bannerPointListView.InitBannerPorintList(bannerInfoList);
	}

	public void BannerClick()
	{
		BannerInfo bannerInfo = bannerInfoList[currUrlIndex];
		UICamp camp = UIManager.instance.world.camp;
		if (bannerInfo == null || !(camp != null))
		{
			return;
		}
		if (bannerInfo.endDate < RemoteObjectManager.instance.GetCurrentTime())
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("6600"));
			RemoteObjectManager.instance.RequestGetRotationBannerInfo();
			return;
		}
		switch (bannerInfo.linkType)
		{
		case BannerListType.WorldMap:
			if (bannerInfo.linkIdx == 0)
			{
				UIManager.instance.world.worldMap.InitAndOpenWorldMap();
			}
			break;
		case BannerListType.Building:
		{
			RoBuilding roBuilding = RemoteObjectManager.instance.localUser.FindBuilding((EBuilding)bannerInfo.linkIdx);
			switch (roBuilding.type)
			{
			case EBuilding.Academy:
				camp.GoNavigation("Academy");
				break;
			case EBuilding.BlackMarket:
				camp.GoNavigation("BlackMarket");
				break;
			case EBuilding.Challenge:
				camp.GoNavigation("Challenge");
				break;
			case EBuilding.Guild:
				camp.GoNavigation("Guild");
				break;
			case EBuilding.Headquarters:
				camp.GoNavigation("HeadQuarter");
				break;
			case EBuilding.Laboratory:
				camp.GoNavigation("Laboratory");
				break;
			case EBuilding.Loot:
				camp.GoNavigation("Loot");
				break;
			case EBuilding.Raid:
				camp.GoNavigation("Raid");
				break;
			case EBuilding.SituationRoom:
				camp.GoNavigation("Situation");
				break;
			case EBuilding.Storage:
				camp.GoNavigation("Storage");
				break;
			case EBuilding.WarMemorial:
				camp.GoNavigation("WarMemorial");
				break;
			case EBuilding.WaveBattle:
				camp.GoNavigation("WaveBattle");
				break;
			case EBuilding.EventBattle:
				RemoteObjectManager.instance.RequestGetEventBattleData(bannerInfo.eventIdx);
				break;
			}
			break;
		}
		case BannerListType.BattleEvent:
			RemoteObjectManager.instance.RequestGetEventBattleData(bannerInfo.eventIdx);
			break;
		case BannerListType.Gacha:
			camp.GoNavigation("Gacha");
			StartCoroutine(SelectBanner_OpenGacah(bannerInfo.linkIdx.ToString()));
			break;
		case BannerListType.DiaShop:
			UIManager.instance.world.mainCommand.OpenDiamonShop(bannerInfo.linkIdx);
			break;
		case BannerListType.Carnival:
			RemoteObjectManager.instance.RequestGetCarnivalList(ECarnivalCategory.Basic);
			break;
		case BannerListType.Welfare:
			RemoteObjectManager.instance.RequestGetCarnivalList(ECarnivalCategory.Reward);
			break;
		}
	}

	private IEnumerator SelectBanner_OpenGacah(string idx)
	{
		while (!UIManager.instance.world.existGacha)
		{
			yield return null;
		}
		while (!UIManager.instance.world.gacha.isActive || UIManager.instance.world.gacha.bannerListView.itemList == null)
		{
			yield return null;
		}
		UIManager.instance.world.gacha.GoNavigationGacha(idx);
	}

	private void DeleteDiffBannerInfo()
	{
		List<BannerInfo> OriginList = RemoteObjectManager.instance.localUser.OriginBannerList;
		List<BannerInfo> list = new List<BannerInfo>();
		if (OriginList == null || OriginList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < OriginList.Count; i++)
		{
			BannerInfo bannerInfo = bannerInfoList.Find((BannerInfo row) => row.url == OriginList[i].url);
			if (bannerInfo == null)
			{
				list.Add(OriginList[i]);
			}
		}
		if (list.Count > 0)
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				OriginList.Remove(list[num]);
			}
		}
	}
}
