using System;
using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using Spine.Unity;
using UnityEngine;

public class UIGacha : UIPopup
{
	public class BoxData
	{
		public EGachaAnimationType gachaType { get; set; }

		public ERewardType rewardType { get; set; }

		public CommanderCompleteType getType { get; set; }

		public string rewardId { get; set; }

		public int rewardCount { get; set; }

		public bool isNew { get; set; }

		public int currentCommanderMedal { get; set; }

		public int getCommanderMedal { get; set; }

		public GoodsDataRow goodsData
		{
			get
			{
				if (rewardType != ERewardType.Goods)
				{
					return null;
				}
				return RemoteObjectManager.instance.regulation.goodsDtbl[rewardId];
			}
		}

		public PartDataRow partsData
		{
			get
			{
				if (rewardType != ERewardType.UnitMaterial)
				{
					return null;
				}
				return RemoteObjectManager.instance.regulation.partDtbl[rewardId];
			}
		}

		public CommanderDataRow commanderData
		{
			get
			{
				if (rewardType != ERewardType.Medal && rewardType != ERewardType.Commander)
				{
					return null;
				}
				return RemoteObjectManager.instance.regulation.commanderDtbl[rewardId];
			}
		}

		public CommanderCostumeDataRow costumeData
		{
			get
			{
				if (rewardType != ERewardType.Costume)
				{
					return null;
				}
				return RemoteObjectManager.instance.regulation.commanderCostumeDtbl[rewardId];
			}
		}

