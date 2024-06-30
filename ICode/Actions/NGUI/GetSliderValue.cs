using System;

namespace ICode.Actions.NGUI
{
	[Serializable]
	[Category("NGUI")]
	[Tooltip("Gets the value of an UISilder.")]
	public class GetSliderValue : StateAction
	{
		[SharedPersistent]
		[Tooltip("The game object to use.")]
		public FsmGameObject gameObject;

		[Shared]
		[Tooltip("Store the value.")]
		public FsmFloat store;

		[Tooltip("Execute the action every frame.")]
		public bool everyFrame;

		private UISlider slider;

		public override void OnEnter()
		{
			slider = gameObject.Value.GetComponent<UISlider>();
			store.Value = slider.value;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			store.Value = slider.value;
		}
	}
}
