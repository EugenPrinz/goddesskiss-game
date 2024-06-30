namespace Step.Tutorial
{
	public class DialogueEvent : AbstractStepAction
	{
		public int eventNum;

		protected ClassicRpgManager dialogMrg;

		public override bool IsLock => true;

		public override bool IsEveryFrameUpdate => true;

		public override bool Enter()
		{
			dialogMrg = UIManager.instance.battle.DialogMrg;
			if (dialogMrg == null)
			{
				return false;
			}
			return base.Enter();
		}

		protected override void OnEnter()
		{
			string arg = "Tutorial";
			string dialogeName = $"{arg}-{eventNum:00}";
			if (!ClassicRpgManager.HasDialogue(dialogeName))
			{
				_isFinish = false;
				return;
			}
			UISetter.SetActive(dialogMrg, active: true);
			dialogMrg.InitWorldMapStart(dialogeName, isTutorial: true);
			UISetter.SetActive(dialogMrg.btnSkip, active: false);
		}

		protected override void OnUpdate()
		{
			if (!dialogMrg.gameObject.activeSelf)
			{
				_isFinish = true;
			}
		}
	}
}
