using AOT;

public class GLinkRecordiOS : IGLinkRecord
{
	private delegate void RecordStartDelegate();

	private delegate void RecordErrorDelegate(string result);

	private delegate void RecordFinishDelegate();

	private delegate void RecordFinishWithPreviewDelegate();

	[MonoPInvokeCallback(typeof(RecordStartDelegate))]
	public static void _RecordStartCallback()
	{
	}

	[MonoPInvokeCallback(typeof(RecordErrorDelegate))]
	public static void _RecordErrorCallback(string result)
	{
	}

	[MonoPInvokeCallback(typeof(RecordFinishDelegate))]
	public static void _RecordFinishCallback()
	{
	}

	[MonoPInvokeCallback(typeof(RecordFinishWithPreviewDelegate))]
	public static void _RecordFinishWithPreviewCallback()
	{
	}

	public void startRecord()
	{
	}

	public void stopRecord()
	{
	}
}
