using UnityEngine;

public class UIScrollWrap : UIWrapContent
{
	protected Vector2 orgClipOffset;

	protected bool cacheScView;

	protected SpringPanel _spring;

	public SpringPanel spring
	{
		get
		{
			if (_spring == null)
			{
				_spring = mScroll.GetComponent<SpringPanel>();
			}
			return _spring;
		}
	}

	private void Awake()
	{
		CacheScrollView();
	}

	protected override void Start()
	{
		base.Start();
		orgClipOffset = mPanel.clipOffset;
	}

	protected new bool CacheScrollView()
	{
		if (cacheScView)
		{
			return true;
		}
		mTrans = base.transform;
		mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
		mScroll = mPanel.GetComponent<UIScrollView>();
		if (mScroll == null)
		{
			return false;
		}
		if (mScroll.movement == UIScrollView.Movement.Horizontal)
		{
			mHorizontal = true;
		}
		else
		{
			if (mScroll.movement != UIScrollView.Movement.Vertical)
			{
				return false;
			}
			mHorizontal = false;
		}
		cacheScView = true;
		return true;
	}

	public override void WrapContent()
	{
		WrapContent(forceUpdate: false);
	}

	public void WrapContent(bool forceUpdate)
	{
		float num = (float)(itemSize * mChildren.Count) * 0.5f;
		Vector3[] worldCorners = mPanel.worldCorners;
		for (int i = 0; i < 4; i++)
		{
			Vector3 position = worldCorners[i];
			position = mTrans.InverseTransformPoint(position);
			worldCorners[i] = position;
		}
		Vector3 vector = Vector3.Lerp(worldCorners[0], worldCorners[2], 0.5f);
		float num2 = num * 2f;
		if (mHorizontal)
		{
			float num3 = worldCorners[0].x - (float)itemSize;
			float num4 = worldCorners[2].x + (float)itemSize;
			int j = 0;
			for (int count = mChildren.Count; j < count; j++)
			{
				Transform transform = mChildren[j];
				float num5 = transform.localPosition.x - vector.x;
				if (num5 < 0f - num)
				{
					Vector3 localPosition = transform.localPosition;
					localPosition.x += num2;
					num5 = localPosition.x - vector.x;
					int num6 = Mathf.RoundToInt(localPosition.x / (float)itemSize);
					if (minIndex == maxIndex || (minIndex <= num6 && num6 <= maxIndex))
					{
						transform.localPosition = localPosition;
						UpdateItem(transform, j);
					}
					else if (forceUpdate)
					{
						UpdateItem(transform, j);
					}
				}
				else if (num5 > num)
				{
					Vector3 localPosition2 = transform.localPosition;
					localPosition2.x -= num2;
					num5 = localPosition2.x - vector.x;
					int num7 = Mathf.RoundToInt(localPosition2.x / (float)itemSize);
					if (minIndex == maxIndex || (minIndex <= num7 && num7 <= maxIndex))
					{
						transform.localPosition = localPosition2;
						UpdateItem(transform, j);
					}
					else if (forceUpdate)
					{
						UpdateItem(transform, j);
					}
				}
				else if (mFirstTime || forceUpdate)
				{
					UpdateItem(transform, j);
				}
				if (cullContent)
				{
					num5 += mPanel.clipOffset.x - mTrans.localPosition.x;
					if (!UICamera.IsPressed(transform.gameObject))
					{
						NGUITools.SetActive(transform.gameObject, num5 > num3 && num5 < num4, compatibilityMode: false);
					}
				}
			}
		}
		else
		{
			float num8 = worldCorners[0].y - (float)itemSize;
			float num9 = worldCorners[2].y + (float)itemSize;
			int k = 0;
			for (int count2 = mChildren.Count; k < count2; k++)
			{
				Transform transform2 = mChildren[k];
				float num10 = transform2.localPosition.y - vector.y;
				if (num10 < 0f - num)
				{
					Vector3 localPosition3 = transform2.localPosition;
					localPosition3.y += num2;
					num10 = localPosition3.y - vector.y;
					int num11 = Mathf.RoundToInt(localPosition3.y / (float)itemSize);
					if (minIndex == maxIndex || (minIndex <= num11 && num11 <= maxIndex))
					{
						transform2.localPosition = localPosition3;
						UpdateItem(transform2, k);
					}
					else if (forceUpdate)
					{
						UpdateItem(transform2, k);
					}
				}
				else if (num10 > num)
				{
					Vector3 localPosition4 = transform2.localPosition;
					localPosition4.y -= num2;
					num10 = localPosition4.y - vector.y;
					int num12 = Mathf.RoundToInt(localPosition4.y / (float)itemSize);
					if (minIndex == maxIndex || (minIndex <= num12 && num12 <= maxIndex))
					{
						transform2.localPosition = localPosition4;
						UpdateItem(transform2, k);
					}
					else if (forceUpdate)
					{
						UpdateItem(transform2, k);
					}
				}
				else if (mFirstTime || forceUpdate)
				{
					UpdateItem(transform2, k);
				}
				if (cullContent)
				{
					num10 += mPanel.clipOffset.y - mTrans.localPosition.y;
					if (!UICamera.IsPressed(transform2.gameObject))
					{
						NGUITools.SetActive(transform2.gameObject, num10 > num8 && num10 < num9, compatibilityMode: false);
					}
				}
			}
		}
		mScroll.InvalidateBounds();
	}

	[ContextMenu("ResetPosition")]
	public void ResetPosition()
	{
		if (spring != null)
		{
			spring.enabled = false;
		}
		mScroll.currentMomentum = Vector3.zero;
		mScroll.transform.localPosition = new Vector3(mScroll.transform.localPosition.x, 0f - orgClipOffset.y, 0f);
		mPanel.clipOffset = orgClipOffset;
		int i = 0;
		for (int count = mChildren.Count; i < count; i++)
		{
			Transform transform = mChildren[i];
			transform.localPosition = ((!mHorizontal) ? new Vector3(0f, -i * itemSize, 0f) : new Vector3(i * itemSize, 0f, 0f));
		}
		Refresh();
	}

	public new void Refresh()
	{
		WrapContent(forceUpdate: true);
	}
}
