namespace Step.OutGame
{
	public class GetView : AbstractStepAction
	{
		public enum EViewType
		{
			UiRoot,
			UiCommander,
			UiCamp,
			UiGacha,
			UiStorage,
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
			UiGuild
		}

		public EViewType viewType;

		public GameObjectData ret;

		public override bool Enter()
		{
			if (ret == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			switch (viewType)
			{
			case EViewType.UiRoot:
				ret.value = UIRoot.list[0].gameObject;
				break;
			case EViewType.UiCommander:
				ret.value = UIManager.instance.world.mainCommand.gameObject;
				break;
			case EViewType.UiCamp:
				ret.value = UIManager.instance.world.camp.gameObject;
				break;
			case EViewType.UiGacha:
				ret.value = UIManager.instance.world.gacha.gameObject;
				break;
			case EViewType.UiStorage:
				ret.value = UIManager.instance.world.storage.gameObject;
				break;
			case EViewType.UiWorldMap:
				ret.value = UIManager.instance.world.worldMap.gameObject;
				break;
			case EViewType.UiWarMemorial:
				ret.value = UIManager.instance.world.warHome.gameObject;
				break;
			case EViewType.UiSituationRoom:
				ret.value = UIManager.instance.world.situation.gameObject;
				break;
			case EViewType.UiMetroBank:
				ret.value = UIManager.instance.world.metroBank.gameObject;
				break;
			case EViewType.UiCommanderDetail:
				ret.value = UIManager.instance.world.commanderDetail.gameObject;
				break;
			case EViewType.UiReadyBattle:
				ret.value = UIManager.instance.world.readyBattle.gameObject;
				break;
			case EViewType.UiSeaRobberSweep:
				ret.value = UIManager.instance.world.seaRobberSweep.gameObject;
				break;
			case EViewType.UiFader:
				break;
			case EViewType.UiGuild:
				ret.value = UIManager.instance.world.guild.gameObject;
				break;
			case EViewType.UiSupplyBaseEditFormation:
			case EViewType.UiSeaFloorExplore:
			case EViewType.UiUnitLaboratory:
			case EViewType.UiUnitLaboratory_Detail:
			case EViewType.UiMilitaryAcademy:
			case EViewType.UiCommanderSelect:
				break;
			}
		}
	}
}
