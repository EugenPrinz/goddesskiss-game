public class TutorialData
{
	public bool enable { get; set; }

	public int curStep { get; set; }

	public int nxtStep { get; set; }

	public bool skip { get; set; }

	public TutorialData()
	{
		enable = false;
		curStep = 0;
		nxtStep = 0;
		skip = false;
	}
}
