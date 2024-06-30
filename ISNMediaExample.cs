using System.Collections.Generic;
using UnityEngine;

public class ISNMediaExample : BaseIOSFeaturePreview
{
	private void Awake()
	{
		ISN_MediaController.ActionQueueUpdated += HandleActionQueueUpdated;
		ISN_MediaController.ActionMediaPickerResult += HandleActionMediaPickerResult;
		ISN_MediaController.ActionPlaybackStateChanged += HandleActionPlaybackStateChanged;
		ISN_MediaController.ActionNowPlayingItemChanged += HandleActionNowPlayingItemChanged;
	}

	private void HandleActionNowPlayingItemChanged(MP_MediaItem item)
	{
		ISN_Logger.Log("Now Playing Item Changed: " + ISN_Singleton<ISN_MediaController>.Instance.NowPlayingItem.Title);
	}

	private void HandleActionPlaybackStateChanged(MP_MusicPlaybackState state)
	{
		ISN_Logger.Log("Playback State Changed: " + ISN_Singleton<ISN_MediaController>.Instance.State);
	}

	private void HandleActionQueueUpdated(List<MP_MediaItem> items)
	{
		foreach (MP_MediaItem item in items)
		{
			ISN_Logger.Log("Item: " + item.Title + " / " + item.Id);
		}
	}

	private void HandleActionMediaPickerResult(MP_MediaPickerResult res)
	{
		if (res.IsSucceeded)
		{
			ISN_Logger.Log("Media piacker Succeeded");
		}
		else
		{
			ISN_Logger.Log("Media piacker failed: " + res.Error.Description);
		}
	}

	private void OnGUI()
	{
		UpdateToStartPos();
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40f), "Selecting Songs", style);
		StartY += YLableStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Picker"))
		{
			ISN_Singleton<ISN_MediaController>.Instance.ShowMediaPicker();
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Set Perviostly Picked Song"))
		{
			ISN_Singleton<ISN_MediaController>.Instance.Pause();
			ISN_Logger.Log(ISN_Singleton<ISN_MediaController>.Instance.CurrentQueue[0].Title);
			ISN_Singleton<ISN_MediaController>.Instance.SetCollection(ISN_Singleton<ISN_MediaController>.Instance.CurrentQueue[0]);
			ISN_Singleton<ISN_MediaController>.Instance.Play();
		}
		StartX = XStartPos;
		StartY += YButtonStep;
		StartY += YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40f), "Controling Playback", style);
		StartY += YLableStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Play"))
		{
			ISN_Singleton<ISN_MediaController>.Instance.Play();
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Pause"))
		{
			ISN_Singleton<ISN_MediaController>.Instance.Pause();
		}
		StartX = XStartPos;
		StartY += YButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Next"))
		{
			ISN_Singleton<ISN_MediaController>.Instance.SkipToNextItem();
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Previous"))
		{
			ISN_Singleton<ISN_MediaController>.Instance.SkipToPreviousItem();
		}
		StartX += XButtonStep;
		if (GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Skip To Beginning"))
		{
			ISN_Singleton<ISN_MediaController>.Instance.SkipToBeginning();
		}
	}
}
