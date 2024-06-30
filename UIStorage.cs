using System;
using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIStorage : UIPopup
{
	[Serializable]
	public class UISellContents : UIInnerPartBase
	{
		public UIStorageListItem storageItem;

		public UILabel itemCountLabel;

		public UILabel priceLabel;

		public GameObject decreaseBtn;

		public GameObject addBtn;

		public UILabel title;

		public UILabel inputLabel;

		public UIInput input;

		private int sellCount;

		private string itemId;

		private int itemCount;

		private int price;

		private UIPanelBase parentPanel;

		private bool isPress;

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
			parentPanel = parent;
		}

		public void Set(RoPart _part)
		{
			Regulation regulation = RemoteObjectManager.instance.regulation;
			UISetter.SetLabel(title, Localization.Get("1312"));
			ItemExchangeDataRow itemExchangeDataRow = regulation.FindExchangeItemData(EStorageType.Part, _part.id);
			price = itemExchangeDataRow.price;
			itemId = _part.id;
			itemCount = _part.count;
			sellCount = 1;
			isPress = false;
			storageItem.Set(_part);
			SetValue();
		}

		public void Set(string key, int count, EStorageType type)
		{
			Regulation regulation = RemoteObjectManager.instance.regulation;
			if (type != EStorageType.Goods && type != EStorageType.Food && type != EStorageType.Box)
			{
				UISetter.SetLabel(title, Localization.Get("1312"));
				itemId = key;
			}
			else
			{
				GoodsDataRow goodsDataRow = base.regulation.FindGoodsByServerFieldName(key);
				UISetter.SetLabel(title, (type != EStorageType.Box) ? Localization.Get("1312") : Localization.Get("13011"));
				itemId = int.Parse(goodsDataRow.type).ToString();
			}
			ItemExchangeDataRow itemExchangeDataRow = regulation.FindExchangeItemData(type, itemId);
			price = itemExchangeDataRow.price;
			itemCount = count;
			sellCount = 1;
			isPress = false;
			storageItem.Set(key, count, type);
			SetValue();
		}

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			UIStorage uiParent = parent as UIStorage;
			switch (sender.name)
			{
			case "SellCancleBtn":
				UISetter.SetActive(root, active: false);
				break;
			case "SellOkBtn":
				SoundManager.PlaySFX("SE_Sale_001");
				CreateSellConfirmPopup(uiParent);
				break;
			case "OpenOkBtn":
				SoundManager.PlaySFX("SE_Sale_001");
				base.network.RequestOpenItem(EStorageType.Box, itemId, sellCount);
				UISetter.SetActive(root, active: false);
				break;
			}
		}

		private void CreateSellConfirmPopup(UIStorage uiParent)
		{
			string key = string.Empty;
			if (uiParent.selectType == EStorageType.Goods || uiParent.selectType == EStorageType.EventItem || uiParent.selectType == EStorageType.CollectionItem || uiParent.selectType == EStorageType.Box || uiParent.selectType == EStorageType.Item || uiParent.selectType == EStorageType.Food)
			{
				GoodsDataRow goodsDataRow = base.regulation.goodsDtbl[itemId];
				key = goodsDataRow.name;
			}
			else if (uiParent.selectType == EStorageType.Medal || uiParent.selectType == EStorageType.Commander)
			{
				CommanderDataRow commanderDataRow = base.regulation.commanderDtbl[itemId];
				key = commanderDataRow.nickname;
			}
			else if (uiParent.selectType == EStorageType.Part)
			{
				PartDataRow partDataRow = base.regulation.partDtbl[itemId];
				key = partDataRow.name;
			}
			else if (uiParent.selectType != EStorageType.Costume)
			{
			}
			UISimplePopup uISimplePopup = UISimplePopup.CreateBool(localization: false, Localization.Get("1303"), Localization.Format("20071", Localization.Get(key)), string.Empty, Localization.Get("1304"), Localization.Get("1305"));
			uISimplePopup.onClick = delegate(GameObject sender)
			{
				string name = sender.name;
				if (name == "OK")
				{
					base.network.RequestSellItem(uiParent.selectType, itemId, sellCount);
					UISetter.SetActive(root, active: false);
				}
			};
		}

		private void SetValue()
		{
			input.value = string.Empty;
			UISetter.SetLabel(inputLabel, sellCount);
			UISetter.SetLabel(priceLabel, sellCount * price);
			UISetter.SetButtonEnable(decreaseBtn, sellCount > 1);
			UISetter.SetButtonEnable(addBtn, sellCount < itemCount);
		}

		public void DecreaseItemStart()
		{
			parentPanel.StartCoroutine(ItemCalculation(-1));
		}

		public void DecreaseItemEnd()
		{
			isPress = false;
			if (ItemCheck(-1))
			{
				sellCount--;
				SetValue();
			}
		}

		public void AddItemStart()
		{
			parentPanel.StartCoroutine(ItemCalculation(1));
		}

		public void AddItemEnd()
		{
			isPress = false;
			if (ItemCheck(1))
			{
				sellCount++;
				SetValue();
			}
		}

		public void ItemCountMax()
		{
			sellCount = itemCount;
			SetValue();
		}

		private bool ItemCheck(int value)
		{
			if (value > 0)
			{
				if (sellCount < itemCount)
				{
					return true;
				}
			}
			else if (sellCount > 1)
			{
				return true;
			}
			return false;
		}

		private IEnumerator ItemCalculation(int value)
		{
			float speed = 0.05f;
			isPress = true;
			yield return new WaitForSeconds(1f);
			while (ItemCheck(value) && isPress)
			{
				sellCount += value;
				SetValue();
				yield return new WaitForSeconds(speed);
			}
			yield return true;
		}

		public void SetInputValue()
		{
			if (string.IsNullOrEmpty(input.value) || int.Parse(input.value) == 0)
			{
				sellCount = 1;
			}
			else if (int.Parse(input.value) > itemCount)
			{
				sellCount = itemCount;
			}
			else
			{
				sellCount = int.Parse(input.value);
			}
			SetValue();
		}

		public void SetLimitLength(int limit)
		{
			input.characterLimit = limit;
		}
	}

	[Serializable]
	public class UISelectContents : UIInnerPartBase
	{
		public UIDefaultListView rewardListView;

		public UILabel title;

		public UILabel itemName;

		public UILabel itemDesc;

		public GameObject selectBtn;

		private string selectId = string.Empty;

		private string itemId = string.Empty;

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
		}

		public void Set(string s_name)
		{
			selectId = string.Empty;
			GoodsDataRow boxData = base.regulation.FindGoodsByServerFieldName(s_name);
			itemId = boxData.type;
			List<RandomBoxRewardDataRow> list = base.regulation.randomBoxRewardDtbl.FindAll((RandomBoxRewardDataRow row) => row.idx == boxData.type);
			rewardListView.InitBoxRewardList(list, "BoxReward-");
			rewardListView.ResetPosition();
			UISetter.SetLabel(title, Localization.Get(boxData.name));
			SelectItem(selectId);
		}

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			string name = sender.name;
			if (name == "SelectCancleBtn")
			{
				UISetter.SetActive(root, active: false);
			}
			else if (rewardListView.Contains(name))
			{
				string pureId = rewardListView.GetPureId(name);
				SelectItem(pureId);
			}
			else if (name == "SelectOpenBtn")
			{
				SoundManager.PlaySFX("SE_Sale_001");
				string[] array = selectId.Split('_');
				base.network.RequestOpenItem(EStorageType.Box, itemId, 1, int.Parse(array[0]), int.Parse(array[1]));
				UISetter.SetActive(root, active: false);
			}
		}

		private void SelectItem(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				UISetter.SetActive(itemName, active: false);
				UISetter.SetActive(itemDesc, active: false);
				UISetter.SetButtonEnable(selectBtn, enable: false);
				return;
			}
			UISetter.SetButtonEnable(selectBtn, enable: true);
			UISetter.SetActive(itemName, active: true);
			UISetter.SetActive(itemDesc, active: true);
			selectId = key;
			rewardListView.SetSelection(key, selected: true);
			string[] array = key.Split('_');
			ERewardType eRewardType = (ERewardType)int.Parse(array[0]);
			string rewardIdx = array[1];
			switch (eRewardType)
			{
			case ERewardType.Goods:
			case ERewardType.Box:
			case ERewardType.Favor:
			case ERewardType.EventItem:
			case ERewardType.CollectionItem:
			{
				GoodsDataRow goodsDataRow = base.regulation.goodsDtbl[rewardIdx.ToString()];
				UISetter.SetLabel(itemName, Localization.Get(goodsDataRow.name));
				UISetter.SetLabel(itemDesc, Localization.Get(goodsDataRow.description));
				break;
			}
			case ERewardType.Medal:
			{
				CommanderDataRow commanderDataRow2 = base.regulation.commanderDtbl[rewardIdx.ToString()];
				UISetter.SetLabel(itemName, commanderDataRow2.nickname);
				UISetter.SetLabel(itemDesc, Localization.Get(commanderDataRow2.explanation));
				break;
			}
			case ERewardType.Commander:
			{
				CommanderDataRow commanderDataRow = base.regulation.commanderDtbl[rewardIdx.ToString()];
				UISetter.SetLabel(itemName, commanderDataRow.nickname);
				UISetter.SetLabel(itemDesc, Localization.Get(commanderDataRow.explanation));
				break;
			}
			case ERewardType.UnitMaterial:
			{
				PartDataRow partDataRow = base.regulation.partDtbl[rewardIdx.ToString()];
				UISetter.SetLabel(itemName, Localization.Get(partDataRow.name));
				UISetter.SetLabel(itemDesc, Localization.Get(partDataRow.description));
				break;
			}
			case ERewardType.Costume:
			{
				CommanderCostumeDataRow commanderCostumeDataRow = base.regulation.commanderCostumeDtbl[rewardIdx.ToString()];
				UISetter.SetLabel(itemName, Localization.Get(commanderCostumeDataRow.name));
				UISetter.SetLabel(itemDesc, Localization.Get(commanderCostumeDataRow.Desc));
				break;
			}
			case ERewardType.Item:
			{
				EquipItemDataRow equipItemDataRow = base.regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == rewardIdx.ToString());
				if (equipItemDataRow != null)
				{
					UISetter.SetLabel(itemName, Localization.Get(equipItemDataRow.equipItemName));
					UISetter.SetLabel(itemDesc, Localization.Get(equipItemDataRow.equipItemSubText));
				}
				break;
			}
			}
		}
	}

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIFlipSwitch partTab;

	public UIFlipSwitch medalTab;

	public UIFlipSwitch foodTab;

	public UIFlipSwitch goodsTab;

	public UIFlipSwitch itemsTab;

	public GameObject bottomRoot;

	public UILabel empty;

	public UIStorageListItem storageItem;

	public UISpineAnimation spineAnimation;

	public UIDefaultListView itemListView;

	public UISellContents sellContents;

	public UISelectContents selectContents;

	private string selectItemKey;

	public EStorageType selectType;

	public static readonly string partIdPrefix = "PartItem-";

	public static readonly string medalIdPrefix = "MedalItem-";

	public static readonly string goodsIdPrefix = "GoodsItem-";

	public static readonly string foodIdPrefix = "FoodItem-";

	public static readonly string itemIdPrefix = "InvenItem-";

	private List<RoItem> items;

	public GameObject goBG;

	private void Start()
	{
		UISetter.SetSpine(spineAnimation, "n_003");
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

	public void InitAndOpenStorage()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			selectItemKey = string.Empty;
			UISetter.SetActive(sellContents, active: false);
			UISetter.SetFlipSwitch(partTab, state: true);
			UISetter.SetFlipSwitch(medalTab, state: false);
			UISetter.SetFlipSwitch(foodTab, state: false);
			UISetter.SetFlipSwitch(goodsTab, state: false);
			UISetter.SetFlipSwitch(itemsTab, state: false);
			OpenPopupShow();
			InitStoragePartList();
			SendOnInitToInnerParts();
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		if (selectType == EStorageType.Part)
		{
			InitStoragePartList();
		}
		else if (selectType == EStorageType.Medal)
		{
			InitStorageMedalList();
		}
		else if (selectType == EStorageType.Food)
		{
			InitStorageGoodsOrFoodList(selectType);
		}
		else if (selectType == EStorageType.Goods)
		{
			InitStorageGoodsOrFoodList(selectType);
		}
		else if (selectType == EStorageType.Item)
		{
			InitStorageItemList();
		}
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Close")
		{
			if (!bBackKeyEnable)
			{
				Close();
			}
			return;
		}
		if (itemListView.Contains(text))
		{
			string pureId = itemListView.GetPureId(text);
			selectItemKey = text;
			itemListView.SetSelection(pureId, selected: true);
			if (text.StartsWith(partIdPrefix))
			{
				RoPart partData = base.localUser.FindPart(pureId);
				storageItem.Set(partData);
			}
			else if (text.StartsWith(medalIdPrefix))
			{
				Dictionary<string, int> dictionary = base.localUser.FindSrorageCommanderMedalList();
				storageItem.Set(pureId, dictionary[pureId], EStorageType.Medal);
			}
			else if (text.StartsWith(goodsIdPrefix))
			{
				Dictionary<string, int> resourceList = base.localUser.resourceList;
				storageItem.Set(pureId, resourceList[pureId], EStorageType.Goods);
			}
			else if (text.StartsWith(foodIdPrefix))
			{
				Dictionary<string, int> resourceList2 = base.localUser.resourceList;
				storageItem.Set(pureId, resourceList2[pureId], EStorageType.Food);
			}
			else
			{
				if (!text.StartsWith(itemIdPrefix))
				{
					return;
				}
				string text2 = text.Substring(itemIdPrefix.Length);
				string[] strArr = text2.Split('-');
				int num = 0;
				if (items != null)
				{
					RoItem roItem = items.Find((RoItem row) => row.id == strArr[0] && row.level == int.Parse(strArr[1]));
					if (roItem != null)
					{
						num = roItem.level;
						storageItem.SetItemDetailDesc(roItem);
						storageItem.Set(roItem.id, roItem.itemCount, EStorageType.Item, num);
					}
				}
			}
			return;
		}
		switch (text)
		{
		case "Part":
			selectItemKey = string.Empty;
			InitStoragePartList();
			itemListView.scrollView.ResetPosition();
			break;
		case "Medal":
			selectItemKey = string.Empty;
			InitStorageMedalList();
			itemListView.scrollView.ResetPosition();
			break;
		case "Food":
			selectItemKey = string.Empty;
			InitStorageGoodsOrFoodList(EStorageType.Food);
			itemListView.scrollView.ResetPosition();
			break;
		case "Goods":
			selectItemKey = string.Empty;
			InitStorageGoodsOrFoodList(EStorageType.Goods);
			itemListView.scrollView.ResetPosition();
			break;
		case "Items":
			selectItemKey = string.Empty;
			InitStorageItemList();
			itemListView.scrollView.ResetPosition();
			break;
		case "SellContents":
			UISetter.SetActive(sellContents, active: true);
			if (selectItemKey.StartsWith(partIdPrefix))
			{
				string pureId4 = itemListView.GetPureId(selectItemKey);
				RoPart part = base.localUser.FindPart(pureId4);
				sellContents.Set(part);
			}
			else if (selectItemKey.StartsWith(medalIdPrefix))
			{
				string pureId5 = itemListView.GetPureId(selectItemKey);
				Dictionary<string, int> dictionary2 = base.localUser.FindSrorageCommanderMedalList();
				sellContents.Set(pureId5, dictionary2[pureId5], EStorageType.Medal);
			}
			else if (selectItemKey.StartsWith(goodsIdPrefix))
			{
				string pureId6 = itemListView.GetPureId(selectItemKey);
				Dictionary<string, int> resourceList4 = base.localUser.resourceList;
				sellContents.Set(pureId6, resourceList4[pureId6], EStorageType.Goods);
			}
			else if (selectItemKey.StartsWith(foodIdPrefix))
			{
				string pureId7 = itemListView.GetPureId(selectItemKey);
				Dictionary<string, int> resourceList5 = base.localUser.resourceList;
				sellContents.Set(pureId7, resourceList5[pureId7], EStorageType.Food);
			}
			break;
		case "OpenContents":
		{
			UISetter.SetActive(sellContents, active: true);
			string pureId3 = itemListView.GetPureId(selectItemKey);
			Dictionary<string, int> resourceList3 = base.localUser.resourceList;
			sellContents.Set(pureId3, resourceList3[pureId3], EStorageType.Box);
			break;
		}
		case "SelectContents":
		{
			UISetter.SetActive(selectContents, active: true);
			string pureId2 = itemListView.GetPureId(selectItemKey);
			selectContents.Set(pureId2);
			break;
		}
		default:
			SendOnClickToInnerParts(sender);
			break;
		}
	}

	private void InitStoragePartList()
	{
		selectType = EStorageType.Part;
		List<RoPart> list = base.localUser.FindSroragePartList();
		itemListView.InitStoragePartList(list, partIdPrefix);
		UISetter.SetActive(empty, list.Count <= 0);
		UISetter.SetActive(bottomRoot, list.Count > 0);
		if (list.Count == 0)
		{
			UISetter.SetLabel(empty, Localization.Format("1317", Localization.Get("4314")));
			return;
		}
		if (string.IsNullOrEmpty(selectItemKey))
		{
			itemListView.SetSelection(list[0].id, selected: true);
			storageItem.Set(list[0]);
			selectItemKey = itemListView.itemList[0].name;
			return;
		}
		string pureId = itemListView.GetPureId(selectItemKey);
		if (itemListView.Contains(pureId))
		{
			itemListView.SetSelection(pureId, selected: true);
			RoPart partData = base.localUser.FindPart(pureId);
			storageItem.Set(partData);
		}
		else
		{
			itemListView.SetSelection(list[0].id, selected: true);
			storageItem.Set(list[0]);
			selectItemKey = itemListView.itemList[0].name;
		}
	}

	private void InitStorageMedalList()
	{
		selectType = EStorageType.Medal;
		Dictionary<string, int> dictionary = base.localUser.FindSrorageCommanderMedalList();
		itemListView.InitStorageList(dictionary, EStorageType.Medal, medalIdPrefix);
		UISetter.SetActive(empty, dictionary.Count <= 0);
		UISetter.SetActive(bottomRoot, dictionary.Count > 0);
		if (dictionary.Count == 0)
		{
			UISetter.SetLabel(empty, Localization.Format("1317", Localization.Get("4020")));
			return;
		}
		if (string.IsNullOrEmpty(selectItemKey))
		{
			string[] array = new string[dictionary.Count];
			dictionary.Keys.CopyTo(array, 0);
			itemListView.SetSelection(array[0], selected: true);
			storageItem.Set(array[0], dictionary[array[0]], EStorageType.Medal);
			selectItemKey = itemListView.itemList[0].name;
			return;
		}
		string pureId = itemListView.GetPureId(selectItemKey);
		if (itemListView.Contains(pureId))
		{
			itemListView.SetSelection(pureId, selected: true);
			sellContents.Set(pureId, dictionary[pureId], EStorageType.Medal);
			return;
		}
		string[] array2 = new string[dictionary.Count];
		dictionary.Keys.CopyTo(array2, 0);
		itemListView.SetSelection(array2[0], selected: true);
		storageItem.Set(array2[0], dictionary[array2[0]], EStorageType.Medal);
		selectItemKey = itemListView.itemList[0].name;
	}

	private void InitStorageGoodsOrFoodList(EStorageType storageType)
	{
		selectType = storageType;
		Dictionary<string, int> userResourceList = base.localUser.GetUserResourceList(storageType);
		switch (storageType)
		{
		case EStorageType.Goods:
			itemListView.InitStorageList(userResourceList, storageType, goodsIdPrefix);
			break;
		case EStorageType.Food:
			itemListView.InitStorageList(userResourceList, storageType, foodIdPrefix);
			break;
		}
		UISetter.SetActive(empty, userResourceList.Count <= 0);
		UISetter.SetActive(bottomRoot, userResourceList.Count > 0);
		if (userResourceList.Count == 0)
		{
			switch (storageType)
			{
			case EStorageType.Goods:
				UISetter.SetLabel(empty, Localization.Format("1317", Localization.Get("4316")));
				break;
			case EStorageType.Food:
				UISetter.SetLabel(empty, Localization.Format("1317", Localization.Get("20031")));
				break;
			}
		}
		else if (string.IsNullOrEmpty(selectItemKey))
		{
			string[] array = new string[userResourceList.Count];
			userResourceList.Keys.CopyTo(array, 0);
			itemListView.SetSelection(array[0], selected: true);
			storageItem.Set(array[0], userResourceList[array[0]], storageType);
			selectItemKey = itemListView.itemList[0].name;
		}
		else
		{
			string pureId = itemListView.GetPureId(selectItemKey);
			if (itemListView.Contains(pureId))
			{
				itemListView.SetSelection(pureId, selected: true);
				storageItem.Set(pureId, userResourceList[pureId], storageType);
				return;
			}
			string[] array2 = new string[userResourceList.Count];
			userResourceList.Keys.CopyTo(array2, 0);
			itemListView.SetSelection(array2[0], selected: true);
			storageItem.Set(array2[0], userResourceList[array2[0]], storageType);
			selectItemKey = itemListView.itemList[0].name;
		}
	}

	private void InitStorageItemList()
	{
		items = base.localUser.GetEquipPossibleItemList();
		if (items == null)
		{
			return;
		}
		selectType = EStorageType.Item;
		itemListView.InitStorageItemList(items, itemIdPrefix);
		UISetter.SetActive(empty, items.Count <= 0);
		UISetter.SetActive(bottomRoot, items.Count > 0);
		if (items.Count == 0)
		{
			UISetter.SetLabel(empty, Localization.Format("1317", Localization.Get("4314")));
			return;
		}
		if (string.IsNullOrEmpty(selectItemKey))
		{
			itemListView.SetSelection(items[0].id + "-" + items[0].level, selected: true);
			storageItem.Set(items[0]);
			storageItem.SetItemDetailDesc(items[0]);
			selectItemKey = itemListView.itemList[0].name;
			return;
		}
		string pureId = itemListView.GetPureId(selectItemKey);
		if (itemListView.Contains(pureId))
		{
			itemListView.SetSelection(pureId, selected: true);
			storageItem.Set(items[0]);
			return;
		}
		itemListView.SetSelection(items[0].id + "-" + items[0].level, selected: true);
		storageItem.Set(items[0]);
		storageItem.SetItemDetailDesc(items[0]);
		selectItemKey = itemListView.itemList[0].name;
	}

	public void DecreaseItemStart()
	{
		sellContents.DecreaseItemStart();
	}

	public void DecreaseItemEnd()
	{
		sellContents.DecreaseItemEnd();
	}

	public void AddItemStart()
	{
		sellContents.AddItemStart();
	}

	public void AddItemEnd()
	{
		sellContents.AddItemEnd();
	}

	public void ItemCountMax()
	{
		sellContents.ItemCountMax();
	}

	public void CloseSellContents()
	{
		UISetter.SetActive(sellContents, active: false);
	}

	public void SetInputValue()
	{
		sellContents.SetInputValue();
	}

	public void CloseSelectContents()
	{
		UISetter.SetActive(selectContents, active: false);
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
		AnimBG.Reset();
		AnimNpc.Reset();
		AnimTitle.Reset();
		AnimBlock.Reset();
		UIManager.instance.SetPopupPositionY(base.gameObject);
		AnimBG.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimTitle.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void OnAnimClose()
	{
		AnimBG.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimNpc.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
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
