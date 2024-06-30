using System;

namespace ICode.Actions
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class GetActivedWorldMapStageObject : StateAction
	{
		[Shared]
		[Tooltip("Store the result.")]
		public FsmGameObject store;

		public override void OnEnter()
		{
			UIWorldMap worldMap = UIManager.instance.world.worldMap;
			RoWorldMap currentWorldMap = worldMap.currentWorldMap;
			int num = int.Parse(currentWorldMap.lastOpenedStageId);
			UIItemBase uIItemBase = worldMap.stageListView.itemList[num - 1];
			store.Value = uIItemBase.gameObject;
			Finish();
		}
	}
}
