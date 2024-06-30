using Shared.Regulation;

public class UIEventRemaingItem : UIItemBase
{
	public UISprite icon;

	public UISprite multiple;

	public UITimer remainTimer;

	public void Set(EventRemaingTimeDataRow data)
	{
		UISetter.SetSprite(icon, data.img);
		icon.width = data.xaxis;
		icon.height = data.yaxis;
		UISetter.SetSprite(multiple, data.metro);
		TimeData data2 = RemoteObjectManager.instance.localUser.eventRemaingTime[data.idx];
		remainTimer.Set(data2);
		remainTimer.RegisterOnFinished(delegate
		{
			UIManager.instance.world.mainCommand.sideMenu.GetComponent<UISideMenu>().OnRefresh();
		});
	}
}
