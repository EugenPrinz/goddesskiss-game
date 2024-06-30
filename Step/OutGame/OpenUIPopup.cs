namespace Step.OutGame
{
	public class OpenUIPopup : AbstractStepAction
	{
		public enum EUIPopupType
		{
			Gacha,
			SupplyBase
		}

		public EUIPopupType popup;

		protected override void OnEnter()
		{
			if (popup == EUIPopupType.Gacha)
			{
				UIManager.instance.world.gacha.InitAndOpenGacha();
			}
		}
	}
}
