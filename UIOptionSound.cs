public class UIOptionSound : UIOptionField
{
	public override void UpdateStatus()
	{
		base.UpdateStatus();
		Manager<UIOptionController>.GetInstance().opt_sound = status;
	}
}
