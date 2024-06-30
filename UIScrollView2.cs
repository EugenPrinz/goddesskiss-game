using System.Collections.Generic;
using UnityEngine;

public class UIScrollView2 : UIScrollView
{
	public delegate void ChangeIndexDelegate(UIListItem item, int index);

	public float cellWidth;

	public float cellHeight;

	public int lineCount;

	private GameObject TemplatePrefab;

	private int itemCount;

	private Transform firstTemplate;

	private Transform lastTemplate;

	private Vector3 firstPosition = Vector3.zero;

	private Vector3 prevPosition = Vector3.zero;

	private List<UIListItem> items = new List<UIListItem>();

	private int minShowCount;

	private ChangeIndexDelegate callbackMethod;

	private UIListItem headItem => (items.Count > 0) ? items[0] : null;

	private UIListItem tailItem => (items.Count > 0) ? items[items.Count - 1] : null;

	private int maxCol
	{
		get
		{
			if (movement == Movement.Vertical)
			{
				return lineCount;
			}
			return Mathf.CeilToInt(base.panel.baseClipRegion.z / cellWidth);
		}
	}

	private int maxRow
	{
		get
		{
			if (movement == Movement.Horizontal)
			{
				return Mathf.CeilToInt(base.panel.baseClipRegion.w / cellHeight);
			}
			return lineCount;
		}
	}

	public override Bounds bounds
	{
		get
		{
			if (!mCalculatedBounds)
			{
				mCalculatedBounds = true;
				mBounds = CalculateRelativeWidgetBounds2();
			}
			return mBounds;
		}
	}

	public override void Awake()
	{
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
		firstPosition = mTrans.localPosition;
		prevPosition = mTrans.localPosition;
	}

	public void Destory()
	{
		RemoveAll();
	}

	public void Init(int count, ChangeIndexDelegate callback, GameObject pfObject)
	{
		callbackMethod = callback;
		if (TemplatePrefab != null && TemplatePrefab != pfObject)
		{
			RemoveAll();
		}
		TemplatePrefab = pfObject;
		itemCount = count;
		SetTemplate(count);
		if (movement == Movement.Vertical)
		{
			minShowCount = maxCol * (maxRow + 1);
		}
		else
		{
			minShowCount = (maxCol + 1) * maxRow;
		}
		int num = Mathf.Min(count, minShowCount);
		GameObject gameObject = null;
		UIListItem uIListItem = null;
		for (int i = 0; i < num; i++)
		{
			UIListItem uIListItem2 = null;
			if (i >= items.Count)
			{
				gameObject = NGUITools.AddChild(base.gameObject, TemplatePrefab);
				if (gameObject.GetComponent<UIDragScrollView>() == null)
				{
					gameObject.AddComponent<UIDragScrollView>().scrollView = this;
				}
				uIListItem2 = new UIListItem();
				uIListItem2.Target = gameObject;
				items.Add(uIListItem2);
			}
			else
			{
				uIListItem2 = items[i];
			}
			uIListItem2.SetIndex(i);
			if (!uIListItem2.Target.gameObject.activeSelf)
			{
				uIListItem2.Target.gameObject.SetActive(value: true);
			}
			uIListItem2.Prev = uIListItem;
			uIListItem2.Next = null;
			if (uIListItem != null)
			{
				uIListItem.Next = uIListItem2;
			}
			uIListItem = uIListItem2;
			callbackMethod(uIListItem2, i);
		}
		for (int j = num; j < items.Count; j++)
		{
			items[j].Target.gameObject.SetActive(value: false);
		}
		UpdatePosition();
	}

	public override bool RestrictWithinBounds(bool instant, bool horizontal, bool vertical, SpringPanel.OnFinished finish = null)
	{
		return base.RestrictWithinBounds(instant, horizontal, vertical, UpdateCurrentPosition);
	}

	public override void SetDragAmount(float x, float y, bool updateScrollbars)
	{
		base.SetDragAmount(x, y, updateScrollbars);
		UpdateCurrentPosition();
	}

	public override void MoveRelative(Vector3 relative)
	{
		base.MoveRelative(relative);
		UpdateCurrentPosition();
	}

	public void TailToHead()
	{
		int num = ((movement != Movement.Vertical) ? maxRow : maxCol);
		for (int i = 0; i < num; i++)
		{
			UIListItem uIListItem = tailItem;
			if (uIListItem == null || uIListItem == headItem)
			{
				break;
			}
			if (uIListItem.Prev != null)
			{
				uIListItem.Prev.Next = null;
			}
			uIListItem.Next = headItem;
			uIListItem.Prev = null;
			headItem.Prev = uIListItem;
			items.RemoveAt(items.Count - 1);
			items.Insert(0, uIListItem);
		}
	}

