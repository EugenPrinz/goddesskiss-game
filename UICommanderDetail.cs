using System;
using System.Collections;
using System.Collections.Generic;
using Cache;
using Shared.Regulation;
using UnityEngine;

public class UICommanderDetail : UIPopup
{
	[Serializable]
	public class WeaponSkillDescription : UIInnerPartBase
	{
		public UILabel title;

		public UILabel type;

		public UILabel description;

		public void SetLabel(string title, string type, string description)
		{
			UISetter.SetActive(root, active: true);
			UISetter.SetLabel(this.title, title);
			UISetter.SetLabel(this.type, type);
			UISetter.SetLabel(this.description, description);
		}

		public void Close()
		{
			UISetter.SetActive(root, active: false);
		}
	}

	[Serializable]
	public class CommanderLevelUp : UIInnerPartBase
	{
		public UISprite imageLabel;

		public UILabel attackStat;

		public UILabel healthStat;

		public UILabel defenseStat;

		public UILabel luckStat;

		public UILabel accStat;

		public UISprite commanderIcon;

		public UILabel attack;

		public UILabel health;

		public UILabel defense;

		public UILabel luck;

		public UILabel accur;

		public Animation animation;

		public void Set(RoCommander commander)
		{
			RoCommander roCommander = commander.CreateBeforeLevel();
			RoCommander roCommander2 = commander.CreateOriginCommander();
			int num = roCommander2.currLevelUnitReg.attackDamage - roCommander.currLevelUnitReg.attackDamage;
			int num2 = roCommander2.currLevelUnitReg.defense - roCommander.currLevelUnitReg.defense;
			int num3 = roCommander2.currLevelUnitReg.maxHealth - roCommander.currLevelUnitReg.maxHealth;
			int num4 = roCommander2.currLevelUnitReg.luck - roCommander.currLevelUnitReg.luck;
			int num5 = roCommander2.currLevelUnitReg.accuracy - roCommander.currLevelUnitReg.accuracy;
			UISetter.SetLabel(attackStat, Localization.Get("1"));
			UISetter.SetLabel(healthStat, Localization.Get("4"));
			UISetter.SetLabel(defenseStat, Localization.Get("2"));
			UISetter.SetLabel(luckStat, Localization.Get("3"));
			UISetter.SetLabel(accStat, Localization.Get("5"));
			UISetter.SetSprite(imageLabel, "ma_img_levelup");
			UISetter.SetLabel(attack, $"+{num.ToString()}");
			UISetter.SetLabel(defense, $"+{num2.ToString()}");
			UISetter.SetLabel(health, $"+{num3.ToString()}");
			UISetter.SetLabel(luck, $"+{num4.ToString()}");
			UISetter.SetLabel(accur, $"+{num5.ToString()}");
		}

		public void Set(string resourceId, StatType statType, int stat)
		{
			UISetter.SetLabel(attackStat, string.Empty);
			UISetter.SetLabel(defenseStat, string.Empty);
			UISetter.SetLabel(luckStat, string.Empty);
			UISetter.SetLabel(accStat, string.Empty);
			UISetter.SetLabel(attack, string.Empty);
			UISetter.SetLabel(defense, string.Empty);
			UISetter.SetLabel(luck, string.Empty);
			UISetter.SetLabel(accur, string.Empty);
			string text = string.Empty;
			switch (statType)
			{
			case StatType.ATK:
				text = Localization.Get("1");
				break;
			case StatType.DEF:
				text = Localization.Get("2");
				break;
			case StatType.HP:
				text = Localization.Get("4");
				break;
			case StatType.ACCUR:
				text = Localization.Get("5");
				break;
			case StatType.LUCK:
				text = Localization.Get("3");
				break;
			case StatType.CRITR:
				text = Localization.Get("6");
				break;
			case StatType.CRITDMG:
				text = Localization.Get("8");
				break;
			case StatType.MOB:
				text = Localization.Get("7");
				break;
			}
			UISetter.SetLabel(healthStat, text);
			UISetter.SetSprite(imageLabel, "ma_img_statsup", snap: true);
			UISetter.SetLabel(health, (statType != StatType.CRITDMG && statType != StatType.CRITR) ? (" + " + stat) : (" + " + Localization.Format("5781", stat)));
		}

		public IEnumerator StartRemoveCount()
		{
			animation.Play("CommanderLevelUp");
			while (animation.IsPlaying("CommanderLevelUp"))
			{
				yield return null;
			}
			UISetter.SetActive(root, active: false);
		}
	}

	[Serializable]
	public class UIBuyContents : UIInnerPartBase
	{
		public CommanderCostumeListItem item;

		public UILabel itemCount;

		public GameObject itemCountRoot;

		private UIPanelBase parentPanel;

		private CommanderCostumeDataRow costumeData;

		private bool isPress;

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
			parentPanel = parent;
			UISetter.SetActive(root, active: false);
		}

