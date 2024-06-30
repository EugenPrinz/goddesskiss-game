namespace Step
{
	public class FadeEvent : AbstractStepAction
	{
		public enum TYPE
		{
			IN,
			OUT
		}

		public TYPE type;

		public float sec;

		protected override void OnEnter()
		{
			switch (type)
			{
			case TYPE.IN:
				UIFade.In(sec);
				break;
			case TYPE.OUT:
				UIFade.Out(sec);
				break;
			}
		}
	}
}
