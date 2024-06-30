using System;

namespace ICode.Actions.NGUI
{
	[Serializable]
	[Category("NGUI")]
	[Tooltip("Sets the new sprite.")]
	public class SetSprite : StateAction
	{
		[SharedPersistent]
		[Tooltip("The game object to use.")]
		public FsmGameObject gameObject;

		[NotRequired]
		[Tooltip("The new atlas to use.")]
		public FsmObject atlas;

		[Tooltip("Sprite name to set.")]
		public FsmString spriteName;

		[Tooltip("Execute the action every frame.")]
		public bool everyFrame;

		private UISprite sprite;

		public override void OnEnter()
		{
			sprite = gameObject.Value.GetComponent<UISprite>();
			if (atlas.Value != null)
			{
				sprite.atlas = (UIAtlas)atlas.Value;
			}
			sprite.spriteName = spriteName.Value;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (atlas.Value != null)
			{
				sprite.atlas = (UIAtlas)atlas.Value;
			}
			sprite.spriteName = spriteName.Value;
		}
	}
}
