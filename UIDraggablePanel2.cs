using System.Collections.Generic;
using UnityEngine;

public class UIDraggablePanel2 : UIScrollView
{
	public enum EArrangement
	{
		Vertical,
		Horizontal
	}

	public delegate void ChangeIndexDelegate(UIListItem item, int index);

	public EArrangement Arrangement;

	public float CellWidth;

	public float CellHeight;

	public int LineCount;

	public GameObject TemplatePrefab;

	private int ItemCount;

	private Transform mFirstTemplate;

	private Transform mLastTemplate;

	private Vector3 mFirstPosition = Vector3.zero;

	private Vector3 mPrevPosition = Vector3.zero;

	private List<UIListItem> mList = new List<UIListItem>();

	private int mMinShowCount;

	private ChangeIndexDelegate mCallback;

	protected Bounds mBaseBounds;

	private UIListItem Head => (mList.Count > 0) ? mList[0] : null;

	private UIListItem Tail => (mList.Count > 0) ? mList[mList.Count - 1] : null;

	private int maxCol
	{
		get
		{
			if (Arrangement == EArrangement.Vertical)
			{
				return LineCount;
			}
			return Mathf.CeilToInt(base.panel.baseClipRegion.z / CellWidth);
		}
	}

	private int maxRow
	{
		get
		{
			if (Arrangement == EArrangement.Vertical)
			{
				return Mathf.CeilToInt(base.panel.baseClipRegion.w / CellHeight);
			}
			return LineCount;
		}
	}

