using Shared.Regulation;

public class CommanderClassMaterialItem : UIItemBase
{
	public UISprite bg;

	public UISprite icon;

	public UISprite partIcon;

	public UILabel count;

	public UISprite dim;

	private int nHaveCount;

	private int nNeedCount;

	public void SetIcon(int idx, int[] value)
	{
		Regulation regulation = RemoteObjectManager.instance.regulation;
		UISetter.SetActive(bg, value[0] != 0);
		UISetter.SetActive(icon, value[0] != 0);
		UISetter.SetActive(partIcon, value[0] != 0);
		UISetter.SetActive(count, value[0] != 0);
		UISetter.SetActive(dim, value[0] != 0);
		if (value[0] != 0)
		{
			UISetter.SetActive(icon, active: false);
			PartDataRow partDataRow = regulation.partDtbl[value[0].ToString()];
			UISetter.SetSprite(bg, partDataRow.bgResource);
			UISetter.SetSprite(partIcon, partDataRow.serverFieldName);
			nHaveCount = RemoteObjectManager.instance.localUser.FindPart(value[0].ToString()).count;
			nNeedCount = value[1];
			UISetter.SetLabel(count, $"{nHaveCount}/{nNeedCount}");
			UISetter.SetActive(dim, nNeedCount > nHaveCount);
		}
	}
}
