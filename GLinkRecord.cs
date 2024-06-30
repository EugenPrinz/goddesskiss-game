public class GLinkRecord
{
	private static IGLinkRecord glinkRecord;

	public static IGLinkRecord sharedInstance()
	{
		if (glinkRecord == null)
		{
			glinkRecord = new GLinkRecordAndroid();
		}
		return glinkRecord;
	}
}
