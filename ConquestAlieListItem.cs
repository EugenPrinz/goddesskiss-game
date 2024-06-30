public class ConquestAlieListItem : UIItemBase
{
	public UILabel title;

	public UILabel stateLabel;

	public UICommander[] commanderItem;

	public void Set(Protocols.ConquestStageUser user)
	{
		if (user == null)
		{
			return;
		}
		UISetter.SetLabel(title, Localization.Format("110318", user.slot));
		UISetter.SetLabel(stateLabel, (!(user.state == "S")) ? Localization.Get("110184") : Localization.Get("110182"));
		UICommander[] array = commanderItem;
		foreach (UICommander mb in array)
		{
			UISetter.SetActive(mb, active: false);
		}
		int num = 0;
		foreach (Protocols.ConquestStageUser.Deck item in user.deck)
		{
			RoCommander commander = RoCommander.Create(item.cid, item.level, item.grade, item.cls, item.costume, 0, item.marry, item.transcendence);
			UISetter.SetActive(commanderItem[num], active: true);
			commanderItem[num].Set(commander);
			num++;
		}
	}
}
