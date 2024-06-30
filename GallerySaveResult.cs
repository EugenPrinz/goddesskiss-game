public class GallerySaveResult : AN_Result
{
	private string _imagePath;

	public string imagePath => _imagePath;

	public GallerySaveResult(string path, bool res)
		: base(res)
	{
		_imagePath = path;
	}
}
