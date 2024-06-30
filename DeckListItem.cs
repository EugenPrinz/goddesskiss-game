using System.Collections.Generic;

public class DeckListItem : UIItemBase
{
	public UILabel title;

	public UICommanderSelectItem[] commanderItem;

	public void Set(Protocols.UserInformationResponse.PreDeck row, BattleData battleData)
	{
		RoLocalUser localUser = RemoteObjectManager.instance.localUser;
		UISetter.SetLabel(title, row.name);
		UICommanderSelectItem[] array = commanderItem;
		foreach (UICommanderSelectItem mb in array)
		{
			UISetter.SetActive(mb, active: false);
		}
		int num = 0;
		foreach (KeyValuePair<string, int> deckDatum in row.deckData)
		{
			RoCommander roCommander = localUser.FindCommander(deckDatum.Value.ToString());
			if (roCommander != null)
			{
				UISetter.SetActive(commanderItem[num], active: true);
				if (battleData.type == EBattleType.Annihilation)
				{
					UISetter.SetActive(commanderItem[num].notEnoughClassRoot, !roCommander.isAdvancePossible);
				}
				commanderItem[num].Set(roCommander, (RoTroop)null, battleData, ECharacterType.None, -1);
			}
			num++;
		}
	}
}
