using System;
using System.Collections;
using System.Collections.Generic;
using Shared.Regulation;
using UnityEngine;

public class UIWeaponResearch : UIPopup
{
	[Serializable]
	public class UIInprogress : UIInnerPartBase
	{
		public UITimer weaponMaterialRechargeTimer1;

		public UITimer weaponMaterialRechargeTimer2;

		public UITimer weaponMaterialRechargeTimer3;

		public UITimer weaponMaterialRechargeTimer4;

		public UILabel weaponMaterial1;

		public UILabel weaponMaterial2;

		public UILabel weaponMaterial3;

		public UILabel weaponMaterial4;

		public UILabel weaponImmediateTicket;

		public UILabel weaponMakeTicket;

		public UIDefaultListView weaponProgressListView;

		public UIWidget progressRoot;

		public GameObject materialSettingRoot;

		public GameObject materialEditRoot;

		public List<UISprite> materialIconList;

		public List<WeaponMaterialSettingItem> materialCount;

		private List<int> materialList;

		public UISprite editMaterialIcon;

		public UILabel inputLabel;

		public UIInput input;

		public UILabel makeTicketName;

		public UILabel timeMachineTicketName;

		public UILabel makeTicketBe;

		public UILabel makeTicketAf;

		public GameObject decreaseBtn;

		public GameObject addBtn;

		public GameObject immediateBtn;

		private int materialMax;

		private int materialMin;

		private int materialCnt;

		private int materialSelectId;

		private int selectSlot;

		private int itemCount;

		private bool isPress;

		private UIWeaponResearch parentPanel;

		[HideInInspector]
		public UIWeaponProgressHistoryPopup historyPopup;

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
			parentPanel = parent.GetComponent<UIWeaponResearch>();
			materialMax = int.Parse(base.regulation.defineDtbl["WEAPON_MAKE_MATERIAL_MAX"].value);
			materialMin = int.Parse(base.regulation.defineDtbl["WEAPON_MAKE_MATERIAL_MIN"].value);
			GoodsDataRow goodsDataRow = base.regulation.FindGoodsByServerFieldName("wmt");
			GoodsDataRow goodsDataRow2 = base.regulation.FindGoodsByServerFieldName("wimt");
			UISetter.SetLabel(makeTicketName, Localization.Get(goodsDataRow.name));
			UISetter.SetLabel(timeMachineTicketName, Localization.Get(goodsDataRow2.name));
			for (int i = 0; i < materialCount.Count; i++)
			{
				WeaponMaterialSettingItem weaponMaterialSettingItem = materialCount[i];
				string fieldName = $"wmat{i + 1}";
				GoodsDataRow row = base.regulation.FindGoodsByServerFieldName(fieldName);
				weaponMaterialSettingItem.Set(row);
			}
			if (materialList == null)
			{
				materialList = new List<int>();
			}
			materialList.Clear();
			if (PlayerPrefs.HasKey("WeaponMaterial"))
			{
				string @string = PlayerPrefs.GetString("WeaponMaterial");
				string[] array = @string.Split(',');
				materialList.Add(int.Parse(array[0]));
				materialList.Add(int.Parse(array[1]));
				materialList.Add(int.Parse(array[2]));
				materialList.Add(int.Parse(array[3]));
			}
			else
			{
				materialList.Add(materialMin);
				materialList.Add(materialMin);
				materialList.Add(materialMin);
				materialList.Add(materialMin);
			}
			progressRoot.alpha = 1f;
			UISetter.SetActive(materialSettingRoot, active: false);
		}

