public class GLinkNaverId
{
	private static IGLinkNaverId glinkNaverId;

	public static IGLinkNaverId sharedInstance()
	{
		if (glinkNaverId == null)
		{
			glinkNaverId = new GLinkNaverIdAndroid();
		}
		return glinkNaverId;
	}
}
