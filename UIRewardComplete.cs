using UnityEngine;

public class UIRewardComplete : UIPopup
{
	public UIDefaultListView RewardItemList;

	public UISpineAnimation spineAnimation;

	private void Start()
	{
		UISetter.SetSpine(spineAnimation, "n_006");
	}

	public void Init(string buildingId)
	{
	}

	public override void OnClick(GameObject sender)
	{
		base.OnClick(sender);
		string text = sender.name;
		if (text == "Close")
		{
			Close();
		}
	}
}
