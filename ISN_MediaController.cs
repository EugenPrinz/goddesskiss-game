using System;
using System.Collections.Generic;
using UnityEngine;

public class ISN_MediaController : ISN_Singleton<ISN_MediaController>
{
	private MP_MediaItem _NowPlayingItem;

	private MP_MusicPlaybackState _State;

	private List<MP_MediaItem> _CurrentQueue = new List<MP_MediaItem>();

	public MP_MediaItem NowPlayingItem => _NowPlayingItem;

	public List<MP_MediaItem> CurrentQueue => _CurrentQueue;

	public MP_MusicPlaybackState State => _State;

	public static event Action<MP_MediaPickerResult> ActionMediaPickerResult;

	public static event Action<List<MP_MediaItem>> ActionQueueUpdated;

	public static event Action<MP_MediaItem> ActionNowPlayingItemChanged;

	public static event Action<MP_MusicPlaybackState> ActionPlaybackStateChanged;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void SetRepeatMode(MP_MusicRepeatMode mode)
	{
	}

	public void SetShuffleMode(MP_MusicShuffleMode mode)
	{
	}

	public void Play()
	{
	}

	public void Pause()
	{
	}

	public void SkipToNextItem()
	{
	}

	public void SkipToBeginning()
	{
	}

	public void SkipToPreviousItem()
	{
	}

	public void ShowMediaPicker()
	{
	}

	public void SetCollection(params MP_MediaItem[] items)
	{
		List<string> list = new List<string>();
		foreach (MP_MediaItem mP_MediaItem in items)
		{
			list.Add(mP_MediaItem.Id);
		}
		SetCollection(list.ToArray());
	}

	public void SetCollection(params string[] itemIds)
	{
	}

	private List<MP_MediaItem> ParseMediaItemsList(string[] data, int index = 0)
	{
		List<MP_MediaItem> list = new List<MP_MediaItem>();
		for (int i = index; i < data.Length && !(data[i] == "endofline"); i += 7)
		{
			MP_MediaItem item = ParseMediaItemData(data, i);
			list.Add(item);
		}
		return list;
	}

	private MP_MediaItem ParseMediaItemData(string[] data, int index)
	{
		return new MP_MediaItem(data[index], data[index + 1], data[index + 2], data[index + 3], data[index + 4], data[index + 5], data[index + 6]);
	}

	private void OnQueueUpdate(string data)
	{
		string[] data2 = data.Split('|');
		_CurrentQueue = ParseMediaItemsList(data2);
		ISN_MediaController.ActionQueueUpdated(_CurrentQueue);
	}

	private void OnMediaPickerResult(string data)
	{
		string[] data2 = data.Split('|');
		_CurrentQueue = ParseMediaItemsList(data2);
		MP_MediaPickerResult obj = new MP_MediaPickerResult(_CurrentQueue);
		ISN_MediaController.ActionMediaPickerResult(obj);
		ISN_MediaController.ActionQueueUpdated(_CurrentQueue);
	}

	private void OnMediaPickerFailed(string errorData)
	{
		MP_MediaPickerResult obj = new MP_MediaPickerResult(errorData);
		ISN_MediaController.ActionMediaPickerResult(obj);
	}

	private void OnNowPlayingItemchanged(string data)
	{
		string[] data2 = data.Split('|');
		_NowPlayingItem = ParseMediaItemData(data2, 0);
		ISN_MediaController.ActionNowPlayingItemChanged(_NowPlayingItem);
	}

	private void OnPlaybackStateChanged(string state)
	{
		int state2 = Convert.ToInt32(state);
		_State = (MP_MusicPlaybackState)state2;
		ISN_MediaController.ActionPlaybackStateChanged(_State);
	}

	static ISN_MediaController()
	{
		ISN_MediaController.ActionMediaPickerResult = delegate
		{
		};
		ISN_MediaController.ActionQueueUpdated = delegate
		{
		};
		ISN_MediaController.ActionNowPlayingItemChanged = delegate
		{
		};
		ISN_MediaController.ActionPlaybackStateChanged = delegate
		{
		};
	}
}
