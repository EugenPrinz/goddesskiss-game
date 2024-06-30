public class UIConquestTroopInformation : UISimplePopup
{
	public ConquestDeckListItem item;

	public void Init(int number, Protocols.ConquestTroopInfo.Troop troop)
	{
		item.Set(number, EConquestStageInfoType.Main, troop);
	}
}