		public void Set(List<Protocols.WeaponProgressSlotData> list)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int b = int.Parse(base.regulation.defineDtbl["WEAPON_MAKE_SLOT_MAX"].value);
			int num = Mathf.Min(base.localUser.statistics.weaponMakeSlotCount + 1, b);
			for (int i = 1; i <= num; i++)
			{
				int slot = i;
				int value = -1;
				Protocols.WeaponProgressSlotData weaponProgressSlotData = list.Find((Protocols.WeaponProgressSlotData row) => row.slot == slot);
				if (weaponProgressSlotData != null)
				{
					value = weaponProgressSlotData.remain;
				}
				dictionary.Add(slot, value);
			}
			weaponProgressListView.InitWeaponProgress(dictionary, "Slot-");
		}

		public void SetResource()
		{
			UISetter.SetLabel(weaponImmediateTicket, base.localUser.weaponImmediateTicket);
			UISetter.SetLabel(weaponMakeTicket, base.localUser.weaponMakeTicket);
			UISetter.SetLabel(weaponMaterial1, base.localUser.weaponMaterial1);
			UISetter.SetLabel(weaponMaterial2, base.localUser.weaponMaterial2);
			UISetter.SetLabel(weaponMaterial3, base.localUser.weaponMaterial3);
			UISetter.SetLabel(weaponMaterial4, base.localUser.weaponMaterial4);
			UISetter.SetLabel(makeTicketBe, base.localUser.weaponMakeTicket);
			UISetter.SetLabel(makeTicketAf, $"{Mathf.Max(base.localUser.weaponMakeTicket - 1, 0)}(-1)");
		}

		public void StartProgress(int slot, int remain)
		{
			MaterialSettingClose();
			UIWeaponProgressListItem uIWeaponProgressListItem = weaponProgressListView.FindItem(slot.ToString()) as UIWeaponProgressListItem;
			uIWeaponProgressListItem.Set(slot, remain);
		}

		public void ResetProgress(int slot)
		{
			UIWeaponProgressListItem uIWeaponProgressListItem = weaponProgressListView.FindItem(slot.ToString()) as UIWeaponProgressListItem;
			uIWeaponProgressListItem.Set(slot, -1);
		}

		public void ProgressImmediate(int slot)
		{
			UIWeaponProgressListItem uIWeaponProgressListItem = weaponProgressListView.FindItem(slot.ToString()) as UIWeaponProgressListItem;
			uIWeaponProgressListItem.Set(slot, 0);
		}

		public override void OnRefresh()
		{
			base.OnRefresh();
			SetResource();
			if (historyPopup != null)
			{
				historyPopup.OnRefresh();
			}
		}

		public void Close()
		{
			MaterialEditClose();
			UISetter.SetActive(root, active: false);
		}

		public void MaterialEditClose()
		{
			UISetter.SetActive(materialEditRoot, active: false);
		}

		public void MaterialSettingClose()
		{
			UISetter.SetActive(materialSettingRoot, active: false);
			UISetter.SetActive(materialEditRoot, active: false);
			progressRoot.alpha = 1f;
		}

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			string name = sender.name;
			if (name == "Close")
			{
				UISetter.SetActive(root, active: false);
				return;
			}
			if (name.StartsWith("SlotLockBtn"))
			{
				RequestWeaponProgressSlotOpen();
				return;
			}
			if (name.StartsWith("ProgressBtn-"))
			{
				selectSlot = int.Parse(name.Substring(name.IndexOf("-") + 1));
				MaterialDataSetting();
				SoundManager.PlaySFX("SE_Sale_001");
				return;
			}
			if (name.StartsWith("ImmediateBtn-"))
			{
				int slot = int.Parse(name.Substring(name.IndexOf("-") + 1));
				RequestGetWeaponUseImmediateTicket(slot);
				return;
			}
			if (name.StartsWith("FinishBtn-"))
			{
				int slot2 = int.Parse(name.Substring(name.IndexOf("-") + 1));
				RequestGetWeapon(slot2);
				return;
			}
			if (name == "MaterialSettingClose")
			{
				MaterialSettingClose();
				return;
			}
			if (name == "ProgressStartBtn")
			{
				WeaponStartProcess();
				return;
			}
			if (name.StartsWith("materialEditBtn-"))
			{
				materialSelectId = int.Parse(name.Substring(name.IndexOf("-") + 1));
				SetEditMaterialData();
				SoundManager.PlaySFX("SE_Sale_001");
				return;
			}
			switch (name)
			{
			case "MaterialEditCancleBtn":
				SoundManager.PlaySFX("SE_Sale_001");
				MaterialEditClose();
				return;
			case "MaterialEditOkBtn":
				SoundManager.PlaySFX("SE_Sale_001");
				EditMaterialConfirm();
				return;
			case "HistoryBtn":
				if (historyPopup == null)
				{
					if (!base.localUser.weaponHistory.ContainsKey(0) || base.localUser.weaponHistory[0].Count == 0)
					{
						base.network.RequestGetWeaponProgressHistory(0);
						return;
					}
					historyPopup = UIPopup.Create<UIWeaponProgressHistoryPopup>("WeaponProgressHistoryPopup");
					historyPopup.OnRefresh();
				}
				return;
			}
			if (name.StartsWith("Up-"))
			{
				string[] array = name.Split('-');
				int idx = int.Parse(array[1]);
				int cnt = int.Parse(array[2]);
				SetWeaponMaterial(idx, cnt);
			}
			else if (name.StartsWith("Down-"))
			{
				string[] array2 = name.Split('-');
				int idx2 = int.Parse(array2[1]);
				int cnt2 = int.Parse(array2[2]) * -1;
				SetWeaponMaterial(idx2, cnt2);
			}
		}

		private void RequestGetWeapon(int slot)
		{
			if (base.localUser.GetWeaponPossible())
			{
				base.network.RequestWeaponProgressFinish(slot);
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("70067"));
			}
		}

		private void RequestGetWeaponUseImmediateTicket(int slot)
		{
			if (!base.localUser.GetWeaponPossible())
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("70067"));
				return;
			}
			int cost = int.Parse(base.regulation.defineDtbl["WEAPON_MAKE_IMMEDIATE_TICKET_CASH"].value);
			int num = int.Parse(base.regulation.defineDtbl["WEAPON_MAKE_IMMEDIATE_TICKET_COUNT"].value);
			if (base.localUser.weaponImmediateTicket < num)
			{
				UISimplePopup.CreateBool(localization: false, Localization.Get("1303"), Localization.Format("70053", cost), string.Empty, Localization.Get("1304"), Localization.Get("1305")).onClick = delegate(GameObject obj)
				{
					string name2 = obj.name;
					if (name2 == "OK")
					{
						if (base.localUser.cash < cost)
						{
							UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject sender)
							{
								string name3 = sender.name;
								if (name3 == "OK")
								{
									UIManager.instance.world.mainCommand.OpenDiamonShop();
								}
							};
						}
						else
						{
							base.network.RequestWeaponProgressBuyImmediateTicket(slot);
						}
					}
				};
				return;
			}
			UISimplePopup.CreateBool(localization: false, Localization.Get("1303"), Localization.Format("70931", num), string.Empty, Localization.Get("1304"), Localization.Get("1305")).onClick = delegate(GameObject obj)
			{
				string name = obj.name;
				if (name == "OK")
				{
					base.network.RequestWeaponProgressUseImmediateTicket(slot);
				}
			};
		}

		private void RequestWeaponProgressSlotOpen()
		{
			int num = int.Parse(base.regulation.defineDtbl["WEAPON_MAKE_SLOT_ADDCASH"].value);
			if (base.localUser.cash >= num)
			{
				UISimplePopup.CreateBool(localization: false, Localization.Get("1303"), Localization.Format("70930", num), string.Empty, Localization.Get("1304"), Localization.Get("1305")).onClick = delegate(GameObject obj)
				{
					string name2 = obj.name;
					if (name2 == "OK")
					{
						base.network.RequestWeaponProgressSlotOpen();
					}
				};
				return;
			}
			UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject sender)
			{
				string name = sender.name;
				if (name == "OK")
				{
					UIManager.instance.world.mainCommand.OpenDiamonShop();
				}
			};
		}

		public void SetWeaponRecipe(int material1, int material2, int material3, int material4)
		{
			materialList[0] = material1;
			materialList[1] = material2;
			materialList[2] = material3;
			materialList[3] = material4;
			MaterialDataSetting();
		}

		public void SetWeaponMaterial(int idx, int cnt)
		{
			int num = materialList[idx - 1];
			num += cnt;
			if (num < materialMin)
			{
				num = materialMin;
			}
			else if (num > materialMax)
			{
				num = materialMax;
			}
			materialList[idx - 1] = num;
			MaterialDataSetting();
		}

		private void WeaponStartProcess()
		{
			switch (CheckMaterialCount())
			{
			case DontProgressType.None:
				base.network.RequestStartWeaponProgress(selectSlot, materialList[0], materialList[1], materialList[2], materialList[3]);
				break;
			case DontProgressType.Material:
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("70050"));
				break;
			case DontProgressType.Ticket:
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("70932"));
				break;
			}
		}

		private DontProgressType CheckMaterialCount()
		{
			for (int i = 0; i < materialList.Count; i++)
			{
				int num = materialList[i];
				if (i == 0 && base.localUser.weaponMaterial1 < num)
				{
					return DontProgressType.Material;
				}
				if (i == 1 && base.localUser.weaponMaterial2 < num)
				{
					return DontProgressType.Material;
				}
				if (i == 2 && base.localUser.weaponMaterial3 < num)
				{
					return DontProgressType.Material;
				}
				if (i == 3 && base.localUser.weaponMaterial4 < num)
				{
					return DontProgressType.Material;
				}
			}
			if (base.localUser.weaponMakeTicket < 1)
			{
				return DontProgressType.Ticket;
			}
			return DontProgressType.None;
		}

		private void MaterialDataSetting()
		{
			UISetter.SetActive(materialSettingRoot, active: true);
			progressRoot.alpha = 0f;
			for (int i = 0; i < materialCount.Count; i++)
			{
				WeaponMaterialSettingItem weaponMaterialSettingItem = materialCount[i];
				weaponMaterialSettingItem.SetMaterial(materialList[i]);
			}
		}

		private void SetEditMaterialData()
		{
			UISetter.SetActive(materialEditRoot, active: true);
			switch (materialSelectId)
			{
			case 1:
				itemCount = base.localUser.weaponMaterial1;
				break;
			case 2:
				itemCount = base.localUser.weaponMaterial2;
				break;
			case 3:
				itemCount = base.localUser.weaponMaterial3;
				break;
			case 4:
				itemCount = base.localUser.weaponMaterial4;
				break;
			}
			materialCnt = materialList[materialSelectId - 1];
			SetValue();
		}

		private void EditMaterialConfirm()
		{
			materialList[materialSelectId - 1] = materialCnt;
			MaterialDataSetting();
			MaterialEditClose();
		}

		private void SetValue()
		{
			input.value = string.Empty;
			UISetter.SetLabel(inputLabel, materialCnt);
			UISetter.SetButtonEnable(decreaseBtn, materialCnt > materialMin);
			UISetter.SetButtonEnable(addBtn, materialCnt < itemCount);
		}

		public void WeaponMaterialControl()
		{
			WeaponMaterialControl1();
			WeaponMaterialControl2();
			WeaponMaterialControl3();
			WeaponMaterialControl4();
		}

		public void WeaponMaterialControl1()
		{
			UISetter.SetLabel(weaponMaterial1, base.localUser.weaponMaterial1);
			UISetter.SetActive(weaponMaterialRechargeTimer1, base.localUser.weaponMaterialRemainTime1.GetRemain() > 0.0);
			if (base.localUser.weaponMaterialRemainTime1.GetRemain() > 0.0)
			{
				UISetter.SetTimer(weaponMaterialRechargeTimer1, base.localUser.weaponMaterialRemainTime1);
				weaponMaterialRechargeTimer1.RegisterOnFinished(delegate
				{
					base.localUser.WeaponMaterialCharge1();
				});
			}
		}

		public void WeaponMaterialControl2()
		{
			UISetter.SetLabel(weaponMaterial2, base.localUser.weaponMaterial2);
			UISetter.SetActive(weaponMaterialRechargeTimer2, base.localUser.weaponMaterialRemainTime2.GetRemain() > 0.0);
			if (base.localUser.weaponMaterialRemainTime2.GetRemain() > 0.0)
			{
				UISetter.SetTimer(weaponMaterialRechargeTimer2, base.localUser.weaponMaterialRemainTime2);
				weaponMaterialRechargeTimer2.RegisterOnFinished(delegate
				{
					base.localUser.WeaponMaterialCharge2();
				});
			}
		}

		public void WeaponMaterialControl3()
		{
			UISetter.SetLabel(weaponMaterial3, base.localUser.weaponMaterial3);
			UISetter.SetActive(weaponMaterialRechargeTimer3, base.localUser.weaponMaterialRemainTime3.GetRemain() > 0.0);
			if (base.localUser.weaponMaterialRemainTime3.GetRemain() > 0.0)
			{
				UISetter.SetTimer(weaponMaterialRechargeTimer3, base.localUser.weaponMaterialRemainTime3);
				weaponMaterialRechargeTimer3.RegisterOnFinished(delegate
				{
					base.localUser.WeaponMaterialCharge3();
				});
			}
		}

		public void WeaponMaterialControl4()
		{
			UISetter.SetLabel(weaponMaterial4, base.localUser.weaponMaterial4);
			UISetter.SetActive(weaponMaterialRechargeTimer4, base.localUser.weaponMaterialRemainTime4.GetRemain() > 0.0);
			if (base.localUser.weaponMaterialRemainTime4.GetRemain() > 0.0)
			{
				UISetter.SetTimer(weaponMaterialRechargeTimer4, base.localUser.weaponMaterialRemainTime4);
				weaponMaterialRechargeTimer4.RegisterOnFinished(delegate
				{
					base.localUser.WeaponMaterialCharge4();
				});
			}
		}
	}

	[Serializable]
	public class UIWeaponList : UIInnerPartBase
	{
		public UILabel inventoryCount;

		public UILabel emptyLabel;

		public GameObject inventoryBtn;

		public GameObject bottomRoot;

		public UICommanderWeaponItem bottomItem;

		public GameObject itemRoot;

		public GameObject decompositionRoot;

		public UIDefaultListView weaponListView;

		public UILabel selectDecompositionDescription;

		public List<UILabel> selectDecompositionCount;

		public UIFlipSwitch allTab;

		public UIFlipSwitch weaponTab;

		public UIFlipSwitch shieldTab;

		public UIFlipSwitch armourTab;

		public UIFlipSwitch specialTab;

		public UIFlipSwitch coreTab;

		private WeaponTabType tabType;

		private UIWeaponResearch parentPanel;

		private int selectWeaponId;

		private bool decompositionMode;

		private List<int> decompositionList;

		private int inventoryMax;

		private int inventoryCost;

		private UIWeaponDecompositionPopup decompositionPopup;

		private UISelectDecompositionPopup selectDecompositionPopup;

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
			parentPanel = parent.GetComponent<UIWeaponResearch>();
			decompositionList = new List<int>();
			tabType = WeaponTabType.All;
			UISetter.SetFlipSwitch(allTab, state: true);
			UISetter.SetFlipSwitch(weaponTab, state: false);
			UISetter.SetFlipSwitch(shieldTab, state: false);
			UISetter.SetFlipSwitch(armourTab, state: false);
			UISetter.SetFlipSwitch(specialTab, state: false);
			UISetter.SetFlipSwitch(coreTab, state: false);
			inventoryMax = int.Parse(base.regulation.defineDtbl["WEAPON_INVENTORY_MAX"].value);
			inventoryCost = int.Parse(base.regulation.defineDtbl["WEAPON_INVENTORY_ADDCASH"].value);
		}

		public override void OnRefresh()
		{
			base.OnRefresh();
			SetInventoryData();
			Set();
			weaponListView.ResetPosition();
		}

		public void Init()
		{
			tabType = WeaponTabType.All;
			UISetter.SetFlipSwitch(allTab, state: true);
			UISetter.SetFlipSwitch(weaponTab, state: false);
			UISetter.SetFlipSwitch(shieldTab, state: false);
			UISetter.SetFlipSwitch(armourTab, state: false);
			UISetter.SetFlipSwitch(specialTab, state: false);
			UISetter.SetFlipSwitch(coreTab, state: false);
			weaponListView.ResetPosition();
			ResetDecomposition();
			Set();
		}

		public void ResetDecompositionList()
		{
			decompositionList.Clear();
			SelectDecompositionLabel();
		}

		private void ResetDecomposition()
		{
			selectWeaponId = 0;
			decompositionMode = false;
			decompositionList.Clear();
			UISetter.SetActive(itemRoot, active: true);
			UISetter.SetActive(decompositionRoot, active: false);
		}

		public void Set()
		{
			UISetter.SetActive(inventoryBtn, base.localUser.statistics.weaponInventoryCount < inventoryMax);
			List<RoWeapon> weaponList = base.localUser.GetWeaponList((int)tabType);
			weaponList.Sort(delegate(RoWeapon row, RoWeapon row1)
			{
				int num = ((row.currEquipCommanderId != 0) ? 1 : 0);
				int num2 = ((row1.currEquipCommanderId != 0) ? 1 : 0);
				if (num < num2)
				{
					return 1;
				}
				if (num > num2)
				{
					return -1;
				}
				if (row.data.weaponGrade < row1.data.weaponGrade)
				{
					return 1;
				}
				if (row.data.weaponGrade > row1.data.weaponGrade)
				{
					return -1;
				}
				if (row.level < row1.level)
				{
					return 1;
				}
				return (row.level > row1.level) ? (-1) : 0;
			});
			UISetter.SetActive(emptyLabel.gameObject, weaponList.Count == 0);
			UISetter.SetActive(bottomRoot, weaponList.Count > 0);
			SetInventoryData();
			SetEmptyLabel();
			weaponListView.InitWeapon(weaponList, decompositionList, "Weapon-");
			if (weaponList.Count > 0)
			{
				RoWeapon roWeapon = weaponList.Find((RoWeapon row) => row.idx == selectWeaponId.ToString());
				if (roWeapon == null || selectWeaponId == 0)
				{
					selectWeaponId = int.Parse(weaponList[0].idx);
				}
				weaponListView.SetSelection(selectWeaponId.ToString(), selected: true);
				SetWeaponData();
			}
		}

		private void SetInventoryData()
		{
			UISetter.SetLabel(inventoryCount, Localization.Format("70060", base.localUser.GetWeaponCount(), base.localUser.statistics.weaponInventoryCount));
		}

		private void SetWeaponData()
		{
			weaponListView.SetSelection(selectWeaponId.ToString(), selected: true);
			RoWeapon weapon = base.localUser.FindWeapon(selectWeaponId.ToString());
			bottomItem.Set(weapon);
		}

		private void SetEmptyLabel()
		{
			string key = string.Empty;
			switch (tabType)
			{
			case WeaponTabType.All:
				key = "70902";
				break;
			case WeaponTabType.Weapon:
				key = "70903";
				break;
			case WeaponTabType.Shield:
				key = "70904";
				break;
			case WeaponTabType.Armour:
				key = "70905";
				break;
			case WeaponTabType.Special:
				key = "70906";
				break;
			case WeaponTabType.Core:
				key = "70907";
				break;
			}
			UISetter.SetLabel(emptyLabel, Localization.Get(key));
		}

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			string name = sender.name;
			switch (name)
			{
			case "Close":
				UISetter.SetActive(root, active: false);
				return;
			case "AllTab":
				tabType = WeaponTabType.All;
				ResetDecomposition();
				OnRefresh();
				return;
			case "WeaponTab":
				tabType = WeaponTabType.Weapon;
				ResetDecomposition();
				OnRefresh();
				return;
			case "ShieldTab":
				tabType = WeaponTabType.Shield;
				ResetDecomposition();
				OnRefresh();
				return;
			case "ArmourTab":
				tabType = WeaponTabType.Armour;
				ResetDecomposition();
				OnRefresh();
				return;
			case "SpecialTab":
				tabType = WeaponTabType.Special;
				ResetDecomposition();
				OnRefresh();
				return;
			case "CoreTab":
				tabType = WeaponTabType.Core;
				ResetDecomposition();
				OnRefresh();
				return;
			}
			if (name.StartsWith("Weapon-"))
			{
				if (!decompositionMode)
				{
					selectWeaponId = int.Parse(name.Substring(name.IndexOf("-") + 1));
					SetWeaponData();
				}
				else
				{
					int weaponId = int.Parse(name.Substring(name.IndexOf("-") + 1));
					SelectDecompositionWeapon(weaponId);
				}
				return;
			}
			switch (name)
			{
			case "DecompositionBtn":
				Decomposition();
				break;
			case "SelectDecompositionMode":
				decompositionMode = !decompositionMode;
				UISetter.SetActive(itemRoot, !decompositionMode);
				UISetter.SetActive(decompositionRoot, decompositionMode);
				if (!decompositionMode)
				{
					decompositionList.Clear();
				}
				SelectDecompositionLabel();
				Set();
				break;
			case "SelectDecomposition":
				if (decompositionList.Count > 0)
				{
					SelectDecomposition();
				}
				else
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("70935"));
				}
				break;
			case "InventoryAddBtn":
				AddInventory();
				break;
			}
		}

		private void AddInventory()
		{
			UISimplePopup.CreateBool(localization: false, Localization.Get("1303"), Localization.Format("70065", inventoryCost), string.Empty, Localization.Get("1304"), Localization.Get("1305")).onClick = delegate(GameObject obj)
			{
				string name = obj.name;
				if (name == "OK")
				{
					if (base.localUser.cash < inventoryCost)
					{
						UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject sender)
						{
							string name2 = sender.name;
							if (name2 == "OK")
							{
								UIManager.instance.world.mainCommand.OpenDiamonShop();
							}
						};
					}
					else
					{
						base.network.RequestUpgradeWeaponInventory();
					}
				}
			};
		}

		private void SelectDecompositionWeapon(int weaponId)
		{
			RoWeapon roWeapon = base.localUser.FindWeapon(weaponId.ToString());
			if (roWeapon.currEquipCommanderId == 0)
			{
				if (decompositionList.Contains(weaponId))
				{
					decompositionList.Remove(weaponId);
				}
				else if (decompositionList.Count < 20)
				{
					decompositionList.Add(weaponId);
				}
				else
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("70071"));
				}
				SelectDecompositionLabel();
				Set();
			}
			else
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("70950"));
			}
		}

		private void Decomposition()
		{
			RoWeapon weapon = base.localUser.FindWeapon(selectWeaponId.ToString());
			if (weapon.currEquipCommanderId != 0)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("70950"));
			}
			else
			{
				if (!(decompositionPopup == null))
				{
					return;
				}
				decompositionPopup = UIPopup.Create<UIWeaponDecompositionPopup>("WeaponDecompositionPopup");
				decompositionPopup.Set(weapon);
				decompositionPopup.onClick = delegate(GameObject sender)
				{
					string name = sender.name;
					if (name == "OK")
					{
						List<int> list = new List<int> { int.Parse(weapon.idx) };
						base.network.RequestDecompositionWeapon(list);
					}
				};
			}
		}

		private void SelectDecomposition()
		{
			List<RoWeapon> weaponList = new List<RoWeapon>();
			decompositionList.ForEach(delegate(int row)
			{
				RoWeapon item = base.localUser.FindWeapon(row.ToString());
				weaponList.Add(item);
			});
			if (!(selectDecompositionPopup == null))
			{
				return;
			}
			selectDecompositionPopup = UIPopup.Create<UISelectDecompositionPopup>("SelectDecompositionPopup");
			selectDecompositionPopup.Set(weaponList);
			selectDecompositionPopup.onClick = delegate(GameObject sender)
			{
				string name = sender.name;
				if (name == "OK")
				{
					base.network.RequestDecompositionWeapon(decompositionList);
				}
			};
		}

		private void SelectDecompositionLabel()
		{
			UISetter.SetLabel(selectDecompositionDescription, Localization.Format("70069", decompositionList.Count));
			List<RoWeapon> weaponList = new List<RoWeapon>();
			decompositionList.ForEach(delegate(int row)
			{
				RoWeapon item = base.localUser.FindWeapon(row.ToString());
				weaponList.Add(item);
			});
			int i;
			for (i = 0; i < selectDecompositionCount.Count; i++)
			{
				List<RoWeapon> list = weaponList.FindAll((RoWeapon row) => row.data.weaponGrade == i + 1);
				UISetter.SetLabel(selectDecompositionCount[i], Localization.Format("70074", list.Count));
			}
		}
	}

	[Serializable]
	public class UIBoxList : UIInnerPartBase
	{
		[Serializable]
		public class UIOpenBox : UIInnerPartBase
		{
			public UISprite boxIcon;

			public UILabel boxCount;

			public UILabel inputLabel;

			public UIInput input;

			public GameObject decreaseBtn;

			public GameObject addBtn;

			private int boxMax;

			private int boxMin;

			private bool isPress;

			[HideInInspector]
			public string itemId;

			[HideInInspector]
			public int boxCnt;

			private UIPanelBase parentPanel;

			public void Set(string idx, UIPanelBase parent)
			{
				parentPanel = parent;
				itemId = idx;
				GoodsDataRow goodsDataRow = base.regulation.goodsDtbl[idx];
				int num = base.localUser.resourceList[goodsDataRow.serverFieldName];
				UISetter.SetSprite(boxIcon, goodsDataRow.iconId);
				UISetter.SetLabel(boxCount, num);
				boxMin = 1;
				boxMax = num;
				boxCnt = 1;
				SetValue();
			}

			private void SetValue()
			{
				input.value = string.Empty;
				UISetter.SetLabel(inputLabel, boxCnt);
				UISetter.SetButtonEnable(decreaseBtn, boxCnt > boxMin);
				UISetter.SetButtonEnable(addBtn, boxCnt < boxMax);
			}

			public void DecreaseItemStart()
			{
				if (!isPress)
				{
					parentPanel.StartCoroutine(ItemCalculation(-1));
				}
			}

			public void DecreaseItemEnd()
			{
				if (isPress)
				{
					isPress = false;
					if (ItemCheck(-1))
					{
						boxCnt--;
						SetValue();
					}
				}
			}

			public void AddItemStart()
			{
				if (!isPress)
				{
					parentPanel.StartCoroutine(ItemCalculation(1));
				}
			}

			public void AddItemEnd()
			{
				if (isPress)
				{
					isPress = false;
					if (ItemCheck(1))
					{
						boxCnt++;
						SetValue();
					}
				}
			}

			public void ItemCountMax()
			{
				boxCnt = boxMax;
				SetValue();
			}

			private bool ItemCheck(int value)
			{
				if (value > 0)
				{
					if (boxCnt < boxMax)
					{
						return true;
					}
				}
				else if (boxCnt > 1)
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
					boxCnt += value;
					SetValue();
					yield return new WaitForSeconds(speed);
				}
				yield return true;
			}

			public void SetInputValue()
			{
				if (string.IsNullOrEmpty(input.value) || int.Parse(input.value) < boxMin)
				{
					boxCnt = boxMin;
				}
				else if (int.Parse(input.value) > boxMax)
				{
					boxCnt = boxMax;
				}
				else
				{
					boxCnt = int.Parse(input.value);
				}
				SetValue();
			}

			public void SetLimitLength(int limit)
			{
				input.characterLimit = limit;
			}
		}

		[Serializable]
		public class UIComposeBox : UIInnerPartBase
		{
			public UISprite boxIcon;

			public UILabel boxCount;

			public UISprite upgradeBoxIcon;

			public UILabel upgradeBoxCount;

			public UILabel inputLabel;

			public UIInput input;

			public UILabel upgradeDescription;

			public GameObject decreaseBtn;

			public GameObject addBtn;

			private int boxMax;

			private int boxMin;

			private bool isPress;

			[HideInInspector]
			public string itemId;

			[HideInInspector]
			public int boxCnt;

			private int itemCount;

			private int composeCnt;

			private UIPanelBase parentPanel;

			public void Set(string idx, UIPanelBase parent)
			{
				parentPanel = parent;
				GoodsDataRow goodsDataRow = base.regulation.goodsDtbl[idx];
				GoodsComposeDataRow goodsComposeDataRow = base.regulation.goodsComposeDtbl[idx];
				GoodsDataRow goodsDataRow2 = base.regulation.goodsDtbl[goodsComposeDataRow.rewardIdx];
				UISetter.SetLabel(upgradeDescription, Localization.Format("70090", goodsComposeDataRow.value));
				itemId = goodsDataRow.type;
				int num = base.localUser.resourceList[goodsDataRow.serverFieldName];
				boxMin = 1;
				itemCount = num;
				composeCnt = goodsComposeDataRow.value;
				boxMax = num / composeCnt;
				boxCnt = 1;
				UISetter.SetSprite(boxIcon, goodsDataRow.iconId);
				UISetter.SetSprite(upgradeBoxIcon, goodsDataRow2.iconId);
				SetValue();
			}

			private void SetValue()
			{
				input.value = string.Empty;
				UISetter.SetLabel(boxCount, boxCnt * composeCnt);
				UISetter.SetLabel(upgradeBoxCount, boxCnt);
				UISetter.SetLabel(inputLabel, boxCnt);
				UISetter.SetButtonEnable(decreaseBtn, boxCnt > boxMin);
				UISetter.SetButtonEnable(addBtn, boxCnt < boxMax);
			}

			public void DecreaseItemStart()
			{
				if (!isPress)
				{
					parentPanel.StartCoroutine(ItemCalculation(-1));
				}
			}

			public void DecreaseItemEnd()
			{
				if (isPress)
				{
					isPress = false;
					if (ItemCheck(-1))
					{
						boxCnt--;
						SetValue();
					}
				}
			}

			public void AddItemStart()
			{
				if (!isPress)
				{
					parentPanel.StartCoroutine(ItemCalculation(1));
				}
			}

			public void AddItemEnd()
			{
				if (isPress)
				{
					isPress = false;
					if (ItemCheck(1))
					{
						boxCnt++;
						SetValue();
					}
				}
			}

			public void ItemCountMax()
			{
				boxCnt = boxMax;
				SetValue();
			}

			private bool ItemCheck(int value)
			{
				if (value > 0)
				{
					if (boxCnt < boxMax)
					{
						return true;
					}
				}
				else if (boxCnt > 1)
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
					boxCnt += value;
					SetValue();
					yield return new WaitForSeconds(speed);
				}
				yield return true;
			}

			public void SetInputValue()
			{
				if (string.IsNullOrEmpty(input.value) || int.Parse(input.value) < boxMin)
				{
					boxCnt = boxMin;
				}
				else if (int.Parse(input.value) > boxMax)
				{
					boxCnt = boxMax;
				}
				else
				{
					boxCnt = int.Parse(input.value);
				}
				SetValue();
			}

			public void SetLimitLength(int limit)
			{
				input.characterLimit = limit;
			}
		}

		public UIDefaultListView boxListView;

		public GameObject bottomRoot;

		public GameObject emptyLabel;

		public UISprite boxIcon;

		public UILabel boxName;

		public UILabel boxCount;

		public UILabel boxDescription;

		public UILabel upgradeDescription;

		public GameObject upgradeBtn;

		public GameObject openBtn;

		public UIOpenBox openBox;

		public UIComposeBox composeBox;

		public GameObject composeBtn;

		private string selectBoxId = string.Empty;

		private int count;

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
			selectBoxId = string.Empty;
		}

		public void Init()
		{
			List<GoodsDataRow> list = base.regulation.goodsDtbl.FindAll((GoodsDataRow row) => (4000 <= int.Parse(row.type) && int.Parse(row.type) < 5000 && base.localUser.resourceList[row.serverFieldName] > 0) ? true : false);
			UISetter.SetActive(bottomRoot, list.Count > 0);
			boxListView.InitBoxList(list, "Box-");
			if (list.Count > 0)
			{
				if (!boxListView.Contains(selectBoxId))
				{
					selectBoxId = list[0].type;
					SelectBox(list[0].type);
					boxListView.ResetPosition();
				}
				else
				{
					SelectBox(selectBoxId);
				}
			}
		}

		private void SelectBox(string boxId)
		{
			boxListView.SetSelection(boxId, selected: true);
			GoodsDataRow goodsDataRow = base.regulation.goodsDtbl[boxId];
			bool flag = base.regulation.goodsComposeDtbl.ContainsKey(boxId);
			count = base.localUser.resourceList[goodsDataRow.serverFieldName];
			UISetter.SetSprite(boxIcon, goodsDataRow.icon);
			UISetter.SetLabel(boxName, Localization.Get(goodsDataRow.name));
			UISetter.SetLabel(boxCount, count);
			UISetter.SetLabel(boxDescription, Localization.Get(goodsDataRow.description));
			UISetter.SetActive(upgradeDescription, flag);
			UISetter.SetActive(upgradeBtn, flag);
			if (flag)
			{
				GoodsComposeDataRow goodsComposeDataRow = base.regulation.goodsComposeDtbl[boxId];
				UISetter.SetButtonGray(composeBtn, count >= goodsComposeDataRow.value);
				UISetter.SetLabel(upgradeDescription, Localization.Format("70939", goodsComposeDataRow.value));
			}
		}

		public override void OnRefresh()
		{
			base.OnRefresh();
			Init();
		}

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			base.OnClick(sender, parent);
			string name = sender.name;
			if (boxListView.Contains(name))
			{
				selectBoxId = boxListView.GetPureId(name);
				SelectBox(selectBoxId);
			}
			else if (name == "OpenBox")
			{
				UISetter.SetActive(openBox.root, active: true);
				openBox.Set(selectBoxId, parent);
			}
			else if (name == "ComposeBox")
			{
				if (!composeBtn.GetComponent<UIButton>().isGray)
				{
					UISetter.SetActive(composeBox.root, active: true);
					composeBox.Set(selectBoxId, parent);
				}
				else
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("70940"));
				}
			}
			if (name == "OpenCancleBtn")
			{
				UISetter.SetActive(openBox.root, active: false);
			}
			else if (name == "OpenOkBtn")
			{
				if (base.localUser.GetWeaponPossible(openBox.boxCnt))
				{
					UISetter.SetActive(openBox.root, active: false);
					base.network.RequestOpenItem(EStorageType.Box, openBox.itemId, openBox.boxCnt);
				}
				else
				{
					NetworkAnimation.Instance.CreateFloatingText(Localization.Get("70067"));
				}
			}
			if (name == "ComposeCancleBtn")
			{
				UISetter.SetActive(composeBox.root, active: false);
			}
			else if (name == "ComposeOkBtn")
			{
				UISetter.SetActive(composeBox.root, active: false);
				base.network.RequestComposeWeaponBox(int.Parse(composeBox.itemId), composeBox.boxCnt);
			}
		}
	}

	public GEAnimNGUI AnimBG;

	public GEAnimNGUI AnimNpc;

	public GEAnimNGUI AnimTitle;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public UIFlipSwitch progressTab;

	public UIFlipSwitch inventoryTab;

	public UIFlipSwitch boxTab;

	public UIInprogress inProgress;

	public UIWeaponList weaponList;

	public UIBoxList boxList;

	public UISpineAnimation spineAnimation;

	private MainTabType selectMainTab;

	public static readonly string partIdPrefix = "PartItem-";

	public static readonly string medalIdPrefix = "MedalItem-";

	public static readonly string goodsIdPrefix = "GoodsItem-";

	public static readonly string foodIdPrefix = "FoodItem-";

	public static readonly string itemIdPrefix = "InvenItem-";

	public GameObject goBG;

	private void Start()
	{
		UISetter.SetSpine(spineAnimation, "n_005");
	}

	protected override void OnDisable()
	{
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
		base.OnDisable();
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

	public void InitAndOpenWeaponResearch()
	{
		if (!bEnterKeyEnable)
		{
			bEnterKeyEnable = true;
			UISetter.SetFlipSwitch(progressTab, state: true);
			UISetter.SetFlipSwitch(inventoryTab, state: false);
			UISetter.SetFlipSwitch(boxTab, state: false);
			selectMainTab = MainTabType.Progress;
			SelectTabContents();
			OpenPopupShow();
			SendOnInitToInnerParts();
		}
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		if (selectMainTab == MainTabType.Progress)
		{
			inProgress.OnRefresh();
		}
		else if (selectMainTab == MainTabType.Inventory)
		{
			weaponList.OnRefresh();
		}
		else if (selectMainTab == MainTabType.Box)
		{
			boxList.OnRefresh();
		}
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		switch (sender.name)
		{
		case "Close":
			if (!bBackKeyEnable)
			{
				Close();
			}
			break;
		case "ProgressTab":
			selectMainTab = MainTabType.Progress;
			base.network.RequestGetWeaponProgressList();
			break;
		case "InventoryTab":
			selectMainTab = MainTabType.Inventory;
			SelectTabContents();
			break;
		case "BoxTab":
			selectMainTab = MainTabType.Box;
			SelectTabContents();
			break;
		default:
			SendOnClickToInnerParts(sender);
			break;
		}
	}

	public void SelectTabContents()
	{
		if (selectMainTab == MainTabType.Progress)
		{
			UISetter.SetActive(inProgress.root, active: true);
			UISetter.SetActive(weaponList.root, active: false);
			UISetter.SetActive(boxList.root, active: false);
		}
		else if (selectMainTab == MainTabType.Inventory)
		{
			UISetter.SetActive(inProgress.root, active: false);
			UISetter.SetActive(weaponList.root, active: true);
			UISetter.SetActive(boxList.root, active: false);
			weaponList.Init();
		}
		else if (selectMainTab == MainTabType.Box)
		{
			UISetter.SetActive(inProgress.root, active: false);
			UISetter.SetActive(weaponList.root, active: false);
			UISetter.SetActive(boxList.root, active: true);
			boxList.Init();
		}
	}

	public void SetWeaponRecipe(int material1, int material2, int material3, int material4)
	{
		inProgress.SetWeaponRecipe(material1, material2, material3, material4);
	}

	public void SetWeaponProgressData(List<Protocols.WeaponProgressSlotData> list)
	{
		inProgress.Set(list);
	}

	public void SetBoxOpenInputValue()
	{
		boxList.openBox.SetInputValue();
	}

	public void DecreaseBoxOpenStart()
	{
		boxList.openBox.DecreaseItemStart();
	}

	public void DecreaseBoxOpenEnd()
	{
		boxList.openBox.DecreaseItemEnd();
	}

	public void AddIBoxOpenStart()
	{
		boxList.openBox.AddItemStart();
	}

	public void AddBoxOpenEnd()
	{
		boxList.openBox.AddItemEnd();
	}

	public void BoxOpenCountMax()
	{
		boxList.openBox.ItemCountMax();
	}

	public void SetBoxComposeInputValue()
	{
		boxList.composeBox.SetInputValue();
	}

	public void DecreaseBoxComposeStart()
	{
		boxList.composeBox.DecreaseItemStart();
	}

	public void DecreaseBoxComposeEnd()
	{
		boxList.composeBox.DecreaseItemEnd();
	}

	public void AddIBoxComposeStart()
	{
		boxList.composeBox.AddItemStart();
	}

	public void AddBoxComposeEnd()
	{
		boxList.composeBox.AddItemEnd();
	}

	public void BoxComposeCountMax()
	{
		boxList.composeBox.ItemCountMax();
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
		base.localUser.newWeaponList.Clear();
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
