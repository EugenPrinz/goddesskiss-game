using System.Collections;
using UnityEngine;

public class SWP_TimeGlobalController : MonoBehaviour
{
	private enum SoundType
	{
		PauseSound,
		ResumeSound
	}

	public delegate void OnGlobalPauseEvent();

	public delegate void OnGlobalResumeEvent();

	public bool DipSoundOnPause;

	public float DipVolumeLevel;

	public float DipTime;

	private float fOldVolume;

	public static bool IsPaused;

	public static float GlobalTimeSinceLevelLoad;

	public bool EnableSound = true;

	public float SoundVolume = 1f;

	public AudioClip GlobalPauseSound;

	public AudioClip GlobalResumeSound;

	private AudioSource asAudio;

	public static float TimeScaleModifier = 1000000f;

	public static SWP_InternalTimedList tlTimedGameObjectList;

	public event OnGlobalPauseEvent OnGlobalPause;

	public event OnGlobalResumeEvent OnGlobalResume;

	private void Awake()
	{
		asAudio = GetComponent<AudioSource>();
	}

	private void Start()
	{
		SoundVolume = Mathf.Clamp(SoundVolume, 0f, 1f);
		if (asAudio == null || !asAudio.enabled)
		{
			EnableSound = false;
		}
	}

	private void Update()
	{
		if (!IsPaused)
		{
			GlobalTimeSinceLevelLoad += Time.deltaTime;
		}
		else
		{
			GlobalTimeSinceLevelLoad += Time.deltaTime * TimeScaleModifier;
		}
	}

	private void BroadcastEvents(string _EventName)
	{
		SWP_InternalTimedGameObject[] array = Object.FindObjectsOfType(typeof(SWP_InternalTimedGameObject)) as SWP_InternalTimedGameObject[];
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SendMessage(_EventName, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void PauseGlobalTime()
	{
		if (IsPaused)
		{
			return;
		}
		BroadcastEvents("OnGlobalPauseBroadcast");
		if (this.OnGlobalPause != null)
		{
			this.OnGlobalPause();
		}
		PlayTimeControlSound(SoundType.PauseSound, SoundVolume);
		if (DipSoundOnPause && DipTime > 0f)
		{
			StartCoroutine(DipAudioCoRoutine());
			return;
		}
		if (DipSoundOnPause)
		{
			fOldVolume = AudioListener.volume;
			AudioListener.volume = DipVolumeLevel;
		}
		IsPaused = true;
		Time.timeScale /= TimeScaleModifier;
	}

	public void ResumeGlobalTime()
	{
		if (IsPaused)
		{
			Time.timeScale *= TimeScaleModifier;
			IsPaused = false;
			BroadcastEvents("OnGlobalResumeBroadcast");
			if (this.OnGlobalResume != null)
			{
				this.OnGlobalResume();
			}
			PlayTimeControlSound(SoundType.ResumeSound, SoundVolume);
			if (DipSoundOnPause && DipTime > 0f)
			{
				StartCoroutine(ResumeAudioCoRoutine());
			}
			else if (DipSoundOnPause)
			{
				AudioListener.volume = fOldVolume;
			}
		}
	}

	private void PlayTimeControlSound(SoundType _SoundType, float _SoundVolume)
	{
		if (EnableSound && !(asAudio == null) && asAudio.enabled)
		{
			if (_SoundType == SoundType.PauseSound && GlobalPauseSound != null)
			{
				asAudio.PlayOneShot(GlobalPauseSound, _SoundVolume);
			}
			else if (_SoundType == SoundType.ResumeSound && GlobalResumeSound != null)
			{
				asAudio.PlayOneShot(GlobalResumeSound, _SoundVolume);
			}
		}
	}

	private IEnumerator DipAudioCoRoutine()
	{
		float thisProgress = 0f;
		fOldVolume = AudioListener.volume;
		for (; thisProgress < 1f; thisProgress += Time.deltaTime / 1f)
		{
			yield return new WaitForEndOfFrame();
			AudioListener.volume = Mathf.Lerp(AudioListener.volume, DipVolumeLevel, thisProgress);
		}
		AudioListener.volume = DipVolumeLevel;
		IsPaused = true;
		Time.timeScale /= TimeScaleModifier;
	}

	private IEnumerator ResumeAudioCoRoutine()
	{
		for (float thisProgress = 0f; thisProgress < 1f; thisProgress += Time.deltaTime / 1f)
		{
			yield return new WaitForEndOfFrame();
			AudioListener.volume = Mathf.Lerp(DipVolumeLevel, fOldVolume, thisProgress);
		}
		AudioListener.volume = fOldVolume;
	}
}
