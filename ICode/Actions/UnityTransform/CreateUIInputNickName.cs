using System;
using UnityEngine;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class CreateUIInputNickName : StateAction
	{
		public FsmGameObject parent;

		public FsmGameObject prefab;

		public FsmInt curStep;

		public FsmInt nextStep;

		public override void OnEnter()
		{
			if (RemoteObjectManager.instance.localUser.isTutorialSkip)
			{
				nextStep.Value = ConstValue.tutorialEndStep;
			}
			GameObject gameObject = NGUITools.AddChild(parent.Value, prefab.Value);
			UIInputNickName component = gameObject.GetComponent<UIInputNickName>();
			component.curStep = curStep.Value;
			component.nextStep = nextStep.Value;
			Finish();
		}
	}
}
