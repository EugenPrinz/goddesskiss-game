namespace Step
{
	public class LoadScene : AbstractStepAction
	{
		public string SceneName;

		protected override void OnEnter()
		{
			Loading.Load(SceneName);
		}
	}
}