	public void HeadToTail()
	{
		int num = ((movement != Movement.Vertical) ? maxRow : maxCol);
		for (int i = 0; i < num; i++)
		{
			UIListItem uIListItem = headItem;
			if (uIListItem == null || uIListItem == tailItem)
			{
				break;
			}
			uIListItem.Next.Prev = null;
			uIListItem.Next = null;
			uIListItem.Prev = tailItem;
			tailItem.Next = uIListItem;
			items.RemoveAt(0);
			items.Insert(items.Count, uIListItem);
		}
	}

	private void SetTemplate(int count)
	{
		if (firstTemplate == null)
		{
			GameObject gameObject = NGUITools.AddChild(base.gameObject, TemplatePrefab);
			gameObject.SetActive(value: false);
			firstTemplate = gameObject.transform;
			firstTemplate.name = "first rect";
		}
		if (lastTemplate == null)
		{
			GameObject gameObject2 = NGUITools.AddChild(base.gameObject, TemplatePrefab);
			gameObject2.SetActive(value: false);
			lastTemplate = gameObject2.transform;
			lastTemplate.name = "last rect";
		}
		float num = base.panel.baseClipRegion.x - (base.panel.baseClipRegion.z - cellWidth) * 0.5f;
		float num2 = base.panel.baseClipRegion.y + (base.panel.baseClipRegion.w - cellHeight + base.panel.clipSoftness.y) * 0.5f;
		if (movement == Movement.Vertical)
		{
			firstTemplate.localPosition = new Vector3(num, num2, 0f);
			lastTemplate.localPosition = new Vector3(num + (float)(lineCount - 1) * cellWidth, num2 - cellHeight * (float)((count - 1) / lineCount), 0f);
		}
		else if (movement == Movement.Horizontal)
		{
			firstTemplate.localPosition = new Vector3(num, num2, 0f);
			lastTemplate.localPosition = new Vector3(num + cellWidth * (float)((count - 1) / lineCount), num2 - (float)(lineCount - 1) * cellHeight, 0f);
		}
		mCalculatedBounds = true;
		Vector3 vector = base.panel.CalculateConstrainOffset(bounds.min, bounds.max);
		SpringPanel.Begin(base.panel.gameObject, mTrans.localPosition + vector, 13f, UpdateCurrentPosition);
	}

	public void UpdateCurrentPosition()
	{
		if (items.Count == 0)
		{
			return;
		}
		Vector3 vector = firstPosition - mTrans.localPosition;
		if (movement == Movement.Vertical)
		{
			bool flag = vector.y > prevPosition.y;
			int num = maxCol;
			int value = (int)((0f - vector.y) / cellHeight) * num;
			value = Mathf.Clamp(value, 0, itemCount - 1);
			if (headItem.Index != value)
			{
				if (flag)
				{
					TailToHead();
				}
				else
				{
					HeadToTail();
				}
				SetIndexHeadtoTail(value);
				UpdatePosition();
			}
		}
		else if (movement == Movement.Horizontal)
		{
			bool flag2 = vector.x > prevPosition.x;
			int num2 = maxRow;
			int value2 = (int)(vector.x / cellWidth) * num2;
			value2 = Mathf.Clamp(value2, 0, itemCount - num2);
			if (headItem.Index != value2)
			{
				if (flag2)
				{
					TailToHead();
				}
				else
				{
					HeadToTail();
				}
				SetIndexHeadtoTail(value2);
				UpdatePosition();
			}
		}
		prevPosition = vector;
	}

	public void SetIndexHeadtoTail(int headIndex)
	{
		UIListItem uIListItem = null;
		int num = -1;
		for (int i = 0; i < items.Count; i++)
		{
			num = i + headIndex;
			uIListItem = items[i];
			if (uIListItem.Index != num)
			{
				uIListItem.SetIndex(num);
				callbackMethod(uIListItem, num);
			}
		}
	}

	public void SetIndexTailtoHead(int tailIndex)
	{
		UIListItem uIListItem = null;
		int num = -1;
		int count = items.Count;
		for (int i = 0; i < count; i++)
		{
			num = tailIndex - i;
			uIListItem = items[count - i - 1];
			if (uIListItem.Index != num)
			{
				uIListItem.SetIndex(num);
				callbackMethod(uIListItem, num);
			}
		}
	}

