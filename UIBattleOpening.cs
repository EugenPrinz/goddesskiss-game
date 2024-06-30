using UnityEngine;

public class UIBattleOpening : UIPopup
{
	public UIUser leftUser;

	public UIUser rightUser;

	public GameObject back;

	public void Set(RoUser left, RoUser right)
	{
		if (leftUser != null)
		{
			leftUser.Set(left);
		}
		if (rightUser != null)
		{
			rightUser.Set(right);
		}
	}

	public override void Close()
	{
		base.Close();
		UISetter.SetActive(back, active: false);
	}
}
