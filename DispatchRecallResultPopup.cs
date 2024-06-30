using UnityEngine;

public class DispatchRecallResultPopup : UISimplePopup
{
	[SerializeField]
	private UILabel timeResult;

	[SerializeField]
	private UILabel getGold_engage;

	[SerializeField]
	private UISprite Block;

	public void SetPopup(int _time, int t_gold, int e_gold)
	{
		int num = _time / 3600;
		int num2 = _time % 3600 / 60;
		timeResult.text = string.Format(Localization.Get("110069"), num, num2, t_gold);
		getGold_engage.text = e_gold.ToString();
		Block.alpha = 1f;
		SetRecyclable(recyclable: false);
	}

	public override void Close()
	{
		RemoteObjectManager.instance.RequestGetDispatchCommanderList();
		base.Close();
	}
}
