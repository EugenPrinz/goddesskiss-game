using Shared.Regulation;
using UnityEngine;

public class UIConquestBattleResultStageListItem : UIItemBase
{
	public UISprite icon;

	public new UILabel name;

	public UILabel state;

	public GameObject resultBtn;

	public UILabel btnLabel;

	public GameObject joinRoot;

	public void Set(int point, int state, bool standby)
	{
		GuildOccupyDataRow guildOccupyDataRow = RemoteObjectManager.instance.regulation.guildOccupyDtbl[point.ToString()];
		UISetter.SetActive(joinRoot, standby);
		UISetter.SetSprite(icon, guildOccupyDataRow.building);
		UISetter.SetLabel(name, Localization.Get(guildOccupyDataRow.s_idx));
		string text = string.Empty;
		switch (state)
		{
		case 0:
			text = Localization.Get("110282");
			break;
		case 1:
			text = Localization.Get("110203");
			break;
		case 2:
			text = Localization.Get("110202");
			break;
		}
		UISetter.SetLabel(this.state, text);
		btnLabel.color = ((state != 0) ? new Color(1f, 0.9f, 0f) : new Color(0.5f, 0.5f, 0.5f));
		UISetter.SetButtonGray(resultBtn, state != 0);
	}
}