	public new Bounds bounds
	{
		get
		{
			if (!mCalculatedBounds)
			{
				mCalculatedBounds = true;
				mBounds = CalculateRelativeWidgetBounds2(mTrans, mFirstTemplate, mLastTemplate);
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
		mFirstPosition = mTrans.localPosition;
		mPrevPosition = mTrans.localPosition;
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		RemoveAll();
	}

	public void Init(int count, ChangeIndexDelegate callback)
	{
		mCallback = callback;
		ItemCount = count;
		SetTemplate(count);
		if (Arrangement == EArrangement.Vertical)
		{
			mMinShowCount = maxCol * (maxRow + 1);
		}
		else
		{
			mMinShowCount = maxRow * (maxCol + 1);
		}
		int num = Mathf.Min(count, mMinShowCount);
		int num2 = 0;
		if (mList.Count > 0)
		{
			num2 = mList[0].Index;
			if (num2 + num > ItemCount)
			{
				num2 = ItemCount - num;
				if (num2 < 0)
				{
					num2 = 0;
				}
			}
		}
		GameObject gameObject = null;
		UIListItem uIListItem = null;
		for (int i = 0; i < num; i++)
		{
			UIListItem uIListItem2 = null;
			if (i >= mList.Count)
			{
				gameObject = NGUITools.AddChild(base.gameObject, TemplatePrefab);
				if (gameObject.GetComponent<UIDragScrollView>() == null)
				{
					gameObject.AddComponent<UIDragScrollView>().draggablePanel = this;
				}
				uIListItem2 = new UIListItem();
				uIListItem2.Target = gameObject;
				mList.Add(uIListItem2);
			}
			else
			{
				uIListItem2 = mList[i];
				uIListItem2.Target.gameObject.SetActive(value: true);
			}
			uIListItem2.SetIndex(i + num2);
			uIListItem2.Prev = uIListItem;
			uIListItem2.Next = null;
			if (uIListItem != null)
			{
				uIListItem.Next = uIListItem2;
			}
			uIListItem = uIListItem2;
			mCallback(uIListItem2, i + num2);
		}
		mBaseBounds = bounds;
		for (int j = num; j < mList.Count; j++)
		{
			mList[j].Target.gameObject.SetActive(value: false);
		}
		UpdatePosition();
		Vector3 vector = base.panel.CalculateConstrainOffset(bounds.min, bounds.max);
		SpringPanel.Begin(base.panel.gameObject, mTrans.localPosition + vector, 13f, UpdateCurrentPosition);
	}

	public override void SetDragAmount(float x, float y, bool updateScrollbars)
	{
		if (mPanel == null)
		{
			mPanel = GetComponent<UIPanel>();
		}
		DisableSpring();
		Bounds bounds = mBaseBounds;
		if (bounds.min.x == bounds.max.x || bounds.min.y == bounds.max.y)
		{
			return;
		}
		Vector4 finalClipRegion = mPanel.finalClipRegion;
		float num = finalClipRegion.z * 0.5f;
		float num2 = finalClipRegion.w * 0.5f;
		float num3 = bounds.min.x + num;
		float num4 = bounds.max.x - num;
		float num5 = bounds.min.y + num2;
		float num6 = bounds.max.y - num2;
		if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
		{
			num3 -= mPanel.clipSoftness.x;
			num4 += mPanel.clipSoftness.x;
			num5 -= mPanel.clipSoftness.y;
			num6 += mPanel.clipSoftness.y;
		}
		float num7 = Mathf.Lerp(num3, num4, x);
		float num8 = Mathf.Lerp(num6, num5, y);
		if (!updateScrollbars)
		{
			Vector3 localPosition = mTrans.localPosition;
			if (base.canMoveHorizontally)
			{
				localPosition.x += finalClipRegion.x - num7;
			}
			if (base.canMoveVertically)
			{
				localPosition.y += finalClipRegion.y - num8;
			}
			mTrans.localPosition = localPosition;
		}
		if (base.canMoveHorizontally)
		{
			finalClipRegion.x = num7;
		}
		if (base.canMoveVertically)
		{
			finalClipRegion.y = num8;
		}
		Vector4 baseClipRegion = mPanel.baseClipRegion;
		mPanel.clipOffset = new Vector2(finalClipRegion.x - baseClipRegion.x, finalClipRegion.y - baseClipRegion.y);
		if (updateScrollbars)
		{
			UpdateScrollbars(mDragID == -10);
		}
		UpdateCurrentPosition();
	}

	public bool Contains(string id)
	{
		if (string.IsNullOrEmpty(id) || mList == null || mList.Count <= 0)
		{
			return false;
		}
		return mList.Find((UIListItem item) => item.Target.name == id) != null;
	}

	public bool Contains(GameObject go)
	{
		if (go == null)
		{
			return false;
		}
		return mList.Find((UIListItem item) => item.Target == go) != null;
	}

	public override void MoveRelative(Vector3 relative)
	{
		base.MoveRelative(relative);
		UpdateCurrentPosition();
	}

	public void TailToHead()
	{
		int num = ((Arrangement != 0) ? maxRow : maxCol);
		for (int i = 0; i < num; i++)
		{
			UIListItem tail = Tail;
			if (tail == null || tail == Head)
			{
				break;
			}
			if (tail.Prev != null)
			{
				tail.Prev.Next = null;
			}
			tail.Next = Head;
			tail.Prev = null;
			Head.Prev = tail;
			mList.RemoveAt(mList.Count - 1);
			mList.Insert(0, tail);
		}
	}

	public void HeadToTail()
	{
		int num = ((Arrangement != 0) ? maxRow : maxCol);
		for (int i = 0; i < num; i++)
		{
			UIListItem head = Head;
			if (head == null || head == Tail)
			{
				break;
			}
			head.Next.Prev = null;
			head.Next = null;
			head.Prev = Tail;
			Tail.Next = head;
			mList.RemoveAt(0);
			mList.Insert(mList.Count, head);
		}
	}

	private void SetTemplate(int count)
	{
		if (mFirstTemplate == null)
		{
			GameObject gameObject = NGUITools.AddChild(base.gameObject, TemplatePrefab);
			gameObject.SetActive(value: false);
			mFirstTemplate = gameObject.transform;
			mFirstTemplate.name = "first rect";
		}
		if (mLastTemplate == null)
		{
			GameObject gameObject2 = NGUITools.AddChild(base.gameObject, TemplatePrefab);
			gameObject2.SetActive(value: false);
			mLastTemplate = gameObject2.transform;
			mLastTemplate.name = "last rect";
		}
		float num = base.panel.baseClipRegion.x - (base.panel.baseClipRegion.z - CellWidth) * 0.5f;
		float num2 = base.panel.baseClipRegion.y + (base.panel.baseClipRegion.w - CellHeight + base.panel.clipSoftness.y) * 0.5f;
		if (Arrangement == EArrangement.Vertical)
		{
			mFirstTemplate.localPosition = new Vector3(num, num2, 0f);
			mLastTemplate.localPosition = new Vector3(num + (float)(LineCount - 1) * CellWidth, num2 - CellHeight * (float)((count - 1) / LineCount), 0f);
		}
		else
		{
			mFirstTemplate.localPosition = new Vector3(num, num2, 0f);
			mLastTemplate.localPosition = new Vector3(num + CellWidth * (float)((count - 1) / LineCount), num2 - (float)(LineCount - 1) * CellHeight, 0f);
		}
		mCalculatedBounds = true;
		mBounds = CalculateRelativeWidgetBounds2(mTrans, mFirstTemplate, mLastTemplate);
	}

	public void UpdateCurrentPosition()
	{
		Vector3 vector = mFirstPosition - mTrans.localPosition;
		if (Arrangement == EArrangement.Vertical)
		{
			bool flag = vector.y > mPrevPosition.y;
			int num = maxCol;
			if (flag)
			{
				int value = (int)((0f - vector.y) / CellHeight) * num;
				value = Mathf.Clamp(value, 0, ItemCount - num);
				if (Head != null && Head.Index != value && value <= ItemCount - mList.Count)
				{
					TailToHead();
					SetIndexHeadtoTail(value);
					UpdatePosition();
				}
			}
			else
			{
				int value2 = Mathf.CeilToInt((0f - vector.y + base.panel.baseClipRegion.w) / CellHeight) * num - 1;
				value2 = Mathf.Clamp(value2, 0, ItemCount - 1);
				if (Tail != null && Tail.Index != value2 && value2 >= mList.Count)
				{
					HeadToTail();
					SetIndexTailtoHead(value2);
					UpdatePosition();
				}
			}
		}
		else
		{
			bool flag2 = vector.x > mPrevPosition.x;
			int num2 = maxRow;
			if (!flag2)
			{
				int value3 = (int)(vector.x / CellWidth) * num2;
				value3 = Mathf.Clamp(value3, 0, ItemCount - num2);
				if (Head != null && Head.Index != value3 && value3 <= ItemCount - mList.Count)
				{
					TailToHead();
					SetIndexHeadtoTail(value3);
					UpdatePosition();
				}
			}
			else
			{
				int value4 = Mathf.CeilToInt((vector.x + base.panel.baseClipRegion.z) / CellWidth) * num2 - 1;
				value4 = Mathf.Clamp(value4, 0, ItemCount - 1);
				if (Tail != null && Tail.Index != value4 && value4 >= mList.Count)
				{
					HeadToTail();
					SetIndexTailtoHead(value4);
					UpdatePosition();
				}
			}
		}
		mPrevPosition = vector;
	}

	public void SetIndexHeadtoTail(int headIndex)
	{
		UIListItem uIListItem = null;
		int num = -1;
		for (int i = 0; i < mList.Count; i++)
		{
			num = i + headIndex;
			if (num < 0)
			{
				break;
			}
			uIListItem = mList[i];
			uIListItem.SetIndex(num);
			mCallback(uIListItem, num);
		}
	}

	public void SetIndexTailtoHead(int tailIndex)
	{
		UIListItem uIListItem = null;
		int num = -1;
		int count = mList.Count;
		for (int i = 0; i < count; i++)
		{
			num = tailIndex - i;
			if (num < 0)
			{
				break;
			}
			uIListItem = mList[count - i - 1];
			uIListItem.SetIndex(num);
			mCallback(uIListItem, num);
		}
	}

	private new void UpdatePosition()
	{
		float num = base.panel.baseClipRegion.x - (base.panel.baseClipRegion.z - CellWidth) * 0.5f;
		float num2 = base.panel.baseClipRegion.y + (base.panel.baseClipRegion.w - CellHeight + base.panel.clipSoftness.y) * 0.5f;
		if (Arrangement == EArrangement.Vertical)
		{
			int num3 = maxCol;
			for (int i = 0; i < mList.Count; i++)
			{
				Transform transform = mList[i].Target.transform;
				Vector3 zero = Vector3.zero;
				int num4 = mList[i].Index / num3;
				int num5 = mList[i].Index - num3 * num4;
				zero.x += num + (float)num5 * CellWidth;
				zero.y -= 0f - num2 + (float)num4 * CellHeight;
				transform.localPosition = zero;
			}
		}
		else
		{
			int num6 = maxRow;
			for (int j = 0; j < mList.Count; j++)
			{
				Transform transform2 = mList[j].Target.transform;
				Vector3 zero2 = Vector3.zero;
				int num7 = mList[j].Index / num6;
				int num8 = mList[j].Index - num6 * num7;
				zero2.x += num + (float)num7 * CellWidth;
				zero2.y -= 0f - num2 + (float)num8 * CellHeight;
				transform2.localPosition = zero2;
			}
		}
	}

	public void RemoveItem(int index)
	{
		UIListItem uIListItem = mList[index];
		if (uIListItem.Prev != null)
		{
			uIListItem.Prev.Next = uIListItem.Next;
		}
		if (uIListItem.Next != null)
		{
			uIListItem.Next.Prev = uIListItem.Prev;
		}
		UIListItem uIListItem2 = uIListItem.Next as UIListItem;
		int index2 = uIListItem.Index;
		while (uIListItem2 != null)
		{
			int index3 = uIListItem2.Index;
			uIListItem2.Index = index2;
			mCallback(uIListItem2, uIListItem2.Index);
			index2 = index3;
			uIListItem2 = uIListItem2.Next as UIListItem;
		}
		UIListItem tail = Tail;
		mList.Remove(uIListItem);
		if (ItemCount < mMinShowCount)
		{
			Object.DestroyImmediate(uIListItem.Target);
		}
		else if (uIListItem == tail || Tail.Index >= ItemCount - 1)
		{
			Head.Prev = uIListItem;
			uIListItem.Next = Head;
			uIListItem.Prev = null;
			uIListItem.Index = Head.Index - 1;
			mList.Insert(0, uIListItem);
			mCallback(uIListItem, uIListItem.Index);
			Vector3 vector = base.panel.CalculateConstrainOffset(bounds.min, bounds.max);
			SpringPanel.Begin(base.panel.gameObject, mTrans.localPosition + vector, 13f, UpdateCurrentPosition);
		}
		else
		{
			Tail.Next = uIListItem;
			uIListItem.Prev = Tail;
			uIListItem.Next = null;
			uIListItem.Index = Tail.Index + 1;
			mList.Add(uIListItem);
			mCallback(uIListItem, uIListItem.Index);
		}
		UpdatePosition();
	}

	public void RemoveAll()
	{
		UIListItem uIListItem = null;
		for (int i = 0; i < mList.Count; i++)
		{
			uIListItem = mList[i];
			Object.DestroyImmediate(uIListItem.Target);
		}
		mList.Clear();
	}

	public void AddItem(int index)
	{
	}

	public static Bounds CalculateRelativeWidgetBounds2(Transform root, Transform firstTemplate, Transform lastTemplate)
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
		Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
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

	public override void UpdateScrollbars(bool recalculateBounds)
	{
		if (mPanel == null)
		{
			return;
		}
		if (horizontalScrollBar != null || verticalScrollBar != null)
		{
			if (recalculateBounds)
			{
				mCalculatedBounds = false;
				mShouldMove = shouldMove;
			}
			Bounds bounds = mBaseBounds;
			Vector2 vector = bounds.min;
			Vector2 vector2 = bounds.max;
			if (horizontalScrollBar != null && vector2.x > vector.x)
			{
				Vector4 finalClipRegion = mPanel.finalClipRegion;
				int num = Mathf.RoundToInt(finalClipRegion.z);
				if (((uint)num & (true ? 1u : 0u)) != 0)
				{
					num--;
				}
				float f = (float)num * 0.5f;
				f = Mathf.Round(f);
				if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
				{
					f -= mPanel.clipSoftness.x;
				}
				float contentSize = CellWidth * (float)ItemCount;
				float viewSize = f * 2f;
				float x = vector.x;
				float x2 = vector2.x;
				float num2 = finalClipRegion.x - f;
				float num3 = finalClipRegion.x + f;
				x = num2 - x;
				x2 -= num3;
				UpdateScrollbars(horizontalScrollBar, x, x2, contentSize, viewSize, inverted: false);
			}
			if (verticalScrollBar != null && vector2.y > vector.y)
			{
				Vector4 finalClipRegion2 = mPanel.finalClipRegion;
				int num4 = Mathf.RoundToInt(finalClipRegion2.w);
				if (((uint)num4 & (true ? 1u : 0u)) != 0)
				{
					num4--;
				}
				float f2 = (float)num4 * 0.5f;
				f2 = Mathf.Round(f2);
				if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
				{
					f2 -= mPanel.clipSoftness.y;
				}
				float contentSize2 = CellHeight * (float)ItemCount;
				float viewSize2 = f2 * 2f;
				float y = vector.y;
				float y2 = vector2.y;
				float num5 = finalClipRegion2.y - f2;
				float num6 = finalClipRegion2.y + f2;
				y = num5 - y;
				y2 -= num6;
				UpdateScrollbars(verticalScrollBar, y, y2, contentSize2, viewSize2, inverted: true);
			}
		}
		else if (recalculateBounds)
		{
			mCalculatedBounds = false;
		}
	}
}
