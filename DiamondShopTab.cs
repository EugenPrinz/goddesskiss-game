using UnityEngine;

public class DiamondShopTab : MonoBehaviour
{
	[SerializeField]
	private UILabel OnName;

	[SerializeField]
	private UILabel OffName;

	public void SetTabName(string _name)
	{
		UISetter.SetLabel(OnName, _name);
		UISetter.SetLabel(OffName, _name);
	}
}
