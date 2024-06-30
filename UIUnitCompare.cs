public class UIUnitCompare : UIPopup
{
	public UIStatus left;

	public UIStatus right;

	public UISprite leftTotalPowerArrow;

	public UISprite leftLeadershipArrow;

	public UISprite rightTotalPowerArrow;

	public UISprite rightLeadershipArrow;

	public string updownArrowPrefix = "logisticsbase_arrow_";

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
	}

	public void Set(RoUnit before, RoUnit after)
	{
		if (left != null)
		{
			left.Set(before);
		}
		if (right != null)
		{
			right.Set(after);
		}
		int totalPower = before.currLevelReg.GetTotalPower();
		int totalPower2 = after.currLevelReg.GetTotalPower();
		_SetArrow(leftTotalPowerArrow, rightTotalPowerArrow, totalPower, totalPower2);
		_SetArrow(leftLeadershipArrow, rightLeadershipArrow, before.currLevelReg.leadership, after.currLevelReg.leadership, invert: true);
	}

	private void _SetArrow(UISprite leftArrow, UISprite rightArrow, int leftValue, int rightValue, bool invert = false)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (leftValue > rightValue)
		{
			text = ((!invert) ? "up" : "down");
			text2 = ((!invert) ? "down" : "up");
		}
		else if (leftValue < rightValue)
		{
			text = ((!invert) ? "down" : "up");
			text2 = ((!invert) ? "up" : "down");
		}
		UISetter.SetSprite(leftArrow, updownArrowPrefix + text);
		UISetter.SetSprite(rightArrow, updownArrowPrefix + text2);
	}

	public void Set(string beforeId, string afterId)
	{
		RoUnit before = RemoteObjectManager.instance.localUser.FindUnit(beforeId);
		RoUnit after = RemoteObjectManager.instance.localUser.FindUnit(afterId);
		Set(before, after);
	}
}
