public class UIUnitInformation : UIPopup
{
	public UIStatus status;

	private void Start()
	{
		SetAutoDestroy(autoDestory: true);
	}

	public void Set(RoUnit unit)
	{
		if (status != null)
		{
			status.Set(unit);
		}
	}
}
