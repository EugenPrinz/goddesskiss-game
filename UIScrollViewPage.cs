using System;
using UnityEngine;

public class UIScrollViewPage : MonoBehaviour
{
	private UIScrollView mScrollView;

	public float childWidth;

	public float childHeight;

	private void Start()
	{
		Init();
	}

	private void OnDisable()
	{
		if ((bool)mScrollView)
		{
			mScrollView.centerOnChild = null;
		}
	}

	private void OnDragFinished()
	{
		if (base.enabled)
		{
			Paging();
		}
	}

	public void Init()
	{
		if (!(mScrollView == null))
		{
			return;
		}
		mScrollView = NGUITools.FindInParents<UIScrollView>(base.gameObject);
		if (mScrollView == null)
		{
			base.enabled = false;
			return;
		}
		if ((bool)mScrollView)
		{
			UIScrollView uIScrollView = mScrollView;
			uIScrollView.onDragFinished = (UIScrollView.OnDragNotification)Delegate.Combine(uIScrollView.onDragFinished, new UIScrollView.OnDragNotification(OnDragFinished));
		}
		if (mScrollView.horizontalScrollBar != null)
		{
			UIProgressBar horizontalScrollBar = mScrollView.horizontalScrollBar;
			horizontalScrollBar.onDragFinished = (UIProgressBar.OnDragFinished)Delegate.Combine(horizontalScrollBar.onDragFinished, new UIProgressBar.OnDragFinished(OnDragFinished));
		}
		if (mScrollView.verticalScrollBar != null)
		{
			UIProgressBar verticalScrollBar = mScrollView.verticalScrollBar;
			verticalScrollBar.onDragFinished = (UIProgressBar.OnDragFinished)Delegate.Combine(verticalScrollBar.onDragFinished, new UIProgressBar.OnDragFinished(OnDragFinished));
		}
	}

	public void Paging()
	{
		if (mScrollView == null)
		{
			return;
		}
		Vector2 clipOffset = mScrollView.panel.clipOffset;
		if (mScrollView.canMoveHorizontally)
		{
			int num = (int)(clipOffset.x / childWidth);
			float num2 = clipOffset.x - childWidth * (float)num;
			if (!(num2 >= childWidth / 2f))
			{
			}
		}
	}
}
