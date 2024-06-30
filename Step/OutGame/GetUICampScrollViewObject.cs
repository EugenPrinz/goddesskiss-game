namespace Step.OutGame
{
	public class GetUICampScrollViewObject : AbstractStepAction
	{
		public GameObjectData ret;

		public override bool Enter()
		{
			if (ret == null)
			{
				return false;
			}
			if (UIManager.instance.world.camp == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			ret.value = UIManager.instance.world.camp.mapScrollView.gameObject;
		}
	}
}
