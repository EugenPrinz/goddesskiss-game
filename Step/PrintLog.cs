namespace Step
{
	public class PrintLog : AbstractStepAction
	{
		public enum EType
		{
			Normal,
			Warning,
			Error
		}

		public EType type;

		public string log;

		protected override void OnEnter()
		{
			switch (type)
			{
			}
		}
	}
}
