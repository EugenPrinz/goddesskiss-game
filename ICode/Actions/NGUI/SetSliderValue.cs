using System;

namespace ICode.Actions.NGUI
{
	[Serializable]
	[Category("NGUI")]
	[Tooltip("Sets the value of an UISilder.")]
	public class SetSliderValue : StateAction
	{
		[SharedPersistent]
		[Tooltip("The game object to use.")]
		public FsmGameObject gameObject;

		[Tooltip("The value to set.")]
		public FsmFloat value;

		[Tooltip("Execute the action every frame.")]
		public bool everyFrame;

		private UISlider slider;

		public override void OnEnter()
		{
			slider = gameObject.Value.GetComponent<UISlider>();
			slider.value = value.Value;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			slider.value = value.Value;
		}
	}
}
