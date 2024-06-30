using Step;

namespace TutorialStep
{
	public class SetLabelText : AbstractStepAction
	{
		public UILabel label;

		public string key;

		protected override void OnEnter()
		{
			if (label != null)
			{
				label.text = Localization.Get(key);
			}
		}
	}
}
