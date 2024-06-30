using System;

namespace ICode.Actions
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class GetChildObjectFromIndex : StateAction
	{
		[SharedPersistent]
		[Tooltip("GameObject to use.")]
		public FsmGameObject gameObject;

		public FsmInt index;

		[Shared]
		[Tooltip("Store the result.")]
		public FsmGameObject store;

		public override void OnEnter()
		{
			store.Value = gameObject.Value.transform.GetChild(index.Value).gameObject;
			Finish();
		}
	}
}
