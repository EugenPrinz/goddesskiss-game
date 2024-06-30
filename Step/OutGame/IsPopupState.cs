namespace Step.OutGame
{
	public class IsPopupState : AbstractStepCondition
	{
		public enum EPopupState
		{
			CAMP,
			GACHA,
			SUPPLY_BASE,
			SUPPLY_BASE_FORMATION,
			WORLD_MAP,
			WARMEMORIAL,
			SITUATION_ROOM,
			SITUATION_ROOM_SEA_FLOOREXPLORE,
			UNIT_LABORATORY,
			UNIT_LABORATORY_DETAIL,
			MILITARY_ACADEMY,
			SWEEP,
			METROBANK,
			COMMANDER_DETAIL,
			BATTLE_READY,
			BATTLE_COMMANDER_SELECT,
			SITUATION_ROOM_SEA_ROBBER_SWEEP,
			GUILD
		}

		public EPopupState state;

		public override bool Validate()
		{
			if (UIPopup.openedPopups.Count <= 0)
			{
				return false;
			}
			return state switch
			{
				EPopupState.CAMP => UIPopup.openedPopups[0] is UICamp, 
				EPopupState.GACHA => UIPopup.openedPopups[0] is UIGacha, 
				EPopupState.WORLD_MAP => UIPopup.openedPopups[0] is UIWorldMap, 
				EPopupState.WARMEMORIAL => UIPopup.openedPopups[0] is UIWarHome, 
				EPopupState.SITUATION_ROOM => UIPopup.openedPopups[0] is UISituation, 
				EPopupState.SWEEP => UIPopup.openedPopups[0] is UISituation, 
				EPopupState.METROBANK => UIPopup.openedPopups[0] is UIMetroBank, 
				EPopupState.COMMANDER_DETAIL => UIPopup.openedPopups[0] is UICommanderDetail, 
				EPopupState.BATTLE_READY => UIPopup.openedPopups[0] is UIReadyBattle, 
				EPopupState.SITUATION_ROOM_SEA_ROBBER_SWEEP => UIPopup.openedPopups[0] is UISeaRobberSweep, 
				EPopupState.GUILD => UIPopup.openedPopups[0] is UIGuild, 
				_ => false, 
			};
		}
	}
}
