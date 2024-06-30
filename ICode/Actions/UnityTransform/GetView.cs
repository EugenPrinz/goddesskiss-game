using System;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class GetView : StateAction
	{
		public enum EViewType
		{
			UiRoot,
			UiCommander,
			UiCamp,
			UiGacha,
			UiHeadquarters,
			UiSupplyBase,
			UiSupplyBaseEditFormation,
			UiWorldMap,
			UiWarMemorial,
			UiSituationRoom,
			UiSeaFloorExplore,
			UiUnitLaboratory,
			UiUnitLaboratory_Detail,
			UiMilitaryAcademy,
			UiMetroBank,
			UiCommanderDetail,
			UiReadyBattle,
			UiCommanderSelect,
			UiSeaRobberSweep,
			UiFader,
			UiCommanderList
		}

		public EViewType viewType;

		[Shared]
		public FsmGameObject store;

		public override void OnEnter()
		{
			switch (viewType)
			{
			case EViewType.UiRoot:
				store.Value = UIRoot.list[0].gameObject;
				break;
			case EViewType.UiCommander:
				store.Value = UIManager.instance.world.mainCommand.gameObject;
				break;
			case EViewType.UiCamp:
				store.Value = UIManager.instance.world.camp.gameObject;
				break;
			case EViewType.UiGacha:
				store.Value = UIManager.instance.world.gacha.gameObject;
				break;
			case EViewType.UiHeadquarters:
				store.Value = UIManager.instance.world.headQuarters.gameObject;
				break;
			case EViewType.UiWorldMap:
				store.Value = UIManager.instance.world.worldMap.gameObject;
				break;
			case EViewType.UiWarMemorial:
				store.Value = UIManager.instance.world.warHome.gameObject;
				break;
			case EViewType.UiSituationRoom:
				store.Value = UIManager.instance.world.situation.gameObject;
				break;
			case EViewType.UiMetroBank:
				store.Value = UIManager.instance.world.metroBank.gameObject;
				break;
			case EViewType.UiCommanderDetail:
				store.Value = UIManager.instance.world.commanderDetail.gameObject;
				break;
			case EViewType.UiReadyBattle:
				store.Value = UIManager.instance.world.readyBattle.gameObject;
				break;
			case EViewType.UiSeaRobberSweep:
				store.Value = UIManager.instance.world.seaRobberSweep.gameObject;
				break;
			case EViewType.UiCommanderList:
				store.Value = UIManager.instance.world.commanderList.gameObject;
				break;
			}
			Finish();
		}
	}
}