		public EquipItemDataRow equipItemData
		{
			get
			{
				if (rewardType != ERewardType.Item)
				{
					return null;
				}
				return RemoteObjectManager.instance.regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == rewardId);
			}
		}
	}

	public class GettableRewardData
	{
		public GachaRewardDataRow rewardData { get; set; }

		public List<int> countList { get; set; }
	}

	[Serializable]
	public class TypeARewardList : UIInnerPartBase
	{
		public UIDefaultListView probabilityList;

		public UIDefaultListView rewardList;

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			base.OnClick(sender, parent);
			string name = sender.name;
			if (name == "Close-PremiumGoodsList")
			{
				SoundManager.PlaySFX("EFM_SelectButton_002");
				UISetter.SetActive(root, active: false);
			}
		}

		public void Set(List<GettableRewardData> list, Dictionary<ERewardType, float> probability)
		{
			SoundManager.PlaySFX("EFM_OpenPopup_001");
			rewardList.CreateItem(list.Count, "Reward-", delegate(UIItemBase item, int index)
			{
				UIGoods uIGoods = item as UIGoods;
				GettableRewardData gettableRewardData = list[index];
				UISetter.SetGameObjectName(uIGoods.gameObject, gettableRewardData.rewardData.rewardId);
				uIGoods.Set(gettableRewardData.rewardData, gettableRewardData.countList);
			});
			probabilityList.InitGachaProbabilityList(probability);
		}
	}

	[Serializable]
	public class TypeBRewardList : UIInnerPartBase
	{
		public UIDefaultListView rewardList;

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			base.OnClick(sender, parent);
			string name = sender.name;
			if (name == "Close-PremiumGoodsList")
			{
				SoundManager.PlaySFX("EFM_SelectButton_002");
				UISetter.SetActive(root, active: false);
			}
		}

		public void Set(List<GettableRewardData> list, RoGacha gacha)
		{
			SoundManager.PlaySFX("EFM_OpenPopup_001");
			rewardList.CreateItem(list.Count, "Reward-", delegate(UIItemBase item, int index)
			{
				UIGoods uIGoods = item as UIGoods;
				GettableRewardData gettableRewardData = list[index];
				float probability = 0f;
				if (gacha.gachaRatingTypeB.ContainsKey(gettableRewardData.rewardData.rewardType))
				{
					Protocols.GachaRatingDataTypeB gachaRatingDataTypeB = gacha.gachaRatingTypeB[gettableRewardData.rewardData.rewardType];
					if (gachaRatingDataTypeB.list.ContainsKey(gettableRewardData.rewardData.rewardId))
					{
						Dictionary<int, float> dictionary = gachaRatingDataTypeB.list[gettableRewardData.rewardData.rewardId];
						if (dictionary.ContainsKey(gettableRewardData.countList[0]))
						{
							probability = dictionary[gettableRewardData.countList[0]];
						}
					}
				}
				UISetter.SetGameObjectName(uIGoods.gameObject, gettableRewardData.rewardData.rewardId);
				uIGoods.Set(gettableRewardData.rewardData, gettableRewardData.countList, probability);
			});
		}
	}

	[Serializable]
	public class BoxOpenSingle : UIInnerPartBase
	{
		public UIGachaBox box;

		public GameObject skipButton;

		public GameObject closeButton;

		public SkeletonAnimation premiumSpine;

		public SkeletonAnimation rainbowSpine;

		public Animation normalBoxAnimation;

		public Animation premiumBoxAnimation;

		public Animation rainbowBoxAnimation;

		private const string normalBoxOpenAnimaionName = "Gacha_Box_Ani_01";

		private const string premiumBoxOpenAnimaionName = "Gacha_Box_Ani_02";

		private const string rainbowBoxOpenAnimaionName = "Gacha_Box_Ani_03";

		private Animation curAnimation;

		private string curAnimationName;

		public BoxData currentBoxData { get; private set; }

		public Coroutine currAnimationRoutine { get; private set; }

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			base.OnClick(sender, parent);
			switch (sender.name)
			{
			case "Close-BoxOpenSingle":
				UISetter.SetActive(root, active: false);
				break;
			case "SingleSkip":
				Skip();
				break;
			case "SingleOpen":
				Open();
				break;
			}
		}

		public void Set(BoxData data)
		{
			currentBoxData = data;
			if (data.rewardType == ERewardType.Goods)
			{
				box.Set(data.gachaType, data.goodsData, data.rewardCount);
			}
			else if (data.rewardType == ERewardType.UnitMaterial)
			{
				box.Set(data.gachaType, data.partsData, data.rewardCount);
			}
			else if (data.rewardType == ERewardType.Costume)
			{
				box.Set(data.gachaType, data.costumeData);
			}
			else if (data.rewardType == ERewardType.Medal || data.rewardType == ERewardType.Commander)
			{
				box.Set(data);
			}
			else if (data.rewardType == ERewardType.Item)
			{
				box.Set(data.gachaType, data.equipItemData, data.rewardCount);
			}
			base.parentPanelBase.StopCoroutine(_PlayOpenAnimation());
			box.GetType(data.getType);
			box.openAnimation.Stop();
			UISetter.SetActive(normalBoxAnimation.gameObject, active: false);
			UISetter.SetActive(premiumBoxAnimation.gameObject, active: false);
			UISetter.SetActive(rainbowBoxAnimation.gameObject, active: false);
			EGachaAnimationType eGachaAnimationType = EGachaAnimationType.Normal;
			if (data.rewardType == ERewardType.Commander)
			{
				eGachaAnimationType = EGachaAnimationType.RainBow;
			}
			else if (data.rewardType == ERewardType.Medal || data.rewardType == ERewardType.Item || data.rewardType == ERewardType.Costume)
			{
				eGachaAnimationType = EGachaAnimationType.Premium;
			}
			switch (eGachaAnimationType)
			{
			case EGachaAnimationType.Normal:
				curAnimation = normalBoxAnimation;
				curAnimationName = "Gacha_Box_Ani_01";
				break;
			case EGachaAnimationType.Premium:
				curAnimation = premiumBoxAnimation;
				curAnimationName = "Gacha_Box_Ani_02";
				break;
			default:
				curAnimation = rainbowBoxAnimation;
				curAnimationName = "Gacha_Box_Ani_03";
				break;
			}
		}

		public void StartAnimation()
		{
			currAnimationRoutine = base.parentPanelBase.StartCoroutine(_PlayOpenAnimation());
		}

		public void StopAnimation()
		{
			if (currAnimationRoutine != null)
			{
				base.parentPanelBase.StopCoroutine(currAnimationRoutine);
				currAnimationRoutine = null;
			}
		}

		public void Skip()
		{
			if (curAnimation.IsPlaying(curAnimationName))
			{
				SoundManager.StopSFX();
				Utility.SampleAnimationEnd(curAnimation, curAnimationName);
				UISetter.SetActive(curAnimation.gameObject, active: false);
			}
		}

		public void Open()
		{
			box.BoxOpen();
			box.BroadcastMessage("MarkAsChanged", SendMessageOptions.DontRequireReceiver);
		}

		private IEnumerator _PlayOpenAnimation()
		{
			UISetter.SetActive(box, active: false);
			UISetter.SetActive(closeButton, active: false);
			UISetter.SetActive(curAnimation.gameObject, active: true);
			BoxData boxData = currentBoxData;
			if (boxData.rewardType == ERewardType.Commander)
			{
				SoundManager.PlaySFX("SE_PremiumGacha_001");
				rainbowSpine.state.SetAnimation(0, "gacha_02", loop: false);
			}
			else if (boxData.rewardType == ERewardType.Medal || boxData.rewardType == ERewardType.Item || boxData.rewardType == ERewardType.Costume)
			{
				SoundManager.PlaySFX("SE_PremiumGacha_001");
				premiumSpine.state.SetAnimation(0, "n_012_gacha", loop: false);
			}
			else
			{
				SoundManager.PlaySFX("SE_NormalGacha_001");
			}
			UISetter.SetActive(skipButton, active: true);
			curAnimation.Play(curAnimationName);
			while (curAnimation.IsPlaying(curAnimationName))
			{
				yield return null;
			}
			UISetter.SetActive(skipButton, active: false);
			UISetter.SetActive(curAnimation.gameObject, active: false);
			SoundManager.PlaySFX("SE_Gacha10_001");
			UISetter.SetActive(box, active: true);
			box.BroadcastMessage("MarkAsChanged", SendMessageOptions.DontRequireReceiver);
			while (box.background.isActiveAndEnabled)
			{
				yield return null;
			}
			UISetter.SetActive(closeButton, active: true);
		}
	}

	[Serializable]
	public class BoxOpenMultiple : UIInnerPartBase
	{
		public UIDefaultListView boxList;

		public GameObject skipButton;

		public GameObject openButton;

		public GameObject closeButton;

		public SkeletonAnimation premiumSpine;

		public SkeletonAnimation rainbowSpine;

		public Animation normalBoxAnimation;

		public Animation premiumBoxAnimation;

		public Animation rainbowBoxAnimation;

		private EGachaAnimationType gtype;

		private const string normalBoxOpenAnimaionName = "Gacha_Box_Ani_01";

		private const string premiumBoxOpenAnimaionName = "Gacha_Box_Ani_02";

		private const string rainbowBoxOpenAnimaionName = "Gacha_Box_Ani_03";

		private Animation curAnimation;

		private string curAnimationName;

		public List<BoxData> currentBoxDataList { get; private set; }

		public Coroutine currAnimationRoutine { get; private set; }

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			base.OnClick(sender, parent);
			UIGacha uIGacha = parent as UIGacha;
			switch (sender.name)
			{
			case "Close-BoxOpenMultiple":
				UISetter.SetActive(root, active: false);
				uIGacha.SetPilotProgress();
				break;
			case "MultipleSkip":
				Skip();
				break;
			case "MultipleOpen":
				Open();
				break;
			}
		}

		public void Set(List<BoxData> dataList)
		{
			int index = 0;
			currentBoxDataList = dataList;
			gtype = EGachaAnimationType.Normal;
			boxList.CollectExistItem("GachaBox_");
			boxList.itemList.ForEach(delegate(UIItemBase item)
			{
				UIGachaBox uIGachaBox = item as UIGachaBox;
				BoxData boxData = dataList[index];
				if (boxData.rewardType == ERewardType.Goods)
				{
					uIGachaBox.Set(boxData.gachaType, boxData.goodsData, boxData.rewardCount);
				}
				else if (boxData.rewardType == ERewardType.UnitMaterial)
				{
					uIGachaBox.Set(boxData.gachaType, boxData.partsData, boxData.rewardCount);
				}
				else if (boxData.rewardType == ERewardType.Costume)
				{
					uIGachaBox.Set(boxData.gachaType, boxData.costumeData);
				}
				else if (boxData.rewardType == ERewardType.Medal || boxData.rewardType == ERewardType.Commander)
				{
					uIGachaBox.Set(boxData);
				}
				else if (boxData.rewardType == ERewardType.Item)
				{
					uIGachaBox.Set(boxData.gachaType, boxData.equipItemData, boxData.rewardCount);
				}
				if (boxData.rewardType == ERewardType.Commander)
				{
					if (gtype < EGachaAnimationType.RainBow)
					{
						gtype = EGachaAnimationType.RainBow;
					}
				}
				else if ((boxData.rewardType == ERewardType.Medal || boxData.rewardType == ERewardType.Item || boxData.rewardType == ERewardType.Costume) && gtype < EGachaAnimationType.Premium)
				{
					gtype = EGachaAnimationType.Premium;
				}
				StopAnimation();
				uIGachaBox.GetType(boxData.getType);
				uIGachaBox.openAnimation.Stop();
				UISetter.SetActive(uIGachaBox, active: false);
				index++;
			});
			UISetter.SetActive(normalBoxAnimation.gameObject, active: false);
			UISetter.SetActive(premiumBoxAnimation.gameObject, active: false);
			UISetter.SetActive(rainbowBoxAnimation.gameObject, active: false);
			if (gtype == EGachaAnimationType.Normal)
			{
				curAnimation = normalBoxAnimation;
				curAnimationName = "Gacha_Box_Ani_01";
			}
			else if (gtype == EGachaAnimationType.Premium)
			{
				curAnimation = premiumBoxAnimation;
				curAnimationName = "Gacha_Box_Ani_02";
			}
			else
			{
				curAnimation = rainbowBoxAnimation;
				curAnimationName = "Gacha_Box_Ani_03";
			}
		}

		public void Skip()
		{
			Utility.SampleAnimationEnd(curAnimation, curAnimationName);
			StopAnimation();
			for (int i = 0; i < currentBoxDataList.Count; i++)
			{
				UIGachaBox uIGachaBox = boxList.itemList[i] as UIGachaBox;
				BoxData boxData = currentBoxDataList[i];
				UISetter.SetActive(uIGachaBox, active: true);
				Utility.SampleAnimationEnd(uIGachaBox.openAnimation, uIGachaBox.moveAnimationName);
			}
			UISetter.SetActive(openButton, active: true);
			UISetter.SetActive(skipButton, active: false);
			StartAnimation(base.parentPanelBase.StartCoroutine(_BoxOpenCheck()));
		}

		public void Open()
		{
			StopAnimation();
			UISetter.SetActive(openButton, active: false);
			StartAnimation(base.parentPanelBase.StartCoroutine(_PlayOpenBoxAnimation()));
		}

		public void StartAnimation()
		{
			currAnimationRoutine = base.parentPanelBase.StartCoroutine(_PlayOpenAnimation());
		}

		public void StartAnimation(Coroutine routine)
		{
			currAnimationRoutine = routine;
		}

		public void StopAnimation()
		{
			if (currAnimationRoutine != null)
			{
				base.parentPanelBase.StopCoroutine(currAnimationRoutine);
				currAnimationRoutine = null;
			}
		}

		private IEnumerator _PlayOpenAnimation()
		{
			UISetter.SetActive(closeButton, active: false);
			UISetter.SetActive(curAnimation.gameObject, active: true);
			if (gtype == EGachaAnimationType.Premium)
			{
				SoundManager.PlaySFX("SE_PremiumGacha_001");
				premiumSpine.state.SetAnimation(0, "n_012_gacha", loop: false);
			}
			else if (gtype == EGachaAnimationType.RainBow)
			{
				SoundManager.PlaySFX("SE_PremiumGacha_001");
				rainbowSpine.state.SetAnimation(0, "gacha_02", loop: false);
			}
			else
			{
				SoundManager.PlaySFX("SE_NormalGacha_001");
			}
			UISetter.SetActive(skipButton, active: true);
			curAnimation.Play(curAnimationName);
			while (curAnimation.IsPlaying(curAnimationName))
			{
				yield return null;
			}
			UISetter.SetActive(curAnimation.gameObject, active: false);
			for (int idx = 0; idx < currentBoxDataList.Count; idx++)
			{
				UIGachaBox box = boxList.itemList[idx] as UIGachaBox;
				BoxData boxData = currentBoxDataList[idx];
				UISetter.SetActive(box, active: true);
				SoundManager.PlaySFX("SE_Gacha10_001");
				box.openAnimation.Play(box.moveAnimationName);
				yield return null;
				box.BroadcastMessage("MarkAsChanged", SendMessageOptions.DontRequireReceiver);
				if (!isActive)
				{
					break;
				}
			}
			UISetter.SetActive(skipButton, active: false);
			UISetter.SetActive(openButton, active: true);
			StartAnimation(base.parentPanelBase.StartCoroutine(_BoxOpenCheck()));
		}

		private IEnumerator _BoxOpenCheck()
		{
			for (int idx = 0; idx < currentBoxDataList.Count; idx++)
			{
				UIGachaBox box = boxList.itemList[idx] as UIGachaBox;
				while (box.background.isActiveAndEnabled)
				{
					yield return null;
				}
				if (!isActive)
				{
					break;
				}
			}
			UISetter.SetActive(openButton, active: false);
			UISetter.SetActive(closeButton, active: true);
		}

		private IEnumerator _PlayOpenBoxAnimation()
		{
			for (int idx = 0; idx < currentBoxDataList.Count; idx++)
			{
				UIGachaBox box = boxList.itemList[idx] as UIGachaBox;
				if (box.background.isActiveAndEnabled)
				{
					yield return new WaitForSeconds(0.2f);
				}
				box.BoxOpen();
				box.BroadcastMessage("MarkAsChanged", SendMessageOptions.DontRequireReceiver);
				while (box.background.isActiveAndEnabled)
				{
					yield return null;
				}
				if (!isActive)
				{
					break;
				}
			}
			UISetter.SetActive(openButton, active: false);
			UISetter.SetActive(closeButton, active: true);
		}
	}

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimNormal;

	public GEAnimNGUI AnimPreminum;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UITimer premiumRemainTime;

	public UILabel premiumOnceCash;

	public UILabel premiumTenTimeCash;

	public UILabel premiumOnceCashLabel;

	public UILabel premiumTenTimeCashLabel;

	public UILabel premiumOnceCashFreeLabel;

	public UISprite premiumOnceCashIcon;

	public UISprite premiumTenTimeCashIcon;

	public GameObject premiumOnceButton;

	public GameObject premiumTenTimeButton;

	public GameObject premiumFreeOnceButton;

	public GameObject gachaRewardBtn;

	public UILabel gachaDescription;

	public TypeARewardList typeATargetList;

	public TypeBRewardList typeBTargetList;

	private GameObject targetList;

	public BoxOpenSingle singleBox;

	public BoxOpenMultiple multipleBox;

	public UIDefaultListView bannerListView;

	public UISpineAnimation spineAnimation;

	public UIProgressBar pilotProgress;

	public UILabel pilotProgressLabel;

	public GameObject goBG;

	public GameObject effectRoot;

	public UILabel gachaName;

	public UISprite bigBanner;

	public GameObject gachaTimeRoot;

	private GameObject infoPopUp;

	private string selectType;

	private void Start()
	{
		UISetter.SetSpine(spineAnimation, "n_012");
	}

	public UICommanderComplete CreateCommanderComplete(CommanderCompleteType _type, string _idx)
	{
		UICommanderComplete uICommanderComplete = UIPopup.Create<UICommanderComplete>("CommanderComplete");
		uICommanderComplete.Init(_type, _idx);
		return uICommanderComplete;
	}

	private void OnDestroy()
	{
		if (goBG != null)
		{
			goBG = null;
		}
		if (AnimBlock != null)
		{
			AnimBlock = null;
		}
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Close" && !bBackKeyEnable)
		{
			Close();
		}
		if (text == "OpenPremiumRewardList")
		{
			if (targetList == null || !targetList.activeSelf)
			{
				_OpenRewardList(selectType);
			}
			else
			{
				_CloseRewardList();
			}
		}
		else if (text == "InfoBtn")
		{
			if (infoPopUp == null)
			{
				UISimplePopup uISimplePopup = UISimplePopup.CreateOK("InformationPopup");
				uISimplePopup.Set(localization: true, "1911", "13028", string.Empty, "1001", string.Empty, string.Empty);
				infoPopUp = uISimplePopup.gameObject;
			}
		}
		else if (text.StartsWith("Banner-"))
		{
			selectType = text.Substring(text.IndexOf("-") + 1);
			bannerListView.SetSelection(selectType, selected: true);
			_Set();
		}
		else if (text.StartsWith("PremiumFree-"))
		{
			_RequestGachaOpenBox(selectType, _FindCount(text), free: true);
		}
		else if (text.StartsWith("Premium-"))
		{
			_RequestGachaOpenBox(selectType, _FindCount(text), free: false);
		}
		else
		{
			switch (text)
			{
			case "Close-BoxOpenSingle":
			case "Close-BoxOpenMultiple":
				AnimNormal.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
				AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
				AnimPreminum.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
				break;
			case "GachaBox":
				singleBox.Open();
				break;
			}
		}
		SendOnClickToInnerParts(sender);
	}

	private void _RequestGachaOpenBox(string type, int count, bool free, bool confirm = false)
	{
		Dictionary<string, RoGacha> gacha = base.localUser.gacha;
		GachaCostDataRow gachaCostDataRow = base.regulation.FindGachaCost(type, count);
		if (gachaCostDataRow != null && !free)
		{
			if (gachaCostDataRow.priceType == EGoods.FreeGold)
			{
				if (!confirm && count == 10)
				{
					UISimplePopup.CreateBool(localization: false, Localization.Get("1303"), Localization.Format("13024", gachaCostDataRow.cost), null, Localization.Get("1304"), Localization.Get("1305")).onClick = delegate(GameObject sender)
					{
						string text4 = sender.name;
						if (text4 == "OK")
						{
							_RequestGachaOpenBox(type, count, free, confirm: true);
						}
					};
					return;
				}
				if (gachaCostDataRow.cost > base.localUser.gold)
				{
					UISimplePopup.CreateBool(localization: true, "1029", "1030", null, "1304", "1305").onClick = delegate(GameObject sender)
					{
						string text3 = sender.name;
						if (text3 == "OK")
						{
							base.uiWorld.camp.GoNavigation("MetroBank");
						}
					};
					return;
				}
			}
			else if (gachaCostDataRow.priceType == EGoods.FreeCash)
			{
				if (!confirm && count == 10)
				{
					UISimplePopup.CreateBool(localization: false, Localization.Get("1303"), Localization.Format("13023", gachaCostDataRow.cost), null, Localization.Get("1304"), Localization.Get("1305")).onClick = delegate(GameObject sender)
					{
						string text2 = sender.name;
						if (text2 == "OK")
						{
							_RequestGachaOpenBox(type, count, free, confirm: true);
						}
					};
					return;
				}
				if (gachaCostDataRow.cost > base.localUser.cash)
				{
					UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "1304", "1305").onClick = delegate(GameObject sender)
					{
						string text = sender.name;
						if (text == "OK")
						{
							UIManager.instance.world.mainCommand.OpenDiamonShop();
						}
					};
					return;
				}
			}
			else
			{
				GoodsDataRow goods = base.regulation.FindGoodsServerFieldName(((int)gachaCostDataRow.priceType).ToString());
				ItemExchangeDataRow item = base.regulation.itemExchangeDtbl.Find((ItemExchangeDataRow row) => row.type == EStorageType.Goods && row.typeidx == goods.type);
				int num = base.localUser.resourceList[goods.serverFieldName];
				if (gachaCostDataRow.cost > num)
				{
					UINavigationPopUp uINavigationPopUp = UIPopup.Create<UINavigationPopUp>("NavigationPopUp");
					uINavigationPopUp.Init(EStorageType.Goods, goods.type, item);
					uINavigationPopUp.title.text = Localization.Get("5608");
					return;
				}
			}
		}
		base.network.RequestGachaOpenBox(type, count);
	}

	private int _FindCount(string key)
	{
		int num = key.LastIndexOf("-");
		if (num < 0)
		{
			return 1;
		}
		int result = 1;
		if (!int.TryParse(key.Substring(num + 1), out result))
		{
			return 1;
		}
		return result;
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		_Set();
		SendOnRefreshToInnerParts();
	}

	public void InitAndOpenGacha()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			SetBannerList();
			_Set();
			UISetter.SetActive(typeATargetList.root, active: false);
			UISetter.SetActive(typeBTargetList.root, active: false);
			UISetter.SetActive(singleBox.root, active: false);
			UISetter.SetActive(multipleBox.root, active: false);
			OpenPopupShow();
			SendOnInitToInnerParts();
		}
	}

	public void RefreshGacha()
	{
		_Set();
	}

	private void RefreshBannerList()
	{
		if (bannerListView.itemList.Count > 0)
		{
			bannerListView.itemList.ForEach(delegate(UIItemBase row)
			{
				UIGachaBannerListItem uIGachaBannerListItem = row as UIGachaBannerListItem;
				uIGachaBannerListItem.OnRefresh();
			});
		}
	}

	private void _SetCost(UISprite costIcon, UILabel label, string gachaType, int count)
	{
		GachaCostDataRow gachaCostDataRow = base.regulation.FindGachaCost(gachaType, count);
		GoodsDataRow goodsDataRow = base.regulation.goodsDtbl[((int)gachaCostDataRow.priceType).ToString()];
		if (gachaCostDataRow != null)
		{
			UISetter.SetSprite(costIcon, goodsDataRow.iconId);
			if (count == 1)
			{
				UISetter.SetLabel(premiumOnceCashLabel, Localization.Format("13004", count));
			}
			else
			{
				UISetter.SetLabel(premiumTenTimeCashLabel, Localization.Format("13004", count));
			}
			UISetter.SetLabel(label, gachaCostDataRow.cost);
		}
	}

	private void _Set()
	{
		RoGacha gacha = base.localUser.gacha[selectType];
		GachaDataRow gachaDataRow = base.regulation.gachaDtbl[selectType];
		GachaCostDataRow gachaCostDataRow = base.regulation.FindGachaCost(gachaDataRow.type, 1);
		UISetter.SetActive(premiumFreeOnceButton, gacha.canOpenFreeBox);
		UISetter.SetActive(premiumOnceButton, !gacha.canOpenFreeBox);
		UISetter.SetSprite(bigBanner, Localization.Get(gachaDataRow.banner));
		UISetter.SetLabel(gachaName, Localization.Get(gachaDataRow.name));
		UISetter.SetActive(gachaDescription, gachaDataRow.eventComment != "0");
		UISetter.SetLabel(gachaDescription, Localization.Get(gachaDataRow.eventComment));
		UISetter.SetActive(gachaTimeRoot, gachaDataRow.count != 0);
		UISetter.SetActive(pilotProgress, gachaDataRow.bonusCount != 0 || gachaDataRow.type == "2");
		bigBanner.height = ((!(gachaDataRow.type == "1") && !(gachaDataRow.type == "2")) ? 320 : 277);
		gachaDescription.transform.localPosition = ((!(gachaDataRow.type == "1") && !(gachaDataRow.type == "2")) ? new Vector3(gachaDescription.transform.localPosition.x, -110f, 0f) : new Vector3(gachaDescription.transform.localPosition.x, -70f, 0f));
		gachaRewardBtn.transform.localPosition = ((!(gachaDataRow.type == "1") && !(gachaDataRow.type == "2")) ? new Vector3(gachaRewardBtn.transform.localPosition.x, -72f, 0f) : new Vector3(gachaRewardBtn.transform.localPosition.x, -30f, 0f));
		_SetCost(premiumOnceCashIcon, premiumOnceCash, gachaDataRow.type, 1);
		_SetCost(premiumTenTimeCashIcon, premiumTenTimeCash, gachaDataRow.type, 10);
		if (gachaDataRow.count > 1)
		{
			UISetter.SetLabel(premiumOnceCashFreeLabel, string.Format("{0}\n{1}/{2}", Localization.Get("13007"), gacha.remainCount, gachaDataRow.count));
		}
		else
		{
			UISetter.SetLabel(premiumOnceCashFreeLabel, Localization.Get("13007"));
		}
		if (premiumRemainTime != null)
		{
			premiumRemainTime.SetFinishString(Localization.Get("6017"));
			if (gacha.remainCount <= 0)
			{
				premiumRemainTime.SetFinishString(Localization.Get("6015"));
			}
			else
			{
				premiumRemainTime.SetFinishString(Localization.Get("6016"));
			}
			premiumRemainTime.RegisterOnFinished(delegate
			{
				UISetter.SetActive(premiumFreeOnceButton, gacha.canOpenFreeBox);
				UISetter.SetActive(premiumOnceButton, !gacha.canOpenFreeBox);
			});
			premiumRemainTime.SetLabelFormat(string.Format("{0} ", Localization.Get("13005")), string.Empty);
			premiumRemainTime.Set(gacha.freeOpenTime);
		}
		base.uiWorld.mainCommand.SetResourceView(((int)gachaCostDataRow.priceType).ToString());
		RefreshBannerList();
		SetPilotProgress();
	}

	private void SetBannerList()
	{
		List<GachaDataRow> list = new List<GachaDataRow>();
		foreach (string key in base.localUser.gacha.Keys)
		{
			list.Add(base.regulation.gachaDtbl[key]);
		}
		list.Sort((GachaDataRow a, GachaDataRow b) => a.sort.CompareTo(b.sort));
		selectType = list[0].type;
		bannerListView.InitGachaBannerList(list, "Banner-");
		bannerListView.SetSelection(selectType, selected: true);
	}

	private void SetPilotProgress()
	{
		RoGacha roGacha = base.localUser.gacha[selectType];
		GachaDataRow gachaDataRow = base.regulation.gachaDtbl[selectType];
		int num = 0;
		num = ((!(gachaDataRow.type == "2")) ? (gachaDataRow.bonusCount - 1) : (int.Parse(base.regulation.defineDtbl["TREASURE_PILOT_RATE"].value) - 1));
		UISetter.SetProgress(pilotProgress, (float)roGacha.pilotRate / (float)num);
		if (roGacha.pilotRate >= num)
		{
			UISetter.SetLabel(pilotProgressLabel, Localization.Format("13027", Localization.Get(gachaDataRow.mainReward)));
			UISetter.SetActive(effectRoot, active: true);
		}
		else
		{
			UISetter.SetLabel(pilotProgressLabel, Localization.Format("13026", roGacha.pilotRate, num, Localization.Get(gachaDataRow.mainReward)));
			UISetter.SetActive(effectRoot, active: false);
		}
	}

	private void _CloseRewardList()
	{
		UISetter.SetActive(targetList, active: false);
	}

	private void _OpenRewardList(string type)
	{
		GachaDataRow gachaDataRow = base.regulation.gachaDtbl[type];
		List<GettableRewardData> list = _CreateGettableRewardList(type);
		if (gachaDataRow.ui == 0)
		{
			targetList = typeATargetList.root;
			Dictionary<ERewardType, float> dictionary = new Dictionary<ERewardType, float>();
			foreach (KeyValuePair<ERewardType, Protocols.GachaRatingDataTypeA> item in base.localUser.gacha[type].gachaRatingTypeA)
			{
				dictionary.Add(item.Key, item.Value.rating);
			}
			typeATargetList.Set(list, dictionary);
			UISetter.SetActive(targetList, active: true);
			typeATargetList.rewardList.ResetPosition();
		}
		else
		{
			targetList = typeBTargetList.root;
			typeBTargetList.Set(list, base.localUser.gacha[type]);
			UISetter.SetActive(targetList, active: true);
			typeBTargetList.rewardList.ResetPosition();
		}
	}

	private List<GettableRewardData> _CreateGettableRewardList(string type)
	{
		GachaDataRow gachaDataRow = base.regulation.gachaDtbl[type];
		List<GachaRewardDataRow> list = base.regulation.FindGachaRewardList(type);
		List<GachaRewardDataRow> list2 = list.FindAll((GachaRewardDataRow row) => row.rewardType == ERewardType.Goods);
		Dictionary<ERewardType, Dictionary<string, List<int>>> cntDict = new Dictionary<ERewardType, Dictionary<string, List<int>>>();
		Dictionary<ERewardType, HashSet<string>> createdRewardSet = new Dictionary<ERewardType, HashSet<string>>();
		List<GettableRewardData> uniqueList = new List<GettableRewardData>();
		list.ForEach(delegate(GachaRewardDataRow row)
		{
			if (!cntDict.ContainsKey(row.rewardType))
			{
				cntDict.Add(row.rewardType, new Dictionary<string, List<int>>());
			}
			if (!cntDict[row.rewardType].ContainsKey(row.rewardId))
			{
				cntDict[row.rewardType].Add(row.rewardId, new List<int>());
			}
			cntDict[row.rewardType][row.rewardId].Add(row.rewardCount);
		});
		if (gachaDataRow.ui == 0)
		{
			list.ForEach(delegate(GachaRewardDataRow row)
			{
				if (!createdRewardSet.ContainsKey(row.rewardType) || !createdRewardSet[row.rewardType].Contains(row.rewardId))
				{
					if (!createdRewardSet.ContainsKey(row.rewardType))
					{
						createdRewardSet.Add(row.rewardType, new HashSet<string>());
					}
					createdRewardSet[row.rewardType].Add(row.rewardId);
					uniqueList.Add(new GettableRewardData
					{
						rewardData = row,
						countList = cntDict[row.rewardType][row.rewardId]
					});
				}
			});
		}
		else
		{
			list.ForEach(delegate(GachaRewardDataRow row)
			{
				for (int i = 0; i < cntDict[row.rewardType][row.rewardId].Count; i++)
				{
					int item = cntDict[row.rewardType][row.rewardId][i];
					List<int> countList = new List<int> { item };
					uniqueList.Add(new GettableRewardData
					{
						rewardData = row,
						countList = countList
					});
				}
			});
		}
		return uniqueList;
	}

	public void OpenBox(List<BoxData> list)
	{
		if (list != null)
		{
			AnimNormal.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
			AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
			AnimPreminum.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
			if (list.Count == 1)
			{
				singleBox.Set(list[0]);
				UISetter.SetActive(singleBox.root, active: true);
				singleBox.StartAnimation();
			}
			else
			{
				multipleBox.Set(list);
				UISetter.SetActive(multipleBox.root, active: true);
				multipleBox.StartAnimation();
			}
		}
	}

	public void GachaSelectCashType()
	{
		selectType = "2";
		bannerListView.SetSelection(selectType, selected: true);
		_Set();
	}

	public void GachaSelectGoldType()
	{
		selectType = "1";
		bannerListView.SetSelection(selectType, selected: true);
		_Set();
	}

	public void GoNavigationGacha(string _selectType)
	{
		selectType = _selectType;
		bannerListView.SetSelection(selectType, selected: true);
		_Set();
	}

	private void OpenPopupShow()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Open()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Close()
	{
		bBackKeyEnable = true;
		base.uiWorld.mainCommand.SetResourceView(EGoods.Bullet);
		HidePopup();
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0f);
		OnAnimOpen();
	}

	private void HidePopup()
	{
		OnAnimClose();
		StartCoroutine(OnEventHidePopup());
	}

	private void OnAnimOpen()
	{
		AnimNormal.Reset();
		AnimNpc.Reset();
		AnimPreminum.Reset();
		AnimTitle.Reset();
		AnimBlock.Reset();
		UIManager.instance.SetPopupPositionY(base.gameObject);
		AnimNormal.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimPreminum.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimNormal.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimPreminum.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	public void AnimOpenFinish()
	{
		UIManager.instance.EnableCameraTouchEvent(isEnable: true);
	}

	protected override void OnEnablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: true);
	}

	protected override void OnDisablePopup()
	{
		UISetter.SetVoice(spineAnimation, active: false);
	}
}
