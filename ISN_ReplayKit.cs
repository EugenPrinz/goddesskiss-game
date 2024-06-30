using System;
using UnityEngine;

public class ISN_ReplayKit : ISN_Singleton<ISN_ReplayKit>
{
	private bool _IsRecodingAvailableToShare;

	public bool IsRecording => false;

	public bool IsRecodingAvailableToShare => _IsRecodingAvailableToShare;

	public bool IsAvailable => false;

	public bool IsMicEnabled => false;

	public static event Action<ISN_Result> ActionRecordStarted;

	public static event Action<ISN_Result> ActionRecordStoped;

	public static event Action<ReplayKitVideoShareResult> ActionShareDialogFinished;

	public static event Action<ISN_Error> ActionRecordInterrupted;

	public static event Action<bool> ActionRecorderDidChangeAvailability;

	public static event Action ActionRecordDiscard;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void StartRecording(bool microphoneEnabled = true)
	{
		_IsRecodingAvailableToShare = false;
	}

	public void StopRecording()
	{
	}

	public void DiscardRecording()
	{
		_IsRecodingAvailableToShare = false;
	}

	public void ShowVideoShareDialog()
	{
		_IsRecodingAvailableToShare = false;
	}

	private void OnRecorStartSuccess(string data)
	{
		ISN_Result obj = new ISN_Result(IsResultSucceeded: true);
		ISN_ReplayKit.ActionRecordStarted(obj);
	}

	private void OnRecorStartFailed(string errorData)
	{
		ISN_Result obj = new ISN_Result(errorData);
		ISN_ReplayKit.ActionRecordStarted(obj);
	}

	private void OnRecorStopFailed(string errorData)
	{
		ISN_Result obj = new ISN_Result(errorData);
		ISN_ReplayKit.ActionRecordStoped(obj);
	}

	private void OnRecorStopSuccess()
	{
		_IsRecodingAvailableToShare = true;
		ISN_Result obj = new ISN_Result(IsResultSucceeded: true);
		ISN_ReplayKit.ActionRecordStoped(obj);
	}

	private void OnRecordInterrupted(string errorData)
	{
		_IsRecodingAvailableToShare = false;
		ISN_Error obj = new ISN_Error(errorData);
		ISN_ReplayKit.ActionRecordInterrupted(obj);
	}

	private void OnRecorderDidChangeAvailability(string data)
	{
		ISN_ReplayKit.ActionRecorderDidChangeAvailability(IsAvailable);
	}

	private void OnSaveResult(string sourcesData)
	{
		string[] sourcesArray = IOSNative.ParseArray(sourcesData);
		ReplayKitVideoShareResult obj = new ReplayKitVideoShareResult(sourcesArray);
		ISN_ReplayKit.ActionShareDialogFinished(obj);
	}

	public void OnRecordDiscard(string data)
	{
		_IsRecodingAvailableToShare = false;
		ISN_ReplayKit.ActionRecordDiscard();
	}

	static ISN_ReplayKit()
	{
		ISN_ReplayKit.ActionRecordStarted = delegate
		{
		};
		ISN_ReplayKit.ActionRecordStoped = delegate
		{
		};
		ISN_ReplayKit.ActionShareDialogFinished = delegate
		{
		};
		ISN_ReplayKit.ActionRecordInterrupted = delegate
		{
		};
		ISN_ReplayKit.ActionRecorderDidChangeAvailability = delegate
		{
		};
		ISN_ReplayKit.ActionRecordDiscard = delegate
		{
		};
	}
}
