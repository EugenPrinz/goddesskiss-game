using Shared.Regulation;
using UnityEngine;

public class UIBoxListItem : UIItemBase
{
	public UISprite goodsIcon;

	public UILabel count;

	public GameObject selectedRoot;

	public override void SetSelection(bool selected)
	{
		UISetter.SetActive(selectedRoot, selected);
	}

	public void Set(string id)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		GoodsDataRow goodsDataRow = regulation.goodsDtbl[id];
		int num = localUser.resourceList[goodsDataRow.serverFieldName];
		UISetter.SetSprite(goodsIcon, goodsDataRow.icon);
		UISetter.SetLabel(count, num);
	}
}