	private new void UpdatePosition()
	{
		float num = base.panel.baseClipRegion.x - (base.panel.baseClipRegion.z - cellWidth) * 0.5f;
		float num2 = base.panel.baseClipRegion.y + (base.panel.baseClipRegion.w - cellHeight + base.panel.clipSoftness.y) * 0.5f;
		int num3 = maxCol;
		int num4 = maxRow;
		Vector3 zero = Vector3.zero;
		Transform transform = null;
		int num5 = 0;
		int num6 = 0;
		for (int i = 0; i < items.Count; i++)
		{
			zero = Vector3.zero;
			transform = items[i].Target.transform;
			if (movement == Movement.Vertical)
			{
				num5 = items[i].Index / num3;
				num6 = items[i].Index - num3 * num5;
				zero.x += num + (float)num6 * cellWidth;
				zero.y -= 0f - num2 + (float)num5 * cellHeight;
			}
			else if (movement == Movement.Horizontal)
			{
				num5 = items[i].Index / num4;
				num6 = items[i].Index - num4 * num5;
				zero.x += num + (float)num5 * cellWidth;
				zero.y -= 0f - num2 + (float)num6 * cellHeight;
			}
			transform.localPosition = zero;
			transform.name = $"item index:{items[i].Index}";
		}
	}

	public void RemoveItem(UIListItem item)
	{
		if (item.Prev != null)
		{
			item.Prev.Next = item.Next;
		}
		if (item.Next != null)
		{
			item.Next.Prev = item.Prev;
		}
		UIListItem uIListItem = item.Next as UIListItem;
		int index = item.Index;
		while (uIListItem != null)
		{
			int index2 = uIListItem.Index;
			uIListItem.Index = index;
			callbackMethod(uIListItem, uIListItem.Index);
			index = index2;
			uIListItem = uIListItem.Next as UIListItem;
		}
		UIListItem uIListItem2 = tailItem;
		items.Remove(item);
		if (itemCount < minShowCount)
		{
			Object.DestroyImmediate(item.Target);
		}
		else if (item == uIListItem2 || tailItem.Index >= itemCount - 1)
		{
			headItem.Prev = item;
			item.Next = headItem;
			item.Prev = null;
			item.Index = headItem.Index - 1;
			items.Insert(0, item);
			callbackMethod(item, item.Index);
			Vector3 vector = base.panel.CalculateConstrainOffset(bounds.min, bounds.max);
			SpringPanel.Begin(base.panel.gameObject, mTrans.localPosition + vector, 13f, UpdateCurrentPosition);
		}
		else
		{
			tailItem.Next = item;
			item.Prev = tailItem;
			item.Next = null;
			item.Index = tailItem.Index + 1;
			items.Add(item);
			callbackMethod(item, item.Index);
		}
		UpdatePosition();
	}

	public void RemoveAll()
	{
		UIListItem uIListItem = null;
		for (int i = 0; i < items.Count; i++)
		{
			uIListItem = items[i];
			Object.DestroyImmediate(uIListItem.Target);
		}
		items.Clear();
	}

	public void AddItem(int index)
	{
	}

	private Bounds CalculateRelativeWidgetBounds2()
	{
		if (firstTemplate == null || lastTemplate == null)
		{
			return new Bounds(Vector3.zero, Vector3.zero);
		}
		UIWidget[] componentsInChildren = firstTemplate.GetComponentsInChildren<UIWidget>(includeInactive: true);
		UIWidget[] componentsInChildren2 = lastTemplate.GetComponentsInChildren<UIWidget>(includeInactive: true);
		if (componentsInChildren.Length == 0 || componentsInChildren2.Length == 0)
		{
			return new Bounds(Vector3.zero, Vector3.zero);
		}
		Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		Matrix4x4 worldToLocalMatrix = mTrans.worldToLocalMatrix;
		bool flag = false;
		int num = componentsInChildren.Length;
		for (int i = 0; i < num; i++)
		{
			UIWidget uIWidget = componentsInChildren[i];
			Vector3[] worldCorners = uIWidget.worldCorners;
			for (int j = 0; j < 4; j++)
			{
				Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(worldCorners[j]);
				vector2 = Vector3.Max(lhs, vector2);
				vector = Vector3.Min(lhs, vector);
			}
			flag = true;
		}
		worldToLocalMatrix = mTrans.worldToLocalMatrix;
		int num2 = componentsInChildren2.Length;
		for (int k = 0; k < num2; k++)
		{
			UIWidget uIWidget2 = componentsInChildren2[k];
			Vector3[] worldCorners2 = uIWidget2.worldCorners;
			for (int l = 0; l < 4; l++)
			{
				Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(worldCorners2[l]);
				vector2 = Vector3.Max(lhs, vector2);
				vector = Vector3.Min(lhs, vector);
			}
			flag = true;
		}
		if (flag)
		{
			Bounds result = new Bounds(vector, Vector3.zero);
			result.Encapsulate(vector2);
			return result;
		}
		return new Bounds(Vector3.zero, Vector3.zero);
	}
}
