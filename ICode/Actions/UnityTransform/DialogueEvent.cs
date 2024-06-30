using System;

namespace ICode.Actions.UnityTransform
{
	[Serializable]
	[Category(Category.Tutorial)]
	[Tooltip("")]
	[HelpUrl("")]
	public class DialogueEvent : StateAction
	{
		public FsmInt eventNum;

		protected ClassicRpgManager dialogMrg;

		public override void OnEnter()
		{
			dialogMrg = UIManager.instance.DialogMrg;
			string arg = "Tutorial";
			string dialogeName = $"{arg}-{eventNum.Value:00}";
			if (!ClassicRpgManager.HasDialogue(dialogeName))
			{
				Finish();
				return;
			}
			UISetter.SetActive(dialogMrg, active: true);
			dialogMrg.InitWorldMapStart(dialogeName, isTutorial: true);
			UISetter.SetActive(dialogMrg.btnSkip, active: false);
		}

		public override void OnUpdate()
		{
			if (!dialogMrg.gameObject.activeSelf)
			{
				Finish();
			}
		}
	}
}
