using Shared.Regulation;

public class CommanderGiftListItem : UIItemBase
{
	public UISprite icon;

	public UILabel giftName;

	public UILabel point;

	public UILabel count;

	public UISprite block;

	public void Set(CommanderGiftDataRow row)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		GoodsDataRow goodsDataRow = regulation.FindGoodsServerFieldName(row.idx.ToString());
		int num = localUser.resourceList[goodsDataRow.serverFieldName];
		UISetter.SetSprite(icon, goodsDataRow.iconId, snap: false);
		UISetter.SetLabel(point, "+" + row.favorPoint);
		UISetter.SetLabel(count, num);
		UISetter.SetActive(block, num == 0);
	}
}
