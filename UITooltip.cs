using System;
using Shared.Regulation;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Tooltip")]
public class UITooltip : UIGoods
{
	private const int TooltipSizeX = 125;

	private const int TooltipSizeY = 75;

	protected static UITooltip mInstance;

	public Camera uiCamera;

	public UILabel lbName;

	public UILabel text;

	public UISprite background;

	public float appearSpeed = 10f;

	public bool scalingTransitions = true;

	protected GameObject mHover;

	protected Transform mTrans;

	protected float mTarget;

	protected float mCurrent;

	protected Vector3 mPos;

	protected Vector3 mSize = Vector3.zero;

	protected UIWidget[] mWidgets;

	public UILabel itemCount;

	public GameObject itemCountRoot;

	public UILabel commanderType;

	public UILabel commanderState;

	public GameObject commanderInfoRoot;

	public GameObject unitRoot;

	public UISprite unit;

	public GameObject challengeRoot;

	public UITroop userTroop;

	public static bool isVisible => mInstance != null && mInstance.mTarget == 1f;

	private void Awake()
	{
		mInstance = this;
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	protected virtual void Start()
	{
		mTrans = base.transform;
		mWidgets = GetComponentsInChildren<UIWidget>();
		mPos = mTrans.localPosition;
		if (uiCamera == null)
		{
			uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		base.gameObject.SetActive(value: false);
	}

	protected virtual void SetText(string name, string tooltipText)
	{
		if (text != null && !string.IsNullOrEmpty(tooltipText))
		{
			mTarget = 1f;
			mHover = UICamera.hoveredObject;
			lbName.text = name;
			text.text = tooltipText;
			mPos = UICamera.lastTouchPosition;
			Transform transform = text.transform;
			Vector3 localPosition = transform.localPosition;
			Vector3 localScale = transform.localScale;
			mSize = text.printedSize;
			mSize.x *= localScale.x;
			mSize.y *= localScale.y;
			if (uiCamera != null)
			{
				mPos.x = Mathf.Clamp01(mPos.x / (float)Screen.width);
				mPos.y = Mathf.Clamp01(mPos.y / (float)Screen.height);
				mTrans.position = uiCamera.ViewportToWorldPoint(mPos);
				mPos = mTrans.localPosition;
				mPos.x = Mathf.Round(mPos.x);
				mPos.y = Mathf.Round(mPos.y);
				if (mPos.x < (float)(Screen.width / 2))
				{
					mPos.x += 125f;
					mPos.y += 75f;
				}
				else if (mPos.x > (float)(Screen.width / 2))
				{
					mPos.x -= 125f;
					mPos.y += 75f;
				}
				mTrans.localPosition = mPos;
			}
			else
			{
				if (mPos.x + mSize.x > (float)Screen.width)
				{
					mPos.x = (float)Screen.width - mSize.x;
				}
				if (mPos.y - mSize.y < 0f)
				{
					mPos.y = mSize.y;
				}
				mPos.x -= (float)Screen.width * 0.5f;
				mPos.y -= (float)Screen.height * 0.5f;
			}
		}
		else
		{
			mHover = null;
			mTarget = 0f;
		}
	}

	protected virtual void SetPosition()
	{
		mTarget = 1f;
		mHover = UICamera.hoveredObject;
		mPos = UICamera.lastTouchPosition;
		Transform transform = text.transform;
		Vector3 localPosition = transform.localPosition;
		Vector3 localScale = transform.localScale;
		mSize = text.printedSize;
		mSize.x *= localScale.x;
		mSize.y *= localScale.y;
		if (uiCamera != null)
		{
			mPos.x = Mathf.Clamp01(mPos.x / (float)Screen.width);
			mPos.y = Mathf.Clamp01(mPos.y / (float)Screen.height);
			mTrans.position = uiCamera.ViewportToWorldPoint(mPos);
			mPos = mTrans.localPosition;
			mPos.x = Mathf.Round(mPos.x);
			mPos.y = Mathf.Round(mPos.y);
			if (mPos.x < (float)(Screen.width / 2))
			{
				mPos.x += 125f;
				mPos.y += 75f;
			}
			else if (mPos.x > (float)(Screen.width / 2))
			{
				mPos.x -= 125f;
				mPos.y += 75f;
			}
			mTrans.localPosition = mPos;
		}
		else
		{
			if (mPos.x + mSize.x > (float)Screen.width)
			{
				mPos.x = (float)Screen.width - mSize.x;
			}
			if (mPos.y - mSize.y < 0f)
			{
				mPos.y = mSize.y;
			}
			mPos.x -= (float)Screen.width * 0.5f;
			mPos.y -= (float)Screen.height * 0.5f;
		}
	}

	[Obsolete("Use UITooltip.Show instead")]
	public static void ShowText(string name, string text)
	{
		if (mInstance != null)
		{
			mInstance.SetText(name, text);
		}
	}

	public static void Show(string name, string text)
	{
		if (mInstance != null)
		{
			mInstance.gameObject.SetActive(value: true);
			mInstance.SetText(name, text);
		}
	}

	public static void Show(ETooltipType type, string idx)
	{
		if (mInstance != null)
		{
			mInstance.gameObject.SetActive(value: true);
			mInstance.Set(type, idx);
			mInstance.SetLabel(type, idx);
		}
	}

	public static void Show(string unitIdx, int level)
	{
		if (mInstance != null)
		{
			mInstance.gameObject.SetActive(value: true);
			mInstance.SetUnitLabel(unitIdx, level);
		}
	}

	public static void Show(RoUser user)
	{
		if (mInstance != null)
		{
			mInstance.gameObject.SetActive(value: true);
			mInstance.SetChallengeLabel(user);
		}
	}

	private void SetLabel(ETooltipType type, string idx)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetActive(itemCountRoot, type == ETooltipType.Goods || type == ETooltipType.UnitMaterial || type == ETooltipType.Medal || type == ETooltipType.Item || type == ETooltipType.Dormitory_NormalDeco || type == ETooltipType.Dormitory_AdvancedDeco || type == ETooltipType.Dormitory_Wallpaper || type == ETooltipType.Dormitory_CostumeBody);
		UISetter.SetActive(commanderInfoRoot, type == ETooltipType.Commander);
		UISetter.SetActive(unitRoot, active: false);
		UISetter.SetActive(dormitoryItemIcon, type == ETooltipType.Dormitory_NormalDeco || type == ETooltipType.Dormitory_AdvancedDeco || type == ETooltipType.Dormitory_Wallpaper || type == ETooltipType.Dormitory_CostumeBody || type == ETooltipType.Dormitory_CostumeHead);
		switch (type)
		{
		case ETooltipType.Goods:
		{
			GoodsDataRow goodsDataRow = regulation.goodsDtbl[idx];
			if (!localUser.resourceList.ContainsKey(goodsDataRow.serverFieldName))
			{
				UISetter.SetActive(itemCountRoot, active: false);
			}
			else if (goodsDataRow.max == 0)
			{
				if (goodsDataRow.type == "1001")
				{
					UISetter.SetLabel(itemCount, localUser.vipExp);
				}
				else
				{
					UISetter.SetLabel(itemCount, localUser.resourceList[goodsDataRow.serverFieldName]);
				}
			}
			else
			{
				UISetter.SetLabel(itemCount, $"{localUser.resourceList[goodsDataRow.serverFieldName]} / {goodsDataRow.max}");
			}
			break;
		}
		case ETooltipType.UnitMaterial:
		{
			PartDataRow partDataRow = regulation.FindPartData(idx);
			RoPart roPart = localUser.FindPart(idx);
			UISetter.SetLabel(itemCount, $"{roPart.count} / {partDataRow.max}");
			break;
		}
		case ETooltipType.Medal:
		{
			RoCommander roCommander = localUser.FindCommander(idx);
			UISetter.SetLabel(itemCount, roCommander.medal);
			break;
		}
		case ETooltipType.Commander:
		{
			RoCommander roCommander2 = localUser.FindCommander(idx);
			if (roCommander2.unitReg.job == EJob.Attack)
			{
				UISetter.SetLabel(commanderType, Localization.Get("1323"));
			}
			else if (roCommander2.unitReg.job == EJob.Defense)
			{
				UISetter.SetLabel(commanderType, Localization.Get("1324"));
			}
			else if (roCommander2.unitReg.job == EJob.Support)
			{
				UISetter.SetLabel(commanderType, Localization.Get("1325"));
			}
			UISetter.SetLabel(commanderState, (roCommander2.state != ECommanderState.Nomal) ? Localization.Get("10004") : Localization.Get("5033"));
			break;
		}
		case ETooltipType.Costume:
			UISetter.SetActive(itemCountRoot, active: false);
			break;
		case ETooltipType.Item:
			UISetter.SetLabel(itemCount, localUser.GetAllItemCount(idx));
			break;
		case ETooltipType.Dormitory_NormalDeco:
		{
			int num4 = 0;
			if (localUser.dormitory.invenData.itemNormal.ContainsKey(idx))
			{
				num4 = localUser.dormitory.invenData.itemNormal[idx].amount;
			}
			UISetter.SetLabel(itemCount, $"{num4} / {localUser.dormitory.config.itemAmountLimit}");
			break;
		}
		case ETooltipType.Dormitory_AdvancedDeco:
		{
			int num3 = 0;
			if (localUser.dormitory.invenData.itemAdvanced.ContainsKey(idx))
			{
				num3 = localUser.dormitory.invenData.itemAdvanced[idx].amount;
			}
			UISetter.SetLabel(itemCount, $"{num3} / {localUser.dormitory.config.itemAmountLimit}");
			break;
		}
		case ETooltipType.Dormitory_Wallpaper:
		{
			int num2 = 0;
			if (localUser.dormitory.invenData.itemWallpaper.ContainsKey(idx))
			{
				num2 = localUser.dormitory.invenData.itemWallpaper[idx].amount;
			}
			UISetter.SetLabel(itemCount, $"{num2} / {localUser.dormitory.config.itemAmountLimit}");
			break;
		}
		case ETooltipType.Dormitory_CostumeHead:
			UISetter.SetActive(itemCountRoot, active: false);
			break;
		case ETooltipType.Dormitory_CostumeBody:
		{
			int num = 0;
			if (localUser.dormitory.invenData.costumeBody.ContainsKey(idx))
			{
				num = localUser.dormitory.invenData.costumeBody[idx].amount;
			}
			UISetter.SetLabel(itemCount, $"{num} / {localUser.dormitory.config.itemAmountLimit}");
			break;
		}
		}
	}

	private void SetUnitLabel(string unitIdx, int level)
	{
		UISetter.SetActive(itemCountRoot, active: false);
		UISetter.SetActive(commanderInfoRoot, active: false);
		UISetter.SetActive(goodsRoot, active: false);
		UISetter.SetActive(commanderRoot, active: false);
		UISetter.SetActive(partIcon, active: false);
		UISetter.SetActive(medalIcon, active: false);
		UISetter.SetActive(costumeIcon, active: false);
		UISetter.SetActive(unitRoot, active: true);
		UISetter.SetActive(ItemIconRoot, active: false);
		UISetter.SetActive(weaponRoot, active: false);
		UISetter.SetActive(dormitoryItemIcon, active: false);
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UnitDataRow unitDataRow = regulation.unitDtbl[unitIdx];
		UISetter.SetSprite(unit, $"{unitDataRow.resourceName}_Front");
		UISetter.SetLabel(nickname, Localization.Get(unitDataRow.nameKey));
		UISetter.SetLabel(description, Localization.Get(unitDataRow.explanation));
	}

	private void SetChallengeLabel(RoUser user)
	{
		UISetter.SetActive(itemCountRoot, active: false);
		UISetter.SetActive(commanderInfoRoot, active: false);
		UISetter.SetActive(goodsRoot, active: false);
		UISetter.SetActive(commanderRoot, active: false);
		UISetter.SetActive(partIcon, active: false);
		UISetter.SetActive(medalIcon, active: false);
		UISetter.SetActive(unitRoot, active: false);
		UISetter.SetActive(costumeIcon, active: false);
		UISetter.SetActive(ItemIconRoot, active: false);
		UISetter.SetActive(weaponRoot, active: false);
		UISetter.SetActive(dormitoryItemIcon, active: false);
	}

	public static void Hide()
	{
		if (mInstance != null)
		{
			mInstance.mHover = null;
			mInstance.mTarget = 0f;
			mInstance.gameObject.SetActive(value: false);
		}
	}
}