		public void Set(CommanderCostumeDataRow data)
		{
			UISetter.SetActive(root, active: true);
			item.SetBuy(data);
			costumeData = data;
		}

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			UICommanderDetail uICommanderDetail = parent as UICommanderDetail;
			string name = sender.name;
			if (name.StartsWith("BuyCancleBtn"))
			{
				SoundManager.PlaySFX("BTN_Negative_001");
			}
			else if (name.StartsWith("BuyOkBtn"))
			{
				SoundManager.PlaySFX("BTN_Buy_001");
				if (costumeData.gid == EPriceType.Cash)
				{
					if (base.localUser.cash < costumeData.sellPrice)
					{
						uICommanderDetail.NotEnough(MultiplePopUpType.NOTENOUGH_CASH);
					}
					else
					{
						RemoteObjectManager.instance.RequestBuyCommanderCostume(costumeData.cid, costumeData.ctid);
					}
				}
				else if (costumeData.gid == EPriceType.Gold)
				{
					if (base.localUser.gold < costumeData.sellPrice)
					{
						uICommanderDetail.NotEnough(MultiplePopUpType.NOTENOUGH_GOLD);
					}
					else
					{
						RemoteObjectManager.instance.RequestBuyCommanderCostume(costumeData.cid, costumeData.ctid);
					}
				}
			}
			uICommanderDetail.SetCostume(uICommanderDetail.commander.currentCostume.ToString(), isResetPosition: false);
			UISetter.SetActive(root, active: false);
		}
	}

	[Serializable]
	public class UISkillLevelUpContents : UIInnerPartBase
	{
		public UICommanderSkill skillItem;

		public UILabel goldCost;

		public GameObject decreaseBtn;

		public GameObject addBtn;

		public UILabel title;

		public UILabel inputLabel;

		public UIInput input;

		private Dictionary<int, int> totalCostList;

		private int upLevel;

		private string commanderId;

		private int maxLevel;

		private int minLevel;

		private int price;

		private int skillIndex;

		private UIPanelBase parentPanel;

		private bool isPress;

		public override void OnInit(UIPanelBase parent)
		{
			base.OnInit(parent);
			parentPanel = parent;
			totalCostList = new Dictionary<int, int>();
			UISetter.SetActive(root, active: false);
		}

		public void Set(RoCommander commander, int index)
		{
			totalCostList.Clear();
			skillIndex = index;
			UISetter.SetLabel(title, Localization.Get("19090"));
			SkillDataRow skillData = base.regulation.skillDtbl[commander.unitReg.skillDrks[index - 1]];
			base.regulation.skillCostDtbl.ForEach(delegate(SkillCostDataRow row)
			{
				totalCostList.Add(row.level, row.typeCost[index - 1]);
			});
			int skillLevel = commander.GetSkillLevel(index);
			int level = base.regulation.skillCostDtbl.Find((SkillCostDataRow row) => row.typeLimitLevel[index - 1] == (int)commander.level + 1).level;
			commanderId = commander.id;
			maxLevel = level;
			upLevel = skillLevel + 1;
			minLevel = upLevel;
			isPress = false;
			skillItem.SetSkill(skillData, skillLevel, 0, 0, isOpen: true, 0);
			SetValue();
		}

		public override void OnClick(GameObject sender, UIPanelBase parent)
		{
			UIStorage uIStorage = parent as UIStorage;
			string name = sender.name;
			if (name == "SkillLevelUpCancleBtn")
			{
				UISetter.SetActive(root, active: false);
			}
			else if (name == "SkillLevelUpBtn")
			{
				SoundManager.PlaySFX("SE_Sale_001");
				SkillLevelUpCheck();
			}
		}

		private void SkillLevelUpCheck()
		{
			if (base.localUser.gold < GetCost(minLevel, upLevel))
			{
				UISimplePopup.CreateBool(localization: true, "1029", "1030", null, "1001", "1000").onClick = delegate(GameObject go)
				{
					string name = go.name;
					if (name == "OK")
					{
						base.uiWorld.camp.GoNavigation("MetroBank");
					}
				};
			}
			else
			{
				SoundManager.PlaySFX("SE_UpgradeSkill_001");
				UISetter.SetActive(root, active: false);
				base.network.RequestCommanderSkillLevelUp(commanderId, skillIndex, upLevel - (minLevel - 1));
			}
		}

		private void SetValue()
		{
			input.value = string.Empty;
			int cost = GetCost(minLevel, upLevel);
			UISetter.SetColor(goldCost, (base.localUser.gold >= cost) ? Color.white : Color.red);
			UISetter.SetLabel(inputLabel, upLevel);
			UISetter.SetLabel(goldCost, cost);
			UISetter.SetButtonEnable(decreaseBtn, upLevel > minLevel);
			UISetter.SetButtonGray(addBtn, upLevel < maxLevel);
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
				upLevel--;
				SetValue();
			}
		}

		public void AddItemStart()
		{
			if (upLevel >= maxLevel)
			{
				NetworkAnimation.Instance.CreateFloatingText(Localization.Get("19092"));
			}
			else
			{
				parentPanel.StartCoroutine(ItemCalculation(1));
			}
		}

		public void AddItemEnd()
		{
			isPress = false;
			if (ItemCheck(1))
			{
				upLevel++;
				SetValue();
			}
		}

		public void ItemCountMax()
		{
			upLevel = maxLevel;
			SetValue();
		}

		private bool ItemCheck(int value)
		{
			if (value > 0)
			{
				if (upLevel < maxLevel)
				{
					return true;
				}
			}
			else if (upLevel > minLevel)
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
				upLevel += value;
				SetValue();
				yield return new WaitForSeconds(speed);
			}
			yield return true;
		}

		public void SetInputValue()
		{
			if (string.IsNullOrEmpty(input.value) || int.Parse(input.value) <= minLevel)
			{
				upLevel = minLevel;
			}
			else if (int.Parse(input.value) > maxLevel)
			{
				upLevel = maxLevel;
			}
			else
			{
				upLevel = int.Parse(input.value);
			}
			SetValue();
		}

		public void SetLimitLength(int limit)
		{
			input.characterLimit = limit;
		}

		private int GetCost(int minLevel, int level)
		{
			int num = 0;
			foreach (KeyValuePair<int, int> totalCost in totalCostList)
			{
				if (minLevel - 1 <= totalCost.Key && totalCost.Key <= level - 1)
				{
					num += totalCost.Value;
				}
			}
			return num;
		}
	}

	public GEAnimNGUI AnimLeft;

	public GEAnimNGUI AnimRight;

	public GEAnimNGUI AnimBlock;

	private bool bBackKeyEnable;

	private bool bEnterKeyEnable;

	public GameObject card;

	public GameObject info;

	public UISprite trainingIcon;

	public UILabel ticket;

	public UIFlipSwitch cardTab;

	[HideInInspector]
	public CommanderDetailType type;

	public RoCommander commander;

	public UICommander uiCommanderLeft;

	public UICommander uiCommanderRight;

	public UICommander uiCommanderUpgrade;

	[SerializeField]
	private UICommander uiDonHaveComander;

	public UIUnit uiUnit;

	public CommanderLevelUp commanderLevelUp;

	public GameObject Tab;

	public UIDragScrollView backDragScrollView;

	public UIDefaultListView skillListView;

	public UIScrollView skillScrollView;

	public UILabel skillPoint;

	public UILabel maxSkillPoint;

	public UITimer skillPointRechargeTimer;

	public GameObject recruit;

	public GameObject training;

	public GameObject commanderMedalInfo;

	public GameObject btnZoomExit;

	private bool isZoom;

	public UILabel trainingGradeGold;

	public UILabel trainingGold;

	public UILabel recruitGradeGold;

	public UILabel classUpGold;

	public UILabel transcendenceLevel;

	public GameObject haveSkillInfo;

	public GameObject dontHaveSkillInfo;

	public GameObject skillRechargeBtn;

	public GameObject skillLevelUpBtn;

	public GameObject trainingBtn;

	public GameObject rankUpBtn;

	public GameObject exchangeBtn;

	public GameObject transcendenceBtn;

	public GameObject classUpBtn;

	public GameObject badgeCommander;

	public GameObject badgeUnit;

	public GameObject badgeRankUp;

	public GameObject badgeClassUp;

	public GameObject badgeTranscendenceSkillUp;

	public UIDefaultListView ticketListView;

	public GameObject progressThumb;

	private UIPlayTween play;

	private float device_width;

	private float device_height;

	private float device_aspectX = 1f;

	private float device_aspectY = 2f;

	private bool bTouchEnable;

	private bool bScaleCard;

	private TimeData skillPointReChargeTime;

	private bool trainigPress;

	private int ticketCnt;

	private int ticketWeight;

	private ETrainingTicketType ticketType;

	public UIDefaultListView favorRewardListView;

	public UIProgressBar favorRewardProgress;

	public UILabel favorRewarValue;

	public UILabel favorStep;

	public static readonly string favorRewardItemIdPrefix = "RewardItem-";

	public UIFlipSwitch commanderTab;

	public UIFlipSwitch unitTab;

	public UIFlipSwitch favorTab;

	public UIFlipSwitch scenarioTab;

	public UIFlipSwitch costumeTab;

	public UIFlipSwitch voiceTab;

	public GameObject CommanderInfo;

	public GameObject UnitInfo;

	public GameObject FavorInfo;

	public GameObject ScenarioInfo;

	public GameObject CostumeInfo;

	public GameObject VoiceInfo;

	public UIDefaultListView commanderClassMaterialView;

	public UILabel commonMedal;

	public GameObject unitRoot;

	public UILabel useTicket;

	public UILabel classUpLimit;

	public UILabel maxRank;

	public UILabel maxClass;

	public GameObject nextBtn;

	public GameObject preBtn;

	public GameObject trainingEffect;

	public Animation rankUpAnimation;

	public UISlider unitScaleSlider;

	private GameObject unitObj;

	private UINavigationPopUp naviPopUp;

	private List<RoCommander> commanderList;

	public UIScrollView costumeScrollView;

	public UIDefaultListView costumeHaveListView;

	public UIDefaultListView costumeDontHaveListView;

	public GameObject costumeHaveLine;

	public UIBuyContents buyContents;

	private string preCostume;

	public UIDefaultListView favorStepListView;

	public UIDefaultListView commanderGiftListView;

	public UILabel commanderFavorStep;

	public UIProgressBar commanderFavorProgress;

	public UILabel fommanderFavorValue;

	public UILabel commanderGift;

	public GameObject commanderGiftBtn;

	public UILabel addGiftCount;

	public GameObject heartRoot;

	public GameObject giftProgressEffect;

	private string preCommanderId;

	public GameObject cardRoot;

	private bool bLevelUpSound;

	private AudioSource voiceAudio;

	public UIDefaultListView scenarioTitleListView;

	public ScenarioContent scenarioContents;

	private List<CommanderScenarioDataRow> scenarioList;

	[SerializeField]
	private GameObject contentsObject;

	[SerializeField]
	private GameObject LeftLockDesc;

	[SerializeField]
	private GameObject RightLockDesc;

	[SerializeField]
	private UISprite LeftScenarioBG;

	[SerializeField]
	private UISprite RightScenarioBG;

	[SerializeField]
	private GameObject toggle_item;

	[SerializeField]
	private GameObject toggle_unit;

	[SerializeField]
	private GameObject itemStatusLeft;

	[SerializeField]
	private GameObject statusRignt;

	[SerializeField]
	private GameObject itemStatusRignt;

	[SerializeField]
	private GameObject basicCostumeCheck;

	[SerializeField]
	private List<UIItemDetail> itemDetailList;

	[SerializeField]
	private List<UISprite> itemSlotImg;

	[SerializeField]
	private List<GameObject> itemSlotLockImg;

	[SerializeField]
	private UIItemDetail itemSetDesc;

	[HideInInspector]
	public UIHaveItemListPopup haveItemListPopup;

	[SerializeField]
	private List<UISprite> equipButton;

	[SerializeField]
	private GameObject upgradeItemBadge;

	public UIScrollView voiceScrollView;

	public UIDefaultListView voiceInteractionView;

	public UIDefaultListView voiceBattleView;

	public GameObject voiceBattleLine;

	public GameObject nextCommander;

	[SerializeField]
	private GameObject donHaveInfo;

	[SerializeField]
	private UIDefaultListView donHaveSkillListView;

	[SerializeField]
	private UILabel donHaveMedal;

	[SerializeField]
	private UIProgressBar donHaveMedalProgress;

	public List<UICommanderWeaponItem> weaponList;

	[SerializeField]
	private GameObject weaponRoot;

	[SerializeField]
	private GameObject weaponGrid;

	[SerializeField]
	private GameObject UnitStat;

	[SerializeField]
	private GameObject UnitIcon;

	[SerializeField]
	private UILabel weaponName;

	[SerializeField]
	private GameObject weaponSkillInfoRoot;

	[SerializeField]
	private UICommanderSkill weaponSkill;

	[SerializeField]
	private List<GameObject> weaponSkillEffect;

	[SerializeField]
	private UILabel weaponTakeLabel;

	[SerializeField]
	private GameObject weaponUpgradeBtn;

	[SerializeField]
	private GameObject weaponTakeOffBtn;

	[SerializeField]
	private WeaponSkillDescription weaponSkillDescription;

	[SerializeField]
	private GameObject weaponSkillEffectEmpty;

	[SerializeField]
	private GameObject weaponSkillEmpty;

	[SerializeField]
	private GameObject weaponEmpty;

	[SerializeField]
	private UILabel weaponSkillTitle;

	[SerializeField]
	private UICommanderWeaponItem selectWeaponItem;

	[SerializeField]
	private UISprite weaponToggleBtn;

	public UILabel weaponSetLabel;

	public UILabel weaponSetEffectLabel;

	public UISprite weaponSetEffectLeftArrow;

	public UISprite weaponSetEffectRightArrow;

	private UIWeaponUpgradePopup weaponUpgradePopup;

	private UIWeaponListPopup weaponListPopup;

	private int selectWeaponSlot;

	public UISkillLevelUpContents skillLevelUpContents;

	[SerializeField]
	private GameObject marryComplete;

	[SerializeField]
	private GameObject ringBoxIcon;

	private int preFavorStep;

	private bool bFavorStepUp;

	private bool bMaxFavorStep;

	private string UnitBundleName;

	private Vector3 unitOriginScale;

	private int giftCount;

	private bool pressGiftBtn;

	private List<int> lockWeaponSlot = new List<int>();

	public override void OnClick(GameObject sender)
	{
		if (trainigPress)
		{
			return;
		}
		string text = sender.name;
		switch (text)
		{
		case "Close":
			ClosePopup();
			UISetter.SetActive(nextBtn, active: false);
			UISetter.SetActive(preBtn, active: false);
			break;
		case "InfoClose":
			UISetter.SetActive(info, active: false);
			break;
		case "InfoOpen":
			UISetter.SetActive(info, active: true);
			UISetter.SetActive(commanderLevelUp, active: false);
			SoundManager.PlaySFX("SE_MenuOpen_001");
			break;
		case "CardBtn":
			if (cardTab.GetState() == SwitchStatus.ON)
			{
				device_width = Screen.width;
				device_height = Screen.height;
				device_aspectX = 1.6f;
				device_aspectY = 1.6f;
				float num7 = (float)Screen.height / ((float)Screen.width / 16f) / 9f;
				InitAndOpenCard();
				CardScaleUpSize();
			}
			else
			{
				InitAndOpenCard();
			}
			break;
		default:
			if (skillListView.Contains(text))
			{
				string pureId = skillListView.GetPureId(text);
				if (string.Equals(pureId, "0"))
				{
					return;
				}
				SetSkillInfo(pureId);
				break;
			}
			if (donHaveSkillListView.Contains(text))
			{
				string pureId2 = donHaveSkillListView.GetPureId(text);
				if (string.Equals(pureId2, "0"))
				{
					return;
				}
				SetSkillInfo(pureId2);
				break;
			}
			if (text.StartsWith("SkillLevelUpBtn-"))
			{
				int skillIndex = commander.GetSkillIndex(text.Replace("SkillLevelUpBtn-", string.Empty));
				int skillLevel = commander.GetSkillLevel(skillIndex);
				SkillCostDataRow skillCostDataRow = base.regulation.skillCostDtbl[skillLevel.ToString()];
				if (skillCostDataRow == null)
				{
					return;
				}
				int num = skillCostDataRow.typeCost[skillIndex - 1];
				int num2 = skillCostDataRow.typeLimitLevel[skillIndex - 1];
				if (num == 0 || num2 == 0)
				{
					return;
				}
				if ((int)commander.level < num2)
				{
					NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), string.Format(Localization.Get("8033"), num2));
					break;
				}
				UISetter.SetActive(skillLevelUpContents.root, active: true);
				skillLevelUpContents.Set(commander, skillIndex);
				break;
			}
			switch (text)
			{
			case "SkillPointRechargeBtn":
				SkillPointRechargePoint();
				break;
			case "RecruitWaitBtn":
				CommanderDelay();
				break;
			case "RecruitWaitCancleBtn":
				CommanderDelayCancle();
				break;
			case "RecruitWaitRenewBtn":
				CommanderDelay();
				break;
			case "RankUpBtn":
				OnPromotionBtnClicked();
				break;
			case "TranscendenceBtn":
				UIPopup.Create<UITranscendencePopup>("TranscendencePopup").InitAndOpenTranscendence(commander);
				break;
			case "ClassUpBtn":
				OnClassUpBtnClicked();
				break;
			case "TutorialClassUpBtn":
				OnTutorialClassUpBtnClicked();
				break;
			case "GetMedalBtn":
				SetNavigationOpen(EStorageType.Medal, commander.id);
				break;
			case "BtnZoomExit":
				CardScaleDownSize();
				break;
			case "TrainingTicketRoot":
				SetTrainingTicketList();
				UISetter.SetActive(ticketListView, !ticketListView.gameObject.activeSelf);
				break;
			default:
				if (text.StartsWith("Ticket-"))
				{
					string value = text.Substring("Ticket-".Length);
					ticketType = (ETrainingTicketType)Enum.Parse(typeof(ETrainingTicketType), value);
					ticketWeight = 0;
					trainigPress = false;
					StopCoroutine(CommanderTraining());
					if (ticketCnt == 0)
					{
						TrainingResult();
					}
					TrainingResultNetwork();
					break;
				}
				switch (text)
				{
				case "CommanderBtn":
					SaveChangeCostume();
					SetTab(comm: true, unit: false, favor: false, secnario: false, costume: false, voice: false);
					break;
				case "UnitBtn":
					SaveChangeCostume();
					SetTab(comm: false, unit: true, favor: false, secnario: false, costume: false, voice: false);
					setActiveItemStatus(isActive: false);
					break;
				case "CostumeBtn":
					SetTab(comm: false, unit: false, favor: false, secnario: false, costume: true, voice: false);
					unitRoot.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 2;
					costumeScrollView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
					cardRoot.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 3;
					favorStepListView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
					commanderGiftListView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
					voiceScrollView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
					skillScrollView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
					nextCommander.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 2;
					backDragScrollView.scrollView = costumeScrollView;
					break;
				case "VoiceBtn":
					SaveChangeCostume();
					SetTab(comm: false, unit: false, favor: false, secnario: false, costume: false, voice: true);
					unitRoot.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth - 2;
					costumeScrollView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
					cardRoot.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 3;
					favorStepListView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
					commanderGiftListView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
					voiceScrollView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
					skillScrollView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
					nextCommander.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 2;
					backDragScrollView.scrollView = voiceScrollView;
					break;
				case "FavorBtn":
					SaveChangeCostume();
					SetTab(comm: false, unit: false, favor: true, secnario: false, costume: false, voice: false);
					unitRoot.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth - 2;
					costumeScrollView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
					cardRoot.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 3;
					favorStepListView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
					commanderGiftListView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
					voiceScrollView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
					skillScrollView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
					break;
				case "ScenarioBtn":
					SaveChangeCostume();
					SetTab(comm: false, unit: false, favor: false, secnario: true, costume: false, voice: false);
					cardRoot.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 3;
					if (scenarioList != null && scenarioList.Count > 0)
					{
						scenarioTitleListView.ResetPosition();
						ScenarioListItem scenarioListItem = scenarioTitleListView.itemList[0] as ScenarioListItem;
						if (scenarioListItem != null)
						{
							scenarioListItem.OnClick();
						}
					}
					break;
				default:
					if (text.StartsWith("Material-"))
					{
						string id = text.Substring("Material-".Length);
						SetNavigationOpen(EStorageType.Part, id);
					}
					else if (text == "ExchangeBtn")
					{
						if (base.localUser.medal > 0)
						{
							SetExchangeOpen();
						}
						else
						{
							NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("8029"));
						}
					}
					else
					{
						if (text == "UnitIcon")
						{
							break;
						}
						if (text.StartsWith("Costume_"))
						{
							string text2 = text.Substring("Costume_".Length);
							if ((int)commander.currentCostume != int.Parse(text2))
							{
								SetCostume(text2, isResetPosition: false);
								uiCommanderRight.Set(commander);
								UIManager.instance.world.mainCommand.spineTest.Set(commander.resourceId);
							}
							break;
						}
						if (text.StartsWith("DonHaveCostume_"))
						{
							string s = text.Substring("DonHaveCostume_".Length);
							CommanderCostumeDataRow costumeData = commander.getCostumeData(int.Parse(s));
							if (costumeData != null)
							{
								if (costumeData.sell == 2)
								{
									buyContents.Set(costumeData);
								}
								else
								{
									buyContents.Set(costumeData);
								}
								uiCommanderLeft.spine.SetSkin(costumeData.skinName);
							}
							break;
						}
						if (favorStepListView.Contains(text))
						{
							int num3 = int.Parse(favorStepListView.GetPureId(text));
							UIItemBase uIItemBase = favorStepListView.FindItem(num3.ToString());
							FavorListItem favorListItem = uIItemBase as FavorListItem;
							if ((int)commander.favorStep < num3)
							{
								break;
							}
							if ((int)commander.favorRewardStep + 1 < num3)
							{
								NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("20048"));
							}
							else if ((int)commander.favorRewardStep < num3 && favorListItem.openState)
							{
								FavorDataRow favorData = commander.GetFavorData(num3);
								if (favorData.statType != 0)
								{
									OpenCommanderStatsUp(favorData.statType, favorData.stat);
								}
								base.network.RequestGetFavorReward(commander.id, num3);
							}
							break;
						}
						switch (text)
						{
						case "CommanderGiftButton":
							SoundManager.PlaySFX("SE_Training_001");
							if (bMaxFavorStep)
							{
								NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("20032"));
								break;
							}
							if (base.localUser.commanderGift < 1)
							{
								base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.CommandersGift);
								break;
							}
							bFavorStepUp = true;
							base.network.RequestGiftFood(commander.id, "56", 1);
							uiCommanderLeft.spine.skeletonAnimation.GetComponent<UIInteraction>().GiftInteraction(InteractionType.COMMGIFT);
							GiftHeartEffect();
							GiftProgressEffect();
							break;
						case "TempScenario":
							NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("6037"));
							break;
						case "checkBox":
							if (commander.isBasicCostume)
							{
								PlayerPrefs.DeleteKey(commander.id);
								uiCommanderRight.Set(commander);
							}
							else
							{
								int ctid = RemoteObjectManager.instance.regulation.commanderCostumeDtbl.Find((CommanderCostumeDataRow row) => row.cid == int.Parse(commander.id) && row.sell == 0).ctid;
								commander.SetBasicCostumeKey(ctid.ToString());
							}
							SetCostume(commander.currentCostume.ToString(), isResetPosition: false);
							UIManager.instance.world.mainCommand.spineTest.Set(commander.resourceId);
							break;
						case "Toggle_item":
						case "Toggle_unit":
							setActiveItemStatus(toggle_item.activeSelf);
							if (toggle_item.activeSelf)
							{
								SetItemEquipButton();
							}
							break;
						case "Toggle_Weapon":
							UISetter.SetActive(toggle_unit, active: true);
							UISetter.SetActive(toggle_item, active: false);
							UISetter.SetActive(weaponToggleBtn, active: false);
							OpenWeaponUI(!weaponRoot.activeSelf);
							break;
						default:
							if (text.StartsWith("Weapon-"))
							{
								int num4 = int.Parse(text.Substring("Weapon-".Length));
								if (selectWeaponSlot != num4)
								{
									selectWeaponSlot = num4;
									SelectWeapon(selectWeaponSlot);
								}
								break;
							}
							if (text.StartsWith("WeaponEffect-"))
							{
								int idx = int.Parse(text.Substring("WeaponEffect-".Length));
								OpenWeaponSkillEffectInfo(idx);
								break;
							}
							switch (text)
							{
							case "WeaponTakeBtn":
								OpenWeaponTakePopup();
								break;
							case "WeaponTakeOffBtn":
							{
								RoWeapon roWeapon = commander.FindWeaponItem(selectWeaponSlot);
								if (roWeapon != null)
								{
									base.network.RequestReleaseWeapon(int.Parse(commander.id), int.Parse(roWeapon.idx));
								}
								break;
							}
							case "WeaponUpgradeBtn":
								OpenWeaponUpgradePopup();
								break;
							default:
								if (text.StartsWith("WeaponSkill-"))
								{
									string weaponSkillInfo = text.Substring("WeaponSkill-".Length);
									SetWeaponSkillInfo(weaponSkillInfo);
								}
								else if (text.StartsWith("Slot_"))
								{
									string text3 = text.Substring("Slot_".Length);
									if (itemSlotLockImg[int.Parse(text3) - 1].activeSelf)
									{
										int num5 = 0;
										int num6 = 0;
										switch (text3)
										{
										case "1":
											num5 = int.Parse(base.regulation.defineDtbl["EQUIPITEM_1SLOT_OPEN_CLASS_LIMIT"].value);
											break;
										case "2":
											num5 = int.Parse(base.regulation.defineDtbl["EQUIPITEM_2SLOT_OPEN_CLASS_LIMIT"].value);
											break;
										case "3":
											num5 = int.Parse(base.regulation.defineDtbl["EQUIPITEM_3SLOT_OPEN_CLASS_LIMIT"].value);
											break;
										case "4":
											num5 = int.Parse(base.regulation.defineDtbl["EQUIPITEM_4SLOT_OPEN_CLASS_LIMIT"].value);
											break;
										}
										NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), string.Format(Localization.Get("5030422"), Localization.Get((8900 + num5).ToString())));
										SoundManager.PlaySFX("BTN_Negative_001");
									}
									else
									{
										if (haveItemListPopup == null)
										{
											haveItemListPopup = UIPopup.Create<UIHaveItemListPopup>("HaveItemPopup");
										}
										haveItemListPopup.currCommander = commander;
										haveItemListPopup.SetHaveItemList(_equipOpen: true, int.Parse(text3));
										SoundManager.PlaySFX("BTN_Positive_001");
									}
								}
								else if (text.StartsWith("VoiceInteraction_"))
								{
									string s2 = text.Substring("VoiceInteraction_".Length);
									PlayVoice(int.Parse(s2));
								}
								else if (text.StartsWith("VoiceBattle_"))
								{
									string s3 = text.Substring("VoiceBattle_".Length);
									PlayVoice(int.Parse(s3), bBattle: true);
								}
								else if (text == "VoiceBlock")
								{
									NetworkAnimation.Instance.CreateFloatingText(Localization.Get("10083"));
								}
								else if (text == "RingBoxIcon")
								{
									if (base.localUser.ring > 0)
									{
										UIMultiplePopup uIMultiplePopup = UIPopup.Create<UIMultiplePopup>("MultiplePopup");
										uIMultiplePopup.SetData(2, Localization.Get("1303"), string.Format(Localization.Get("20101"), commander.nickname) + "\n" + Localization.Format("20102", base.localUser.ring), Localization.Get("20103"), "Close", null, "GetMarried", Localization.Get("1000"), null, Localization.Get("20100"), _otherState: false, _paymentState: true, EPaymentType.Ring, 1);
										uIMultiplePopup.onClick = delegate(GameObject popupSender)
										{
											string text4 = popupSender.name;
											if (text4 == "GetMarried")
											{
												base.network.RequestGetMarried(int.Parse(commander.id));
											}
										};
									}
									else
									{
										NotEnough(MultiplePopUpType.NOTENOUGH_RING);
									}
								}
								else
								{
									SendOnClickToInnerParts(sender);
								}
								break;
							}
							break;
						}
					}
					break;
				}
				break;
			case "TrainingBtn":
				break;
			}
			break;
		}
		base.OnClick(sender);
	}

	private void setActiveItemStatus(bool isActive)
	{
		OpenWeaponUI(open: false);
		UISetter.SetActive(itemStatusRignt, isActive);
		UISetter.SetActive(itemStatusLeft, isActive);
		UISetter.SetActive(toggle_unit, isActive);
		UISetter.SetActive(statusRignt, !isActive);
		UISetter.SetActive(toggle_item, !isActive && type == CommanderDetailType.Training);
		UISetter.SetActive(weaponToggleBtn, !isActive && !weaponRoot.activeSelf && commander.state == ECommanderState.Nomal && type != CommanderDetailType.Recruit);
	}

	private void setItemStatus()
	{
		for (int i = 0; i < itemDetailList.Count; i++)
		{
			itemDetailList[i].SetLock(isLock: true);
		}
		itemSetDesc.SetEffect(commander);
		Dictionary<int, RoItem> equipItemList = commander.GetEquipItemList();
		if (equipItemList == null)
		{
			return;
		}
		foreach (KeyValuePair<int, RoItem> item in equipItemList)
		{
			itemDetailList[item.Key - 1].SetItemDetail(item.Value, commander);
		}
	}

	private void SetEquipItemSlot()
	{
		ResetItemSlot();
		CheckOpenSlotGrade();
		Dictionary<int, RoItem> equipItemList = commander.GetEquipItemList();
		if (equipItemList == null)
		{
			return;
		}
		foreach (KeyValuePair<int, RoItem> pair in equipItemList)
		{
			EquipItemDataRow equipItemDataRow = base.regulation.equipItemDtbl.Find((EquipItemDataRow row) => row.key == pair.Value.id);
			if (equipItemDataRow != null)
			{
				UISetter.SetActive(itemSlotImg[pair.Key - 1], active: true);
				UISetter.SetSprite(itemSlotImg[pair.Key - 1], equipItemDataRow.equipItemIcon);
			}
		}
	}

	private void ResetItemSlot()
	{
		for (int i = 0; i < itemSlotImg.Count; i++)
		{
			UISetter.SetActive(itemSlotImg[i], active: false);
		}
	}

	private void CheckOpenSlotGrade()
	{
		int num = commander.cls;
		for (int i = 0; i < itemSlotImg.Count; i++)
		{
			UISetter.SetActive(itemSlotLockImg[i], active: true);
		}
		if (int.Parse(base.regulation.defineDtbl["EQUIPITEM_1SLOT_OPEN_CLASS_LIMIT"].value) <= num)
		{
			UISetter.SetActive(itemSlotLockImg[0], active: false);
		}
		if (int.Parse(base.regulation.defineDtbl["EQUIPITEM_2SLOT_OPEN_CLASS_LIMIT"].value) <= num)
		{
			UISetter.SetActive(itemSlotLockImg[1], active: false);
		}
		if (int.Parse(base.regulation.defineDtbl["EQUIPITEM_3SLOT_OPEN_CLASS_LIMIT"].value) <= num)
		{
			UISetter.SetActive(itemSlotLockImg[2], active: false);
		}
		if (int.Parse(base.regulation.defineDtbl["EQUIPITEM_4SLOT_OPEN_CLASS_LIMIT"].value) <= num)
		{
			UISetter.SetActive(itemSlotLockImg[3], active: false);
		}
	}

	private void SetItemEquipButton()
	{
		Dictionary<int, RoItem> equipItemList = commander.GetEquipItemList();
		string text = "L_item_rune_";
		string text2 = string.Empty;
		UISetter.SetActive(upgradeItemBadge, active: false);
		for (int i = 0; i < equipButton.Count; i++)
		{
			UISetter.SetActive(equipButton[i], active: false);
		}
		if (equipItemList == null)
		{
			return;
		}
		foreach (KeyValuePair<int, RoItem> pair in equipItemList)
		{
			switch (pair.Value.statType)
			{
			case EItemStatType.ATK:
				text2 = "red0";
				break;
			case EItemStatType.DEF:
				text2 = "green0";
				break;
			case EItemStatType.ACCUR:
				text2 = "yellow0";
				break;
			case EItemStatType.LUCK:
				text2 = "blue0";
				break;
			}
			text2 += pair.Key;
			UISetter.SetActive(equipButton[pair.Key - 1], active: true);
			UISetter.SetSprite(equipButton[pair.Key - 1], text + text2);
			EquipItemUpgradeDataRow equipItemUpgradeDataRow = base.regulation.equipItemUpgradeDtbl.Find((EquipItemUpgradeDataRow row) => row.upgradeType == pair.Value.upgradeType && row.level == pair.Value.level);
			if (equipItemUpgradeDataRow == null)
			{
				continue;
			}
			RoPart roPart = base.localUser.FindPart(equipItemUpgradeDataRow.upgradeMaterial1);
			if (roPart == null || roPart.count <= equipItemUpgradeDataRow.upgradeMaterial1Volume)
			{
				continue;
			}
			RoPart roPart2 = base.localUser.FindPart(equipItemUpgradeDataRow.upgradeMaterial2);
			if (roPart2 == null || roPart2.count <= equipItemUpgradeDataRow.upgradeMaterial2Volume)
			{
				continue;
			}
			RoPart roPart3 = base.localUser.FindPart(equipItemUpgradeDataRow.upgradeMaterial3);
			if (roPart3 == null || roPart3.count <= equipItemUpgradeDataRow.upgradeMaterial3Volume)
			{
				continue;
			}
			RoPart roPart4 = base.localUser.FindPart(equipItemUpgradeDataRow.upgradeMaterial4);
			if (roPart4 != null && roPart4.count > equipItemUpgradeDataRow.upgradeMaterial4Volume)
			{
				RoPart roPart5 = base.localUser.FindPart(equipItemUpgradeDataRow.upgradeMaterial5);
				if (roPart5 != null && roPart5.count > equipItemUpgradeDataRow.upgradeMaterial5Volume)
				{
					UISetter.SetActive(upgradeItemBadge, active: true);
				}
			}
		}
	}

	private void SetCostume(string id, bool isResetPosition = true)
	{
		List<CommanderCostumeDataRow> costumeList = commander.getCostumeList(isHave: true);
		List<CommanderCostumeDataRow> costumeList2 = commander.getCostumeList(isHave: false);
		if (type == CommanderDetailType.Recruit)
		{
			List<int> donHaveCommCostumeList = base.localUser.GetDonHaveCommCostumeList(commander.id);
			if (donHaveCommCostumeList != null)
			{
				foreach (int item in donHaveCommCostumeList)
				{
					CommanderCostumeDataRow commanderCostumeDataRow = base.regulation.FindCostumeData(item);
					if (commanderCostumeDataRow != null)
					{
						if (!costumeList.Contains(commanderCostumeDataRow))
						{
							costumeList.Add(commanderCostumeDataRow);
						}
						if (costumeList2.Contains(commanderCostumeDataRow))
						{
							costumeList2.Remove(commanderCostumeDataRow);
						}
					}
				}
			}
		}
		int num = (int)Mathf.Ceil((float)costumeList2.Count / 3f);
		int num2 = (int)costumeDontHaveListView.grid.cellHeight * num;
		costumeHaveLine.transform.localPosition = new Vector3(costumeHaveLine.transform.localPosition.x, costumeDontHaveListView.transform.localPosition.y - (float)num2 + 55f, costumeHaveListView.transform.localPosition.z);
		costumeHaveListView.transform.localPosition = new Vector3(costumeHaveListView.transform.localPosition.x, costumeHaveLine.transform.localPosition.y - 175f, costumeHaveListView.transform.localPosition.z);
		costumeList.Sort((CommanderCostumeDataRow row, CommanderCostumeDataRow row1) => row.order.CompareTo(row1.order));
		costumeList2.Sort((CommanderCostumeDataRow row, CommanderCostumeDataRow row1) => row.order.CompareTo(row1.order));
		if (commander.isBasicCostume)
		{
			if (commander.id == uiCommanderLeft.commanderId)
			{
				uiCommanderLeft.spine.SetSkin(commander.currentViewCostume);
			}
			if ((int)commander.currentCostume != int.Parse(id))
			{
				commander.currentCostume = int.Parse(id);
			}
		}
		else
		{
			commander.currentCostume = int.Parse(id);
			if (commander.id == uiCommanderLeft.commanderId)
			{
				uiCommanderLeft.spine.SetSkin(commander.getCurrentCostumeName());
			}
		}
		costumeHaveListView.Init(costumeList, id, "Costume_", null, commander);
		costumeDontHaveListView.Init(costumeList2, null, "DonHaveCostume_", commander.eventCostume);
		if (isResetPosition)
		{
			costumeDontHaveListView.scrollView.ResetPosition();
			costumeHaveListView.scrollView.ResetPosition();
		}
		else
		{
			GameObject obj = Utility.LoadAndInstantiateGameObject("Prefabs/UI/CostumeChange", card.transform);
			UnityEngine.Object.Destroy(obj, 1f);
		}
		UISetter.SetActive(basicCostumeCheck, commander.isBasicCostume);
	}

	private void SetFavor()
	{
		if (!pressGiftBtn)
		{
			List<FavorDataRow> favorList = commander.GetFavorList();
			favorStepListView.Init(favorList, commander.id, commander.favorStep, commander.favorRewardStep, "FavorStep_");
		}
		commanderGiftListView.InitGiftList(base.regulation.GetCommanderGiftList(), "Gift_");
		UISetter.SetLabel(commanderFavorStep, commander.favorStep);
		if (bFavorStepUp && preFavorStep != (int)commander.favorStep)
		{
			TweenScale component = heartRoot.GetComponent<TweenScale>();
			component.enabled = true;
			component.ResetToBeginning();
		}
		preFavorStep = commander.favorStep;
		bFavorStepUp = false;
		FavorStepDataRow favorStepDataRow = RemoteObjectManager.instance.regulation.FindFavorStepData((int)commander.favorStep + 1);
		FavorDataRow favorData = commander.GetFavorData((int)commander.favorStep + 1);
		if (!commander.possibleMarry)
		{
			if (favorData == null)
			{
				bMaxFavorStep = true;
			}
			else
			{
				bMaxFavorStep = false;
			}
		}
		else if ((int)commander.marry == 1)
		{
			if (favorData == null)
			{
				bMaxFavorStep = true;
			}
			else
			{
				bMaxFavorStep = false;
			}
		}
		else if ((int)commander.favorStep >= 13)
		{
			bMaxFavorStep = true;
		}
		else
		{
			bMaxFavorStep = false;
		}
		if (bMaxFavorStep)
		{
			UISetter.SetLabel(fommanderFavorValue, Localization.Get("1309"));
			UISetter.SetProgress(commanderFavorProgress, 0f);
		}
		else
		{
			UISetter.SetLabel(fommanderFavorValue, string.Concat(commander.favorPoint, "/", favorStepDataRow.favor));
			UISetter.SetProgress(commanderFavorProgress, (float)(int)commander.favorPoint / (float)favorStepDataRow.favor);
		}
		GoodsDataRow goodsDataRow = RemoteObjectManager.instance.regulation.FindGoodsByServerFieldName("cgft");
		UISetter.SetLabel(commanderGift, base.localUser.commanderGift + "/" + 5);
		UISetter.SetActive(addGiftCount, giftCount > 0);
		UISetter.SetLabel(addGiftCount, "+" + giftCount);
		if (preCommanderId != commander.id)
		{
			favorStepListView.scrollView.ResetPosition();
		}
	}

	private void SetScenario()
	{
		scenarioList = RemoteObjectManager.instance.regulation.FindCommanderScenarioList(commander.id);
		scenarioList.Sort(delegate(CommanderScenarioDataRow row, CommanderScenarioDataRow row1)
		{
			if (row.heart > row1.heart)
			{
				return 1;
			}
			return (row.heart < row1.heart) ? (-1) : row.order.CompareTo(row1.order);
		});
		if (scenarioList == null || scenarioList.Count <= 0)
		{
			SetScenarioBackGround(isLock: true);
		}
		else
		{
			InitScenarioList();
		}
	}

	public void InitScenarioList()
	{
		SetScenarioBackGround(isLock: false);
		scenarioTitleListView.Init(scenarioList, "STORY-");
		if (preCommanderId != commander.id)
		{
			scenarioTitleListView.scrollView.ResetPosition();
		}
	}

	private void SetScenarioBackGround(bool isLock)
	{
		UISetter.SetActive(scenarioTitleListView, !isLock);
		UISetter.SetActive(contentsObject, !isLock);
		UISetter.SetActive(LeftLockDesc, isLock);
		UISetter.SetActive(RightLockDesc, isLock);
		Color color = default(Color);
		color = ((!isLock) ? new Color(1f, 1f, 1f) : new Color(0.75686276f, 0.75686276f, 0.75686276f));
		LeftScenarioBG.color = color;
		RightScenarioBG.color = color;
	}

	private void GiftHeartEffect()
	{
		GameObject obj = Utility.LoadAndInstantiateGameObject("Prefabs/UI/FavorInfo/FavorCardHeart", card.transform);
		UnityEngine.Object.Destroy(obj, 0.5f);
	}

	private void GiftProgressEffect()
	{
		UISetter.SetActive(giftProgressEffect, active: false);
		UISetter.SetActive(giftProgressEffect, active: true);
	}

	private void SetVoice(bool isResetPosition = true)
	{
		List<InteractionDataRow> list = base.regulation.FindVoiceInteractionData(commander.resourceId);
		List<CommanderVoiceDataRow> voiceDataList = commander.reg.GetVoiceDataList();
		int num = (int)Mathf.Ceil((float)list.Count / 5f);
		int num2 = (int)voiceInteractionView.grid.cellHeight * num;
		voiceBattleLine.transform.localPosition = new Vector3(voiceBattleLine.transform.localPosition.x, voiceInteractionView.transform.localPosition.y - (float)num2 - 70f, voiceBattleLine.transform.localPosition.z);
		voiceBattleView.transform.localPosition = new Vector3(voiceBattleView.transform.localPosition.x, voiceBattleLine.transform.localPosition.y - 95f, voiceBattleView.transform.localPosition.z);
		list.Sort((InteractionDataRow row, InteractionDataRow row1) => row.favorup.CompareTo(row1.favorup));
		voiceDataList.Sort(delegate(CommanderVoiceDataRow row, CommanderVoiceDataRow row1)
		{
			int num3 = 0;
			int num4 = 0;
			if (row.type == ECommanderVoiceEventType.Fatal || row.type == ECommanderVoiceEventType.WinFatal || row.type == ECommanderVoiceEventType.Lose)
			{
				num3 = -1;
			}
			if (row1.type == ECommanderVoiceEventType.Fatal || row1.type == ECommanderVoiceEventType.WinFatal || row1.type == ECommanderVoiceEventType.Lose)
			{
				num4 = -1;
			}
			return (num3 == num4) ? row.type.CompareTo(row1.type) : num3.CompareTo(num4);
		});
		voiceInteractionView.InitVoiceInteraction(list, commander.favorRewardStep, commander.marry, "VoiceInteraction_");
		voiceBattleView.InitVoiceBattle(voiceDataList, "VoiceBattle_");
		voiceInteractionView.scrollView.ResetPosition();
		voiceBattleView.scrollView.ResetPosition();
	}

	private void PlayVoice(int idx, bool bBattle = false)
	{
		if (bBattle)
		{
			uiCommanderLeft.spine.skeletonAnimation.GetComponent<UIInteraction>().VoiceBattle(commander.reg.GetVoiceDataRow((ECommanderVoiceEventType)idx));
		}
		else
		{
			uiCommanderLeft.spine.skeletonAnimation.GetComponent<UIInteraction>().VoiceInteraction(idx);
		}
	}

	public void InfoClose()
	{
		UISetter.SetActive(info, active: false);
	}

	private void OnDrag(GameObject go, Vector2 delta)
	{
		if (!(go.name != "UnitIcon") && unitObj != null)
		{
			unitObj.transform.localRotation = Quaternion.Euler(0f, -0.5f * delta.x * 2f, 0f) * unitObj.transform.localRotation;
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Remove(UICamera.onDrag, new UICamera.VectorDelegate(OnDrag));
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Combine(UICamera.onDrag, new UICamera.VectorDelegate(OnDrag));
	}

	private new void Awake()
	{
		skillPointReChargeTime = TimeData.Create();
		unitScaleSlider.value = 0.7f;
		base.Awake();
	}

	private void SetTrainingTicketList()
	{
		ticketListView.InitTicket(base.regulation.GetTrainingTicketList(), commander, "Ticket-");
	}

	public void SetThumbAnimation()
	{
		play = progressThumb.GetComponent<UIPlayTween>();
		play.resetOnPlay = true;
		play.tweenGroup = 1;
	}

	public void StartThumbAnimation()
	{
		if (play != null)
		{
			play.Play(forward: true);
		}
	}

	public void EndThumbAnimation()
	{
	}

	private IEnumerator CreateUnitFromCache()
	{
		if (!string.IsNullOrEmpty(UnitBundleName) && UnitBundleName != commander.unitReg.prefabId + ".assetbundle")
		{
			AssetBundleManager.DeleteAssetBundle(UnitBundleName);
		}
		if (!AssetBundleManager.HasAssetBundle(commander.unitReg.prefabId + ".assetbundle"))
		{
			yield return StartCoroutine(PatchManager.Instance.LoadPrefabAssetBundle(commander.unitReg.prefabId + ".assetbundle"));
		}
		if (unitObj != null)
		{
			UnityEngine.Object.DestroyImmediate(unitObj);
		}
		unitObj = CacheManager.instance.UnitCache.Create(commander.unitReg.prefabId, unitRoot.transform).gameObject;
		Utility.SetLayer(unitObj, unitRoot.layer);
		unitOriginScale = unitObj.transform.localScale;
		unitObj.transform.localPosition = new Vector3(0f, 0f, 100f);
		unitObj.transform.localRotation = Quaternion.Euler(0f, -130f, 0f);
		unitObj.transform.localScale = new Vector3((40f + unitScaleSlider.value * 80f) * unitOriginScale.x, (40f + unitScaleSlider.value * 80f) * unitOriginScale.y, (40f + unitScaleSlider.value * 80f) * unitOriginScale.z);
		Transform shadow = unitObj.transform.Find("unit_shadow");
		if (shadow != null)
		{
			shadow.gameObject.SetActive(value: false);
		}
		Transform effects = unitObj.transform.Find("effects");
		if (effects != null)
		{
			effects.gameObject.SetActive(value: false);
		}
		Transform caterpillar = unitObj.transform.Find("caterpillar");
		if (caterpillar != null)
		{
			caterpillar.gameObject.SetActive(value: false);
		}
		if (unitObj != null)
		{
			UnitBundleName = commander.unitReg.prefabId.ToLower() + ".assetbundle";
		}
		else
		{
			UnitBundleName = string.Empty;
		}
		yield return null;
	}

	private void SetTicketInfo()
	{
		UISetter.SetLabel(ticket, base.localUser.resourceList[ticketType.ToString()]);
		UIManager.instance.world.mainCommand.OnRefresh();
		uiCommanderUpgrade.Set(commander);
		uiCommanderRight.Set(commander);
		if (type == CommanderDetailType.Recruit)
		{
			uiDonHaveComander.Set(commander);
		}
		RoUnit unit = RoUnit.Create(commander.unitId, commander.level, 1, commander.cls, commander.currentCostume, commander.id, commander.favorRewardStep, commander.marry, commander.transcendence);
		uiUnit.Set(unit);
		CommanderClassDataRow commanderClassDataRow = RemoteObjectManager.instance.regulation.commanderClassDtbl.Find((CommanderClassDataRow list) => list.index == int.Parse(commander.id) && list.cls == (int)unit.cls);
		UISetter.SetLabel(classUpGold, (commanderClassDataRow == null) ? Localization.Get("1309") : commanderClassDataRow.gold.ToString());
		commanderClassMaterialView.InitMaterial(commander.GetClassUpMaterial(), "Material-");
		UISetter.SetButtonGray(classUpBtn, commander.IsClassUp());
		UISetter.SetActive(classUpBtn, commanderClassDataRow != null);
		UISetter.SetActive(maxClass, commanderClassDataRow == null);
		if (commanderClassDataRow != null)
		{
			UISetter.SetLabel(classUpLimit, Localization.Format("8038", commanderClassDataRow.level));
		}
		UISetter.SetActive(classUpLimit, commanderClassDataRow != null && commanderClassDataRow.level > (int)commander.level);
	}

	public void Set(string _id, CommanderDetailType _type)
	{
		ticketType = ETrainingTicketType.ctt1;
		InitTap(_type);
		SetCommanderDetailInfo(_id, _type);
		uiCommanderLeft.SetCommanderData(commander);
	}

	private void InitTap(CommanderDetailType detailType)
	{
		type = detailType;
		UISetter.SetFlipSwitch(cardTab, state: true);
		SetTab(comm: false, unit: true, favor: false, secnario: false, costume: false, voice: false);
		UISetter.SetActive(commanderTab, type == CommanderDetailType.Training);
		UISetter.SetActive(favorTab, type == CommanderDetailType.Training);
		UISetter.SetActive(scenarioTab, type == CommanderDetailType.Training);
		UISetter.SetActive(voiceTab, type == CommanderDetailType.Training);
		unitTab.transform.parent.GetComponent<UIGrid>().enabled = true;
	}

	private void SetCommanderDetailInfo(string _id, CommanderDetailType _type)
	{
		ResetUIActive();
		trainigPress = false;
		ticketCnt = 0;
		type = _type;
		UISetter.SetActive(donHaveInfo, type == CommanderDetailType.Recruit);
		UISetter.SetActive(toggle_item, type == CommanderDetailType.Training && !toggle_unit.activeSelf);
		if (commander != null)
		{
			commander.DeleteCurrLevelUnitReg();
		}
		if (type == CommanderDetailType.Training)
		{
			commander = base.localUser.FindCommander(_id);
		}
		else
		{
			commander = RoCommander.Create(_id, 1, base.localUser.FindCommander(_id).rank, 1, 0, 0, 0, new List<int>());
			UISetter.SetLabel(donHaveMedal, base.localUser.FindCommander(_id).medal + "/" + commander.maxMedal);
			UISetter.SetProgress(donHaveMedalProgress, (float)base.localUser.FindCommander(_id).medal / (float)commander.maxMedal);
			commander.rank = int.Parse(base.regulation.defineDtbl["NO_GET_COMMANDER_STAR"].value);
			commander.cls = int.Parse(base.regulation.defineDtbl["NO_GET_COMMANDER_GRADE"].value);
			commander.level = int.Parse(base.regulation.defineDtbl["NO_GET_COMMANDER_LEVEL"].value);
			commander.SetSkillLevel(1, int.Parse(base.regulation.defineDtbl["NO_GET_COMMANDER_SKILL01"].value));
			commander.SetSkillLevel(2, int.Parse(base.regulation.defineDtbl["NO_GET_COMMANDER_SKILL02"].value));
			commander.SetSkillLevel(3, int.Parse(base.regulation.defineDtbl["NO_GET_COMMANDER_SKILL03"].value));
			commander.SetSkillLevel(4, int.Parse(base.regulation.defineDtbl["NO_GET_COMMANDER_SKILL04"].value));
		}
		ECommanderState state = commander.state;
		UISetter.SetActive(haveSkillInfo, state == ECommanderState.Nomal && type != CommanderDetailType.Recruit);
		UISetter.SetActive(dontHaveSkillInfo, state != ECommanderState.Nomal || type == CommanderDetailType.Recruit);
		CommanderRankDataRow commanderRankDataRow = base.regulation.FindCommanderRankData((int)commander.rank + 1);
		int medal = commander.medal;
		UISetter.SetLabel(trainingGradeGold, (commanderRankDataRow == null) ? Localization.Get("1309") : commanderRankDataRow.gold.ToString());
		UISetter.SetLabel(recruitGradeGold, (commanderRankDataRow == null) ? Localization.Get("1309") : commanderRankDataRow.gold.ToString());
		UISetter.SetLabel(transcendenceLevel, "Lv " + commander.CurrentTranscendenceStep());
		UISetter.SetLabel(commonMedal, base.localUser.medal);
		CommanderTrainingTicketDataRow commanderTrainingTicketDataRow = base.regulation.commanderTrainingTicketDtbl[ticketType.ToString()];
		UISetter.SetLabel(trainingGold, commanderTrainingTicketDataRow.gold);
		UISetter.SetButtonGray(rankUpBtn, commanderRankDataRow != null && commander.maxMedal <= medal && commanderRankDataRow.gold <= base.localUser.gold);
		UISetter.SetActive(rankUpBtn, commanderRankDataRow != null);
		UISetter.SetActive(transcendenceBtn, commanderRankDataRow == null);
		UISetter.SetActive(badgeCommander, (commanderRankDataRow != null && commander.maxMedal <= medal) || (commanderRankDataRow == null && commander.IsTranscendenceSkillUp()));
		UISetter.SetActive(badgeRankUp, commanderRankDataRow != null && commander.maxMedal <= medal);
		UISetter.SetActive(maxRank, active: false);
		UISetter.SetActive(badgeTranscendenceSkillUp, commanderRankDataRow == null && commander.IsTranscendenceSkillUp());
		UISetter.SetActive(badgeUnit, commander.IsClassUp(isGoldCheck: false));
		UISetter.SetActive(badgeClassUp, commander.IsClassUp(isGoldCheck: false));
		UISetter.SetButtonGray(exchangeBtn, base.localUser.medal > 0);
		UISetter.SetActive(exchangeBtn, active: true);
		UISetter.SetActive(commanderMedalInfo, type != CommanderDetailType.Recruit);
		if (type != 0)
		{
			UISetter.SetActive(recruit, state != ECommanderState.Nomal);
			UISetter.SetActive(training, state == ECommanderState.Nomal);
		}
		SetTicketInfo();
		SetTrainingTicket();
		SetTrainingTicketList();
		InitSkillData();
		SetArrowBtn();
		if (commander.haveCostumeList == null)
		{
			commander.SetBaseCostume();
		}
		SetCostume(commander.currentCostume.ToString());
		if (preCommanderId != commander.id)
		{
			preCostume = commander.currentCostume.ToString();
		}
		SetFavor();
		SetScenario();
		setItemStatus();
		SetEquipItemSlot();
		if (toggle_item.activeSelf)
		{
			SetItemEquipButton();
		}
		SetVoice();
		SetWeapon();
		UISetter.SetActive(marryComplete, (int)commander.marry == 1);
		marryComplete.GetComponentInChildren<UIPanel>().depth = 50;
		UISetter.SetActive(ringBoxIcon, commander.possibleMarry && (int)commander.marry == 0 && (int)commander.favorRewardStep >= 13);
		preCommanderId = commander.id;
	}

	private IEnumerator SetCommanderDetailSpineUnit()
	{
		yield return StartCoroutine(uiCommanderLeft.SetCommander(commander));
		UISetter.SetActive(uiCommanderLeft.spine, active: true);
		uiCommanderLeft.spine.target.GetComponent<UIInteraction>().marry = commander.marry;
		yield return StartCoroutine(CreateUnitFromCache());
	}

	private IEnumerator SetCommanderDetail(string _id, CommanderDetailType _type)
	{
		SetCommanderDetailInfo(_id, _type);
		StartCoroutine(SetCommanderDetailSpineUnit());
		yield break;
	}

	public void ResetUIActive()
	{
		isZoom = false;
		UISetter.SetActive(recruit, active: false);
		UISetter.SetActive(training, active: false);
		UISetter.SetActive(badgeCommander, active: false);
		UISetter.SetActive(badgeUnit, active: false);
		UISetter.SetActive(badgeRankUp, active: false);
		UISetter.SetActive(badgeClassUp, active: false);
	}

	public void InitAndOpenCard()
	{
		_SetPage(cardState: true, infoState: false, sceanarioState: false, favorState: false);
	}

	private void _SetPage(bool cardState, bool infoState, bool sceanarioState, bool favorState)
	{
		UISetter.SetActive(card, cardState);
		UISetter.SetActive(info, infoState);
		UISetter.SetActive(commanderLevelUp, active: false);
	}

	private void SetTab(bool comm, bool unit, bool favor, bool secnario, bool costume, bool voice)
	{
		UISetter.SetFlipSwitch(commanderTab, comm);
		UISetter.SetFlipSwitch(unitTab, unit);
		UISetter.SetFlipSwitch(favorTab, favor);
		UISetter.SetFlipSwitch(scenarioTab, secnario);
		UISetter.SetFlipSwitch(costumeTab, costume);
		UISetter.SetFlipSwitch(voiceTab, voice);
		UISetter.SetActive(CommanderInfo, comm);
		UISetter.SetActive(UnitInfo, unit);
		UISetter.SetActive(UnitStat, unit);
		UISetter.SetActive(weaponRoot, active: false);
		UISetter.SetActive(FavorInfo, favor);
		UISetter.SetActive(ScenarioInfo, secnario);
		UISetter.SetActive(CostumeInfo, costume);
		UISetter.SetActive(VoiceInfo, voice);
		badgeCommander.transform.localPosition = ((!comm) ? new Vector2(badgeCommander.transform.localPosition.x, 26f) : new Vector2(badgeCommander.transform.localPosition.x, 40f));
		badgeUnit.transform.localPosition = ((!unit) ? new Vector2(badgeUnit.transform.localPosition.x, 26f) : new Vector2(badgeUnit.transform.localPosition.x, 40f));
		if (type != 0 && unit)
		{
			base.network.RequestBulletCharge();
		}
	}

	private void SaveChangeCostume()
	{
		if (type == CommanderDetailType.Training && commander != null && preCommanderId == commander.id && preCostume != commander.currentCostume.ToString())
		{
			base.network.RequestChangeCommanderCostume(int.Parse(commander.id), commander.currentCostume);
			preCostume = commander.currentCostume.ToString();
		}
	}

	public void InitSkillData()
	{
		List<SkillDataRow> list = new List<SkillDataRow>();
		List<string> skillIdList = commander.GetSkillIdList();
		for (int i = 0; i < skillIdList.Count; i++)
		{
			string text = skillIdList[i];
			if (!string.Equals(text, "0"))
			{
				SkillDataRow item = base.regulation.skillDtbl[text];
				list.Add(item);
			}
		}
		if (type == CommanderDetailType.Training)
		{
			skillListView.InitCommanderSkillList(list, commander, "Skill-");
			skillListView.ResetPosition();
		}
		else
		{
			donHaveSkillListView.InitCommanderSkillList(list, commander, "Skill-");
		}
	}

	private void SetTrainingTicket()
	{
		int num = (int)ticketType;
		GoodsDataRow goodsDataRow = base.regulation.goodsDtbl[num.ToString()];
		UISetter.SetSprite(trainingIcon, goodsDataRow.iconId);
		UISetter.SetLabel(ticket, base.localUser.resourceList[ticketType.ToString()]);
		UISetter.SetButtonEnable(trainingBtn, commander.TrainingEnable(ticketType));
	}

	private void SetSkillInfo(string id)
	{
		UISkillInfoPopup uISkillInfoPopup = UIPopup.Create<UISkillInfoPopup>("SkillInfoPopup");
		uISkillInfoPopup.Set(localization: true, "10056", null, null, null, Localization.Get("10048"), null);
		uISkillInfoPopup.SetInfo(commander, id);
	}

	public void GetCommanderStage()
	{
		base.uiWorld.worldMap.InitAndOpenWorldMap("1", "1");
	}

	public void UseTrainingTicket(GameObject sender)
	{
		string text = sender.name;
		string value = text.Substring("Ticket-".Length);
		ticketType = (ETrainingTicketType)Enum.Parse(typeof(ETrainingTicketType), value);
		if (base.localUser.resourceList[ticketType.ToString()] >= 1)
		{
			base.network.RequestCommanderLevelUp(commander.id, 1, ticketType.ToString());
			bLevelUpSound = false;
		}
	}

	public void TrainingResultNetwork()
	{
		if (ticketCnt != 0)
		{
			base.network.RequestCommanderLevelUp(commander.id, ticketCnt, ticketType.ToString());
			bLevelUpSound = false;
			ticketCnt = 0;
			UISetter.SetActive(useTicket, ticketCnt > 0);
		}
	}

	public void OnPressTrainingBtn(GameObject sender)
	{
		string text = sender.name;
		string value = text.Substring("Ticket-".Length);
		ticketType = (ETrainingTicketType)Enum.Parse(typeof(ETrainingTicketType), value);
		if (base.localUser.resourceList[ticketType.ToString()] < 1)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("8031"));
			return;
		}
		trainigPress = true;
		StartCoroutine("CommanderTraining");
	}

	public void OnReleaseTrainingBtn()
	{
		ticketWeight = 0;
		trainigPress = false;
		StopCoroutine("CommanderTraining");
		if (ticketCnt == 0)
		{
			TrainingResult();
			if (commander.Training(ticketType) == CommanderTrainingResult.Fail && base.localUser.resourceList[ticketType.ToString()] > 0)
			{
				NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("8035"));
			}
		}
		TrainingResultNetwork();
		UISetter.SetActive(useTicket, active: false);
	}

	public void TrainingResult()
	{
		switch (commander.Training(ticketType, 1 + ticketWeight))
		{
		case CommanderTrainingResult.Fail:
			return;
		case CommanderTrainingResult.LevelUp:
			OpenCommanderLevelUp();
			break;
		}
		ticketCnt += 1 + ticketWeight;
		UISetter.SetActive(useTicket, ticketCnt > 0);
		UISetter.SetLabel(useTicket, "x " + ticketCnt);
		StartTrainingAnimation();
		SetTicketInfo();
		SetTrainingTicketList();
	}

	private IEnumerator CommanderTraining()
	{
		float speed = 0.05f;
		float time = 0f;
		ticketWeight = 0;
		int trainingMaximumCnt = commander.GetTrainingMaximumCount(ticketType);
		while (trainigPress)
		{
			if (trainingMaximumCnt < 1 || base.localUser.resourceList[ticketType.ToString()] < 1)
			{
				yield break;
			}
			time += Time.deltaTime;
			ticketWeight += (int)time;
			int tmp2 = ticketWeight + 1 - base.localUser.resourceList[ticketType.ToString()];
			if (tmp2 > 0)
			{
				ticketWeight -= tmp2;
				if (ticketWeight < 0)
				{
					ticketWeight = 0;
				}
			}
			tmp2 = ticketCnt + ticketWeight + 1 - trainingMaximumCnt;
			if (tmp2 > 0)
			{
				ticketWeight -= tmp2;
				if (ticketWeight < 0)
				{
					ticketWeight = 0;
				}
			}
			TrainingResult();
			yield return new WaitForSeconds(speed);
			if (speed > 0.15f)
			{
				speed -= 0.05f;
			}
		}
		yield return true;
	}

	private void StartTrainingAnimation()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(trainingIcon.gameObject);
		gameObject.transform.parent = trainingIcon.transform.parent;
		gameObject.transform.localScale = trainingIcon.transform.localScale;
		gameObject.transform.localPosition = trainingIcon.transform.localPosition;
		SoundManager.PlaySFX("SE_Training_001");
		iTween.MoveTo(gameObject, iTween.Hash("y", gameObject.transform.localPosition.y + 20f, "islocal", true, "time", 0.7f, "delay", 0, "easeType", iTween.EaseType.linear, "oncomplete", "EndTrainingAnimation", "oncompletetarget", base.gameObject, "oncompleteparams", gameObject));
		gameObject.GetComponent<TweenAlpha>().PlayForward();
		UISetter.SetActive(trainingEffect, active: false);
		UISetter.SetActive(trainingEffect, active: true);
	}

	public void EndTrainingAnimation(GameObject obj)
	{
		UnityEngine.Object.DestroyImmediate(obj);
	}

	public void OnPromotionBtnClicked()
	{
		CommanderRankDataRow commanderRankDataRow = base.regulation.FindCommanderRankData((int)commander.rank + 1);
		int medal = commander.medal;
		if (commander.maxMedal > medal)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("8030"));
		}
		else if (commanderRankDataRow.gold > base.localUser.gold)
		{
			UISimplePopup.CreateBool(localization: true, "1029", "1030", null, "1001", "1000").onClick = delegate(GameObject sender)
			{
				string text = sender.name;
				if (text == "OK")
				{
					base.uiWorld.camp.GoNavigation("MetroBank");
				}
			};
		}
		else
		{
			RemoteObjectManager.instance.RequestCommanderRankUp(commander.id);
		}
	}

	public void OnClassUpBtnClicked()
	{
		int num = commander.ImPossibleClassUp();
		if (num == 2)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("8032"));
			return;
		}
		if (num >= 100 && num < 999)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), string.Format(Localization.Get("8033"), num - 100));
			return;
		}
		switch (num)
		{
		case 999:
			UISimplePopup.CreateBool(localization: true, "1029", "1030", null, "1001", "1000").onClick = delegate(GameObject sender)
			{
				string text = sender.name;
				if (text == "OK")
				{
					base.uiWorld.camp.GoNavigation("MetroBank");
				}
			};
			break;
		case 0:
			if (!UIManager.instance.world.unitUpgradeComplete.isActive)
			{
				RemoteObjectManager.instance.RequestCommanderClassUp(commander.id);
			}
			break;
		}
	}

	public void OnTutorialClassUpBtnClicked()
	{
		RemoteObjectManager.instance.RequestTutorialCommanderClassUp(commander.id);
	}

	public override void OnRefresh()
	{
		base.OnRefresh();
		StartCoroutine(SetCommanderDetail(commander.id, type));
		if (weaponUpgradePopup != null)
		{
			weaponUpgradePopup.OnRefresh();
		}
	}

	public void OpenCommanderLevelUp()
	{
		if (commanderLevelUp != null && !commanderLevelUp.isActive)
		{
			SoundManager.PlaySFX("SE_PilotLevelUp_001");
		}
		UISetter.SetActive(commanderLevelUp, active: true);
		commanderLevelUp.Set(commander);
		StartCoroutine(commanderLevelUp.StartRemoveCount());
	}

	public void OpenCommanderStatsUp(StatType type, int stat)
	{
		if (commanderLevelUp != null && !commanderLevelUp.isActive)
		{
			SoundManager.PlaySFX("SE_PilotLevelUp_001");
		}
		UISetter.SetActive(commanderLevelUp, active: true);
		commanderLevelUp.Set(commander.resourceId, type, stat);
		StartCoroutine(commanderLevelUp.StartRemoveCount());
	}

	public void CommanderDelay()
	{
		RemoteObjectManager.instance.RequestRecruitCommanderDelay(commander.id);
	}

	public void CommanderDelayCancle()
	{
		RemoteObjectManager.instance.RequestCommanderDelayCancle(commander.id);
	}

	public void InitFavorRewardData()
	{
		FavorStepDataRow favorStepDataRow = RemoteObjectManager.instance.regulation.FindFavorStepData(commander.FavorStep);
		FavorStepDataRow favorStepDataRow2 = RemoteObjectManager.instance.regulation.FindFavorStepData((int)commander.FavorStep + 1);
		List<RewardDataRow> list = RemoteObjectManager.instance.regulation.rewardDtbl.FindAll((RewardDataRow row) => row.category == ERewardCategory.FavorComplete && row.type.ToString() == commander.id);
		int num = 0;
		float value = 1f;
		if (favorStepDataRow != null)
		{
			num = favorStepDataRow.favor;
		}
		if (favorStepDataRow2 == null || (int)commander.FavorStep >= list.Count)
		{
			UISetter.SetLabel(favorStep, string.Format(Localization.Get("10072"), commander.FavorStep));
		}
		else
		{
			value = (float)((int)commander.FavorCount - num) / (float)(favorStepDataRow2.favor - num);
			UISetter.SetLabel(favorStep, string.Format(Localization.Get("10072"), (int)commander.FavorStep + 1));
		}
		favorRewardProgress.value = value;
		UISetter.SetLabel(favorRewarValue, favorRewardProgress.value * 100f + "%");
		if (!(favorRewardListView != null))
		{
			return;
		}
		list.Sort(delegate(RewardDataRow row, RewardDataRow row1)
		{
			if (row.typeIndex < row1.typeIndex)
			{
				return -1;
			}
			return (row.typeIndex > row1.typeIndex) ? 1 : 0;
		});
		favorRewardListView.InitFavorItem(list, favorRewardItemIdPrefix);
	}

	private void SkillPointRechargePoint()
	{
		VipRechargeDataRow vipRechargeDataRow = RemoteObjectManager.instance.regulation.FindVipRechargeData(EVipRechargeType.Skill);
		if (!base.localUser.resourceRechargeList.ContainsKey(109.ToString()))
		{
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Skill);
			return;
		}
		if (base.localUser.resourceRechargeList[109.ToString()] < vipRechargeDataRow.GetMaxRechargeCount(base.localUser.vipLevel))
		{
			base.uiWorld.mainCommand.OpenVipRechargePopUp(EVipRechargeType.Skill);
			return;
		}
		UISimplePopup.CreateBool(localization: true, "1303", "12006", null, "5348", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				base.uiWorld.mainCommand.OpenDiamonShop();
			}
			else if (!(text == "Cancel"))
			{
			}
		};
	}

	public void NextCommander()
	{
		if (trainigPress || commanderList == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < commanderList.Count; i++)
		{
			if (commanderList[i].id == commander.id)
			{
				num = i;
			}
		}
		if (commanderList.Count > num + 1)
		{
			if (type == CommanderDetailType.Training && preCostume != commander.currentCostume.ToString())
			{
				RemoteObjectManager.instance.RequestChangeCommanderCostume(int.Parse(commander.id), commander.currentCostume);
			}
			StartCoroutine(SetCommanderDetail(commanderList[num + 1].id, type));
		}
		if (voiceAudio != null)
		{
			string[] array = new string[0];
			array[0] = voiceAudio.clip.name;
			CacheManager.instance.SoundCache.CleanUp(array);
		}
	}

	public void PreCommander()
	{
		if (trainigPress || commanderList == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < commanderList.Count; i++)
		{
			if (commanderList[i].id == commander.id)
			{
				num = i;
			}
		}
		if (0 < num)
		{
			if (type == CommanderDetailType.Training && preCostume != commander.currentCostume.ToString())
			{
				RemoteObjectManager.instance.RequestChangeCommanderCostume(int.Parse(commander.id), commander.currentCostume);
			}
			StartCoroutine(SetCommanderDetail(commanderList[num - 1].id, type));
		}
	}

	private void SetArrowBtn()
	{
		if (commanderList == null)
		{
			UISetter.SetActive(nextBtn, active: false);
			UISetter.SetActive(preBtn, active: false);
			return;
		}
		int num = 0;
		for (int i = 0; i < commanderList.Count; i++)
		{
			if (commanderList[i].id == commander.id)
			{
				num = i;
			}
		}
		if (commanderList.Count > num + 1)
		{
		}
		UISetter.SetActive(nextBtn, commanderList.Count > num + 1);
		UISetter.SetActive(preBtn, 0 < num);
	}

	private IEnumerator UnitAttackAnimation()
	{
		if (!(unitObj == null))
		{
			yield return new WaitForSeconds(3f);
			unitObj.GetComponent<UnitRenderer>().PlayAnimation("attack");
			yield return new WaitForSeconds(4f);
			StopCoroutine("UnitAttackAnimation");
			StartCoroutine("UnitAttackAnimation");
		}
	}

	public void OnPressGiftBtn(GameObject sender)
	{
		string text = sender.name;
		string id = text.Substring("Gift_".Length);
		giftCount = 0;
		GoodsDataRow goodsDataRow = RemoteObjectManager.instance.regulation.FindGoodsServerFieldName(id);
		if (base.localUser.resourceList[goodsDataRow.serverFieldName] < 1)
		{
			SetNavigationOpen(EStorageType.Food, id);
		}
		else if (bMaxFavorStep)
		{
			NetworkAnimation.Instance.CreateFloatingText(new Vector3(0f, -0.5f, 0f), Localization.Get("20032"));
		}
		else if (!base.localUser.sendingGift)
		{
			pressGiftBtn = true;
			StartCoroutine("FoodGift", goodsDataRow);
		}
	}

	public void OnReleaseGiftBtn(GameObject sender)
	{
		string text = sender.name;
		string cgid = text.Substring("Gift_".Length);
		pressGiftBtn = false;
		StopCoroutine("FoodGift");
		List<FavorDataRow> favorList = commander.GetFavorList();
		favorStepListView.Init(favorList, commander.id, commander.favorStep, commander.favorRewardStep, "FavorStep_");
		if (giftCount > 0)
		{
			bFavorStepUp = true;
			base.localUser.sendingGift = true;
			base.network.RequestGiftFood(commander.id, cgid, giftCount);
			uiCommanderLeft.spine.skeletonAnimation.GetComponent<UIInteraction>().GiftInteraction(InteractionType.GIFT);
			GiftHeartEffect();
		}
		giftCount = 0;
		UISetter.SetActive(addGiftCount, active: false);
	}

	private IEnumerator FoodGift(GoodsDataRow goodsData)
	{
		float speed = 0.05f;
		while (pressGiftBtn)
		{
			if (base.localUser.resourceList[goodsData.serverFieldName] < 1)
			{
				yield return true;
			}
			else
			{
				FoodGiftResult(goodsData);
			}
			yield return new WaitForSeconds(speed);
			if (speed > 0.15f)
			{
				speed -= 0.05f;
			}
		}
		yield return true;
	}

	private void FoodGiftResult(GoodsDataRow goodsData)
	{
		FavorStepDataRow favorStepDataRow = RemoteObjectManager.instance.regulation.FindFavorStepData((int)commander.favorStep + 1);
		CommanderGiftDataRow commanderGiftDataRow = base.regulation.GetCommanderGift(goodsData.type);
		int num = 0;
		FavorDataRow favorData = commander.GetFavorData((int)commander.favorStep + 1);
		if ((!commander.possibleMarry) ? (favorData == null) : (((int)commander.marry == 1) ? (favorData == null) : ((int)commander.favorStep >= 13)))
		{
			return;
		}
		giftCount++;
		base.localUser.resourceList[goodsData.serverFieldName]--;
		num = commanderGiftDataRow.favorPoint;
		if ((int)commander.favorPoint + num >= favorStepDataRow.favor)
		{
			while ((int)commander.favorPoint + num >= favorStepDataRow.favor)
			{
				++commander.favorStep;
				commander.favorPoint = (int)commander.favorPoint + num - favorStepDataRow.favor;
				num = 0;
				favorStepDataRow = RemoteObjectManager.instance.regulation.FindFavorStepData((int)commander.favorStep + 1);
				bFavorStepUp = true;
				if (favorStepDataRow == null)
				{
					break;
				}
			}
		}
		else
		{
			RoCommander roCommander = commander;
			roCommander.favorPoint = (int)roCommander.favorPoint + num;
		}
		SoundManager.PlaySFX("SE_Training_001");
		GiftProgressEffect();
		SetFavor();
	}

	public void OnSlideValueChange()
	{
		float value = unitScaleSlider.value;
		if (unitObj != null)
		{
			unitObj.transform.localScale = new Vector3((40f + value * 80f) * unitOriginScale.x, (40f + value * 80f) * unitOriginScale.y, (40f + value * 80f) * unitOriginScale.z);
		}
	}

	public void OpenPopupShow()
	{
		base.Open();
		OnAnimOpen();
	}

	public void ClosePopup()
	{
		if (!bBackKeyEnable)
		{
			bBackKeyEnable = true;
			HidePopup();
			if (type == CommanderDetailType.Training && preCostume != commander.currentCostume.ToString())
			{
				RemoteObjectManager.instance.RequestChangeCommanderCostume(int.Parse(commander.id), commander.currentCostume);
			}
			preCommanderId = string.Empty;
		}
	}

	public override void Open()
	{
		base.Open();
		OnAnimOpen();
	}

	public override void Close()
	{
		base.Close();
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
		if (naviPopUp != null)
		{
			naviPopUp.Close();
		}
		if (voiceAudio != null)
		{
			string[] array = new string[0];
			array[0] = voiceAudio.clip.name;
			CacheManager.instance.SoundCache.CleanUp(array);
		}
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
		AnimLeft.Reset();
		AnimRight.Reset();
		AnimBlock.Reset();
		AnimLeft.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimRight.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveIn(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private IEnumerator OnEventHidePopup()
	{
		yield return new WaitForSeconds(0.8f);
		base.Close();
		bBackKeyEnable = false;
		bEnterKeyEnable = false;
		UISetter.SetActive(unitObj, active: false);
	}

	private void OnAnimClose()
	{
		AnimLeft.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimRight.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
		AnimBlock.MoveOut(GUIAnimSystemNGUI.eGUIMove.SelfAndChildren);
	}

	private void Update()
	{
		if (isZoom && Input.GetKeyDown(KeyCode.Escape))
		{
			CardScaleDownSize();
		}
	}

	private void OpenWeaponUI(bool open)
	{
		UISetter.SetActive(weaponRoot, open);
		UISetter.SetActive(UnitStat, !open);
		UnitIcon.transform.localPosition = ((!weaponRoot.activeSelf) ? new Vector3(-189f, 50f, 0f) : new Vector3(-178f, -95f, 0f));
		if (open)
		{
			unitScaleSlider.value = 0.7f;
			OnSlideValueChange();
		}
		unitRoot.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
		weaponGrid.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 2;
		cardRoot.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 3;
		skillScrollView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
		nextCommander.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 2;
	}

	private void SetWeapon()
	{
		UnitIcon.transform.localPosition = ((!weaponRoot.activeSelf) ? new Vector3(-189f, 50f, 0f) : new Vector3(-178f, -95f, 0f));
		if (lockWeaponSlot.Count == 0)
		{
			lockWeaponSlot.Add(int.Parse(base.regulation.defineDtbl["WEAPON_OPEN_CLASS_HEAD"].value));
			lockWeaponSlot.Add(int.Parse(base.regulation.defineDtbl["WEAPON_OPEN_CLASS_RIGHTHAND"].value));
			lockWeaponSlot.Add(int.Parse(base.regulation.defineDtbl["WEAPON_OPEN_CLASS_LEFTHAND"].value));
			lockWeaponSlot.Add(int.Parse(base.regulation.defineDtbl["WEAPON_OPEN_CLASS_BODY"].value));
			lockWeaponSlot.Add(int.Parse(base.regulation.defineDtbl["WEAPON_OPEN_CLASS_SPECIAL"].value));
		}
		weaponSkillDescription.Close();
		for (int i = 0; i < weaponList.Count; i++)
		{
			UICommanderWeaponItem uICommanderWeaponItem = weaponList[i];
			RoWeapon weapon = commander.FindWeaponItem(i + 1);
			List<RoWeapon> list = base.localUser.GetWeaponList(i + 1);
			if (lockWeaponSlot[i] > (int)commander.cls || list.Count == 0)
			{
				uICommanderWeaponItem.Lock(commander.unitId, i + 1);
			}
			else
			{
				uICommanderWeaponItem.Set(weapon, commander.unitId, i + 1);
			}
		}
		SelectWeapon(selectWeaponSlot);
	}

	private int GetFirstSelectId()
	{
		for (int i = 0; i < weaponList.Count; i++)
		{
			RoWeapon roWeapon = commander.FindWeaponItem(i + 1);
			if (roWeapon != null)
			{
				selectWeaponSlot = i + 1;
				break;
			}
		}
		return selectWeaponSlot;
	}

	private void SelectWeapon(int selectSlot)
	{
		RoWeapon roWeapon = commander.FindWeaponItem(selectSlot);
		selectWeaponItem.Set(roWeapon, commander.unitId, selectSlot);
		UISetter.SetLabel(weaponTakeLabel, (roWeapon == null) ? Localization.Get("5030411") : Localization.Get("5030418"));
		for (int i = 0; i < weaponList.Count; i++)
		{
			UICommanderWeaponItem uICommanderWeaponItem = weaponList[i];
			uICommanderWeaponItem.SetSelection(i + 1 == selectSlot);
		}
		UISetter.SetActive(weaponUpgradeBtn, roWeapon != null);
		UISetter.SetActive(weaponTakeOffBtn, roWeapon != null);
		UISetter.SetActive(weaponSkillTitle, roWeapon != null);
		if (roWeapon == null)
		{
			EmptyWeapon();
			return;
		}
		WeaponSetInfo(roWeapon.data.weaponSetType);
		UISetter.SetActive(weaponEmpty, active: false);
		WeaponDataRow data = roWeapon.data;
		UISetter.SetLabel(weaponName, Localization.Get(roWeapon.data.weaponName));
		UISetter.SetActive(weaponSkillInfoRoot, data.skillPoint != EWeaponSkill.None);
		UISetter.SetActive(weaponSkillEmpty, data.skillPoint == EWeaponSkill.None);
		if (data.skillPoint != 0)
		{
			string skillId = commander.GetSkillId((int)data.skillPoint);
			if (!string.Equals(skillId, "0"))
			{
				int skillLevel = commander.skillList[(int)(data.skillPoint - 1)];
				SkillDataRow skillDataRow = base.regulation.skillDtbl[skillId];
				weaponSkill.SetSkill(skillDataRow, skillLevel, 1, 1, isOpen: true, 1);
				UISetter.SetGameObjectName(weaponSkill.gameObject, $"WeaponSkill-{skillDataRow.key}");
			}
			else
			{
				UISetter.SetActive(weaponSkillInfoRoot, active: false);
				UISetter.SetActive(weaponSkillEmpty, active: true);
			}
		}
		bool active = true;
		for (int j = 0; j < data.statusEffectDrks.Count; j++)
		{
			if (data.statusEffectDrks[j] != "0")
			{
				active = false;
			}
			UISetter.SetActive(weaponSkillEffect[j], data.statusEffectDrks[j] != "0");
		}
		if (data.privateWeapon == 0)
		{
			UISetter.SetLabel(weaponSkillTitle, Localization.Get("70004"));
		}
		else if (data.skillPoint == EWeaponSkill.AddSkill)
		{
			UISetter.SetLabel(weaponSkillTitle, Localization.Format("70005", (int)data.skillPoint));
		}
		else
		{
			UISetter.SetLabel(weaponSkillTitle, Localization.Format("70943", (int)data.skillPoint));
		}
		UISetter.SetActive(weaponSkillEffectEmpty, active);
	}

	private void EmptyWeapon()
	{
		UISetter.SetActive(weaponEmpty, active: true);
		UISetter.SetActive(weaponSkillInfoRoot, active: false);
		UISetter.SetActive(weaponSkillEmpty, active: true);
		WeaponSetInfo();
	}

	private void OpenWeaponSkillEffectInfo(int idx)
	{
		RoWeapon roWeapon = commander.FindWeaponItem(selectWeaponSlot);
		if (roWeapon != null)
		{
			string text = ((idx != 0) ? Localization.Get("70011") : Localization.Get("70010"));
			string text2 = string.Empty;
			WeaponDataRow data = roWeapon.data;
			string skillId = commander.GetSkillId((int)data.skillPoint);
			SkillDataRow skillDataRow = base.regulation.skillDtbl[skillId];
			ESkillTargetType eSkillTargetType = ((skillDataRow.targetType == ESkillTargetType.Own || skillDataRow.targetType == ESkillTargetType.Friend) ? ESkillTargetType.Friend : ESkillTargetType.Enemy);
			if (data.targetType != eSkillTargetType)
			{
				text2 = ((data.targetType != ESkillTargetType.Friend) ? Localization.Get("70013") : Localization.Get("70014"));
			}
			weaponSkillDescription.SetLabel(text, text2, Localization.Get(data.explanation[idx]));
		}
	}

	public void WeaponSkillDescriptionClose()
	{
		weaponSkillDescription.Close();
	}

	private void OpenWeaponTakePopup()
	{
		if (weaponListPopup == null)
		{
			weaponListPopup = UIPopup.Create<UIWeaponListPopup>("WeaponListPopup");
			weaponListPopup.Set(selectWeaponSlot, 0, commander.id);
		}
	}

	private void OpenWeaponUpgradePopup()
	{
		RoWeapon roWeapon = commander.FindWeaponItem(selectWeaponSlot);
		if (weaponUpgradePopup == null && roWeapon != null)
		{
			weaponUpgradePopup = UIPopup.Create<UIWeaponUpgradePopup>("WeaponUpgradePopup");
			weaponUpgradePopup.Set(roWeapon.idx);
		}
	}

	private void SetWeaponSkillInfo(string id)
	{
		UISkillInfoPopup uISkillInfoPopup = UIPopup.Create<UISkillInfoPopup>("SkillInfoPopup");
		uISkillInfoPopup.Set(localization: true, "10056", null, null, null, Localization.Get("10048"), null);
		int weaponSkillLevel = commander.GetWeaponSkillLevel(selectWeaponSlot);
		uISkillInfoPopup.SetInfo(id, weaponSkillLevel);
	}

	private void WeaponSetInfo(string type = "0")
	{
		UISetter.SetActive(weaponSetLabel, type != "0");
		UISetter.SetActive(weaponSetEffectLabel, type != "0");
		UISetter.SetActive(weaponSetEffectLeftArrow, type != "0");
		UISetter.SetActive(weaponSetEffectRightArrow, type != "0");
		if (type != "0")
		{
			WeaponSetDataRow weaponSetDataRow = base.regulation.weaponSetDtbl[type];
			int num = commander.EnableWeaponSet(type);
			string spriteName = ((num != 5) ? "eq-arrow-off" : "eq-arrow-on");
			int num2 = ((num == 5) ? 1 : (-1));
			UISetter.SetSprite(weaponSetEffectLeftArrow, spriteName);
			UISetter.SetSprite(weaponSetEffectRightArrow, spriteName);
			UISetter.SetScale(weaponSetEffectLeftArrow, new Vector3(num2, 1f, 1f));
			UISetter.SetScale(weaponSetEffectRightArrow, new Vector3(num2 * -1, 1f, 1f));
			UISetter.SetColor(weaponSetEffectLabel, (num != 5) ? new Color(23f / 51f, 41f / 85f, 0.5137255f) : new Color(1f, 0.9411765f, 0f));
			weaponSetEffectLabel.effectColor = ((num != 5) ? Color.white : new Color(0.8f, 1f / 51f, 1f / 51f));
			UISetter.SetLabel(weaponSetLabel, Localization.Format("70948", num));
			string text = weaponSetDataRow.weaponSetStatAddPoint.ToString();
			if (weaponSetDataRow.weaponSetStatType == EItemSetType.CRITDMG || weaponSetDataRow.weaponSetStatType == EItemSetType.CRITR)
			{
				text += "%";
			}
			UISetter.SetLabel(weaponSetEffectLabel, $"{ItemSetTypeString(weaponSetDataRow.weaponSetStatType)} +{text}");
		}
	}

	private string ItemSetTypeString(EItemSetType setType)
	{
		return setType switch
		{
			EItemSetType.ATK => Localization.Get("1"), 
			EItemSetType.DEF => Localization.Get("2"), 
			EItemSetType.ACCUR => Localization.Get("5"), 
			EItemSetType.LUCK => Localization.Get("3"), 
			EItemSetType.CRITDMG => Localization.Get("8"), 
			EItemSetType.CRITR => Localization.Get("6"), 
			_ => string.Empty, 
		};
	}

	public void OpenAnimation()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("y", 0, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeOutBack));
	}

	public void CloseAnimation()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("y", -1000, "islocal", true, "time", 0.3, "delay", 0, "easeType", iTween.EaseType.easeInBack, "oncomplete", "Close", "oncompletetarget", base.gameObject));
	}

	public void OnCardScaleEvent()
	{
		if (!bTouchEnable)
		{
			bScaleCard = !bScaleCard;
			if (bScaleCard)
			{
				CardScaleUpSize();
			}
			else
			{
				CardScaleDownSize();
			}
		}
	}

	public void CardScaleUpSize()
	{
		ForwardBackKeyEvent.DTouchLock();
		bTouchEnable = true;
		UISetter.SetActive(Tab, active: false);
		iTween.MoveTo(card.gameObject, iTween.Hash("position", new Vector3(0f, 0f, -100f), "islocal", true, "time", 0.2f));
		iTween.ScaleTo(card.gameObject, iTween.Hash("scale", new Vector3(device_aspectX, device_aspectY, 1f), "islocal", true, "time", 0.2f));
		iTween.RotateTo(card.gameObject, iTween.Hash("z", 90, "islocal", true, "oncomplete", "OnCompScaleUp", "onCompleteTarget", base.gameObject, "time", 0.2f));
	}

	private void OnCompScaleUp()
	{
		bTouchEnable = false;
		isZoom = true;
		btnZoomExit.SetActive(value: true);
	}

	private void OnCompScaleDown()
	{
		bTouchEnable = false;
		btnZoomExit.SetActive(value: false);
		ForwardBackKeyEvent.DTouchUnLock();
		UISetter.SetActive(Tab, active: true);
	}

	public void CardScaleDownSize()
	{
		bTouchEnable = true;
		iTween.MoveTo(card.gameObject, iTween.Hash("position", new Vector3(-400f, -30f, 0f), "islocal", true, "time", 0.2f));
		iTween.ScaleTo(card.gameObject, iTween.Hash("scale", new Vector3(1f, 1f, 1f), "islocal", true, "time", 0.2f));
		iTween.RotateTo(card.gameObject, iTween.Hash("z", 0, "islocal", true, "oncomplete", "OnCompScaleDown", "onCompleteTarget", base.gameObject, "time", 0.2f));
	}

	public void SetNavigationOpen(EStorageType storageType, string id)
	{
		if (naviPopUp != null)
		{
			return;
		}
		ItemExchangeDataRow itemExchangeDataRow = base.regulation.itemExchangeDtbl.Find((ItemExchangeDataRow row) => row.type == storageType && row.typeidx == id);
		if (itemExchangeDataRow != null)
		{
			naviPopUp = UIPopup.Create<UINavigationPopUp>("NavigationPopUp");
			naviPopUp.Init(storageType, id, itemExchangeDataRow);
			if (storageType == EStorageType.Part)
			{
				naviPopUp.title.text = Localization.Get("8023");
			}
			else if (storageType == EStorageType.Medal)
			{
				naviPopUp.title.text = Localization.Get("5608");
			}
			else if (storageType == EStorageType.Food)
			{
				naviPopUp.title.text = Localization.Get("8023");
			}
		}
	}

	private void SetExchangeOpen()
	{
		UIPopup.Create<UIMedalExchangePopup>("MedalExchangePopup").Set(commander);
	}

	public void InitOpenPopup(string strId, CommanderDetailType type, List<RoCommander> list = null)
	{
		if (bEnterKeyEnable)
		{
			return;
		}
		SendOnInitToInnerParts();
		bEnterKeyEnable = true;
		if (list != null)
		{
			commanderList = list;
			switch (type)
			{
			case CommanderDetailType.Training:
				commanderList.RemoveAll((RoCommander comm) => comm.state != ECommanderState.Nomal);
				break;
			case CommanderDetailType.Recruit:
				commanderList.RemoveAll((RoCommander comm) => comm.state != ECommanderState.Undefined);
				break;
			}
		}
		InitAndOpenCard();
		Set(strId, type);
		OpenPopupShow();
		UISetter.SetActive(uiCommanderLeft.spine, active: false);
		selectWeaponSlot = 1;
		this.type = type;
		setActiveItemStatus(isActive: false);
		unitRoot.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 2;
		costumeScrollView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
		cardRoot.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 3;
		skillScrollView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
		favorStepListView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
		commanderGiftListView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
		scenarioTitleListView.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 1;
		nextCommander.GetComponent<UIPanel>().depth = base.gameObject.GetComponent<UIPanel>().depth + 2;
	}

	public void PopUpOpenEnd()
	{
		StartCoroutine(SetCommanderDetailSpineUnit());
	}

	public void ZoomExit()
	{
		if (isZoom)
		{
			isZoom = false;
			CardScaleDownSize();
		}
	}

	public void NotEnough(MultiplePopUpType type)
	{
		switch (type)
		{
		case MultiplePopUpType.NOTENOUGH_CASH:
			UISimplePopup.CreateBool(localization: true, "1031", "1032", null, "5348", "1000").onClick = delegate(GameObject sender)
			{
				string text2 = sender.name;
				if (text2 == "OK")
				{
					UIManager.instance.world.mainCommand.OpenDiamonShop();
				}
			};
			break;
		case MultiplePopUpType.NOTENOUGH_GOLD:
			UISimplePopup.CreateBool(localization: true, "1029", "1030", null, "1001", "1000").onClick = delegate(GameObject sender)
			{
				string text3 = sender.name;
				if (text3 == "OK")
				{
					UIManager.instance.world.camp.GoNavigation("MetroBank");
				}
			};
			break;
		}
		if (type != MultiplePopUpType.NOTENOUGH_RING)
		{
			return;
		}
		UISimplePopup.CreateBool(localization: true, "20099", "20104", null, "5348", "1000").onClick = delegate(GameObject sender)
		{
			string text = sender.name;
			if (text == "OK")
			{
				UIManager.instance.world.mainCommand.OpenDiamonShop();
			}
		};
	}

	public void CloseBuyContents()
	{
		SetCostume(commander.currentCostume.ToString(), isResetPosition: false);
		UISetter.SetActive(buyContents, active: false);
	}

	public void DecreaseItemStart()
	{
		skillLevelUpContents.DecreaseItemStart();
	}

	public void DecreaseItemEnd()
	{
		skillLevelUpContents.DecreaseItemEnd();
	}

	public void AddItemStart()
	{
		skillLevelUpContents.AddItemStart();
	}

	public void AddItemEnd()
	{
		skillLevelUpContents.AddItemEnd();
	}

	public void ItemCountMax()
	{
		skillLevelUpContents.ItemCountMax();
	}

	public void CloseSkillContents()
	{
		UISetter.SetActive(skillLevelUpContents, active: false);
	}

	public void SetInputValue()
	{
		skillLevelUpContents.SetInputValue();
	}

	protected override void OnEnablePopup()
	{
		UISetter.SetVoice(uiCommanderLeft.spine, active: true);
	}

	protected override void OnDisablePopup()
	{
		UISetter.SetVoice(uiCommanderLeft.spine, active: false);
	}
}
