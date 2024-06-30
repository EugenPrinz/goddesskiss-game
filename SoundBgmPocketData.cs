using System;
using System.Collections.Generic;
using Cache;
using UnityEngine;

[Serializable]
public class SoundBgmPocketData
{
	public string level;

	public SoundManager.PlayMethod method;

	public List<string> clipNames;

	public SoundConnection GetConnection()
	{
		List<AudioClip> list = new List<AudioClip>();
		for (int i = 0; i < clipNames.Count; i++)
		{
			AudioClip audioClip = CacheManager.instance.SoundCache.Create(clipNames[i], isBgm: true);
			if (!(audioClip == null))
			{
				list.Add(audioClip);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		return new SoundConnection(level, method, list.ToArray());
	}
}
