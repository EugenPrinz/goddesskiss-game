using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundPlayer : MonoBehaviour
{
	private static SoundPlayer _singleton;

	public AudioMixerGroup mixerGroup;

	public int generateSECount = 4;

	public List<AudioSource> seSourceList;

	public AudioSource bgmSource;

	private void Awake()
	{
		if (_singleton == null)
		{
			_singleton = this;
		}
	}

	private void OnDestroy()
	{
		if (_singleton == this)
		{
			_singleton = null;
		}
	}

	public static void PlaySE(AudioClip clip)
	{
		if (!(_singleton == null))
		{
			_singleton._PlaySE(clip);
		}
	}

	public static void PlayBGM(AudioClip clip)
	{
		if (!(_singleton == null))
		{
			_singleton.bgmSource.clip = clip;
			_singleton.bgmSource.Play();
		}
	}

	public static void SetSEVolume(float volume)
	{
		if (!(_singleton == null))
		{
			for (int i = 0; i < _singleton.seSourceList.Count; i++)
			{
				AudioSource audioSource = _singleton.seSourceList[i];
				audioSource.volume = volume;
				audioSource.enabled = volume > 0f;
			}
		}
	}

	public static void SetBGMVolume(float volume)
	{
		if (!(_singleton == null))
		{
			_singleton.bgmSource.volume = volume;
			_singleton.bgmSource.enabled = volume > 0f;
		}
	}

	private void Start()
	{
		AudioSource[] components = base.gameObject.GetComponents<AudioSource>();
		seSourceList.AddRange(components);
		if (components.Length < generateSECount)
		{
			for (int i = components.Length; i < generateSECount; i++)
			{
				AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
				audioSource.outputAudioMixerGroup = mixerGroup;
				seSourceList.Add(audioSource);
			}
		}
		bgmSource = base.gameObject.AddComponent<AudioSource>();
		bgmSource.loop = true;
		GameSetting instance = GameSetting.instance;
		SetSEVolume((!instance.se) ? 0f : 1f);
		SetBGMVolume((!instance.bgm) ? 0f : 1f);
	}

	private void _PlaySE(AudioClip clip)
	{
		if (!GameSetting.instance.se)
		{
			return;
		}
		AudioSource audioSource = null;
		float num = float.MinValue;
		bool flag = false;
		foreach (AudioSource seSource in seSourceList)
		{
			if (!seSource.isPlaying)
			{
				seSource.PlayOneShot(clip);
				flag = true;
			}
			if (num < seSource.time)
			{
				num = seSource.time;
				audioSource = seSource;
			}
		}
		if (!flag)
		{
			audioSource.PlayOneShot(clip);
		}
	}
}
