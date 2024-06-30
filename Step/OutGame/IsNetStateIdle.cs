namespace Step.OutGame
{
	public class IsNetStateIdle : AbstractStepCondition
	{
		public override bool Validate()
		{
			return JsonRpcClient.netStateIdle;
		}
	}
}
