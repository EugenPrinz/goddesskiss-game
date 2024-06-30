using System;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class GetPopupState : StateAction
	{
		[Shared]
		public FsmPopupState store;

		public bool everyFrame;

		public override void OnEnter()
		{
			store.Value = FsmPopupState.EPopupState.NONE;
			if (UIPopup.openedPopups[0] is UICamp)
			{
				if (UIPopup.openedPopups.Count == 1)
				{
					store.Value = FsmPopupState.EPopupState.CAMP;
				}
			}
			else if (UIPopup.openedPopups[0] is UIGacha)
			{
				store.Value = FsmPopupState.EPopupState.GACHA;
			}
			else if (UIPopup.openedPopups[0] is UIWorldMap)
			{
				if (UIPopup.openedPopups.Count == 2)
				{
					store.Value = FsmPopupState.EPopupState.WORLD_MAP;
				}
			}
			else if (UIPopup.openedPopups[0] is UIWarHome)
			{
				store.Value = FsmPopupState.EPopupState.WARMEMORIAL;
			}
			else if (UIPopup.openedPopups[0] is UISituation)
			{
				store.Value = FsmPopupState.EPopupState.SITUATION_ROOM;
			}
			else if (UIPopup.openedPopups[0] is UISituation)
			{
				store.Value = FsmPopupState.EPopupState.SWEEP;
			}
			else if (UIPopup.openedPopups[0] is UIMetroBank)
			{
				store.Value = FsmPopupState.EPopupState.METROBANK;
			}
			else if (UIPopup.openedPopups[0] is UICommanderDetail)
			{
				store.Value = FsmPopupState.EPopupState.COMMANDER_DETAIL;
			}
			else if (UIPopup.openedPopups[0] is UIReadyBattle)
			{
				store.Value = FsmPopupState.EPopupState.BATTLE_READY;
			}
			else if (UIPopup.openedPopups[0] is UISeaRobberSweep)
			{
				store.Value = FsmPopupState.EPopupState.SITUATION_ROOM_SEA_ROBBER_SWEEP;
			}
			else if (UIPopup.openedPopups[0] is UICommanderList)
			{
				store.Value = FsmPopupState.EPopupState.COMMANDERLIST;
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			store.Value = FsmPopupState.EPopupState.NONE;
			if (UIPopup.openedPopups[0] is UICamp)
			{
				if (UIPopup.openedPopups.Count == 1)
				{
					store.Value = FsmPopupState.EPopupState.CAMP;
				}
			}
			else if (UIPopup.openedPopups[0] is UIGacha)
			{
				store.Value = FsmPopupState.EPopupState.GACHA;
			}
			else if (UIPopup.openedPopups[0] is UIWorldMap)
			{
				if (UIPopup.openedPopups.Count == 2)
				{
					store.Value = FsmPopupState.EPopupState.WORLD_MAP;
				}
			}
			else if (UIPopup.openedPopups[0] is UIWarHome)
			{
				store.Value = FsmPopupState.EPopupState.WARMEMORIAL;
			}
			else if (UIPopup.openedPopups[0] is UISituation)
			{
				store.Value = FsmPopupState.EPopupState.SITUATION_ROOM;
			}
			else if (UIPopup.openedPopups[0] is UISituation)
			{
				store.Value = FsmPopupState.EPopupState.SWEEP;
			}
			else if (UIPopup.openedPopups[0] is UIMetroBank)
			{
				store.Value = FsmPopupState.EPopupState.METROBANK;
			}
			else if (UIPopup.openedPopups[0] is UICommanderDetail)
			{
				store.Value = FsmPopupState.EPopupState.COMMANDER_DETAIL;
			}
			else if (UIPopup.openedPopups[0] is UIReadyBattle)
			{
				store.Value = FsmPopupState.EPopupState.BATTLE_READY;
			}
			else if (UIPopup.openedPopups[0] is UISeaRobberSweep)
			{
				store.Value = FsmPopupState.EPopupState.SITUATION_ROOM_SEA_ROBBER_SWEEP;
			}
			else if (UIPopup.openedPopups[0] is UIHeadQuarters)
			{
				store.Value = FsmPopupState.EPopupState.HEADQUARTERS;
			}
			else if (UIPopup.openedPopups[0] is UICommanderList)
			{
				store.Value = FsmPopupState.EPopupState.COMMANDERLIST;
			}
		}
	}
}
