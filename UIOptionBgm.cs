public class UIOptionBgm : UIOptionField
{
	public override void UpdateStatus()
	{
		base.UpdateStatus();
		Manager<UIOptionController>.GetInstance().opt_bgm = status;
	}
}
