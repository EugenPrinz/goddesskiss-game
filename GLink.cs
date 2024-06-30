public class GLink
{
	private static IGLink glink;

	public static IGLink sharedInstance()
	{
		if (glink == null)
		{
			glink = new GLinkAndroid();
		}
		return glink;
	}
}
