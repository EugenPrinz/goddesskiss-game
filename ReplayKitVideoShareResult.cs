public class ReplayKitVideoShareResult
{
	private string[] _Sources = new string[0];

	public string[] Sources => _Sources;

	public ReplayKitVideoShareResult(string[] sourcesArray)
	{
		_Sources = sourcesArray;
	}
}
