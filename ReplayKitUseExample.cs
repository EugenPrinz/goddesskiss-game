using UnityEngine;

public class ReplayKitUseExample : BaseIOSFeaturePreview
{
	private void Awake()
	{
		ISN_ReplayKit.ActionRecordStarted += HandleActionRecordStarted;
		ISN_ReplayKit.ActionRecordStoped += HandleActionRecordStoped;
		ISN_ReplayKit.ActionRecordInterrupted += HandleActionRecordInterrupted;
		ISN_ReplayKit.ActionShareDialogFinished += HandleActionShareDialogFinished;
		ISN_ReplayKit.ActionRecorderDidChangeAvailability += HandleActionRecorderDidChangeAvailability;
		IOSNativePopUpManager.showMessage("Welcome", "Hey there, welcome to the ReplayKit testing scene!");
		ISN_Logger.Log("ReplayKit Is Avaliable: " + ISN_Singleton<ISN_ReplayKit>.Instance.IsAvailable);
	}

	private void OnDestroy()
	{
		ISN_ReplayKit.ActionRecordStarted -= HandleActionRecordStarted;
		ISN_ReplayKit.ActionRecordStoped -= HandleActionRecordStoped;
		ISN_ReplayKit.ActionRecordInterrupted -= HandleActionRecordInterrupted;
	}

	private void OnGUI()
	{
		UpdateToStartPos();
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40f), "Replay Kit", style);
		StartY += YLableStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Start Recording"))
		{
			ISN_Singleton<ISN_ReplayKit>.Instance.StartRecording();
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Stop Recording"))
		{
			ISN_Singleton<ISN_ReplayKit>.Instance.StopRecording();
		}
	}

	private void HandleActionRecordInterrupted(ISN_Error error)
	{
		IOSNativePopUpManager.showMessage("Video was interrupted with error: ", " " + error.Description);
	}

	private void HandleActionRecordStoped(ISN_Result res)
	{
		if (res.IsSucceeded)
		{
			ISN_Singleton<ISN_ReplayKit>.Instance.ShowVideoShareDialog();
		}
		else
		{
			IOSNativePopUpManager.showMessage("Fail", "Error: " + res.Error.Description);
		}
	}

	private void HandleActionShareDialogFinished(ReplayKitVideoShareResult res)
	{
		if (res.Sources.Length > 0)
		{
			string[] sources = res.Sources;
			foreach (string text in sources)
			{
				IOSNativePopUpManager.showMessage("Success", "User has shared the video to" + text);
			}
		}
		else
		{
			IOSNativePopUpManager.showMessage("Fail", "User declined video sharing!");
		}
	}

	private void HandleActionRecordStarted(ISN_Result res)
	{
		if (res.IsSucceeded)
		{
			IOSNativePopUpManager.showMessage("Success", "Record was successfully started!");
		}
		else
		{
			ISN_Logger.Log("Record start failed: " + res.Error.Description);
			IOSNativePopUpManager.showMessage("Fail", "Error: " + res.Error.Description);
		}
		ISN_ReplayKit.ActionRecordStarted -= HandleActionRecordStarted;
	}

	private void HandleActionRecorderDidChangeAvailability(bool IsRecordingAvaliable)
	{
		ISN_Logger.Log("Is Recording Avaliable: " + IsRecordingAvaliable);
		ISN_ReplayKit.ActionRecordDiscard += HandleActionRecordDiscard;
		ISN_Singleton<ISN_ReplayKit>.Instance.DiscardRecording();
	}

	private void HandleActionRecordDiscard()
	{
		ISN_Logger.Log("Record Discarded");
	}
}
