namespace Step.OutGame
{
	public class GetActivedStage : AbstractStepAction
	{
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
			UIWorldMap worldMap = UIManager.instance.world.worldMap;
			RoWorldMap currentWorldMap = worldMap.currentWorldMap;
			if (currentWorldMap != null)
			{
				int num = int.Parse(currentWorldMap.lastOpenedStageId);
				UIItemBase uIItemBase = worldMap.stageListView.itemList[num - 1];
				ret.value = uIItemBase.gameObject;
			}
		}
	}
}
