using UnityEngine;

public class UIRootContent : MonoBehaviour
{
	private UIRoot _root;

	public float enableFitHeightVal = 2f;

	private void Awake()
	{
		_root = GetComponent<UIRoot>();
		_root.scalingStyle = UIRoot.Scaling.ConstrainedOnMobiles;
		UpdateFit();
	}

	public void UpdateFit()
	{
		Vector2 screenSize = NGUITools.screenSize;
		float num = screenSize.x / screenSize.y;
		if (num >= enableFitHeightVal)
		{
			_root.fitHeight = true;
		}
		else
		{
			_root.fitHeight = false;
		}
	}
}
