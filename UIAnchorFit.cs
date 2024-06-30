using System.Collections;
using UnityEngine;

public class UIAnchorFit : MonoBehaviour
{
	private UIAnchor _anchor;

	private float adjust;

	private void Awake()
	{
		_anchor = GetComponent<UIAnchor>();
		UpdateFit();
	}

	private IEnumerator Start()
	{
		yield return null;
		UpdateFit();
	}

	public void UpdateFit()
	{
		UIRoot uIRoot = UIRoot.list[0];
		if (_anchor == null)
		{
			_anchor = GetComponent<UIAnchor>();
		}
		if (adjust != uIRoot.pixelSizeAdjustment)
		{
			adjust = uIRoot.pixelSizeAdjustment;
			if (_anchor.side == UIAnchor.Side.Left)
			{
				Vector2 screenSize = NGUITools.screenSize;
				float num = screenSize.x * adjust;
				float num2 = screenSize.y * adjust;
				_anchor.relativeOffset.x = (num / (float)uIRoot.manualWidth - 1f) * 0.5f;
			}
			else if (_anchor.side == UIAnchor.Side.Right)
			{
				Vector2 screenSize2 = NGUITools.screenSize;
				float num3 = screenSize2.x * adjust;
				float num4 = screenSize2.y * adjust;
				_anchor.relativeOffset.x = (1f - num3 / (float)uIRoot.manualWidth) * 0.5f;
			}
			_anchor.enabled = true;
		}
	}
}
