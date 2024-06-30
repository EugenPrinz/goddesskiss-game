public class UIPartList : UIPanelBase
{
	public UIDefaultListView partListView;

	public static readonly string itemIdPrefix = "PartItem-";

	public void Set(RoLocalUser user)
	{
		partListView.Init(user.partList, itemIdPrefix);
	}
}
